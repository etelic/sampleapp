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
                var isAdmin = ((CustomPrincipal)User).IsAdmin();
                List<Permission> permissions = new List<Permission>();
                using (var pc = new PermissionContext())
                {
                    // so we only make one database call instead of one per entity?
                    var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).ToList();
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
                                                var blockedverbs = allverbs.GroupBy(p => p).Where(p => p.Count() == verbrolecount);
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
					 //data monitoring
                    var dcalculated = new Permission();
                    var draw = rolePermissions.Where(p => p.EntityName == "DataMonitoring");
                    dcalculated.EntityName = "DataMonitoring";
                    dcalculated.CanEdit = isAdmin || draw.Any(p => p.CanEdit);
                    dcalculated.CanDelete = isAdmin || draw.Any(p => p.CanDelete);
                    dcalculated.CanAdd = isAdmin || draw.Any(p => p.CanAdd);
                    dcalculated.CanView = isAdmin || draw.Any(p => p.CanView);
                    permissions.Add(dcalculated);
                    //
                }
                ((CustomPrincipal)User).permissions = permissions;
                List<BusinessRule> businessrules = new List<BusinessRule>();
                using (var br = new BusinessRuleContext())
                {
                     var rolebr = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable).ToList();
                    foreach (var rules in rolebr)
                    {
                        if ((((CustomPrincipal)User).IsInRole(rules.Roles.Split(",".ToCharArray()))))
                        {
                            businessrules.Add(rules);
                        }
                    }
                }
                ((CustomPrincipal)User).businessrules = businessrules.ToList();
            }
        }	
    }
}


