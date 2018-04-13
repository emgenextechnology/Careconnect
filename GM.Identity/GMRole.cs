using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GM.Identity
{
    public class GMRole : IdentityRole<int, GMUserRole>, IRole<int>
    {
        public string Description { get; set; }

        public GMRole() { }

        public GMRole(string name) { Name = name; }

        public GMRole(string name, string description)
            : this(name)
        {
            this.Description = description;
        }
    }
}
