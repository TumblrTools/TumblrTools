namespace TumblrTools.Viewer.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using TumblrTools.Application;
    using TumblrTools.Domain;
    using TumblrTools.Generic;

    public class DownloadController : BaseController
    {
        private readonly IDownloadRepository downloadRepository;

        public DownloadController(IConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
            IRepositoryBuilder repositoryBuilder = this.GetRepositoryBuilder();
            this.downloadRepository = repositoryBuilder.DownloadRepository;
        }

        [Route("blogs")]
        [HttpGet]
        public ActionResult Index()
        {
            return this.View(this.downloadRepository.GetAllSortedByBlogId());
        }

        [Route("blogs/create")]
        [HttpPost]
        public ActionResult Create(string blogId, string tags)
        {
            EntriesService entryService = new EntriesService(this.Logger, this.downloadRepository);
            entryService.AddOrUpdateEntry(blogId, DownloadEntry.SplitTags(tags).ToArray());
            return this.View("Index", this.downloadRepository.GetAllSortedByBlogId());
        }

        [Route("blogs/delete")]
        [HttpPost]
        public ActionResult Delete(string blogId)
        {
            EntriesService entryService = new EntriesService(this.Logger, this.downloadRepository);
            entryService.RemoveEntry(blogId);
            return this.View("Index", this.downloadRepository.GetAllSortedByBlogId());
        }

    }
}