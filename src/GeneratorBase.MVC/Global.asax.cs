using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Http;
using System.Web.Routing;
using System.Globalization;
namespace GeneratorBase.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
			ModelBinders.Binders.Add(typeof(Decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(Nullable<Decimal>), new DecimalModelBinder());
        }
		protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpContext.Current.User = new CustomPrincipal(User);
        }
		 protected void Application_AuthorizeRequest(Object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
				var roles = ((CustomPrincipal)User).GetRoles();
                var isAdmin = ((CustomPrincipal)User).IsAdminUser();
                List<Permission> permissions = new List<Permission>();
				((CustomPrincipal)User).IsAdmin = isAdmin;
				 List<PermissionAdminPrivilege> adminprivilegeslist = new List<PermissionAdminPrivilege>();
                using (var pc = new PermissionContext())
                {
                    // so we only make one database call instead of one per entity?
                    var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).ToList();
					var adminprivileges = pc.AdminPrivileges.Where(p => roles.Contains(p.RoleName)).ToList();
					foreach (var item in (new AdminFeaturesDictionary()).getDictionary())
                    {
                        var adminprivilege = new PermissionAdminPrivilege();
                        var raw = adminprivileges.Where(p => p.AdminFeature == item.Key);
                        adminprivilege.AdminFeature = item.Key;
                        adminprivilege.IsAllow = isAdmin || raw.Any(p => p.IsAllow);
                        adminprivilege.IsAdd = isAdmin || raw.Any(p => p.IsAdd);
                        adminprivilege.IsEdit = isAdmin || raw.Any(p => p.IsEdit);
                        adminprivilege.IsDelete = isAdmin || raw.Any(p => p.IsDelete);
                        adminprivilegeslist.Add(adminprivilege);
                    }
                    ((CustomPrincipal)User).adminprivileges = adminprivilegeslist;
                    foreach (var entity in GeneratorBase.MVC.ModelReflector.Entities)
					{
                        var calculated = new Permission();
                        var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                        calculated.EntityName = entity.Name;
                        calculated.CanEdit = isAdmin || raw.Any(p => p.CanEdit);
                        calculated.CanDelete = isAdmin || raw.Any(p => p.CanDelete);
                        calculated.CanAdd = isAdmin || raw.Any(p => p.CanAdd);
                        calculated.CanView = isAdmin || raw.Any(p => p.CanView);
						calculated.IsOwner = raw.Any(p => p.IsOwner!=null && p.IsOwner.Value);
						if (!isAdmin)
                            calculated.SelfRegistration = raw.Any(p => p.SelfRegistration != null && p.SelfRegistration.Value);
                        else calculated.SelfRegistration = false;
                        if (calculated.IsOwner != null && calculated.IsOwner.Value)
                            calculated.UserAssociation = raw.FirstOrDefault().UserAssociation;
                        else
                            calculated.UserAssociation = string.Empty;

						//code for verb action security
                        var verblist = raw.Select(x => x.Verbs).ToList();
                        var verbrolecount = verblist.Count();
                        List<string> allverbs = new List<string>();
                        foreach (var verb in verblist)
                        {
                            if (verb != null)
                                allverbs.AddRange(verb.Split(",".ToCharArray()).ToList());
                        }

                        var blockedverbs = allverbs.GroupByMany(p => p);

                        if (blockedverbs.Count() > 0)
                            calculated.Verbs = string.Join(",", blockedverbs.Select(b => b.Key).ToList());
                        else
                            calculated.Verbs = string.Empty;
                        //
						//FLS
                        if (!isAdmin)
                        {
                            var listEdit = raw.Select(p => p.NoEdit).ToList();
                            var listView = raw.Select(p => p.NoView).ToList();
                            var resultEdit = "";
                            var resultView = "";
                           foreach(var str in listEdit)
                           {
                               if (str != null)
                                   resultEdit += str;
                           }
                           foreach (var str in listView)
                           {
                               if (str != null)
                                   resultView += str;
                           }
                           calculated.NoEdit = resultEdit;
                           calculated.NoView = resultView;
                        }
                        //
                        permissions.Add(calculated);
                    }
                }
                ((CustomPrincipal)User).permissions = permissions;
                List<BusinessRule> businessrules = new List<BusinessRule>();
                using (var br = new BusinessRuleContext())
                {
                     var rolebr = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID != 5).ToList();
                    foreach (var rules in rolebr)
                    {
                        //if ((((CustomPrincipal)User).IsInRole(rules.Roles.Split(",".ToCharArray()))))
						if(((CustomPrincipal)User).IsInRole(rules.Roles.Split(",".ToCharArray()),roles))
                        {
                            businessrules.Add(rules);
                        }
                    }
                }
                ((CustomPrincipal)User).businessrules = businessrules.ToList();
            }
        }
		protected void Application_Error(object sender, EventArgs e)
        { // Do whatever you want to do with the error

            //Show the custom error page...
            if ((Context.Server.GetLastError() is HttpException) && ((Context.Server.GetLastError() as HttpException).GetHttpCode() == 404))
            {
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Error";
                Response.StatusCode = 404;
                routeData.Values["action"] = "NotFound404";
                Response.TrySkipIisCustomErrors = true; // If you are using IIS7, have this line
                IController errorsController = new GeneratorBase.MVC.Controllers.ErrorController();
                HttpContextWrapper wrapper = new HttpContextWrapper(Context);
                var rc = new System.Web.Routing.RequestContext(wrapper, routeData);
                errorsController.Execute(rc);
            }

        }
    }

	 public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (string.IsNullOrEmpty(valueResult.AttemptedValue))
            {
                return 0m;
            }
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                actualValue = Convert.ToDecimal(
                    valueResult.AttemptedValue.Replace(",", ""),
                    CultureInfo.InvariantCulture
                );
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}


