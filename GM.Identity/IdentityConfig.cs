using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using GM.Identity.Config;


namespace GM.Identity.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    // *** PASS IN TYPE ARGUMENT TO BASE CLASS:
    public class GMUserManager : UserManager<GMUser, int>
    {
        // *** ADD INT TYPE ARGUMENT TO CONSTRUCTOR CALL:
        public GMUserManager(IUserStore<GMUser, int> store)
            : base(store)
        {
        }

        //public override async Task<IdentityResult> AddToRoleAsync(int userId, string roleId)
        //{
        //    //var user = await FindByIdAsync(userId);

        //    GMDbContext context = new GMDbContext();
        //    var user = context.Users.Find(userId);

        //    var existingRole = user.Roles;
        //    var count = user.Roles.Count();

        //    for (var i = 0; i < count; i++)// role in existingRole)
        //    {
        //        var result = user.Roles.Remove(existingRole.ElementAt(i));
        //        context.SaveChanges();
        //    }


        //    user.Roles.Add(new GMUserRole { RoleId = int.Parse(roleId), UserId = userId });
        //    context.SaveChanges();

        //    return new IdentityResult();
        //}

        public override async Task<IdentityResult> AddToRoleAsync(int userId, string roleId)
        {
            GMDbContext context = new GMDbContext();
            var user = context.Users.Find(userId);
            user.Roles.Add(new GMUserRole { RoleId = int.Parse(roleId), UserId = userId });
            context.SaveChanges();

            return new IdentityResult();
        }

        public override async Task<IdentityResult> RemoveFromRoleAsync(int userId, string roleId)
        {
            //var user = await FindByIdAsync(userId);

            GMDbContext context = new GMDbContext();
            var user = context.Users.Find(userId);

            var existingRole = user.Roles;
            var count = user.Roles.Count();

            for (var i = 0; i < count; i++)
            {
                var result = user.Roles.Remove(existingRole.ElementAt(0));
            };

            context.SaveChanges();
            return new IdentityResult();
        }

        //public override async Task<IdentityResult> AddToRoleAsync(int userId, string roles)
        //{
        //    //var user = await FindByIdAsync(userId);

        //    GMDbContext context = new GMDbContext();
        //    var user = context.Users.Find(userId);

        //    var existingRole = user.Roles;
        //    var count = user.Roles.Count();

        //    for (var i = 0; i < count; i++)// role in existingRole)
        //    {
        //        var result = user.Roles.Remove(existingRole.ElementAt(i));
        //        context.SaveChanges();
        //    }
        //    if (!string.IsNullOrEmpty(roles))
        //    {
        //        int[] roleIds = roles.Split(',').Select(int.Parse).ToArray();
        //        foreach (var roleId in roleIds)
        //        {
        //            user.Roles.Add(new GMUserRole { RoleId = roleId, UserId = userId });
        //            context.SaveChanges();
        //        }
        //    }

        //    return new IdentityResult();
        //}

        public static GMUserManager Create(
            IdentityFactoryOptions<GMUserManager> options,
            IOwinContext context)
        {
            // *** PASS CUSTOM APPLICATION USER STORE AS CONSTRUCTOR ARGUMENT:
            var manager = new GMUserManager(
                new GMUserStore(context.Get<GMDbContext>()));

            // Configure validation logic for usernames

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.UserValidator = new UserValidator<GMUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 25;

            // Register two factor authentication providers. 
            // This application uses Phone and Emails as a step of receiving a 
            // code for verifying the user You can write your own provider and plug in here.

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.RegisterTwoFactorProvider("PhoneCode",
                new PhoneNumberTokenProvider<GMUser, int>
                {
                    MessageFormat = "Your security code is: {0}"
                });

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.RegisterTwoFactorProvider("EmailCode",
                new EmailTokenProvider<GMUser, int>
                {
                    Subject = "SecurityCode",
                    BodyFormat = "Your security code is {0}"
                });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<GMUser, int>(
                        dataProtectionProvider.Create("Careconnect Identity"));
            }
            return manager;
        }
    }

    // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO BASE:
    public class GMRoleManager : RoleManager<GMRole, int>
    {
        // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO CONSTRUCTOR:
        public GMRoleManager(IRoleStore<GMRole, int> roleStore)
            : base(roleStore)
        {

        }

        // PASS CUSTOM APPLICATION ROLE AS TYPE ARGUMENT:
        public static GMRoleManager Create(
            IdentityFactoryOptions<GMRoleManager> options, IOwinContext context)
        {
            return new GMRoleManager(
                new GMRoleStore(context.Get<GMDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var mail = new GMEmail();

            //if (ApplicationConfig.GetAppSettings("IsInDemo") == "true")
            //{
            //    mail.To.Add(ApplicationConfig.GetAppSettings("DemoEmailId"));
            //}
            //else
            //{
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            #region manage bcc for developers

            string _bcc = ConfigurationManager.AppSettings["DevNotification"];
            if (!string.IsNullOrEmpty(_bcc))
            {
                var arrayBcc = _bcc.Split(',');
                foreach (var email in arrayBcc)
                {
                    if (!string.IsNullOrEmpty(email))
                        mail.Bcc.Add(email);
                }
            }

            #endregion
            try
            {
                mail.SendMessage();
            }
            catch(Exception ex) { }
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer //: DropCreateDatabaseIfModelChanges<GMDbContext>
    {
        //protected override void Seed(GMDbContext context)
        //{
        //    InitializeIdentityForEF(context);
        //    base.Seed(context);
        //}

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        

        public static void InitializeIdentityForEF(GMDbContext db)
        {
            InitializeIdentityForEF();
        }

        public static void InitializeIdentityForEF()
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<GMUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<GMRoleManager>();
            const string name = "admin@emgenex.com";
            const string password = "Test!23";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new GMRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new GMUser { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    public class GMSignInManager : SignInManager<GMUser, int>
    {
        public GMSignInManager(GMUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(GMUser user)
        {
            return user.GenerateUserIdentityAsync((GMUserManager)UserManager);
        }

        public static GMSignInManager Create(IdentityFactoryOptions<GMSignInManager> options, IOwinContext context)
        {
            return new GMSignInManager(context.GetUserManager<GMUserManager>(), context.Authentication);
        }

        public async Task<SignInStatus> SignInAsync(string userName, string password, bool rememberMe)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null) return SignInStatus.Failure;

            if (await UserManager.IsLockedOutAsync(user.Id)) return SignInStatus.LockedOut;

            if (!await UserManager.CheckPasswordAsync(user, password))
            {
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }

                return SignInStatus.Failure;
            }

            //if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //{
            //    return SignInStatus.RequiresVerification;
            //}

            await base.SignInAsync(user, rememberMe, false);
            return SignInStatus.Success;
        }
    }
}