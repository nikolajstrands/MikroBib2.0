using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.Services;
using LibraryClient.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    public class PatronStatusViewModel : BaseViewModel
    {
        
        // Forespørgselsobjekt ved søgning
        private int? patronId;
        public int? PatronId
        {
            get => patronId;
            set
            {
                patronId = value;
                OnPropertyChanged();
            }
        }

        // Låner der fremvises
        private PatronDTO patron;
        public PatronDTO Patron
        {
            get => patron;
            set
            {
                patron = value;
                OnPropertyChanged();
            }
        }

        // Lånerens bøger
        private ObservableCollection<BookDTO> patronBooks;
        public ObservableCollection<BookDTO> PatronBooks
        {
            get => patronBooks;
            set
            {
                patronBooks = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public PatronStatusViewModel(AppState appState)
            : base(appState)
        {
            Name = "Lånerstatus";
            AuthorizedUserRoles.AddRange(new string[] { "Desk" });

        }

        // Kommando til at hente låner
        private ICommand getPatronCommand;
        public ICommand GetPatronCommand
        {
            get
            {
                if (getPatronCommand == null)
                {
                    getPatronCommand = new RelayCommand(
                        param => GetPatron((int.Parse((string)param))),
                        param => param != null && (string)param != "" && AppState.CanShiftView && int.TryParse((string)param, out int result)
                        );
                }
                return getPatronCommand;
            }
        }

        // Hjælpefunktion der henter låner
        private async void GetPatron(int id)
        {
            try
            {          
                var result = await AppState.Repo.GetPatronAsync(id);

                if (result != null)
                {
                    Patron = result;

                    var items = await AppState.Repo.GetPatronBooksAsync(id);

                    if(items != null)
                    {
                        PatronBooks = new ObservableCollection<BookDTO>();

                        foreach (var b in items)
                        {
                            patronBooks.Add(b);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Der skete en fejl. Lånerens bøger kunne ikke hentes.");
                    }

                }
                else
                {
                    MessageBox.Show("Der findes ingen låner med det valgte id.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                PatronId = null;
                CommandManager.InvalidateRequerySuggested();
            }

        }

        // Kommando til at rydde view'et
        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new RelayCommand(
                        param => Clear(),
                        param => Patron != null
                    );
                }
                return clearCommand;
            }
        }

        // Hjælpefunktion der rydder viewet
        private void Clear()
        {
            Patron = null;
            PatronBooks.Clear();
        }

    }
}
