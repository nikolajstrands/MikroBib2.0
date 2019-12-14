using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryDTOs
{
    public class PatronDTO
    {
        // Denne klasse definerer udvekslingsformatet (Data Tranfer Object) mellem klient og server for låner-objekter

        public int Id { get; set; }

        public string FirstName { get; set; } = String.Empty;

        public string LastName { get; set; } = String.Empty;

        public string FullName {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string Address { get; set; }

        public int NumberOfBooks { get; set; }

    }
}