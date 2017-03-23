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
            if (roles.Count() > 1 && Request.Cookies["CurrentRole"]==null && Convert.ToBoolean(MultipleRoleSelection))
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
                        ViewBag.PageContent = (new UserDefinePagesContext()).UserDefinePagess.FirstOrDefault(p => p.Id == userpage.PageId).PageContent;
                    }
                }
                else { ViewBag.PageContent = "<br/><a href=\"javascript:document.getElementById('logoutForm').submit()\" class=\"btn btn-primary btn-sm\">You are not assigned to an application role, please contact application administrator.</a>"; }
            }
            if (isItemZero || (ViewBag.PageRoles == null && ViewBag.PageContent == null))
            {
				ApplicationContext db = new ApplicationContext(User);
		            ViewBag.T_SchoolCount = db.T_Schools.Count();
            ViewBag.T_StudentCount = db.T_Students.Count();
            ViewBag.T_DepartmentCount = db.T_Departments.Count();
				CompanyProfileRepository _cp = new CompanyProfileRepository();
                CompanyProfile cp = _cp.GetCompanyProfile();
                if (cp != null)
                {
                    ViewBag.AboutCompany = cp.AboutCompany;
                }
				var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
                if (lstFavoriteItem.Count() > 0)
                {
                    ViewBag.FavoriteItem = lstFavoriteItem;
                    ViewBag.FavoriteCount = lstFavoriteItem.Count();
                }
		   }
		   ApplicationDbContext userdb = new ApplicationDbContext();
             var userinfo = userdb.Users.FirstOrDefault(p=>p.UserName == User.Name);
             ViewBag.UserName = userinfo != null ? userinfo.FirstName +" "+ userinfo.LastName : "";
             ViewBag.Useremail = userinfo != null ? userinfo.Email : "";
			  var lastlogin = userdb.LoginAttempts.Where(p => p.UserId == userinfo.Id).OrderByDescending(p => p.Id).ToList();
              ViewBag.LastLoggedIn =lastlogin.Count()>1 ? lastlogin[1].Date.ToString() : "";
			   ViewBag.LoginRoles = roles;
            return View();
        }
		public JsonResult setRoleValue(string key)
        {
            HttpCookie cookierole = new HttpCookie("CurrentRole");
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
                    objFs.LinkAddress =FavoriteUrl.Replace(CommonFunction.Instance.AppURL() + "/", "");
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
                        return Redirect("/" + CommonFunction.Instance.AppURL().ToString() + defaultPage.PageUrl);
                }
            }
            catch { }
            return RedirectToAction("Index", EntityName);
        }
    }
}
