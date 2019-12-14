namespace LibraryWebAPI.DataContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatronAndBookUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "AuthorFirstName", c => c.String());
            AddColumn("dbo.Books", "AuthorLastName", c => c.String());
            AddColumn("dbo.Books", "NumberOfPages", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "Publisher", c => c.String());
            AddColumn("dbo.Books", "YearPublished", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "DueDate", c => c.DateTime());
            AddColumn("dbo.Patrons", "FirstName", c => c.String(nullable: false));
            AddColumn("dbo.Patrons", "LastName", c => c.String(nullable: false));
            AddColumn("dbo.Patrons", "Address", c => c.String());
            DropColumn("dbo.Books", "Author");
            DropColumn("dbo.Patrons", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patrons", "Name", c => c.String(nullable: false));
            AddColumn("dbo.Books", "Author", c => c.String());
            DropColumn("dbo.Patrons", "Address");
            DropColumn("dbo.Patrons", "LastName");
            DropColumn("dbo.Patrons", "FirstName");
            DropColumn("dbo.Books", "DueDate");
            DropColumn("dbo.Books", "YearPublished");
            DropColumn("dbo.Books", "Publisher");
            DropColumn("dbo.Books", "NumberOfPages");
            DropColumn("dbo.Books", "AuthorLastName");
            DropColumn("dbo.Books", "AuthorFirstName");
        }
    }
}
