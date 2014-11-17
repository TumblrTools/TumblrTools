namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DownloadFile : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.DownloadFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocalFilePath = c.String(),
                        PostId = c.String(),
                        PhotosetIndex = c.Int(nullable: false),
                        DownloadEntry_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DownloadEntries", t => t.DownloadEntry_Id)
                .Index(t => t.DownloadEntry_Id);
            
        }
        
        public override void Down()
        {
            this.DropForeignKey("dbo.DownloadFiles", "DownloadEntry_Id", "dbo.DownloadEntries");
            this.DropIndex("dbo.DownloadFiles", new[] { "DownloadEntry_Id" });
            this.DropTable("dbo.DownloadFiles");
        }
    }
}
