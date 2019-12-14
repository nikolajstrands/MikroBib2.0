using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class SearchBookViewModel : BaseViewModel
    {
       
        // Forespørgselsobjekt ved søgning
        private BookQuery query = new BookQuery();

        public BookQuery Query
        {
            get => query;
            set
            {
                query = value;
                OnPropertyChanged();
            }
        }
        
        // Bogliste med resultater fra søgningen
        public ObservableCollection<BookDTO> Books { get; set; } = new ObservableCollection<BookDTO>();

        // Constructor
        public SearchBookViewModel(AppState appState, ICommand containerChangePageCommand)
            : base(appState)
        {
            ContainerChangePageCommand = containerChangePageCommand;
            Name = "Søg bøger";
            AuthorizedUserRoles.AddRange(new string[] { "Desk" });
        }

        // Kommando til søge efter bøger
        private ICommand getBooksCommand;
        public ICommand GetBooksCommand
        {
            get
            {
                if (getBooksCommand == null)
                {
                    getBooksCommand = new RelayCommand(
                        param => GetBooks((BookQuery)param),                                           
                        param => true
                        );
                }
                return getBooksCommand;
            }
        }

        // Kommando til at fortryde søgningen
        private ICommand dismissQueryCommand;
        public ICommand DismissQueryCommand
        {
            get
            {
                if (dismissQueryCommand == null)
                {
                    dismissQueryCommand = new RelayCommand(
                        param => Query = new BookQuery(),
                        param => (Query.Title != null && Query.Title != "")  || (Query.Author != null && Query.Author != "") || Query.Id != null
                   );
                }
                return dismissQueryCommand;
            }
        }
   
        // Kommando til at vise detaljer om en bog på listen
        private ICommand showBookDetailsCommand;
        public ICommand ShowBookDetailsCommand
        {
            get
            {
                if (showBookDetailsCommand == null)
                {
                    showBookDetailsCommand = new RelayCommand(
                        param => ShowBook((BookDTO)param),
                        param => (BookDTO)param != null                
                        );
                }
                return showBookDetailsCommand;
            }
        }

        // Hjælpefunktion der viser den valgte bog i en ny BookViewModel
        private void ShowBook(BookDTO book)
        {          
            var newBookViewModel = new BookViewModel(AppState, book, ContainerChangePageCommand);
            ContainerChangePageCommand.Execute(newBookViewModel);          
            ClearBooks();
        }


        // Kommando til at rydde billedet
        private ICommand clearBooksCommand;
        public ICommand ClearBooksCommand
        {
            get
            {
                if (clearBooksCommand == null)
                {
                    clearBooksCommand = new RelayCommand(
                        param => ClearBooks(),
                        param => Books.Count > 0
                    );
                }
                return clearBooksCommand;
            }
        }

        // Hjælpefunktion der ryder billedet
        private void ClearBooks()
        {
            Books.Clear();            
        }

        // Hjælpefunktion der laver søgningen
        private async void GetBooks(BookQuery query)
        {
            // Slet evt. nuværende bøger i listen
            Books.Clear();

            try
            {
                // Lav søgning vha. repo
                var result = await AppState.Repo.GetBooksAsync(query);

                if (result != null)
                {
                    if(result.Count == 0)
                    {
                        MessageBox.Show("Ingen bøger matcher disse søgekriterier.");
                    }
                    else
                    {
                        foreach (BookDTO b in result)
                        {
                            Books.Add(b);
                        }
                    }                               
                }
                else
                {
                    MessageBox.Show("Det skete en fejl. Du har muligvis ikke rettigheder til lave denne søgning.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                Query = new BookQuery();
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
