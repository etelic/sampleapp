using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GeneratorBase.MVC.Models;

namespace GeneratorBase.MVC.Controllers
{
    public class UserServicesController : Controller
    {
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public string Authenticate(string userName, string password)
        {
            var Db = new AuthenticationDbContext();
            var ssoUsers = Db.Users.Where(p => p.UserName == userName).ToList();
            var user = ssoUsers[0];
            PasswordHasher ph = new PasswordHasher();
            string hashedpwd = ph.HashPassword(password);
            var result = ph.VerifyHashedPassword(user.PasswordHash, password);
            if (result.ToString() == "Success")
            {
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    return user.Id;
                }

            }
            return null;
        }
    }
}