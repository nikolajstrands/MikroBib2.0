using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryClient.Models;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        // Forespørgselsobjekt til brugernavn
        private string userName = String.Empty;
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }
  
        // Forespørgselsobjekt til password (bemærk SecureString)
        private SecureString securePassword = new SecureString();
        public SecureString SecurePassword
        {
            get => securePassword;
            set
            {
                securePassword = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public HomeViewModel(AppState appState) 
            : base(appState)
        {
            // Kræver ikke rettigheder
       
        }

        // Kommando til at logge ind
        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new RelayCommand(
                        param => Login(),
                        param => SecurePassword.Length > 0
                        );
                }
                return loginCommand;
            }
        }

        // Kommando til at logge ud
        private ICommand logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                if (logoutCommand == null)
                {
                    logoutCommand = new RelayCommand(
                        param => AppState.User = new UserDTO(),
                        param => AppState.User.Roles.Count != 0
                       );
                }
                return logoutCommand;
            }
        }
        
        // Hjælpefunktion til at logge ind
        private async void Login()
        {                 
            try
            {
                var result = await AppState.Repo.LoginAsync(UserName, SecurePassword);

                if (result != null)
                {
                    AppState.User = result;                  
                }
                else
                {
                    MessageBox.Show("Ingen bruger fundet med dette brugernavn og adgangskode!");
                }

            }
            catch(Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
         
            
            finally
            {
                // SecurePassword fjernes så hurtigt som muligt
                SecurePassword.Dispose();
                SecurePassword = new SecureString();

                // Opdter view
                UserName = String.Empty;
                CommandManager.InvalidateRequerySuggested();
            }
            

        }
               
    }
}
