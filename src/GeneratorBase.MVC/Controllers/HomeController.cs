using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace GeneratorBase.MVC.Controllers
{
    public class HomeController : BaseController
    {
		public ActionResult LiveMonitoring()
        {
		    if (!User.CanView("DataMonitoring"))
            {
                return RedirectToAction("Index", "Error");
            }
            return View();
        }
        public ActionResult Index(string RegistrationEntity, string TokenId, bool? isThirdParty)
        {
            if (!string.IsNullOrEmpty(RegistrationEntity) && (Request.UrlReferrer == null || (Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))))
            {
                ViewBag.RegistrationEntity = RegistrationEntity;
                ViewBag.UserId = TokenId;
                return View();
            }

            bool isItemZero = true;
            var roles = ((CustomPrincipal)User).GetRoles();
            var MultipleRoleSelection = CommonFunction.Instance.MultipleRoleSelection();
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
			ApplicationContext db = new ApplicationContext(User);
            if (roles.Count() > 1 && Request.Cookies[AppUrl+"CurrentRole"]==null && Convert.ToBoolean(MultipleRoleSelection))
            {
                ViewBag.PageRoles = roles.ToList();
                return View();
            }
            else
            {
                if (roles.Count() > 0)
                {
                    var user = new UserDefinePagesRoleContext();
                    var userpagelist = user.UserDefinePagesRoles;
                    var role = roles.ToArray()[0];
                    var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role);
                    if (userpage != null)
                    {
                        isItemZero = false;
                        ViewBag.PageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent.Replace("Root_App_Path", GetBaseUrl());
                    }
                }
                else { ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>"; }
            }
			
            var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
            if (lstFavoriteItem.Count() > 0)
            {
                ViewBag.FavoriteItem = lstFavoriteItem;
                ViewBag.FavoriteCount = lstFavoriteItem.Count();
            }
            if (isItemZero || (ViewBag.PageRoles == null && ViewBag.PageContent == null))
            {
		            ViewBag.T_ClientCount = db.T_Clients.Count();
            ViewBag.T_LearningCenterCount = db.T_LearningCenters.Count();
            ViewBag.T_SessionCount = db.T_Sessions.Count();
            ViewBag.T_TimeSlotsCount = db.T_TimeSlotss.Count();
				CompanyProfileRepository _cp = new CompanyProfileRepository();
                CompanyProfile cp = _cp.GetCompanyProfile();
                if (cp != null)
                {
                    ViewBag.AboutCompany = cp.AboutCompany;
                }
		   }

		   ApplicationDbContext userdb = new ApplicationDbContext();
             var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
             ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
             ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
			 if (userinfo!=null)
            {
                var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).ToList();
                ViewBag.LastLoggedIn = lastlogin.Count() > 1 ? lastlogin[1].Date.ToString() : "";
				if (!User.IsAdmin)
                {
                    //enforce password policy on first login
                    var appSettings = db.AppSettings;
                    bool securitypolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
                    if (securitypolicy)
                    {
                        bool enforcePwdPolicy = appSettings.Where(p => p.Key == "EnforceChangePassword").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
                        if (enforcePwdPolicy)
                        {
                            var pwdhistorycount = userdb.PasswordHistorys.Where(p => p.UserId == userinfo.Id).Count();
                            if (pwdhistorycount == 0)
                                return RedirectToAction("Manage", "Account");
                        }
                    }
                }
            }
			   ViewBag.LoginRoles = roles;
            return View();
        }
		public JsonResult setRoleValue(string key)
        {
			string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            HttpCookie cookierole = new HttpCookie(AppUrl+"CurrentRole");
            cookierole.Expires = DateTime.Now.AddDays(1);
            cookierole.Value = key;
            Response.Cookies.Add(cookierole);
            bool result = true;
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FavoriteSave(string Id, string Name)
        {
            string result = string.Empty;
            try
            {
                FavoriteItem objFs = new FavoriteItem();
                if (string.IsNullOrEmpty(Id))
                {
                    objFs.Name = Name;
                    objFs.LinkAddress =FavoriteUrl;
					objFs.EntityName = FavoriteUrlEntityName;
                    db.FavoriteItems.Add(objFs);
                }
                else
                {
                    long objId = Int64.Parse(Id);
                    objFs = db.FavoriteItems.Find(objId);
                    objFs.Name = Name;
					objFs.EntityName = FavoriteUrlEntityName;
                    db.Entry(objFs).State = EntityState.Modified;
                }
                db.SaveChanges();
                result = "success";
            }
            catch
            {
                result = "error";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FavoriteDelete(long Id)
        {
            FavoriteItem objFs = db.FavoriteItems.Find(Id);
            db.Entry(objFs).State = EntityState.Deleted;
            db.FavoriteItems.Remove(objFs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FavoriteDeleteUDF(long Id)
        {
            string result = string.Empty;
            FavoriteItem objFs = db.FavoriteItems.Find(Id);
            db.Entry(objFs).State = EntityState.Deleted;
            db.FavoriteItems.Remove(objFs);
            db.SaveChanges();
            result = "success";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
		 public ActionResult RedirectToEntity(string EntityName)
        {
            try
            {
                var roles = ((CustomPrincipal)User).GetRoles();
                var defaultPages = db.DefaultEntityPages.Where(p => p.EntityName == EntityName);
                foreach (var defaultPage in defaultPages)
                {
                    var pageRole = defaultPage.Roles.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    if (roles.Any(r => pageRole.Contains(r)) || pageRole.Contains("All"))
                    {
                        var url = CommonFunction.Instance.getBaseUri();
                        return Redirect(url + defaultPage.PageUrl);
                    }
                }
            }
            catch { }
            return RedirectToAction("Index", EntityName);
        }
		public JsonResult isAdmin()
        {
            return this.Json(User.IsAdmin, JsonRequestBehavior.AllowGet);
        }
    }
}
