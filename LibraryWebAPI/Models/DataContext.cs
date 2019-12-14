using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web; 

namespace LibraryWebAPI.Models
{
    public class DataContext : DbContext
    {
        // Her defineres den DataContext der håndterer data vedr. bøger og lånere       
        public DataContext() : base("name=LibraryWebAPIDataContext")
        {          
        }

        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
