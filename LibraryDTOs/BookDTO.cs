using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LibraryDTOs
{
    public class BookDTO
    {
        // Denne klasse definerer udvekslingsformatet (Data Tranfer Object) mellem klient og server for bog-objekter

        public int? Id { get; set; }

        public string Title { get; set; } = String.Empty;
    
        public string AuthorFirstName { get; set; } = String.Empty;

        public string AuthorLastName { get; set; } = String.Empty;

        public string AuthorFullName {

            get
            {
                return AuthorFirstName + " " + AuthorLastName;
            }
        }

        public int? NumberOfPages { get; set; }

        public string Publisher { get; set; }

        public int? YearPublished { get; set; }

        public int? PatronId { get; set; } = null;

        public bool IsBorrowed { get; set; } = false;

        // Eventuel afleveringsdato
        public Nullable<DateTime> DueDate { get; set; } = null;

    }
}