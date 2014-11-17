namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemovedEntryFileRelationship : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.DownloadFiles", "DownloadEntry_Id", "dbo.DownloadEntries");
            this.DropIndex("dbo.DownloadFiles", new[] { "DownloadEntry_Id" });
            this.DropColumn("dbo.DownloadFiles", "DownloadEntry_Id");
        }
        
        public override void Down()
        {
            this.AddColumn("dbo.DownloadFiles", "DownloadEntry_Id", c => c.Int());
            this.CreateIndex("dbo.DownloadFiles", "DownloadEntry_Id");
            this.AddForeignKey("dbo.DownloadFiles", "DownloadEntry_Id", "dbo.DownloadEntries", "Id");
        }
    }
}
