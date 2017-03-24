using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Configuration;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
    public class TokenServicesController : Controller
    {
        #region Public member methods.

        public ApplicationContext db { get; private set; } //removed static for race condition
        private ApplicationDbContext UserContext = new ApplicationDbContext();
        public new IUser User { get; private set; }//removed static for race condition
        /// <summary>
        ///  Function to generate unique token with expiry against the provided userId.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ApiToken GenerateToken(string userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime issuedOn = DateTime.Now;
            DateTime expiredOn = DateTime.Now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
            var tokendomain = new ApiToken
            {
                T_UsersID = userId,
                T_AuthToken = token,
                T_IssuedOn = issuedOn,
                T_ExpiresOn = expiredOn
            };
            db.ApiTokens.Add(tokendomain);
            db.SaveChanges();
            var tokenModel = new ApiToken
            {
                T_UsersID = userId,
                T_AuthToken = token,
                T_IssuedOn = issuedOn,
                T_ExpiresOn = expiredOn
            };

            return tokenModel;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool ValidateToken(string tokenId)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var token = db1.ApiTokens.Where(t => t.T_AuthToken == tokenId && t.T_ExpiresOn > DateTime.Now).FirstOrDefault();
            if (token != null && !(DateTime.Now > token.T_ExpiresOn))
            {
                token.T_ExpiresOn = token.T_ExpiresOn.AddSeconds(
                                              Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                db1.Entry(token).State = EntityState.Modified;
                db1.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId">true for successful delete</param>
        public bool Kill(string tokenId)
        {
            var token = db.ApiTokens.Where(t => t.T_AuthToken == tokenId && t.T_ExpiresOn > DateTime.Now).FirstOrDefault();
            db.Entry(token).State = EntityState.Deleted;
            db.ApiTokens.Remove(token);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(string userId)
        {
            var token = db.ApiTokens.Where(t => t.T_UsersID == userId).FirstOrDefault();
            db.Entry(token).State = EntityState.Deleted;
            db.ApiTokens.Remove(token);
            db.SaveChanges();
            return true;
        }

        #endregion
    }
}