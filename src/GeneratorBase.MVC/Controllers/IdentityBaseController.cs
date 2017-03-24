using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace GeneratorBase.MVC.Controllers
{
    [NoCache]
    public class IdentityBaseController : Controller
    {
        public ApplicationDbContext Identitydb { get; private set; } //removed static for race condition
        public new GeneratorBase.MVC.Models.IUser LogggedInUser { get; private set; }//removed static for race condition
        public UserManager<ApplicationUser> UserManager { get; private set; }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            LogggedInUser = base.User as GeneratorBase.MVC.Models.IUser;
            Identitydb = new ApplicationDbContext(base.User as GeneratorBase.MVC.Models.IUser);
            //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
            //var dataProtectionProvider = Startup.DataProtectionProvider;
            //this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext(LogggedInUser)));
            var dataProtectionProvider = Startup.DataProtectionProvider;
            this.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("AppName"));
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var appSettings = db.AppSettings;
            string applySecurityPolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
            UserManager.UserLockoutEnabledByDefault = true;
            if (applySecurityPolicy.ToLower() == "yes")
            {
                UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(Double.Parse(appSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                UserManager.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(appSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value);
                UserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = Convert.ToInt32(appSettings.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value),
                    RequireNonLetterOrDigit = appSettings.Where(p => p.Key == "PasswordRequireAlphaNumericCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireDigit = appSettings.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireLowercase = appSettings.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                    RequireUppercase = appSettings.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false,
                };
            }
            base.OnActionExecuting(filterContext);
        }
        
        
    }
   
}