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
    class BookViewModel : BaseViewModel
    {

        // Forespørgselsobjekt ved søgning
        private int? bookId;
        public int? BookId
        {
            get => bookId;
            set
            {
                bookId = value;
                OnPropertyChanged();
            }
        }
  
        // Bog der fremvises
        private BookDTO book;
        public BookDTO Book          
        {
            get => book;
            set
            {
                book = value;
                OnPropertyChanged();

            }
        }

        // Constructor
        public BookViewModel(AppState appState, BookDTO book, ICommand containerChangePageCommand)
            : base(appState)
        {
            ContainerChangePageCommand = containerChangePageCommand;    
            Book = book;
            Name = "Boginformation";
            AuthorizedUserRoles.AddRange(new string[] { "Desk" });

        }

        // Kommando til at hente bog
        private ICommand getBookCommand;
        public ICommand GetBookCommand
        {
            get
            {
                if (getBookCommand == null)
                {
                    getBookCommand = new RelayCommand(
                        param => GetBook((int.Parse((string)param))),
                        param => param != null && (string)param != "" && int.TryParse((string)param, out int result)
                        );
                }
                return getBookCommand;
            }
        }

        // Hjælpemetode der rent faktisk henter bogen
        private async void GetBook(int id)
        {
            Book = null;

            try
            {
                var result = await AppState.Repo.GetBookAsync(id);

                if (result != null)
                {

                    Book = result;

                }
                else
                {
                    MessageBox.Show("Ingen bog med dette id.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                BookId = null;
                CommandManager.InvalidateRequerySuggested();
            }
         
        }

        // Kommando der rydder view'et
        private ICommand clearBookCommand;
        public ICommand ClearBookCommand
        {
            get
            {
                if (clearBookCommand == null)
                {

                    clearBookCommand = new RelayCommand(
                        p => ClearBookView(),
                        p => Book != null
                        );
                }

                return clearBookCommand;
            }
        }

        private void ClearBookView()
        {
            Book = null;

        }

        // Kommando til at redigere bogen
        private ICommand editBookCommand;
        public ICommand EditBookCommand
        {
            get
            {
                if (editBookCommand == null)
                {

                    editBookCommand = new RelayCommand(
                        p => EditBook(Book),
                        p => Book != null && AppState.CanShiftView == true && UserCanEditBooks(AppState.User)
                        );
                }

                return editBookCommand;
            }
        }
      
        // Hjælpefunktion der navigerer til bogredigering
        private void EditBook(BookDTO book)
        {
            // Opret ny ViewModel til redigere af bøger og tilføj den nuværende bog til constructor'en
            var newEditBookViewModel = new EditBookViewModel(AppState, book);

            // Benyt Skift side-kommandoen fra den omgivende ViewModel ved at kalde Execute()-metode. 
            ContainerChangePageCommand.Execute(newEditBookViewModel);

            // Ret app-status
            AppState.CanShiftView = false;

            // Ryd bog-view
            ClearBookView();
        }

        // Hjælpefunktion til at afgøre om bruger har rettigheder til at redigere bogen
        // Ikke helt kønt ...
        private bool UserCanEditBooks(UserDTO user)
        {
            var testEditBookView = new EditBookViewModel(AppState, null);
            bool canExecute = ContainerChangePageCommand.CanExecute(testEditBookView);
            testEditBookView = null;
            return canExecute;

        }


        



    }
}
