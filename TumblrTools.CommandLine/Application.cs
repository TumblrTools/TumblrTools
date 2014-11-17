namespace TumblrTools.CommandLine
{
    using System;
    using System.Linq;
    using CLAP;
    using TumblrSharp2;
    using TumblrSharp2.Clients;
    using TumblrSharp2.Responses.Posts;
    using TumblrTools.Application;
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    internal class Application
    {
        private readonly IConfiguration configuration;
        private readonly IDownloadRepository downloadRepository;
        private readonly ILogger logger;
        private readonly IPostRepository postRepository;

        public Application(
            IConfiguration configuration,
            ILogger logger,
            IDownloadRepository downloadRepository,
            IPostRepository postRepository)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.downloadRepository = downloadRepository;
            this.postRepository = postRepository;
        }

        [Verb(Aliases = "d", IsDefault = true)]
        public void Download(
            string blogId,
            bool crawlAll = false)
        {
            ITumblrClient client = this.BuildTumblrClient();
            var downloadService = new DownloadService(
                this.logger,
                this.downloadRepository,
                this.postRepository,
                this.configuration.PhotosDirectory,
                client,
                !crawlAll,
                true,
                stopWhenDownloadedImageFound: !crawlAll,
                // ReSharper disable once RedundantArgumentDefaultValue
                downloadImagesFromReblog: false);
            if (string.IsNullOrEmpty(blogId))
            {
                downloadService.ProcessBlogs();
            }
            else
            {
                downloadService.ProcessBlog(blogId);
            }
        }

        [Verb(Aliases = "a", Description = "Adds or updates tags for a download entry; if it is new downloads it")]
        public void AddAndDownload(
            [Required] string blogId,
            string[] tagList)
        {
            var entryService = new EntriesService(this.logger, this.downloadRepository);
            entryService.AddOrUpdateEntry(blogId, tagList);
            this.Download(blogId);
        }

        [Verb(Aliases = "r", Description = "Removes a download entry")]
        public void Remove(
            [Required] string blogId,
            string[] tagList)
        {
            var entryService = new EntriesService(this.logger, this.downloadRepository);

            entryService.RemoveEntry(blogId);
        }

        [Verb(Aliases = "l", Description = "Lists registered download entries")]
        public void List()
        {
            foreach (DownloadEntry entry in this.downloadRepository.GetAllSortedByBlogId())
            {
                Console.WriteLine(entry.ToString());
            }
        }

        [Verb(Aliases = "e")]
        public void EnticingOthers()
        {
            ITumblrClient client = this.BuildTumblrClient();
            var crawler = new TumblrCrawler(this.logger, client);

            IOrderedEnumerable<string> names = crawler.AllPosts<PhotoPost>("enticingothers", null, PostType.Photo, true)
                .Take(100)
                .Where(
                    post => post.RebloggedFromId.HasValue && this.downloadRepository.Get(post.RebloggedFromName) == null)
                .Select(post => post.RebloggedFromName)
                .Distinct()
                .OrderBy(name => name);

            foreach (string name in names)
            {
                Console.WriteLine(name);
            }
        }

        [Verb(Aliases = "s")]
        public void RebuildDraftStatus(string blogId)
        {
            var service = new MetadataRebuildService(this.logger, this.postRepository, this.downloadRepository);


            if (blogId == null)
            {
                service.RebuildDraftStatus();
            }
            else
            {
                service.RebuildDraftStatus(blogId);
            }
        }

        [Help]
        public void Help(string help)
        {
            Console.WriteLine(help);
        }

        private ITumblrClient BuildTumblrClient()
        {
            ITumblrClient client = new TumblrClientFactory().Create(
                this.configuration.ConsumerKey,
                this.configuration.ConsumerSecret
                );

            return client;
        }
    }
}