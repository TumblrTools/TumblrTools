namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DownloadFile_PostUrl : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.DownloadFiles", "PostUrl", c => c.String());
        }
        
        public override void Down()
        {
            this.DropColumn("dbo.DownloadFiles", "PostUrl");
        }
    }
}
