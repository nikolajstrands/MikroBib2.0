using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.Models;
using System.Configuration;
using System.Runtime.InteropServices;
using LibraryDTOs;

namespace LibraryClient.Services
{
    public class LibraryRepo : IRepo
    {

        // Hjælpeklasse til at hente data ud fra autoriserings-svar
        public class LoginTokenResult
        {
           
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }

            [JsonProperty(PropertyName = "error")]
            public string Error { get; set; }

            [JsonProperty(PropertyName = "error_description")]
            public string ErrorDescription { get; set; }
      
        }

        // Hjælperfunktion til at fiske string ud af SecureString.
        // Herfra https://stackoverflow.com/questions/818704/how-to-convert-securestring-to-system-string/33679932#33679932
        static String SecureStringToString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        // Basis endpoint for API
        private string baseUrl;

        // HTTP-klient
        private HttpClient client;

        public LibraryRepo()
        {
            // Opret en HTTP-klient
            client = new HttpClient();

            // Hent basis-URL fra konfigurationsfil (http://localhost/port/) og tilføj til klient
            baseUrl = ConfigurationManager.AppSettings["baseURL"];       
            client.BaseAddress = new Uri(baseUrl);

            // Tilføj headers
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json"));


        }


        /****************************************************************************************************************
         *                                   Metoder vedrørende systembrugere                                           *
         * **************************************************************************************************************/

        // Log systembruger ind
        public async Task<UserDTO> LoginAsync(string username, SecureString securePassword)
        {   
            
            // Der sendes en forespørgsel til autentificerings-endpoint med henblik på at få en access token
            HttpResponseMessage response = await client.PostAsync("oauth/token",
                new StringContent(string.Format("grant_type=password&username={0}&password={1}", username,
                    SecureStringToString(securePassword)), Encoding.UTF8,
                    "application/x-www-form-urlencoded"));

            // Hvis login-oplysninger er korrekte svares der med statuskode 200
            if (response.IsSuccessStatusCode)
            {
                // Resultatet parses
                string resultJSON = await response.Content.ReadAsStringAsync();
                LoginTokenResult result = JsonConvert.DeserializeObject<LoginTokenResult>(resultJSON);

                // Den access token der er modtaget, sættes som autorisations-header på klienten.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                // Den autoriserede bruger kan nu hente sine egne oplysninger
                HttpResponseMessage response2 = await client.GetAsync("api/users/me");

                if (response2.IsSuccessStatusCode)
                {
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(await response2.Content.ReadAsStringAsync());
                    return user;
                }    
                   
            }
            return null;
        
        }

        // Hent én systembruger
        public async Task<UserDTO> GetUserAsync(string username)
        {
            string url = baseUrl + "api/users/" + username;

            UserDTO user = null;

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<UserDTO>();
            }
            return user;

        }


        // Hent alle systembrugere
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            HttpResponseMessage response = await client.GetAsync(baseUrl + "api/users");

            if (response.IsSuccessStatusCode)
            {           
                var users = JsonConvert.DeserializeObject<List<UserDTO>>(await response.Content.ReadAsStringAsync());

                return users;
            }

            return null;
        }
        
        // Opdater systembruger
        public async Task<bool> UpdateUserAsync(UserDTO user)
        {
            string json = JsonConvert.SerializeObject(user.Roles);
            string relativeURL = "api/users/" + user.Id + "/roles";

            HttpResponseMessage response = await client.PutAsync(relativeURL, new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;      
        }

        
        /****************************************************************************************************************
        *                                          Metoder vedrørende bøger                                             *
        * **************************************************************************************************************/

        // Find en enkelt bog
        public async Task<BookDTO> GetBookAsync(int bookId)
        {
            string url = baseUrl + "api/books/" + bookId;
            
            BookDTO book = null;

            HttpResponseMessage response = await client.GetAsync(url);
                  
            if (response.IsSuccessStatusCode)
            {
                book = await response.Content.ReadAsAsync<BookDTO>();
            }
            return book;

        }

        // Find bøger ud fra søgkriterier
        public async Task<List<BookDTO>> GetBooksAsync(BookQuery query)
        {

            string relativeURL = "api/books/?title=" + query.Title + "&author=" + query.Author
                + "&id=" + query.Id + "&onlyOnShelf=" + query.OnlyOnShelf + "&publisher=" + query.Publisher
                + "&yearpublished=" + query.YearPublished;
    
            HttpResponseMessage response = await client.GetAsync(relativeURL);

            if (response.IsSuccessStatusCode)
            {
                var items = JsonConvert.DeserializeObject<List<BookDTO>>(await response.Content.ReadAsStringAsync());

                return items;

            }
            return null;
        }


        // Opdater bog
        public async Task<bool> UpdateBookAsync(BookDTO book)
        {

            string json = JsonConvert.SerializeObject(book);
            string relativeURL = "/api/books/" + book.Id;
              
            HttpResponseMessage response = await client.PutAsync(relativeURL, new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {          
                return true;

            }
            return false;
       
        }

        // Tilføj bog
        public async Task<bool> AddBookAsync(BookDTO book)
        {  
            
            string json = JsonConvert.SerializeObject(book);
            string relativeURL = "/api/books/";

            HttpResponseMessage response = await client.PostAsync(relativeURL, new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
           
        }

        // Slet bog
        public async Task<bool> DeleteBookAsync(int id)
        {
           
            HttpResponseMessage response = await client.DeleteAsync("api/books/" + id);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
           
        }

        /****************************************************************************************************************
        *                                          Metoder vedrørende lånere                                            *
        * **************************************************************************************************************/

        // Hent låner
        public async Task<PatronDTO> GetPatronAsync(int patronId)
        {
            PatronDTO patron = null;

            HttpResponseMessage response = await client.GetAsync("api/patrons/" + patronId);

            if (response.IsSuccessStatusCode)
            {
                patron = JsonConvert.DeserializeObject<PatronDTO>(await response.Content.ReadAsStringAsync());
            
            }
            return patron;
        }


        // Tilføj en låner
        public async Task<bool> AddPatronAsync(PatronDTO patron)
        {

            string json = JsonConvert.SerializeObject(patron);
            string relativeURL = "/api/patrons/";

            HttpResponseMessage response = await client.PostAsync(relativeURL, new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
            
        }

        // Hent en låners bøger
        public async Task<List<BookDTO>> GetPatronBooksAsync(int patronId)
        {

            HttpResponseMessage response = await client.GetAsync("api/patrons/" + patronId + "/books");

            if (response.IsSuccessStatusCode)
            {
                var items = JsonConvert.DeserializeObject<List<BookDTO>>(await response.Content.ReadAsStringAsync());

                return items;

            }

            return null;
        }

        // Tilføj bog til låners udlånsliste
        public async Task<bool> AddPatronBookAsync(int patronId, BookDTO book)
        {
          
            string json = JsonConvert.SerializeObject(book);
            string relativeURL = "/api/patrons/" + patronId + "/books";

            HttpResponseMessage response = await client.PostAsync(relativeURL, new StringContent(json, Encoding.UTF8, "application/json"));
          
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
            
        }
   
        // Slet en bog fra en låners udlånsliste
        public async Task<bool> DeletePatronBookAsync(int patronId, int bookId)
        {
            string relativeURL = baseUrl + "api/patrons/" + patronId + "/books/" + bookId;
        
            HttpResponseMessage response = await client.DeleteAsync(relativeURL);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;

        }
    }
}
  