namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;
    using Icm.TumblrDownloader.Infrastructure;
    using Icm.TumblrDownloader.Infrastructure.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<TumblrDownloaderContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TumblrDownloaderContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
