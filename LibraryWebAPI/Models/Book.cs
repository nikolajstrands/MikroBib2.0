using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Book
    {
        // Denne klasse definerer en bog

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string AuthorFirstName { get; set; }

        public string AuthorLastName { get; set; }

        public int NumberOfPages { get; set; }

        public string Publisher { get; set; }

        public int YearPublished { get; set; }

        public bool IsBorrowed { get; set; } = false;

        // Foreign Key
        public int? PatronId { get; set; } = null;

        // Navigation property
        public Patron Patron { get; set; } = null;

        // Afleveringsdato (når bog er udlånt)
        public Nullable<DateTime> DueDate {get; set; } = null;
    }
}