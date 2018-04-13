using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace GM.Identity
{

    public class GMUserRole : IdentityUserRole<int> { }
    public class GMUserClaim : IdentityUserClaim<int> { }
    public class GMUserLogin : IdentityUserLogin<int> { }
        
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    

}