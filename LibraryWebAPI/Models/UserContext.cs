using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibraryWebAPI.Models;

namespace LibraryWebAPI.Models
{
    public class UserContext : IdentityDbContext<ApplicationUser>
    {
        // Her defineres database-kontekst for brugere vha. af den særlige IdentityDbContext
        public UserContext()
            : base("LibraryWebAPIUserContext", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static UserContext Create()
        {
            return new UserContext();
        }

    }
}