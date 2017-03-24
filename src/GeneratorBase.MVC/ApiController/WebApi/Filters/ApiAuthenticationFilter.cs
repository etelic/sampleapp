using System.Threading;
using System.Web.Http.Controllers;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using GeneratorBase.MVC.Controllers;

namespace GeneratorBase.MVC.Controllers
{
    /// <summary>
    /// Custom Authentication Filter Extending basic Authentication
    /// </summary>
    /// 
    [Authorize]
    public class ApiAuthenticationFilter : GenericAuthenticationFilter
    {
        /// <summary>
        /// Default Authentication Constructor
        /// </summary>
        public ApiAuthenticationFilter()
        {
        }

        /// <summary>
        /// AuthenticationFilter constructor with isActive parameter
        /// </summary>
        /// <param name="isActive"></param>
        public ApiAuthenticationFilter(bool isActive)
            : base(isActive)
        {
        }

        /// <summary>
        /// Protected overriden method for authorizing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            UserServicesController provider = new UserServicesController();

            if (provider != null)
            {
                var userId = provider.Authenticate(username, password);
                if (!string.IsNullOrEmpty(userId))
                {
                    var basicAuthenticationIdentity = Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                    if (basicAuthenticationIdentity != null)
                        basicAuthenticationIdentity.UserId = userId;
                    return true;
                }
            }
            return false;
        }
    }
}