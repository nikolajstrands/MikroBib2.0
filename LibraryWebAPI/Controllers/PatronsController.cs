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
    // Controller-klasser der håndterer forespørglser vedr. lånere

    [RoutePrefix("api/patrons")]
    public class PatronsController : ApiController
    {

        // Datakontekst
        private DataContext db = new DataContext();

        // GET: api/patrons (hent alle lånere)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("")]
        public IHttpActionResult GetPatrons()
        {
            try
            {
                var patrons = db.Patrons.ToList().Select(p =>
                new PatronDTO()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Address = p.Address
                });

                return Ok(patrons);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        
        }

        // GET: api/patrons/5 (hent låner efter id)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("{id}", Name = "DefaultPatronApi")]
        public async Task<IHttpActionResult> GetPatron(int id)
        {
            try
            {
                var patron = await db.Patrons.Select(p =>
                new PatronDTO()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Address = p.Address
                }).SingleOrDefaultAsync(p => p.Id == id);

                if (patron == null)
                {
                    return NotFound();
                }

                return Ok(patron);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
    
        }

        // PUT: api/patrons/5 (ret låner efter id)
        [Authorize(Roles = "Librarian")]
        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> PutPatron(int id, Patron patron)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != patron.Id)
                {
                    return BadRequest();
                }

                db.Entry(patron).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatronExists(id))
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

        // POST: api/patrons (opret låner)
        [Authorize(Roles = "Librarian")]
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> PostPatron(Patron patron)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Patrons.Add(patron);
                await db.SaveChangesAsync();

                var dto = new PatronDTO()
                {
                    Id = patron.Id,
                    FirstName = patron.FirstName,
                    LastName = patron.LastName,
                    Address = patron.Address
                };

                return CreatedAtRoute("DefaultPatronApi", new { id = patron.Id }, dto);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        // DELETE: api/patrons/5 (slet låner efter id)
        [Authorize(Roles = "Librarian")]
        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> DeletePatron(int id)
        {
            try
            {
                Patron patron = await db.Patrons.FindAsync(id);
                if (patron == null)
                {
                    return NotFound();
                }

                db.Patrons.Remove(patron);
                await db.SaveChangesAsync();

                return Ok(patron);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
         
        }



        // GET: api/patrons/{id}/books (hent låners udlånte bøger)
        [Authorize(Roles = "Desk")]
        [HttpGet, Route("{id}/books")]
        public IHttpActionResult GetPatronBooks(int id)
        {
            try
            {
                var books = from b in db.Books
                            where b.Patron.Id == id
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

        // POST: api/patrons/{id}/books (tilføj bog til brugers liste = udlån)
        [Authorize(Roles = "Desk")]
        [HttpPost, Route("{id}/books")]
        public async Task<IHttpActionResult> PostPatronBook(int id, Book book)
        {
            try
            {
                // Find låner
                Patron patron = await db.Patrons.FindAsync(id);

                // Hvis låner ikke kan findes, returner Not Found
                if (patron == null)
                {
                    return NotFound();
                }

                // Hvis bog-objektet ikke matcher modellen, returner Bad Request
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Find bogen i databasen
                book = await db.Books.FindAsync(book.Id);

                // Hvis bogen ikke findes, returner Not Found
                if (book == null)
                {
                    return NotFound();
                }

                // Hvis bogen allerede er udlånt, returner Bad Request
                if (book.IsBorrowed)
                {
                    return BadRequest("Bogen er allerede udlånt");
                }

                // Hvis alt går godt:

                // Tilføj til låners bogliste
                patron.Books.Add(book);

                // Ret udlånsstatus på bogen
                book.IsBorrowed = true;

                // Sæt afleveringsdato (+ dage) på bogen.
                book.DueDate = DateTime.Now + new TimeSpan(30, 0, 0, 0);

                // Gem ændringer
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

        // DELETE: api/patrons/{patronId}/books/{bookId} (slet bog på låners udlånsliste = aflevering)
        [HttpDelete, Route("{patronId}/books/{bookId}")]
        public async Task<IHttpActionResult> DeletePatronBook(int patronId, int bookId)
        {
            try
            {
                // Findes låner?
                Patron patron = await db.Patrons.FindAsync(patronId);
                if (patron == null)
                {
                    return NotFound();
                }

                // Findes bogen?
                Book book = await db.Books.FindAsync(bookId);
                if (book == null)
                {
                    return NotFound();
                }

                // Har låner rent faktisk lånt bogen?
                if (!patron.Books.Contains(book))
                {
                    return BadRequest("Låner har ikke lånet bogen");
                }


                // Fjern fra låners liste
                patron.Books.Remove(book);

                // Sæt udlånstatus på bogen
                book.IsBorrowed = false;

                // Fjern afleveringdato
                book.DueDate = null;

                // Gem ændringer til databasen
                await db.SaveChangesAsync();

                return Ok();

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
        private bool PatronExists(int id)
        {
            return db.Patrons.Count(e => e.Id == id) > 0;
        }
    }
}