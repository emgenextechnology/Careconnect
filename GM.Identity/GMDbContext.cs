using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GM.Identity
{

    public class GMDbContext : IdentityDbContext<GMUser, GMRole, int, GMUserLogin, GMUserRole, GMUserClaim>
    {
        public GMDbContext()
            : base("DefaultConnection")
        {
        }

        static GMDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            // Database.SetInitializer<GMDbContext>(new ApplicationDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Database.SetInitializer<GMDbContext>(null);
            modelBuilder.Entity<GMUser>().ToTable("Users");
            modelBuilder.Entity<GMRole>().ToTable("Roles");
            modelBuilder.Entity<GMUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<GMUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<GMUserClaim>().ToTable("UserClaims");
        }

        public static GMDbContext Create()
        {
            return new GMDbContext();
        }
    }
}
