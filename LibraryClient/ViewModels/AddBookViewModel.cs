using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.Services;
using LibraryClient.Models;
using System.Windows.Input;
using System.Windows;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
    class AddBookViewModel : BaseViewModel
    {      
        // Bog-objekt som skal oprettes
        private BookDTO book = new BookDTO();
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
        public AddBookViewModel(AppState appState)
            : base(appState)
        {
            Name = "Tilføj bog";
            AuthorizedUserRoles.Add("Librarian");
        }

        // Kommando til at afbryde oprettelsen
        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new RelayCommand(
                        p => Book = new BookDTO(),
                        p => Book.Title != "" || Book.AuthorFirstName != "" || Book.AuthorLastName != ""
                        );
                }
                return cancelCommand;
            }
        }
  
        // Kommando til at gemme bog
        private ICommand saveBookCommand;
        public ICommand SaveBookCommand
        {
            get
            {
                if (saveBookCommand == null)
                {

                    saveBookCommand = new RelayCommand(
                        p => SaveBook(Book),
                        p => Book.Title != "" && Book.AuthorFirstName != "" && Book.AuthorLastName != ""
                        );
                }

                return saveBookCommand;
            }
        }

        // Hjælpefunktion der gemmer bog
        private async void SaveBook(BookDTO book)
        {
            
            try
            {
                book.Id = 0;
                bool success = await AppState.Repo.AddBookAsync(book);

                if (success)
                {

                    MessageBox.Show("Bogen er oprettet!");
                    Book = new BookDTO();

                }
                else
                {
                    MessageBox.Show("Det skete en fejl. Bogen kunne ikke oprettes.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Der skete en fejl:\n" + e);
                Console.WriteLine(e.Message);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }

        }
    }
    
}
