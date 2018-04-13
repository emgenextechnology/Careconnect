using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GM.Identity
{
    public class GMUserStore : UserStore<GMUser, GMRole, int,
        GMUserLogin, GMUserRole, GMUserClaim>
    {
        
        public GMUserStore(GMDbContext context)
            : base(context)
        {
        }
    }
}
