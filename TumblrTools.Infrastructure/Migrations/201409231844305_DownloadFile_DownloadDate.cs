namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DownloadFile_DownloadDate : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.DownloadFiles", "DownloadDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.DownloadFiles", "DownloadDate");
        }
    }
}
