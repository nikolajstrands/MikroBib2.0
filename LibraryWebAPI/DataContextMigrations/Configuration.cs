namespace LibraryWebAPI.DataContextMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using LibraryWebAPI.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContextMigrations";
        }

        protected override void Seed(DataContext context)
        {
            // Databasen seedes med en låner og tre bøger.

            context.Patrons.AddOrUpdate(x => x.Id,
                new Patron() {
                    Id = 1,
                    FirstName = "Peter",
                    LastName = "Hansen",
                    Address =  "Godthåbsvej 48, 2. tv. 2000 Frederiksberg"
                }
            );

            context.Books.AddOrUpdate(x => x.Id,
                new Book()
                {
                    Id = 1,
                    Title = "Pride and Prejudice",
                    AuthorFirstName = "Jane",
                    AuthorLastName = "Austen",
                    NumberOfPages = 360,
                    Publisher = "New York : St. Martin's Griffin",
                    YearPublished = 2017,
                    PatronId = 1,
                    IsBorrowed = true,
                    DueDate = DateTime.Now.AddDays(30),
                },
                new Book()
                {
                    Id = 2,
                    Title = "Northanger Abbey",
                    AuthorFirstName = "Jane",
                    AuthorLastName = "Austen",
                    NumberOfPages = 370,
                    Publisher = "London : Collins",
                    YearPublished = 1990,
                    PatronId = 1,
                    IsBorrowed = true,
                    DueDate = DateTime.Now.AddDays(30),
                },
                new Book()
                {
                    Id = 3,
                    Title = "Krig og fred",
                    AuthorFirstName = "Lev",
                    AuthorLastName = "Tolstoj",
                    NumberOfPages = 1405,
                    Publisher = "København Lademann",
                    YearPublished = 1973,
                    PatronId = null,
                    IsBorrowed = false,
                    DueDate = null,
                }
             );
        }
    }
}
