using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryClient.Models;
using LibraryClient.Services;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    class AddPatronViewModel : BaseViewModel
    {
        
        // Forespørgselsobjekt ved oprettelse
        private PatronDTO patron = new PatronDTO();
        public PatronDTO Patron
        {
            get => patron;
            set
            {
                patron = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public AddPatronViewModel(AppState appState)
            : base(appState)
        {
            Name = "Tilføj låner";
            AuthorizedUserRoles.Add("Librarian");
        }

        // Kommando der afbryder oprettelsen
        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {

                    cancelCommand = new RelayCommand(
                        p => Patron = new PatronDTO(),
                        p => Patron.FirstName != "" || Patron.LastName != "" | Patron.Address != ""
                        );
                }

                return cancelCommand;
            }
        }

        // Kommando der gemmer låner
        private ICommand savePatronCommand;
        public ICommand SavePatronCommand
        {
            get
            {
                if (savePatronCommand == null)
                {

                    savePatronCommand = new RelayCommand(
                        p => SavePatron(Patron),
                        p => Patron.FirstName != "" && Patron.LastName != "" && Patron.Address != ""
                        );
                }

                return savePatronCommand;
            }
        }

        // Hjælpefunktion der gemmer låner
        private async void SavePatron(PatronDTO book)
        {
            try
            {
                Patron.Id = 0;
                Patron.NumberOfBooks = 0;
                bool success = await AppState.Repo.AddPatronAsync(patron);

                if (success)
                {


                    MessageBox.Show("Låneren er oprettet!");
                    Patron = new PatronDTO();

                }
                else
                {
                    MessageBox.Show("Det skete en fejl. Låneren kunne ikke oprettes.");
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
    }
}
