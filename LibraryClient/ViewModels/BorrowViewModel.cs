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
    public class BorrowViewModel : BaseViewModel
    {

        // Id for låner
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

        // Id til fremsøgning af bøger til udlån
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

        // Låner-objekt for låner
        private PatronDTO patron =  null;
        public PatronDTO Patron
        {
            get => patron;
            set
            {
                patron = value;
                OnPropertyChanged();
            }
        }

        // Lister med bøger til udlån
        private ObservableCollection<BookDTO> booksToBorrow = new ObservableCollection<BookDTO>();
        public ObservableCollection<BookDTO> BooksToBorrow
        {
            get => booksToBorrow;
            set
            {
                booksToBorrow = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public BorrowViewModel(AppState appState)
           : base(appState)
        {
            Name = "Udlån";
            AuthorizedUserRoles.AddRange(new string[] { "Desk" });
        }

        // Kommando til hente bruger
        private ICommand getPatronCommand;
        public ICommand GetPatronCommand
        {
            get
            {
                if (getPatronCommand == null)
                {
                    getPatronCommand = new RelayCommand(
                        param => GetPatron((int.Parse((string)param))),
                        param => param != null && (string)param != "" && Patron == null && int.TryParse((string)param, out int result)
                        );
                }
                return getPatronCommand;
            }
        }

        // Kommando til at tilføje bog til udlånslisten
        private ICommand addBookCommand;
        public ICommand AddBookCommand
        {
            get
            {
                if (addBookCommand == null)
                {
                    addBookCommand = new RelayCommand(
                        param => AddBook(param),
                        param => Patron != null && BookId != null && int.TryParse((string)param, out int result)
                        );
                }
                return addBookCommand;
            }
        }

        // Kommanod til at fortryde igangværende udlån
        private ICommand dismissBorrowCommand;
        public ICommand DismissBorrowCommand
        {
            get
            {
                if (dismissBorrowCommand == null)
                {
                    dismissBorrowCommand = new RelayCommand(
                        param => DismissBorrow(),
                        param => Patron != null
                        );
                }
                return dismissBorrowCommand;
            }
        }

        // Kommando til at afslut (og udføre) udlånet
        private ICommand completeBorrowCommand;
        public ICommand CompleteBorrowCommand
        {
            get
            {
                if (completeBorrowCommand == null)
                {
                    completeBorrowCommand = new RelayCommand(
                        param => CompleteBorrow(),
                        param => Patron != null && BooksToBorrow.Count > 0
                        );
                }
                return completeBorrowCommand;
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
                    PatronId = null;
                    AppState.CanShiftView = false;

                }
                else
                {
                    MessageBox.Show("Ingen låner fundet med dette id.");
                    PatronId = null;
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

        // Hjælpefunktion der tilføjer bog til udlånslisten
        private async void AddBook(object bookId)
        {
            try
            {
                int id = int.Parse((string)bookId);

                BookDTO bookToBorrow = await AppState.Repo.GetBookAsync(id);

                if (bookToBorrow != null)
                {
                    if (bookToBorrow.IsBorrowed)
                    {
                        MessageBox.Show("Bogen er allerede udlånt.");

                    } else
                    {
                        BooksToBorrow.Add(bookToBorrow);                  
                    }
                    BookId = null;
                } else
                {
                    MessageBox.Show("Den findes ingen bog med dette id.");
                    BookId = null;
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

         
        // Hjælpefunktion der fortryder udlånet
        private void DismissBorrow()
        {
            Patron = null;
            BooksToBorrow.Clear();

            AppState.CanShiftView = true;

        }

        // Hjælpefunktion der gennemfører udlånet
        private async void CompleteBorrow()
        {
            try
            {
                // Der holdes styr på om der er fejl under vejs
                bool error = false;

                foreach (BookDTO b in BooksToBorrow)
                {
                    bool success = await AppState.Repo.AddPatronBookAsync(Patron.Id, b);

                    if (!success)
                    {
                        error = true;
                    }
                }

                Patron = null;
                BooksToBorrow.Clear();
                AppState.CanShiftView = true;

                if (error)
                {
                    MessageBox.Show("En eller flere bøger kunne ikke udlånes.");

                } else
                {
                    int total = BooksToBorrow.Count;
                    MessageBox.Show(((total == 1) ? "En bog " : total + " bøger") + " blev udlånt.");
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
