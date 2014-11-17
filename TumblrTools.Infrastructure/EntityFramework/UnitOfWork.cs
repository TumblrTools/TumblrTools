namespace Icm.TumblrDownloader.Infrastructure.EntityFramework
{
    using Icm.TumblrDownloader.Domain;
    using Icm.TumblrDownloader.Generic;
    using Icm.TumblrDownloader.Infrastructure.EntityFramework.Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly TumblrDownloaderContext context;

        public UnitOfWork(TumblrDownloaderContext context, ILogger logger)
        {
            this.context = context;
            this.DownloadRepository = new DownloadRepository(context, logger);
            this.DownloadFileRepository = new DownloadFileRepository(context);
        }

        public IDownloadRepository DownloadRepository { get; private set; }
        public IDownloadFileRepository DownloadFileRepository { get; private set; }

        public void Save()
        {
            var entries = this.context.ChangeTracker.Entries();
            this.context.SaveChanges();
        }
    }
}