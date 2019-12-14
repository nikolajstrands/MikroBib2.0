namespace LibraryWebAPI.DataContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Author = c.String(),
                        IsBorrowed = c.Boolean(nullable: false),
                        PatronId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patrons", t => t.PatronId)
                .Index(t => t.PatronId);
            
            CreateTable(
                "dbo.Patrons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "PatronId", "dbo.Patrons");
            DropIndex("dbo.Books", new[] { "PatronId" });
            DropTable("dbo.Patrons");
            DropTable("dbo.Books");
        }
    }
}
