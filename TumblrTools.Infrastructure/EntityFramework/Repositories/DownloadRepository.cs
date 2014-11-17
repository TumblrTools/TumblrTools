namespace Icm.TumblrDownloader.Infrastructure.EntityFramework.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Icm.TumblrDownloader.Domain;
    using Icm.TumblrDownloader.Generic;
    using Icm.TumblrDownloader.Infrastructure.EntityFramework;

    public class DownloadRepository : IDownloadRepository
    {
        private readonly TumblrDownloaderContext context;
        private readonly ILogger logger;

        public DownloadRepository(TumblrDownloaderContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public DownloadEntry Get(string blogId, IEnumerable<string> tags)
        {
            string tagList = DownloadEntry.JoinTags(tags);
            DownloadEntry existing = this.context.DownloadEntries.SingleOrDefault(e => e.BlogId == blogId && e.TagList == tagList);
            return existing;
        }

        public DownloadEntry CreateIfNotExists(string blogId, IEnumerable<string> tags)
        {
            DownloadEntry existing = this.Get(blogId, tags);
            if (existing == null)
            {
                DownloadEntry newEntry = new DownloadEntry(blogId, tags);
                this.context.DownloadEntries.Add(newEntry);
                this.context.SaveChanges();
                return newEntry;
            }

            return existing;
        }

        public IList<DownloadEntry> GetAll()
        {
            return this.context.DownloadEntries.ToList();
        }

        public IList<DownloadEntry> GetAllSortedByBlogId()
        {
            return this.context.DownloadEntries.OrderBy(entry => entry.BlogId).ToList();
        }

        public bool Remove(string blogId, string[] tagList)
        {
            DownloadEntry entity = this.Get(blogId, tagList);

            if (entity == null)
            {
                return false;
            }

            this.context.DownloadEntries.Remove(entity);
            return true;
        }
    }
}