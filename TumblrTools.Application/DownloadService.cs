namespace TumblrTools.Application
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using TumblrSharp2;
    using TumblrSharp2.Clients;
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    public class DownloadService : IPhotoLocalFileNamer
    {
        private readonly ILogger logger;
        private readonly ITumblrClient client;
        private readonly IDownloadRepository downloadRepo;
        private readonly IPostRepository postRepo;
        private readonly string rootPath;
        private readonly bool stopWhenFoundInDatabase;
        private readonly bool downloadImages;
        private readonly bool downloadImagesFromReblog;
        private readonly bool stopWhenDownloadedImageFound;
        private DateTime? lastDownloadDate;

        public DownloadService(
            ILogger logger,
            IDownloadRepository downloadRepo, 
            IPostRepository postRepo,
            string rootPath, 
            ITumblrClient client,
            bool stopWhenFoundInDatabase = true, 
            bool downloadImages = false,
            bool downloadImagesFromReblog = false,
            bool stopWhenDownloadedImageFound = false)
        {
            this.logger = logger;
            this.downloadRepo = downloadRepo;
            this.postRepo = postRepo;
            this.rootPath = rootPath;
            this.stopWhenFoundInDatabase = stopWhenFoundInDatabase;
            this.downloadImages = downloadImages;
            this.downloadImagesFromReblog = downloadImagesFromReblog;
            this.stopWhenDownloadedImageFound = stopWhenDownloadedImageFound;
            this.client = client;
        }

        public void ProcessBlogs()
        {
            this.logger.Info("Downloading new images from all blogs");
            IList<DownloadEntry> downloadConfigs = this.downloadRepo.GetAll();
            int remaining = downloadConfigs.Count;
            this.logger.Info("{0} blogs will be crawled", remaining);
            foreach (DownloadEntry downloadConfig in downloadConfigs)
            {
                this.ProcessBlogOrRemoveIfNotExists(downloadConfig, remaining);
                remaining -= 1;
            }
        }

        public void ProcessBlog(string blogId)
        {
            this.logger.Info("Downloading new images from {0}", blogId);
            DownloadEntry downloadEntry = this.downloadRepo.Get(blogId);
            if (downloadEntry == null)
            {
                this.logger.Error("The download entry specified does not exist. Please add it.");
                return;
            }

            this.ProcessBlogOrRemoveIfNotExists(downloadEntry, 1);
        }

        private void ProcessBlogOrRemoveIfNotExists(DownloadEntry entry, int remaining)
        {
            try
            {
                BlogInfo blogInfo = this.GetBlogInfo(entry.BlogId);
                this.ProcessBlog(entry, remaining, blogInfo);
            }
            catch (BlogNotFoundException)
            {
                this.downloadRepo.Remove(entry.BlogId);
                this.logger.Info("Removed " + entry.BlogId + " from database...");
            }
        }

        private bool PostMatchesTags(Post post, IList<string> tags)
        {
            if (!tags.Any() || tags.Any(post.Tags.Contains))
            {
                return true;
            }

            if (post.Tags.Any())
            {
                this.logger.Trace("No tag match (post tags: {0})", string.Join(", ", post.Tags));
            }
            else
            {
                this.logger.Trace("No tag match (no tags)");
            }

            return false;
        }

        private void ProcessBlog(DownloadEntry entry, int remaining, BlogInfo blogInfo)
        {
            string blogId = entry.BlogId;
            IList<string> tags = entry.Tags.ToList();

            this.logger.Info("{2} Downloading {0} to {1}", entry, this.rootPath, remaining);

            this.lastDownloadDate = null;

            using (WebClient webClient = new WebClient())
            {
                var crawler = new TumblrCrawler(this.logger, this.client);
                foreach (PhotoPost post in crawler.AllPosts<PhotoPost>(entry.BlogId, tags, PostType.Photo, includeReblogInfo: true))
                {
                    if (!this.PostMatchesTags(post, tags))
                    {
                        post.State = PostCreationState.Draft;
                    }

                    bool postIsReblog = post.RebloggedFromId.HasValue;
                    if (postIsReblog)
                    {
                        this.logger.Debug("Reblog");
                    }
                    PhotoPost storedPost = this.postRepo.GetById(post.Id);

                    if (storedPost == null)
                    {
                        this.postRepo.Add(post);
                        this.logger.Info("Post {0} added to database", post.Id);
                    }
                    else
                    {
                        this.logger.Debug("Post {0} already exists in database", post.Id);

                        if (this.stopWhenFoundInDatabase)
                        {
                            return;
                        }
                    }

                    if (this.downloadImages && (!postIsReblog || this.downloadImagesFromReblog))
                    {
                        bool alreadyDownloaded = this.ProcessPostImages(blogInfo, post, blogId, webClient);

                        if (alreadyDownloaded && this.stopWhenDownloadedImageFound)
                        {
                            return;
                        }
                    }
                }
            }

            if (this.lastDownloadDate.HasValue)
            {
                this.downloadRepo.UpdateLastDownloadTime(entry, this.lastDownloadDate.Value);
            }
        }

        private bool ProcessPostImages(BlogInfo blogInfo, PhotoPost post, string blogId, WebClient webClient)
        {
            this.logger.Info("Processing images...");

            int photoSetIndex = 0;
            bool alreadyDownloaded = false;

            foreach (Photo photo in post.PhotoSet)
            {
                alreadyDownloaded = 
                    this.ProcessPostImage(blogInfo, post, blogId, webClient, photo, photoSetIndex) ||
                    alreadyDownloaded;

                photoSetIndex += 1;
            }

            return alreadyDownloaded;
        }

        private bool ProcessPostImage(
            BlogInfo blogInfo,
            PhotoPost post,
            string blogId,
            WebClient webClient,
            Photo photo,
            int photoSetIndex)
        {
            return photo.AlternateSizes
                .Aggregate(
                    false, 
                    (current, size) => 
                        current || 
                        this.ProcessPostImageSize(blogInfo, post, blogId, webClient, photo, photoSetIndex, size));
        }

        private bool ProcessPostImageSize(
            BlogInfo blogInfo,
            PhotoPost post,
            string blogId,
            WebClient webClient,
            Photo photo,
            int photoSetIndex,
            PhotoInfo size)
        {
            string localFileName = ((IPhotoLocalFileNamer)this).GetLocalFileName(photoSetIndex, size, photo, post, blogInfo);

            string localImagePath = Path.Combine(this.rootPath, blogId);
            string localImageFullName = Path.Combine(localImagePath, localFileName);
            if (File.Exists(localImageFullName))
            {
                this.logger.Debug("Image already downloaded: " + localImageFullName);
                return true;
            }

            this.DownloadImageSize(webClient, size, localImageFullName, localImagePath);
            return false;
        }

        private void DownloadImageSize(WebClient webClient, PhotoInfo size, string localImageFullName, string localImagePath)
        {
            this.logger.Trace("Downloading image...");
            this.lastDownloadDate = DateTime.Now;
            string imageUrl = size.ImageUrl;
            this.DownloadImage(imageUrl, webClient, localImageFullName, localImagePath);
        }

        //private void RemoveImageSize(string localImageFullName)
        //{
        //    this.logger.Info("Removing image...");
        //    File.Delete(localImageFullName);
        //}

        private BlogInfo GetBlogInfo(string blogId)
        {
            BlogInfo blogInfo = null;
            try
            {
                Retrier.Retry(() => blogInfo = this.client.GetBlogInfoAsync(blogId).Result, 3, this.logger);
            }
            catch (AggregateException ex)
            {
                if (AnyInnerException(ex, exception => exception.Message == "Not Found"))
                {
                    throw new BlogNotFoundException();
                }

                throw;
            }

            return blogInfo;
        }

        private static bool AnyInnerException(Exception ex, Func<Exception, bool> test)
        {
            if (test(ex))
            {
                return true;
            }

            if (ex.InnerException != null)
            {
                return AnyInnerException(ex.InnerException, test);
            }

            AggregateException aggregate = ex as AggregateException;
            if (aggregate != null)
            {
                return aggregate.InnerExceptions.Any(x => AnyInnerException(x, test));
            }

            return false;
        }

        string IPhotoLocalFileNamer.GetLocalFileName(int photoSetIndex, PhotoInfo photoInfo, Photo photo, PhotoPost photoPost, BlogInfo blogInfo)
        {
            string postId = photoPost.Id.ToString(CultureInfo.InvariantCulture);
            string localExtension = photoInfo.ImageUrl.Split('.').Last();
            return string.Format("{0}_{1}_{2}x{3}.{4}", postId, photoSetIndex, photoInfo.Width, photoInfo.Height, localExtension);
        }

        private void DownloadImage(string imageUrl, WebClient webClient, string localImageFullName, string localImagePath)
        {
            if (!Directory.Exists(localImagePath))
            {
                Directory.CreateDirectory(localImagePath);
            }

            this.logger.Info("Downloading file");
            this.logger.Debug("Url: {0}", imageUrl);
            this.logger.Debug("File: {0}", localImageFullName);
            try
            {
                Retrier.Retry(() => webClient.DownloadFile(imageUrl, localImageFullName), 3, this.logger);
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to download: ", ex.Message);
            }
        }

        public void DownloadPost(PhotoPost post)
        {
            using (var webClient = new WebClient())
            {
                this.ProcessPostImages(null, post, post.BlogName, webClient);
            }
        }

        public void DownloadImage(PhotoPost post, Photo photo, int photoSetIndex, PhotoInfo size)
        {
            using (var webClient = new WebClient())
            {
                this.ProcessPostImageSize(null, post, post.BlogName, webClient, photo, photoSetIndex, size);
            }
        }
    }

    internal class BlogNotFoundException : Exception
    {
    }
}