using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GM.Identity
{
    public class GMUser : IdentityUser<int, GMUserLogin, GMUserRole, GMUserClaim>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; } 
        public string LastName { get; set; }
        public int? BusinessId { get; set; }
        public bool? IsActive { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<GMUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
