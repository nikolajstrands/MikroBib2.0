using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace LibraryWebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Denne klasse definerer en applikationsbruger. Der arves fra IdentityUser hvor id er defineret som et GUID      
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        // Brugerens oprettelsestidspunkt
        public Nullable<DateTime> JoinDate { get; set; }

        // Brugerens sidste login
        public Nullable<DateTime> LastLogin { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }

    }
}