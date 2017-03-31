using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Configuration;
using Microsoft.Owin.Security.Cookies;

using Owin;

namespace GeneratorBase.MVC.Controllers.OtherDefaultControllers
{
    public class AppSettingController : BaseController
    {
        public ActionResult Index()
        {
            var lstAppSetting = from s in db.AppSettings
                                select s;

            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder").Value != ConfigurationManager.AppSettings["ReportFolder"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "reportfolder");
                obj.Value = CommonFunction.Instance.ReportFolder();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "needsharedusersystem").Value))
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "needsharedusersystem");
                obj.Value = CommonFunction.Instance.NeedSharedUserSystem();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl").Value != ConfigurationManager.AppSettings["AppURL"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appurl");
                obj.Value = CommonFunction.Instance.AppURL();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname").Value != ConfigurationManager.AppSettings["AppName"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "appname");
                obj.Value = CommonFunction.Instance.AppName();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (lstAppSetting.Where(s => s.Key.ToLower() == "domainname").Count() > 0)
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "domainname");
                obj.Value = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }


            if (lstAppSetting.Where(s => s.Key.ToLower() == "useactivedirectory").Count() > 0)
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "useactivedirectory");
                obj.Value = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"];
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (lstAppSetting.Where(s => s.Key.ToLower() == "useactivedirectoryrole").Count() > 0)
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "useactivedirectoryrole");
                obj.Value = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }


            if (lstAppSetting.Where(s => s.Key.ToLower() == "administratorroles").Count() > 0)
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "administratorroles");
                obj.Value = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            //google analytics settings
            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics").Value != ConfigurationManager.AppSettings["enableGA"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "enable google analytics");
                obj.Value = CommonFunction.Instance.EnableGoogleAnalytics();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id").Value != ConfigurationManager.AppSettings["trackingID"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "tracking id");
                obj.Value = CommonFunction.Instance.TrackingID();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (string.IsNullOrEmpty(lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name").Value) || lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name").Value != ConfigurationManager.AppSettings["customdimensionname"])
            {
                AppSetting obj = lstAppSetting.FirstOrDefault(s => s.Key.ToLower() == "custom dimension name");
                obj.Value = CommonFunction.Instance.CustomDimensionName();
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            //

            var _AppSetting = lstAppSetting.Include(t => t.associatedappsettinggroup);

            ViewBag.AppSettingGroup = db.AppSettingGroups.ToList();
            if (!Request.IsAjaxRequest())
                return View(_AppSetting);
            else
                return PartialView("IndexPartial", _AppSetting);
        }

        public ActionResult CreateSetting(string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("AppSetting"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingParentUrl"] = UrlReferrer;


            var objAssociatedAppSettingGroup = new List<AppSettingGroup>();
            ViewBag.AssociatedAppSettingGroupID = new SelectList(objAssociatedAppSettingGroup, "ID", "DisplayValue");
            return View();
        }

        [HttpPost]
        public ActionResult CreateSetting([Bind(Include = "Id,ConcurrencyKey,Key,Value,AssociatedAppSettingGroupID,Description,IsDefault")] AppSetting appsetting, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.AppSettings.Add(appsetting);
                db.SaveChanges();

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult EditSetting(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("AppSetting"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AppSetting appsetting = db.AppSettings.Find(id);
            if (appsetting == null)
            {
                return HttpNotFound();
            }

            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingParentUrl"] = UrlReferrer;

            var objAssociatedAppSettingGroup = new List<AppSettingGroup>();
            objAssociatedAppSettingGroup.Add(appsetting.associatedappsettinggroup);
            ViewBag.AssociatedAppSettingGroupID = new SelectList(objAssociatedAppSettingGroup, "ID", "DisplayValue", appsetting.AssociatedAppSettingGroupID);
            return View(appsetting);
        }

        [HttpPost]
        public ActionResult EditSetting([Bind(Include = "Id,ConcurrencyKey,Key,Value,AssociatedAppSettingGroupID,Description,IsDefault")] AppSetting appsetting, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appsetting).State = EntityState.Modified;
                db.SaveChanges();
                var appSettings = db.AppSettings;
                //set session time out
                if (appsetting.Key == "ApplicationSessionTimeOut")
                {
                    var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["SessionTimeOut"].Value = Convert.ToString(appsetting.Value);
                    myConfiguration.Save();
                }
                //
                //Changing Application Name
                if (appsetting.Key == "AppName")
                {
                    var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["AppName"].Value = Convert.ToString(appsetting.Value);
                    myConfiguration.Save();
                }
                //google analytics settings
                if (appsetting.Key.ToLower() == "enable google analytics")
                {
                    var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["enableGA"].Value = Convert.ToString(appsetting.Value);
                    myConfiguration.Save();
                }
                if (appsetting.Key.ToLower() == "tracking id")
                {
                    var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["trackingID"].Value = Convert.ToString(appsetting.Value);
                    myConfiguration.Save();
                }
                if (appsetting.Key.ToLower() == "custom dimension name")
                {
                    var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["customdimensionname"].Value = Convert.ToString(appsetting.Value);
                    myConfiguration.Save();
                }

                //

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult DeleteSetting(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanDelete("AppSetting"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppSetting appsetting = db.AppSettings.Find(id);

            if (appsetting == null)
            {
                throw (new Exception("Deleted"));
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingParentUrl"] = UrlReferrer;
            return View(appsetting);
        }

        [HttpPost]
        public ActionResult DeleteSettingConfirmed(AppSetting appsetting, string UrlReferrer)
        {
            if (!User.CanDelete("AppSetting"))
            {
                return RedirectToAction("Index", "Error");
            }

            db.Entry(appsetting).State = EntityState.Deleted;
            db.AppSettings.Remove(appsetting);
            db.SaveChanges();

            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateGroup(string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("AppSettingGroup"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingGroupParentUrl"] = UrlReferrer;
            return View();
        }

        [HttpPost]
        public ActionResult CreateGroup([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] AppSettingGroup appsettinggroup, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.AppSettingGroups.Add(appsettinggroup);
                db.SaveChanges();

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult EditGroup(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("AppSettingGroup"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AppSettingGroup appsettinggroup = db.AppSettingGroups.Find(id);
            if (appsettinggroup == null)
            {
                return HttpNotFound();
            }

            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingGroupParentUrl"] = UrlReferrer;


            return View(appsettinggroup);
        }

        [HttpPost]
        public ActionResult EditGroup([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] AppSettingGroup appsettinggroup, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appsettinggroup).State = EntityState.Modified;
                db.SaveChanges();

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult DeleteGroup(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanDelete("AppSettingGroup"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppSettingGroup appsettinggroup = db.AppSettingGroups.Find(id);

            if (appsettinggroup == null)
            {
                throw (new Exception("Deleted"));
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["AppSettingGroupParentUrl"] = UrlReferrer;
            return View(appsettinggroup);
        }

        [HttpPost]
        public ActionResult DeleteGroupConfirmed(AppSettingGroup appsettinggroup, string UrlReferrer)
        {
            if (!User.CanDelete("AppSettingGroup"))
            {
                return RedirectToAction("Index", "Error");
            }

            db.Entry(appsettinggroup).State = EntityState.Deleted;
            db.AppSettingGroups.Remove(appsettinggroup);
            db.SaveChanges();

            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplySetting()
        {
            CommonFunction.ResetInstance();
            return RedirectToAction("Index", "AppSetting");
        }

        public ActionResult Cancel(string UrlReferrer)
        {
            if (!string.IsNullOrEmpty(UrlReferrer))
            {
                var uri = new Uri(UrlReferrer);
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (Convert.ToBoolean(query.Get("IsFilter")) == true)
                    return RedirectToAction("Index");
                else
                    return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {

            IQueryable<AppSettingGroup> list = db.AppSettingGroups;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.AppSettingGroups;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(AppSettingGroup), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<AppSettingGroup, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);

                    IQueryable q = query.Provider.CreateQuery(methodCall);

                    list = ((IQueryable<AppSettingGroup>)q);
                }
            }

            if (key != null && key.Length > 0)
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

            }
        }
    }
}
public class AppBuilderProvider : IDisposable
{
    private Owin.IAppBuilder _app;
    public AppBuilderProvider(Owin.IAppBuilder app)
    {
        _app = app;
    }
    public Owin.IAppBuilder Get() { return _app; }
    public void Dispose() { }
}