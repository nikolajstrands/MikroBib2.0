using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LibraryClient.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using LibraryClient.Services;
using LibraryDTOs;

namespace LibraryClient.ViewModels
{
 
    public class MainWindowViewModel : NavigationViewModel
    {
        public ObservableObject DetailsPageViewModel { get; set; }

        public MainWindowViewModel()
        {
            // Sæt dataservice repository
            IRepo repo = new LibraryRepo();

            // Sæt repo til test
            //IRepo repo = new TestRepo();

            // Sæt applikationstilstand
            AppState = new AppState(repo);
            AppState.User = new UserDTO();
                
            // Opret Hjem-kategorien
            HomeViewModel home = new HomeViewModel(AppState);
            home.Name = "Forside";        

            // Opret Bøger-kategorien
            NavigationViewModel books = new NavigationViewModel(AppState);
            books.Name = "Bøger";
            books.AuthorizedUserRoles.AddRange(new string[] { "Desk", "Librarian", "Administrator" });        
            books.PageViewModels.Add(new SearchBookViewModel(AppState, books.ChangePageCommand));
            books.PageViewModels.Add(new BookViewModel(AppState, null, books.ChangePageCommand));
            books.PageViewModels.Add(new EditBookViewModel(AppState, null));
            books.PageViewModels.Add(new AddBookViewModel(AppState));
            books.CurrentPageViewModel = books.PageViewModels[0];

            // Opret Låner-kategorien
            NavigationViewModel patrons = new NavigationViewModel(AppState);
            patrons.Name = "Lånere";
            patrons.AuthorizedUserRoles.AddRange(new string[] { "Desk", "Librarian", "Administrator" });
            patrons.PageViewModels.Add(new PatronStatusViewModel(AppState));
            patrons.PageViewModels.Add(new AddPatronViewModel(AppState));         
            patrons.CurrentPageViewModel = patrons.PageViewModels[0];

            // Opret Cirkulation-kategorien
            NavigationViewModel circulation = new NavigationViewModel(AppState);
            circulation.Name = "Cirkulation";
            circulation.AuthorizedUserRoles.AddRange(new string[] { "Desk", "Librarian", "Administrator" });
            circulation.PageViewModels.Add(new BorrowViewModel(AppState));
            circulation.PageViewModels.Add(new ReturnViewModel(AppState ));
            circulation.CurrentPageViewModel  = circulation.PageViewModels[0];

            // Opret Administration-kategorien
            NavigationViewModel administration = new NavigationViewModel(AppState);
            administration.Name = "Administration";
            administration.AuthorizedUserRoles.AddRange(new string[] { "Administrator" });
            administration.AuthorizedUserRoles.Add( "Administrator" );

            administration.PageViewModels.Add(new ShowUsersViewModel(AppState, administration.ChangePageCommand));
            administration.PageViewModels.Add(new EditUserViewModel(AppState, null));
            administration.CurrentPageViewModel = administration.PageViewModels[0];

            PageViewModels.Add(home);
            PageViewModels.Add(books);
            PageViewModels.Add(patrons);            
            PageViewModels.Add(circulation);
            PageViewModels.Add(administration);
            
            // Sæt startside
            CurrentPageViewModel = PageViewModels[0];

        }

    }
}
