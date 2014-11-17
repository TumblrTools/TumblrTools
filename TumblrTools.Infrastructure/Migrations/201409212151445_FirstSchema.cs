namespace Icm.TumblrDownloader.Infrastructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FirstSchema : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.DownloadEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlogId = c.String(),
                        TagList = c.String(),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            this.DropTable("dbo.DownloadEntries");
        }
    }
}
