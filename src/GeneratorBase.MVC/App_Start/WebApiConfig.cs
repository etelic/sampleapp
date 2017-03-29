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
           //config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
           // ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            
			//builder.EntitySet<T_Client>("T_ClientOData");
            
			//builder.EntitySet<T_SessionEvents>("T_SessionEventsOData");
            
			//builder.EntitySet<T_SessionEventsClient>("T_SessionEventsClientOData");
            
			//builder.EntitySet<T_SessionClientAssociation>("T_SessionClientAssociationOData");
            
			//builder.EntitySet<T_TimeSlots>("T_TimeSlotsOData");
            
			//builder.EntitySet<T_LearningCenter>("T_LearningCenterOData");
            
			//builder.EntitySet<T_Session>("T_SessionOData");
         // builder.EntitySet<Document>("DocumentOData");      
			//config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
