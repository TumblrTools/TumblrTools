namespace TumblrTools.Viewer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using TumblrSharp2;
    using TumblrSharp2.Clients;
    using TumblrSharp2.Requests;
    using TumblrSharp2.Responses;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Application;
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    public class GalleryController : BaseController
    {
        public GalleryController(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }

        [Route("")]
        public ActionResult Index()
        {
            return this.View();
        }

        [Route("dashboard/{id?}")]
        public JsonResult Dashboard(long? id)
        {
            IRepositoryBuilder repositoryBuilder = this.GetRepositoryBuilder();

            IEnumerable<PhotoPost> postsPage = repositoryBuilder.PostRepository.GetNewestFrom(id, 20);

            return this.Json(postsPage, JsonRequestBehavior.AllowGet);
        }

        [Route("gallery/image/{blogId}/{postId}/{photoSetIndex}/thumbnail/{desiredWidth}")]
        public ActionResult ThumbnailImage(string blogId, long postId, int photoSetIndex, int desiredWidth = 250)
        {
            return this.GetImage(
                blogId,
                postId,
                photoSetIndex,
                infos =>
                {
                    IEnumerable<PhotoInfo> photoInfos = infos as PhotoInfo[] ?? infos.ToArray();
                    return photoInfos
                                 .First(info => Math.Abs(desiredWidth - info.Width) == photoInfos.Min(x => Math.Abs(desiredWidth - x.Width)));
                });
        }

        [Route("gallery/image/{blogId}/{postId}/{photoSetIndex}")]
        public ActionResult FullImage(string blogId, long postId, int photoSetIndex)
        {
            return this.GetImage(
                blogId,
                postId,
                photoSetIndex,
                infos =>
                {
                    IEnumerable<PhotoInfo> photoInfos = infos as PhotoInfo[] ?? infos.ToArray();
                    return photoInfos.First(info => info.Width == photoInfos.Max(x => x.Width));
                });
        }

        private static string MimeType(string extension)
        {
            switch (extension)
            {
                case "jpg":
                    return "image/jpeg";
                default:
                    return "image/" + extension;
            }
        }

        private ActionResult GetImage(string blogId, long postId, int photoSetIndex, Func<IEnumerable<PhotoInfo>, PhotoInfo> sizeFinder)
        {
            IRepositoryBuilder uow = this.GetRepositoryBuilder();
            PhotoPost post = uow.PostRepository.GetById(postId);
            if (post.BlogName != blogId)
            {
                return this.HttpNotFound("Could not find post " + postId + " inside blog " + blogId);
            }

            Photo photo = post.PhotoSet[photoSetIndex];
            PhotoInfo photoInfo = sizeFinder(photo.AlternateSizes);
            int width = photoInfo.Width;
            int height = photoInfo.Height;
            string extension = photoInfo.ImageUrl.Split('.').Last();

            string fullPath = Path.Combine(
                this.Configuration.PhotosDirectory, 
                blogId, 
                GetLocalFileName(post.Id, photoSetIndex, width, height, extension));

            if (!System.IO.File.Exists(fullPath))
            {
                ITumblrClient client = new FakeTumblrClient();
                var downloadService = new DownloadService(
                    this.Logger,
                    uow.DownloadRepository,
                    uow.PostRepository,
                    this.Configuration.PhotosDirectory,
                    client,
                    false,
                    true,
                    false,
                    false);

                downloadService.DownloadImage(post, photo, photoSetIndex, photoInfo);
            }

            return this.File(fullPath, MimeType(extension));
        }

        private static string GetLocalFileName(long postId, int photoSetIndex, int width, int height, string extension)
        {
            return string.Format("{0}_{1}_{2}x{3}.{4}", postId, photoSetIndex, width, height, extension);
        }


        //private static void TATETI()
        //{
        //    OAuthClientFactory factory = new OAuthClientFactory();
        //    string consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        //    string consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        //    IOAuthClient authClient = factory.Create(
        //        consumerKey,
        //        consumerSecret);
        //    Token requestToken = authClient.GetRequestTokenAsync("http://example.com/callback").Result;
        //    Uri authorizeUrl = authClient.GetAuthorizeUrl(requestToken);

        //    Process.Start(authorizeUrl.ToString());

        //    string formKey;
        //    using (HttpClient webClient = new HttpClient())
        //    {
        //        HttpResponseMessage response = webClient.GetAsync(authorizeUrl).Result;

        //        string html = response.Content.ReadAsStringAsync().Result;

        //        Regex formKeyRegex = new Regex("<input type=\"hidden\" name=\"form_key\" value=\"([^\"]+)\" />");

        //        Match formKeyMatch = formKeyRegex.Match(html);

        //        formKey = formKeyMatch.Groups[1].ToString();
        //        HttpContent content = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("form_key", formKey),
        //            new KeyValuePair<string, string>("oauth_token", requestToken.Key),
        //        });
        //        HttpResponseMessage postResult = webClient.PostAsync(authorizeUrl, content).Result;

        //        Debug.Assert(postResult.IsSuccessStatusCode);
        //    }

        //    Token accessToken = authClient.GetAccessTokenAsync(requestToken, formKey).Result;

        //}


        //private void FollowMyEntries()
        //{
        //    //this.logger = new Log4NetLogger("TumblrViewer");
        //    //log4net.Config.XmlConfigurator.Configure();
        //    //this.logger.Info("Log set");
        //    //var repo = new DownloadRepository(new TumblrDownloaderContext(), this.logger);

        //    //ITumblrClient client = BuildTumblrClient();
        //    //foreach (DownloadEntry entry in repo.GetAll())
        //    //{
        //    //    BlogInfo blogInfo = null;
        //    //    try
        //    //    {
        //    //        blogInfo = client.GetBlogInfoAsync(entry.BlogId).Result;
        //    //    }
        //    //    catch (AggregateException ex)
        //    //    {
        //    //        this.logger.Error("Blog " + entry.BlogId + " is no more on Tumblr");
        //    //    }

        //    //    if (blogInfo != null)
        //    //    {
        //    //        client.FollowAsync(blogInfo.Url).Wait();
        //    //        this.logger.Info("Followed " + entry.BlogId + " successfully!");
        //    //    }
        //    //}
        //}

    }

    internal class FakeTumblrClient : ITumblrClient
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ITumblrRawClient RawClient { get; private set; }
        public Task<BlogInfo> GetBlogInfoAsync(string blogName)
        {
            throw new NotImplementedException();
        }

        public Task<TPost> GetPostAsync<TPost>(string blogName, long postId) where TPost : Post
        {
            throw new NotImplementedException();
        }

        public Task<BlogAndPosts> GetPostsAsync(
            string blogName,
            Pagination pagination,
            PostType type = PostType.All,
            bool includeReblogInfo = false,
            bool includeNotesInfo = false,
            PostFilter filter = PostFilter.Html,
            string tag = null)
        {
            throw new NotImplementedException();
        }

        public Task<Paginated<Post>> GetBlogLikesAsync(string blogName, Pagination pagination)
        {
            throw new NotImplementedException();
        }

        public Task<Paginated<Follower>> GetFollowersAsync(string blogName, Pagination pagination)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreatePostAsync(string blogName, PostData postData)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreatePostAsync(string blogName, PostData postData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<long> EditPostAsync(string blogName, long postId, PostData postData)
        {
            throw new NotImplementedException();
        }

        public Task<long> EditPostAsync(string blogName, long postId, PostData postData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<long> ReblogAsync(string blogName, long postId, string reblogKey, string comment = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetQueuedPostsAsync(string blogName, Pagination pagination, PostFilter filter = PostFilter.Html)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetDraftPostsAsync(string blogName, long sinceId = 0, PostFilter filter = PostFilter.Html)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetSubmissionPostsAsync(string blogName, long startIndex = 0, PostFilter filter = PostFilter.Html)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostAsync(string blogName, long postId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Paginated<Following>> GetFollowingAsync(Pagination pagination)
        {
            throw new NotImplementedException();
        }

        public Task<Paginated<Post>> GetLikesAsync(Pagination pagination)
        {
            throw new NotImplementedException();
        }

        public Task LikeAsync(long postId, string reblogKey)
        {
            throw new NotImplementedException();
        }

        public Task UnlikeAsync(long postId, string reblogKey)
        {
            throw new NotImplementedException();
        }

        public Task FollowAsync(string blogUrl)
        {
            throw new NotImplementedException();
        }

        public Task UnfollowAsync(string blogUrl)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetTaggedPostsAsync(string tag, DateTime? before = null, int count = 20, PostFilter filter = PostFilter.Html)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetDashboardPostsAsync(
            long sinceId,
            Pagination pagination,
            PostType type = PostType.All,
            bool includeReblogInfo = false,
            bool includeNotesInfo = false)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetUserLikesAsync(Pagination pagination)
        {
            throw new NotImplementedException();
        }
    }
}