using GeneratorBase.MVC.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
namespace GeneratorBase.MVC.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    /// 
    [Authorize]
    public class ApiHelpController : BaseController
    {
        //Remove constructors and existing Configuration property.

        //public HelpController()
        //    : this(GlobalConfiguration.Configuration)
        //{
        //}

        //public HelpController(HttpConfiguration config)
        //{
        //    Configuration = config;
        //}

        //public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// Add new Configuration Property
        /// </summary>
        protected static HttpConfiguration Configuration
        {
            get { return GlobalConfiguration.Configuration; }
        }

        public System.Web.Mvc.ActionResult Index()
        {
            ApplicationDbContext userdb = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var userinfo = userdb.Users.FirstOrDefault(p => p.UserName == User.Name);
            if (userinfo != null)
            {
                var tokeninfo = db.ApiTokens.FirstOrDefault(p => p.T_UsersID == userinfo.Id);
                if (tokeninfo != null)
                    ViewData["Token"] = tokeninfo.T_AuthToken;
                else
                {
                    var token = GenerateToken.GetToken(userinfo.Id);
                    ViewData["Token"] = token.T_AuthToken;
                }
            }
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public System.Web.Mvc.ActionResult Api(string apiId, string Token)
        {
            ViewData["Token"] = Token;
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View("Error");
        }
    }
}