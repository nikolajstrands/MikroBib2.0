using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.Models;
using LibraryDTOs;


namespace LibraryClient.Services
{
    public interface IRepo
    {
        // Dette interface definerer den funktionalitet klienten har brug for fra en dataservice.
        // Det kan implementeres med et test-repository, direkte fra en database, fra et web API el.lign.

        /******************************************** Metoder vedr. brugere **************************************************/

        // Log en bruger ind
        // Returnerer null, hvis brugeren ikke findes. Efter login, kan nedenstående metoder bruges (i det omfang brugeren har rettigheder)
        Task<UserDTO> LoginAsync(string username, SecureString password);

       
        // Hent en bruger efter brugernav
        // Returnerer null, hvis bruger ikke findes
        Task<UserDTO> GetUserAsync(string username);

        // Hent alle brugere
        Task<List<UserDTO>> GetUsersAsync();

        // Opdater en bruger
        // Returnerer true ved succes, false ved fiasko
        Task<bool> UpdateUserAsync(UserDTO user);
        
        /********************************************* Metoder vedr. bøger ***************************************************/

        // Hent bøger ud fra søge-query
        // Returnerer null ved fejl
        Task<List<BookDTO>> GetBooksAsync(BookQuery searchQuery);

        // Hent en enkelt bog ud fra id
        // Returnerer null ved fejl
        Task<BookDTO> GetBookAsync(int bookId);

        // Tilføj en bog
        // Returner true ved succes, false ved fiasko
        Task<bool> AddBookAsync(BookDTO book);

        // Opdater bog
        // Returnerer true ved succes, false ved fiasko
        Task<bool> UpdateBookAsync(BookDTO book);

        // Slet bog efter id
        // Returnerer true ved succes, false ved fiasko
        Task<bool> DeleteBookAsync(int id);
    
        /******************************************** Metoder vedr. lånere **************************************************/

        // Hent en enkelt låner efter id
        // Returnerer null ved fejl
        Task<PatronDTO> GetPatronAsync(int patronId);

        // Tilføj en låner
        // Returnerer true ved succes, false ved fiasko
        Task<bool> AddPatronAsync(PatronDTO patron);

        // Hent en låners udlånte bøger
        // Returnerer null ved fejl
        Task<List<BookDTO>> GetPatronBooksAsync(int patronId);

        // Udlån en bog til låner
        // Returnerer true ved succes, false ved fiasko
        Task<bool> AddPatronBookAsync(int patronId, BookDTO book);

        // Aflever en bog fra låner
        // Returnerer true ved succes, false ved fiasko
        Task<bool> DeletePatronBookAsync(int patronId, int bookId);
    }

}
