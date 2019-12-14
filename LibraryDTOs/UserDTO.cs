using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDTOs
{
    public class UserDTO
    {
        // Denne klasse definerer udvekslingsformatet (Data Tranfer Object) mellem klient og server for bruger-objekter

        public string Id { get; set; }

        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string>();
       
        public string AllRoles
        {
            get => string.Join(", ", Roles);
        }

        public string UserName { get; set; } = "Anonym";
      
        public DateTime Created { get; set; } = new DateTime();

        public DateTime LastLogin { get; set; } = new DateTime();

        public DateTime JoinDate { get; set; } = new DateTime();

    }
}
