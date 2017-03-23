using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using GeneratorBase.MVC.Models;
namespace GeneratorBase.MVC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            
			builder.EntitySet<T_School>("T_SchoolOData");
            
			builder.EntitySet<T_StudentPerformance>("T_StudentPerformanceOData");
            
			builder.EntitySet<T_Student>("T_StudentOData");
            
			builder.EntitySet<T_Department>("T_DepartmentOData");
           
			config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
