namespace Icm.TumblrDownloader.Infrastructure.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Icm.TumblrDownloader.Domain;
    using Icm.TumblrDownloader.Infrastructure.EntityFramework;

    public class DownloadFileRepository : IDownloadFileRepository
    {
        private readonly TumblrDownloaderContext context;

        public DownloadFileRepository(TumblrDownloaderContext context)
        {
            this.context = context;
        }

        public IEnumerable<DownloadFile> GetAll()
        {
            return this.context.DownloadFiles;
        }

        public void Remove(DownloadFile downloadFile)
        {
            this.context.DownloadFiles.Remove(downloadFile);
        }

        public void Add(DownloadFile downloadFile)
        {
            this.context.DownloadFiles.Add(downloadFile);
        }

        public IList<DownloadFile> GetAllSortedByBlogId()
        {
            return this.context.DownloadFiles.OrderBy(file => file.DownloadFileId.BlogId).ToList();
        }

        public DownloadFile GetById(DownloadFileId id)
        {
            return this.context.DownloadFiles.SingleOrDefault(id.ExpressionEquals());
        }

        public void Remove(string postId)
        {
            foreach (DownloadFile file in this.context.DownloadFiles.Where(df => df.DownloadFileId.PostId == postId))
            {
                this.context.DownloadFiles.Remove(file);
            }
        }

        public DownloadFile GetOneByPostId(string postId)
        {
            return this.context.DownloadFiles.First(df => df.DownloadFileId.PostId == postId);
        }

        public IList<DownloadFile> GetDownloadedToday()
        {
            DateTime limit = DateTime.Now.Subtract(TimeSpan.FromDays(2));
            return this.context.DownloadFiles.Where(df => df.DownloadDate >= limit).ToList();
        }
    }
}