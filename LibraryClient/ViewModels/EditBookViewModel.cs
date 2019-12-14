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
    class EditBookViewModel : BaseViewModel
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

        // Bog-objekt der skal redigeres
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
        public EditBookViewModel(AppState appState, BookDTO book)
            : base(appState)
        {          
            Book = book;
            Name = "Rediger bog";
            AuthorizedUserRoles.Add("Librarian");
        }

        // Kommando til at finde en bog
        private ICommand getBookCommand;
        public ICommand GetBookCommand
        {
            get
            {
                if (getBookCommand == null)
                {
                    getBookCommand = new RelayCommand(
                        param => GetBook((int.Parse((string)param))),
                        param => param != null && (string)param != "" && AppState.CanShiftView && int.TryParse((string)param, out int result)
                        );
                }
                return getBookCommand;
            }
        }
        
        // Hjælpefunktion der finder bog
        private async void GetBook(int id)
        {
            Book = null;

            try
            {
                var result = await AppState.Repo.GetBookAsync(id);

                if (result != null)
                {

                    Book = result;
                    AppState.CanShiftView = false;

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

        // Kommando til at fortryde redigering
        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {

                    cancelCommand = new RelayCommand(
                        p => Cancel(),
                        p => Book != null
                        );
                }

                return cancelCommand;
            }
        }

        // Hjælpefunktion der sletter igangværende redigering
        private void Cancel()
        {
            Book = null;
            AppState.CanShiftView = true;
        }

        // Kommado til at gemme redigerede bog
        private ICommand saveBookCommand;
        public ICommand SaveBookCommand
        {
            get
            {
                if (saveBookCommand == null)
                {

                    saveBookCommand = new RelayCommand(
                        p => SaveBook((BookDTO)p),
                        p => Book != null
                        );
                }

                return saveBookCommand;
            }
        }

        // Hjælpefunktion der gemmer redigere bog
        private async void SaveBook(BookDTO book)
        {            
            try
            {
                var result = await AppState.Repo.UpdateBookAsync(book);

                if (result)
                {

                    Book = null;
                    AppState.CanShiftView = true;
                    MessageBox.Show("Bogen er opdateret!");

                }
                else
                {
                    MessageBox.Show("Der skete en fejl. Bogen kunne ikke opdateres.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }

        }

        // Kommando til at slette bog
        private ICommand deleteBookCommand;
        public ICommand DeleteBookCommand
        {
            get
            {
                if (deleteBookCommand == null)
                {

                    deleteBookCommand = new RelayCommand(
                        p => DeleteBook((BookDTO)p),
                        p => (BookDTO)p != null
                        );
                }

                return deleteBookCommand;
            }
        }

        // Hjælpefunktion til at slette bog
        private async void DeleteBook(BookDTO book)
        {
            try
            {
                bool success = await AppState.Repo.DeleteBookAsync((int)book.Id);

                if (success)
                {
                    Book = null;
                    AppState.CanShiftView = true;
                    MessageBox.Show("Bogen blev slettet.");


                }
                else
                {
                    MessageBox.Show("Der skete en fejl. Bogen kunne ikke slettes.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Der skete en fejl:\n" + e);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }          
        }
    }
}
