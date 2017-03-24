using System.Web.Http;
using AttributeRouting.Web.Http.WebHost;
using GeneratorBase.MVC.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(GeneratorBase.MVC.AttributeRoutingHttpConfig), "Start")]

namespace GeneratorBase.MVC
{
    public static class AttributeRoutingHttpConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {

            // To debug routes locally using the built in ASP.NET development server, go to /routes.axd
            routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{id}",
              defaults: new { id = RouteParameter.Optional });
        }

        public static void Start()
        {
            RegisterRoutes(GlobalConfiguration.Configuration.Routes);
        }
    }
}
