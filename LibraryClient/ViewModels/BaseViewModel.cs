using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraryClient.Models;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    // Basisklasse der ligger til grund for alle ViewModels i projektet

    public class BaseViewModel : ObservableObject
    {
        // Hvilke roller kan tilgå dette view?
        public List<string> AuthorizedUserRoles { get; set; } = new List<string>();

        // Viewets navn
        public string Name { get; set; }

        // Applikations-status
        public AppState AppState { get; set; }

        // Skift side-kommando fra det omgivende view
        public ICommand ContainerChangePageCommand { get; set; }

        // Hjælpefunktion til autorisering
        public bool IsUserAuthorized(UserDTO user)
        {
            if(AuthorizedUserRoles.Count == 0)
            {
                return true;
            }

            foreach (string role in user.Roles)
            {
                if (AuthorizedUserRoles.Contains(role))
                {
                    return true;
                }
            }

            return false;
        }

        // Constructors
        public BaseViewModel(AppState appState)
        {
            AppState = appState;

        }

        public BaseViewModel() { }

    }
}
