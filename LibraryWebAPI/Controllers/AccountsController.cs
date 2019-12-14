using LibraryWebAPI.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LibraryDTOs;
using System.Collections.ObjectModel;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;

namespace LibraryWebAPI.Controllers
{
    // Controller-klasser der håndterer forespørglser vedr. systembrugere

    [RoutePrefix("api/users")]
    public class AccountsController : ApiController
    {
        // Der kræves en UserManager og en RoleManager for at besvare forespørgsler vedr. brugere og roller.
        private ApplicationUserManager appUserManager = null;
        private ApplicationRoleManager appRoleManager = null;

        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return appUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }


        // GET: api/users (hent alle brugere)
        [Authorize(Roles= "Administrator")]
        [HttpGet, Route("")]
        public IHttpActionResult GetUsers()
        {
            try
            {
                var users = AppUserManager.Users.ToList().Select(u =>
                new UserDTO()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    JoinDate = u.JoinDate.Value,
                    LastLogin = u.LastLogin.Value,
                    Roles = new ObservableCollection<string>(AppUserManager.GetRolesAsync(u.Id).Result),
                }
            );

                return Ok(users);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
       
        }

        // GET: api/users/admin (hent bruger ud fra brugernavn)
        [Authorize(Roles = "Administrator")]
        [HttpGet, Route("{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            try
            {
                var user = await AppUserManager.FindByNameAsync(username);

                if (user != null)
                {
                    var userDTO = new UserDTO()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        JoinDate = user.JoinDate.Value,
                        LastLogin = user.LastLogin.Value,
                        Roles = new ObservableCollection<string>(AppUserManager.GetRolesAsync(user.Id).Result),
                    };

                    return Ok(userDTO);
                }

                return NotFound();
            }           
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }
        
        // GET: api/users/me (hent den pt. autoriserede bruger)
        [Authorize]
        [HttpGet, Route("me")]
        public async Task<IHttpActionResult> GetOwnUserInfo()
        {
            try
            {
                var userName = User.Identity.GetUserName();

                var user = await AppUserManager.FindByNameAsync(userName);

                if (user != null)
                {
                    var userDTO = new UserDTO()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        JoinDate = user.JoinDate.Value,
                        LastLogin = user.LastLogin.Value,
                        Roles = new ObservableCollection<string>(AppUserManager.GetRolesAsync(user.Id).Result),
                    };


                    return Ok(userDTO);
                }

                return NotFound();

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
       
        }
       
        // PUT: api/users/xxxxxx...xxx/roles (ret en brugers roller. Alle roller fjernes og de nye tilføjes.)
        [Authorize(Roles = "Administrator")]
        [HttpPut, Route("{id:guid}/roles")]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            try
            {
                // Find brugeren
                var appUser = await AppUserManager.FindByIdAsync(id);

                // Hvis brugere ikke findes, returneres Not Found
                if (appUser == null)
                {
                    return NotFound();
                }

                // Hent alle brugerens roller
                var currentRoles = await AppUserManager.GetRolesAsync(appUser.Id);

                // Find eventuelt ikke-eksisterende roller i det tilsendte (de tilsendte undtagen dem der findes i systemet)
                var rolesNotExists = rolesToAssign.Except(AppRoleManager.Roles.Select(x => x.Name)).ToArray();

                // Hvis der er roller der ikke findes i system returneres Bad Request
                if (rolesNotExists.Count() > 0)
                {

                    ModelState.AddModelError("", string.Format("Rollerne '{0}' eksisterer ikke i systemet", string.Join(",", rolesNotExists)));
                    return BadRequest(ModelState);
                }

                // Fjern brugerens roller
                IdentityResult removeResult = await AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

                // Hvis de ikke kan fjernes, returnes Bad Reqeust
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Roller kunne ikke fjernes");
                    return BadRequest(ModelState);
                }

                // Tilføj de nye roller
                IdentityResult addResult = await AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

                // Hvis de ikke kan tilføjes, returneres Bad Request
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Roller kunne ikke tilføjes");
                    return BadRequest(ModelState);
                }

                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

    }
}