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
    public class EditUserViewModel : BaseViewModel
    {

        // Forespørgselsobjekt ved søgning
        private string userName;
        public string UserName
        {
            get => userName;
            set                
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        // Bruger der skal redigeres
        private UserDTO user;
        public UserDTO User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        // Liste over de mulige roller
        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string>(new string[] { "Desk", "Librarian", "Administrator" });

        // Constructor
        public EditUserViewModel(AppState appState, UserDTO user )
            : base(appState)
        {
            User = user;
            Name = "Rediger bruger";
            AuthorizedUserRoles.Add("Administrator");
        }

        // Kommando til at finde en bruger ud fra brugernavn
        private ICommand getUserCommand;
        public ICommand GetUserCommand
        {
            get
            {
                if (getUserCommand == null)
                {
                    getUserCommand = new RelayCommand(
                        param => GetUser(((string)param)),
                        param => param != null && (string)param != "" && AppState.CanShiftView
                        );
                }
                return getUserCommand;
            }
        }

        // Hjælpefunktion der finder bruger
        private async void GetUser(string username)
        {
            User = null;

            try
            {
                var result = await AppState.Repo.GetUserAsync(username);

                if (result != null)
                {

                    User = result;
                    AppState.CanShiftView = false;

                }
                else
                {
                    MessageBox.Show("Ingen bruger med dette id.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                UserName = null;
                CommandManager.InvalidateRequerySuggested();
            }

        }

        // Kommando til at tilføje rolle til bruger
        private ICommand addRoleCommand;
        public ICommand AddRoleCommand
        {
            get
            {
                if (addRoleCommand == null)
                {

                    addRoleCommand = new RelayCommand(
                        p => User.Roles.Add((string)p),
                        p => User != null && !User.Roles.Contains((string)p) && (string)p != "" && (string)p != null
                        );
                }

                return addRoleCommand;
            }
        }

        // Kommando til fjern rolle fra bruger
        private ICommand removeRoleCommand;
        public ICommand RemoveRoleCommand
        {
            get
            {
                if (removeRoleCommand == null)
                {

                    removeRoleCommand = new RelayCommand(
                        p => User.Roles.Remove((string)p),
                        p => User != null && User.Roles.Contains((string)p)
                        );
                }

                return removeRoleCommand;
            }
        }

        // Kommando til at gemme redigeret bruger
        private ICommand saveUserCommand;
        public ICommand SaveUserCommand
        {
            get
            {
                if (saveUserCommand == null)
                {

                    saveUserCommand = new RelayCommand(
                        p => SaveUser(),
                        p => User != null
                        );
                }

                return saveUserCommand;
            }
        }

        // Hjælpefunktion der gemmer bruger
        private async void SaveUser()
        {
            try
            {
                bool success = await AppState.Repo.UpdateUserAsync(User);

                if(!success)
                {
                    MessageBox.Show("Der skete en fejl! Brugeren kunne ikke redigeres.");
                }
                else
                {
                    if (User.Id == AppState.User.Id)
                    {
                        AppState.User = User;
                    }

                    User = null;
                    AppState.CanShiftView = true;
                    MessageBox.Show("Brugerens roller blev opdateret.");
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

    
        // Kommando til at afbryde redigeringen
        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {

                    cancelCommand = new RelayCommand(
                        p => Cancel(),
                        p => User != null
                        );
                }

                return cancelCommand;
            }
        }

        // Hjælpefunktion til at afbryde redigeringen
        private void Cancel()
        {
            User = null;
            AppState.CanShiftView = true;
        }
    }
}
