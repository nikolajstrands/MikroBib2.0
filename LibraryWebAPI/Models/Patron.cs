using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Patron
    {
        // Denne klasse definerer en låner

        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Address { get; set; }

        // Liste med bøger låneren har lånt
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    }
}