namespace LibraryWebAPI.UserContextMigrations
{
    using LibraryWebAPI.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"UserContextMigrations";
        }

        protected override void Seed(UserContext context)
        {
  
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new UserContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new UserContext()));

            // tre roller oprettes
            roleManager.Create(new IdentityRole { Name = "Desk" });
            roleManager.Create(new IdentityRole { Name = "Librarian" });
            roleManager.Create(new IdentityRole { Name = "Administrator" });

            // Databasen seedes med 2 brugere
            var user1 = new ApplicationUser()
            {
                UserName = "admin",
                Email = null,
                EmailConfirmed = true,
                FirstName = "Nikolaj",
                LastName = "Strands",
                JoinDate = DateTime.Now.AddYears(-3),
                LastLogin = DateTime.Now
            };

            manager.Create(user1, "123456");

            var adminUser = manager.FindByName("admin");
            manager.AddToRoles(adminUser.Id, new string[] { "Administrator" });

            var user2 = new ApplicationUser()
            {
                UserName = "nikolajstrands",
                Email = null,
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "Testesen",
                JoinDate = DateTime.Now.AddYears(-2),
                LastLogin = DateTime.Now
            };

            manager.Create(user2, "123456");

            var deskUser = manager.FindByName("nikolajstrands");
            manager.AddToRoles(deskUser.Id, new string[] { "Desk" });

        }
    }
}
