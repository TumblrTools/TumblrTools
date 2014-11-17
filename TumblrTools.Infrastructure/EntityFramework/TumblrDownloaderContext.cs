namespace Icm.TumblrDownloader.Infrastructure.EntityFramework
{
    using System.Data.Entity;
    using Icm.TumblrDownloader.Domain;

    public class TumblrDownloaderContext : DbContext
    {
        public TumblrDownloaderContext()
            : base("TumblrDownloader")
        {
        }

        public IDbSet<DownloadEntry> DownloadEntries { get; set; }
        public IDbSet<DownloadFile> DownloadFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DownloadEntry>()
                .Ignore(e => e.Tags);


            modelBuilder.Entity<DownloadFile>()
                .Property(t => t.DownloadFileId.BlogId)
                .HasColumnName("BlogId");

            modelBuilder.Entity<DownloadFile>()
                .Property(t => t.DownloadFileId.PostId)
                .HasColumnName("PostId");

            modelBuilder.Entity<DownloadFile>()
                .Property(t => t.DownloadFileId.PhotosetIndex)
                .HasColumnName("PhotosetIndex");
        }
    }
}