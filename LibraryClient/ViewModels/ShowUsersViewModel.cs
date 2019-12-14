using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryClient.Models;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    class ShowUsersViewModel : BaseViewModel
    {
        // Liste over alle systembrugere
        public ObservableCollection<UserDTO> Users { get; set; } = new ObservableCollection<UserDTO>();

        // Constructor
        public ShowUsersViewModel(AppState appState, ICommand containerChangePageCommand)
            : base(appState)
        {
            ContainerChangePageCommand = containerChangePageCommand;         
            AuthorizedUserRoles.Add("Administrator");
            Name = "Vis systembrugere";

        }
 
        // Kommando til at hente systemebrugere
        private ICommand showUsersCommand;
        public ICommand ShowUsersCommand
        {
            get
            {
                if (showUsersCommand == null)
                {
                    showUsersCommand = new RelayCommand(
                        param => ShowUsers(),
                        param => true
                        );
                }
                return showUsersCommand;
            }
        }

        // Hjæpefunktion der henter brugere
        private async void ShowUsers()
        {
            try
            {

                var users = await AppState.Repo.GetUsersAsync();

                if (users != null)
                {
                    Users.Clear();

                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                }
                else
                {
                    MessageBox.Show("Der skete en fejl. Kunne ikke hente brugere fra serveren.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Kommando til at redigere en valgt bruger
        private ICommand editUserCommand;
        public ICommand EditUserCommand
        {
            get
            {
                if (editUserCommand == null)
                {
                    editUserCommand = new RelayCommand(
                        param => EditUser((UserDTO)param),
                        param => (UserDTO)param != null
                        );
                }
                return editUserCommand;
            }
        }

        // Hjælpefunktion der skifter til nyt view for redigering af den valgte bog
        private void EditUser(UserDTO user)
        {

            var newEditUserView = new EditUserViewModel(AppState, user);
            ContainerChangePageCommand.Execute(newEditUserView);
            AppState.CanShiftView = false;
            Users = new ObservableCollection<UserDTO>();

        }
    }
}
