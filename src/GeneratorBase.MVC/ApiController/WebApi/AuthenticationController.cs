using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AttributeRouting.Web.Http;
using GeneratorBase.MVC.Models;
using GeneratorBase.MVC.Controllers;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System;
using System.Web.Http.OData;
using Microsoft.AspNet.Identity;
namespace GeneratorBase.MVC.ApiControllers
{
   // [AuthorizationRequired]
    public class AuthenticationController : ApiController
    {
        public HttpResponseMessage AuthenticateUser(string UserName, string Password)
        {
            ApplicationDbContext UserDb = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var ssoUsers = UserDb.Users.Where(p => p.UserName == UserName).ToList();
            if (ssoUsers != null && ssoUsers.Count > 0)
            {
                var user = ssoUsers[0];
                var roles = user.Roles;
                PasswordHasher ph = new PasswordHasher();
                string hashedpwd = ph.HashPassword(Password);
                var result = ph.VerifyHashedPassword(user.PasswordHash, Password);
                if (result.ToString() == "Success")
                {
                    var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == user.Id);
                    if (obj != null)
                        return Request.CreateResponse(HttpStatusCode.OK, obj.T_AuthToken);
                    else
                    {
                        var token = GenerateToken.GetToken(user.Id);
                        return Request.CreateResponse(HttpStatusCode.OK, token.T_AuthToken);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Password !");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Username/Password");
        }

        public HttpResponseMessage GetTokenByUserId(string id)
        {
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            else
            {
                var token = GenerateToken.GetToken(id);
                return Request.CreateResponse(HttpStatusCode.OK, token);
            }
        }
        public HttpResponseMessage GetTokenByUserName(string UserName)
        {
            ApplicationContext db = new ApplicationContext(new SystemUser());
            ApplicationDbContext userdb = new ApplicationDbContext();
            var userinfo = userdb.Users.FirstOrDefault(p => p.UserName == UserName);
            var obj = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userinfo.Id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            else
            {
                var token = GenerateToken.GetToken(userinfo.Id);
                return Request.CreateResponse(HttpStatusCode.OK, token);
            }
        }
    }
}
