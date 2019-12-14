namespace LibraryWebAPI.UserContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixUserModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "JoinDate", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "LastLogin", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "LastLogin", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "JoinDate", c => c.DateTime(nullable: false));
        }
    }
}
