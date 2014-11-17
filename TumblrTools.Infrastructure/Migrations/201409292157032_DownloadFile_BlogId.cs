namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DownloadFile_BlogId : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.DownloadFiles", "BlogId", c => c.String());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.DownloadFiles", "BlogId");
        }
    }
}
