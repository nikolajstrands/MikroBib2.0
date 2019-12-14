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
    public class ReturnViewModel : BaseViewModel
    {
   
        // Id for bog til aflevering
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

        // Afleverenden låner
        private PatronDTO patron = null;
        public PatronDTO Patron
        {
            get => patron;
            set
            {
                patron = value;
                OnPropertyChanged();
            }
        }

        // Bøger der skal afleveres
        private ObservableCollection<BookDTO> booksToReturn = new ObservableCollection<BookDTO>();
        public ObservableCollection<BookDTO> BooksToReturn
        {
            get => booksToReturn;
            set
            {
                booksToReturn = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public ReturnViewModel(AppState appState)
           : base(appState)
        {
            AppState = appState;
            Name = "Aflevering";
            AuthorizedUserRoles.AddRange(new string[] { "Desk" });
        }

        // Kommando til at tilføje bog til afleveringslisten
        private ICommand addBookCommand;
        public ICommand AddBookCommand
        {
            get
            {
                if (addBookCommand == null)
                {
                
                    addBookCommand = new RelayCommand(
                        param => AddBook(int.Parse((string)param)),
                        param => BookId != null && int.TryParse((string)param, out int result)
                        );
                }
                return addBookCommand;
            }
        }


        // Kommando til fotryde afleveringen
        private ICommand dismissReturnCommand;
        public ICommand DismissReturnCommand
        {
            get
            {
                if (dismissReturnCommand == null)
                {
                    dismissReturnCommand = new RelayCommand(
                        param => DismissReturn(),
                        param => Patron != null
                        );
                }
                return dismissReturnCommand;
            }
        }

        // Kommando til at gennemføre afleveringen
        private ICommand completeReturnCommand;
        public ICommand CompleteReturnCommand
        {
            get
            {
                if (completeReturnCommand == null)
                {
                    completeReturnCommand = new RelayCommand(
                        param => CompleteReturn(),
                        param => Patron != null && BooksToReturn.Count > 0
                        );
                }
                return completeReturnCommand;
            }
        }

        // Hjælpefunktion til at tilføje bog til afleveringen
        private async void AddBook(int bookId)
        {
            try
            {
                var book = await AppState.Repo.GetBookAsync(bookId);

                if (book != null)
                {

                    if (!book.IsBorrowed)
                    {
                        MessageBox.Show("Bogen er ikke udlånt!");
                    }
                    else if (Patron == null)
                    {
                        var patron = await AppState.Repo.GetPatronAsync(book.PatronId.Value);
                        Patron = patron;
                        BooksToReturn.Add(book);
                        AppState.CanShiftView = false;

                    }
                    else if (Patron != null)
                    {
                        if (Patron.Id != book.PatronId)
                        {
                            MessageBox.Show("Ikke samme låner! Afslut afleveringen og start en ny.");
                        }
                        else
                        {
                            BooksToReturn.Add(book);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ingen bog fundet med dette id.");
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

        // Hjælpefunktion til at afbryde afleveringen
        private void DismissReturn()
        {

            Patron = null;
            BooksToReturn.Clear();
            AppState.CanShiftView = true;

        }

        // Hjælpefunktion til gennemføre afleveringen
        private async void CompleteReturn()
        {
            try
            {
                bool error = false;

                foreach (BookDTO b in BooksToReturn)
                {
                    bool success = await AppState.Repo.DeletePatronBookAsync(Patron.Id, b.Id.Value);

                    if (!success)
                    {
                        error = true;
                    }
                }

                if (error)
                {
                    MessageBox.Show("En eller flere bøger kunne ikke afleveres.");
                }
                else
                {
                
                    int total = BooksToReturn.Count;
                    MessageBox.Show(((total == 1) ? "En bog " : total + " bøger") + " blev afleveret.");
                }

                Patron = null;
                BooksToReturn.Clear();
                AppState.CanShiftView = true;

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
