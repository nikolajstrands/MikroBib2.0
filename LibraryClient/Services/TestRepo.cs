using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.Models;
using LibraryDTOs;

namespace LibraryClient.Services
{
    public class TestRepo : IRepo
    { 

        private List<BookDTO> books = new List<BookDTO>(new BookDTO[]
            {

                
                new BookDTO()
                {
                    Id = 1,
                    Title = "Pride and Prejudice",
                    AuthorFirstName = "Jane",
                    AuthorLastName = "Austen",
                    NumberOfPages = 360,
                    Publisher = "New York : St. Martin's Griffin",
                    YearPublished = 2017,
                    PatronId = 1,
                    IsBorrowed = true,
                    DueDate = DateTime.Now.AddDays(30),
                },
                new BookDTO()
                {
                    Id = 2,
                    Title = "Northanger Abbey",
                    AuthorFirstName = "Jane",
                    AuthorLastName = "Austen",
                    NumberOfPages = 370,
                    Publisher = "London : Collins",
                    YearPublished = 1990,
                    PatronId = 1,
                    IsBorrowed = true,
                    DueDate = DateTime.Now.AddDays(30),
                },
                new BookDTO()
                {
                    Id = 3,
                    Title = "Krig og fred",
                    AuthorFirstName = "Lev",
                    AuthorLastName = "Tolstoj",
                    NumberOfPages = 1405,
                    Publisher = "København : Lademann",
                    YearPublished = 1973,
                    PatronId = null,
                    IsBorrowed = false,
                    DueDate = null,
                }
              
            });

        private List<UserDTO> users = new List<UserDTO>(new UserDTO[]
            {
                new UserDTO()
                {
                   Id = "1",
                   UserName = "testadmin",
                   Roles = new ObservableCollection<string>(new string[] { "Desk", "Administrator", "Librarian" }),
             
                },
                new UserDTO()
                {
                   Id = "2",
                   UserName = "testbibliotekar",
                   Roles = new ObservableCollection<string>(new string[] { "Librarian" }),
                }
            });

        private List<PatronDTO> patrons = new List<PatronDTO>(
            new PatronDTO[]{
                new PatronDTO() {
                    Id = 1,
                    FirstName = "Nikolaj",
                    LastName = "Strands"
                }
            });
            
        // Metoder vedr. brugere

        public async Task<UserDTO> LoginAsync(string username, SecureString password)
        {
            //throw new TimeoutException();
            return users[0];
        }


        public async Task<bool> UpdateUserAsync(UserDTO user)
        {
            return true;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {

            var user = users.FirstOrDefault(u => u.Id == id);
            Console.WriteLine(user.UserName);
            users.Remove(user);

            return true;
        }

        public async Task<UserDTO> GetUserAsync(string username)
        {
            return users[0];
        }

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            return users;
        }

        // Metoder til CRUD for bøger
        public async Task<List<BookDTO>> GetBooksAsync(BookQuery searchQuery)
        {        
            return books;
           
        }

        public async Task<BookDTO> GetBookAsync(int bookId)
        {
            return books[0];
        }

        public async Task<bool> AddBookAsync(BookDTO book)
        {
            books.Add(book);
            return true;
        }

        public async Task<bool> UpdateBookAsync(BookDTO book)
        {
            return true;
        }


        public async Task<bool> DeleteBookAsync(int id)
        {
            return true;
        }

        // Metoder til CRUD for lånere
        public async Task<PatronDTO> GetPatronAsync(int patronId)
        {
            return patrons[0];
        }

        public async Task<bool> AddPatronAsync(PatronDTO patron)
        {
            patrons.Add(patron);
            return true;
        }


        // Metoder til interagere med låners bøger
        public async Task<List<BookDTO>> GetPatronBooksAsync(int patronId)
        {
            return books;
        }

        public async Task<bool> AddPatronBookAsync(int patronId, BookDTO book)
        {
            return true;
        }

        public async Task<bool> DeletePatronBookAsync(int patronId, int bookId)
        {
            return true;
        }
    }

}
