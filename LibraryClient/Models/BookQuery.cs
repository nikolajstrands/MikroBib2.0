using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClient.Models
{
    public class BookQuery: ObservableObject
    {
        // Klasse der indkapsler data ved en søgning efter bog.

        public int? Id { get; set; } = null;

        public string Title { get; set; } = String.Empty;

        public string Author { get; set; } = String.Empty;

        public string Publisher { get; set; } = String.Empty;

        public string YearPublished { get; set; } = String.Empty;

        public bool? OnlyOnShelf { get; set; } = false;
       
    }
}
