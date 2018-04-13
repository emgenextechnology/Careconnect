using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GM.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GM.Identity
{
    public class GMRoleStore : RoleStore<GMRole, int, GMUserRole>,
        IQueryableRoleStore<GMRole, int>,
        IRoleStore<GMRole, int>, IDisposable
    {
        public GMRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public GMRoleStore(DbContext context)
            : base(context)
        {
        }


    } 

}
