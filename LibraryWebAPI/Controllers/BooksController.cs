using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using LibraryWebAPI.Models;
using LibraryDTOs;

namespace LibraryWebAPI.Controllers
{
    // Controller-klasser der håndterer forespørglser vedr. bøger

    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        // Datakontekst
        private DataContext db = new DataContext();

        // GET: api/books (hent alle bøger)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("")]
        public IHttpActionResult GetBooks()
        {
            try
            {
                var books = db.Books.ToList().Select(b => 
                new BookDTO()
                    {
                        Id = b.Id,
                        Title = b.Title,
                        AuthorFirstName = b.AuthorFirstName,
                        AuthorLastName = b.AuthorLastName,
                        NumberOfPages = b.NumberOfPages,
                        Publisher = b.Publisher,
                        YearPublished = b.YearPublished,
                        PatronId = b.PatronId,
                        IsBorrowed = b.IsBorrowed,
                        DueDate = b.DueDate
                    }
                );
                
                return Ok(books);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        
        }

        // GET: api/books + query (hent bøger der matcher query)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("")]
        public IHttpActionResult GetBooks(string title, string author, int? id, bool? onlyOnShelf, string publisher, int? yearpublished)
        {
            try
            {
                // Hvis titel, forfatter og udgiver er null, sættes de til den tomme streng, så de ignoreres i søgningen
                if (title == null)
                    title = String.Empty;

                if (author == null)
                    author = String.Empty;

                if (publisher == null)
                    publisher = String.Empty;

                // Her laves søgningen, hvor der søges i både forfatterfor- og efternavn, og hvor der tages højde for at id, udgivelsesår og "på hylden" kan være null.
                var books = from b in db.Books
                            where b.Title.Contains(title) &&
                            (b.AuthorFirstName.Contains(author) || b.AuthorLastName.Contains(author)) &&
                            b.Publisher.Contains(publisher) &&
                            ((yearpublished == null) ? true : (yearpublished == b.YearPublished)) &&
                            ((id == null) ? true : (id == b.Id)) &&
                            ((onlyOnShelf == null || onlyOnShelf == false) ? true : b.IsBorrowed == false)
                            select new BookDTO()
                            {
                                Id = b.Id,
                                Title = b.Title,
                                AuthorFirstName = b.AuthorFirstName,
                                AuthorLastName = b.AuthorLastName,
                                NumberOfPages = b.NumberOfPages,
                                Publisher = b.Publisher,
                                YearPublished = b.YearPublished,
                                PatronId = b.PatronId,
                                IsBorrowed = b.IsBorrowed,
                                DueDate = b.DueDate
                            };

                return Ok(books.ToList<BookDTO>());

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
         
        }

        // GET: api/books/5 (hent en bog efter id)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("{id}", Name = "DefaultBookApi")]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            try
            {
                var book = await db.Books.Select(b =>
                new BookDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorFirstName = b.AuthorFirstName,
                    AuthorLastName = b.AuthorLastName,
                    NumberOfPages = b.NumberOfPages,
                    Publisher = b.Publisher,
                    YearPublished = b.YearPublished,
                    PatronId = b.PatronId,
                    IsBorrowed = b.IsBorrowed,
                    DueDate = b.DueDate
                }).SingleOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
           
        }

        // PUT: api/books/5 (opdater en bog efter id)
        [Authorize(Roles = "Librarian")]
        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != book.Id)
                {
                    return BadRequest();
                }

                db.Entry(book).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
           
        }

        // POST: api/books (opret en ny bog)
        [Authorize(Roles = "Librarian")]
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Books.Add(book);
                await db.SaveChangesAsync();

                var dto = new BookDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorFirstName = book.AuthorFirstName,
                    AuthorLastName = book.AuthorLastName,
                    NumberOfPages = book.NumberOfPages,
                    Publisher = book.Publisher,
                    YearPublished = book.YearPublished,
                    PatronId = book.PatronId,
                    IsBorrowed = book.IsBorrowed,
                    DueDate = book.DueDate
                };

                return CreatedAtRoute("DefaultBookApi", new { id = book.Id }, dto);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        // DELETE: api/Books/5 (slet bog efter id)
        [Authorize(Roles = "Librarian")]
        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            try
            {
                Book book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                db.Books.Remove(book);
                await db.SaveChangesAsync();

                return Ok(book);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        // Frigiv ressourcer
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Hjælpefunktion
        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}