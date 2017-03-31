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
            
			//builder.EntitySet<T_Address>("T_AddressOData");
            
			//builder.EntitySet<T_City>("T_CityOData");
            
			//builder.EntitySet<T_Country>("T_CountryOData");
            
			//builder.EntitySet<T_Employee>("T_EmployeeOData");
            
			//builder.EntitySet<T_Employeestatus>("T_EmployeestatusOData");
            
			//builder.EntitySet<T_Employeetype>("T_EmployeetypeOData");
            
			//builder.EntitySet<T_EmployeeOrganizationAssociation>("T_EmployeeOrganizationAssociationOData");
            
			//builder.EntitySet<T_State>("T_StateOData");
            
			//builder.EntitySet<T_Organization>("T_OrganizationOData");
         // builder.EntitySet<Document>("DocumentOData");      
			//config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
