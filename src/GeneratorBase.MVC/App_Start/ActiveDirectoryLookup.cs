using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace GeneratorBase.MVC
{
    /// <summary>
    /// Finds names for Active Directory domain objects given an SID.
    /// </summary>
    public class ActiveDirectoryLookup
    {
        private WindowsIdentity identity;

        /// <summary>
        /// Initializes an instance of the SidLookup class.
        /// </summary>
        /// <param name="principal">The HttpContext.User object in a Windows Authentication environment.</param>
        public ActiveDirectoryLookup(WindowsPrincipal principal)
            : this((WindowsIdentity)principal.Identity) { }

        /// <summary>
        /// Initializes an instance of the SidLookup class.
        /// </summary>
        /// <param name="identity">The HttpContext.User.Identity object in a Windows Authentication environment.</param>
        public ActiveDirectoryLookup(WindowsIdentity identity)
        {
            this.identity = identity;
            if (!this.identity.Claims.Any(c => c.Type.Equals(this.identity.RoleClaimType, StringComparison.Ordinal)))
                throw new InvalidOperationException("No roles available for user: " + identity.Name);

        }

        public string GetFirstName()
        {
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(ctx, IdentityType.Sid, this.identity.Owner.Value);
                return user.GivenName;
            }
        }

        public string GetLastName()
        {
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(ctx, IdentityType.Sid, this.identity.Owner.Value);
                return user.Surname;
            }
        }

        public IEnumerable<string> GetRoles()
        {
            // Would also work with the following, but I'm not sure if either is faster.
            // var groups = identity.Groups.Select(g => ((NTAccount)g.Translate(typeof(NTAccount))).Value);
            var names = ActiveDirectoryLookup.GetNames(this.identity.RoleClaimType, this.identity.Claims);
                       
            //Radio button options to login user in multiple roles
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies["CurrentRole"].Value;
            if (!string.IsNullOrEmpty(UserCurrentRole))
            {
                string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p=>p.Trim()).ToArray();
                names = names.Where(u => splittedarray.Contains(u.Trim()));
            }


            return names;
        }

        private static IEnumerable<string> GetNames(string type, IEnumerable<System.Security.Claims.Claim> claims)
        {
            var matching = claims.Where(c => c.Type.Equals(type, StringComparison.Ordinal));
            return matching.Select(c => GetNameBySid(c.Value)).Where(n => !String.IsNullOrWhiteSpace(n));
        }

        public static string GetNameBySid(string sid)
        {
            var cache = HttpRuntime.Cache;
            var name = (string)cache["sid:" + sid];
            if (name == null)
            {
                name = GetNameBySidFromFile(sid);

                if (name == null)
                {
                    name = GetNameBySidFromDomain(sid);
                }

                var cacheSpan = Cache.NoSlidingExpiration;
                if (name == null)
                {
                    name = "";
                    cacheSpan = TimeSpan.FromMinutes(15);
                }

                // SIDs are unique and are never reused. (Caching is OK.)
                cache.Add("sid:" + sid, name,
                          null,
                          Cache.NoAbsoluteExpiration,
                          cacheSpan,
                          CacheItemPriority.Normal,
                          null);
            }

            return name;
        }

        private static string GetNameBySidFromFile(string sid)
        {
            var path = HttpContext.Current.Server.MapPath("~/roles.csv");
            var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path);
            parser.Delimiters = new string[] { "," };
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields[1] == sid)
                {
                    return fields[0];
                }
            }

            return null;
        }

        private static string GetNameBySidFromDomain(string sid)
        {
            try
            {
                if (String.IsNullOrEmpty(sid))
                    return null;
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"]; //GeneratorBase.MVC.Models.CommonFunction.Instance.DomainName(); 
                using (var ctx = new PrincipalContext(ContextType.Domain, DomainName))
                {
                    var principal = Principal.FindByIdentity(ctx, IdentityType.Sid, sid);

                    return principal == null ? default(string) : principal.Name;
                }
            }
            // to catch random DirectoryServicesCOMExceptions
            catch (System.Runtime.InteropServices.COMException)
            {
                return null;
            }
        }

        private static string GetSidByNameFromDomain(string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                    return null;
                var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"]; //GeneratorBase.MVC.Models.CommonFunction.Instance.DomainName(); 
                using (var ctx = new PrincipalContext(ContextType.Domain, DomainName))
                {
                    var principal = Principal.FindByIdentity(ctx, IdentityType.Name, name);

                    return principal == null ? default(string) : principal.Sid.Value;
                }
            }
            // to catch random DirectoryServicesCOMExceptions
            catch (System.Runtime.InteropServices.COMException)
            {
                return null;
            }
        }

    }
}