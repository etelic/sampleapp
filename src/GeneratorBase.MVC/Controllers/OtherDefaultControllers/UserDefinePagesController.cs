using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
    public class UserDefinePagesController : Controller
    {
        private UserDefinePagesContext db = new UserDefinePagesContext();
        // GET: /UserDefinePages/
        public ActionResult Index()
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            return View(db.UserDefinePagess.ToList());
        }

        public ActionResult permissionlist(string roleslist)
        {
            var roles = roleslist.Split(new string[] { "\r\n", "\n", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<Permission> permissions = new List<Permission>();
            using (var pc = new PermissionContext())
            {
                var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).ToList();
                foreach (var entity in GeneratorBase.MVC.ModelReflector.Entities)
                {
                    var calculated = new Permission();
                    var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                    calculated.EntityName = entity.Name;
                    calculated.CanEdit = raw.Any(p => p.CanEdit);
                    calculated.CanDelete = raw.Any(p => p.CanDelete);
                    calculated.CanAdd = raw.Any(p => p.CanAdd);
                    calculated.CanView = raw.Any(p => p.CanView);
                    permissions.Add(calculated);
                }
            }
            var objPerlst = permissions.Where(e => e.CanAdd || e.CanView).Select(p => new
            {
                EntityName = p.EntityName,
                EntityDisplayName = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == p.EntityName).DisplayName,
                CanAdd = p.CanAdd,
                CanView = p.CanView
            });

            return Json(objPerlst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeletePage")]
        public ActionResult DeletePage(long id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            UserDefinePages userdefinepages = db.UserDefinePagess.Find(id);
            var userdefinepagesrole = new UserDefinePagesRoleContext();
            userdefinepagesrole.UserDefinePagesRoles.RemoveRange(userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId == id));
            userdefinepagesrole.SaveChanges();
            db.Entry(userdefinepages).State = EntityState.Deleted;
            db.UserDefinePagess.Remove(userdefinepages);
            try
            {
                db.SaveChanges();
            }
            catch { }

            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        // GET: /UserDefinePages/Details/5
        public ActionResult Details(long? id)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDefinePages userdefinepages = db.UserDefinePagess.Find(id);
            if (userdefinepages == null)
            {
                return HttpNotFound();
            }
            return View(userdefinepages);
        }
        // GET: /UserDefinePages/Create
        public ActionResult UserDefinePage(Int64? page)
        {
            ApplicationContext dbfav = new ApplicationContext(new SystemUser());
            var lstFavoriteItem = dbfav.FavoriteItems.Where(p => p.LastUpdatedByUser == ((CustomPrincipal)User).Name);
            if (lstFavoriteItem.Count() > 0)
            {
                ViewBag.FavoriteItem = lstFavoriteItem;
            }
            if (!((CustomPrincipal)User).IsAdmin)
                return RedirectToAction("Index", "Home");
            Int64 PageId = page ?? 0;
            if (page == null)
            {
                if (db.UserDefinePagess.Count() > 0)
                    PageId = db.UserDefinePagess.ToList()[0].Id;
            }
            var model = new EditUserDefinePageViewModel(PageId);
            if (PageId == 0)
            {
                UserDefinePages obj = new UserDefinePages();
                obj.Id = 0;
                obj.PageName = "Select Page";
                List<UserDefinePages> newList = new List<UserDefinePages>();
                newList.Add(obj);
                ViewBag.UserPages = new SelectList(newList, "Id", "PageName", PageId);
            }
            else
                ViewBag.UserPages = new SelectList(db.UserDefinePagess, "Id", "PageName", PageId);

            if (model.Roles.Where(r => r.Selected).Count() > 0)
            {
                var roles = model.Roles.Where(r => r.Selected).Select(s => s.RoleName).ToList();
                ViewBag.RoleList = String.Join(",", roles);
            }

            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        // GET: /UserDefinePages/Create
        public ActionResult SaveUserDefinePage(EditUserDefinePageViewModel model)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                //if (model.Roles.Count() > 0)
                //{
                var list = db.UserDefinePagess.FirstOrDefault(q => q.Id == model.Id);
                UserDefinePages userpages = (list != null ? list : new UserDefinePages());
                userpages.PageName = model.PageName;
                userpages.PageContent = model.PageContent; //.Replace(" turanto-row-edit", "").Replace("turanto-row-delete", "")
                //var userdefinepagesrole = new UserDefinePagesRoleContext();
                //userdefinepagesrole.UserDefinePagesRoles.RemoveRange(userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId == model.Id));
                //userdefinepagesrole.SaveChanges();
                //foreach (var ent in model.Roles)
                //{
                //    if (ent.Selected)
                //    {
                //        UserDefinePagesRole objUDPR = new UserDefinePagesRole();
                //        objUDPR.PageId = model.Id;
                //        objUDPR.RoleName = ent.RoleName;
                //        userdefinepagesrole.UserDefinePagesRoles.Add(objUDPR);
                //        userdefinepagesrole.SaveChanges();
                //    }
                //}
                if (list == null)
                    db.UserDefinePagess.Add(userpages);
                db.SaveChanges();
                //}
                return RedirectToAction("UserDefinePage", "UserDefinePages", new { page = model.Id });
            }
            return View();
        }
        // GET: /UserDefinePages/Create
        public ActionResult Create()
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");

            var model = new CreateUserDefinePageViewModel((new UserDefinePages()));
            return View(model);
        }

        // POST: /UserDefinePages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserDefinePageViewModel model, string SelectedRole)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                UserDefinePages userpages = new UserDefinePages();
                userpages.PageName = model.PageName;
                userpages.PageContent = model.PageContent; //.Replace(" turanto-row-edit", "").Replace("turanto-row-delete", "")

                db.UserDefinePagess.Add(userpages);
                db.SaveChanges();
                Int64 pageId = userpages.Id;

                var userdefinepagesrole = new UserDefinePagesRoleContext();
                //foreach (var ent in model.Roles)
                //{
                //    if (ent.Selected)
                //    {
                if (!string.IsNullOrEmpty(SelectedRole))
                {
                    UserDefinePagesRole objUDPR = new UserDefinePagesRole();
                    objUDPR.PageId = pageId;
                    objUDPR.RoleName = SelectedRole;
                    userdefinepagesrole.UserDefinePagesRoles.Add(objUDPR);
                    userdefinepagesrole.SaveChanges();
                }
                //    }
                //}
                return Json(new { success = true, page = pageId });
            }
            else
            {
                var errors = new List<string>();
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
                }
                return Json(errors);
            }
            return View();
        }


        // POST: /UserDefinePages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,PageName,PageContent")] UserDefinePages userdefinepages)
        //{
        //    if (!((CustomPrincipal)User).IsAdmin)
        //        return RedirectToAction("Index", "Home");
        //    if (ModelState.IsValid)
        //    {
        //        db.UserDefinePagess.Add(userdefinepages);
        //        db.SaveChanges();
        //        Int64 pageId = userdefinepages.Id;
        //        return Json(new { success = true, page = pageId });
        //    }
        //    else
        //    {
        //        var errors = new List<string>();
        //        foreach (var modelState in ViewData.ModelState.Values)
        //        {
        //            errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
        //        }
        //        return Json(errors);
        //    }
        //    return View();
        //}
        // GET: /UserDefinePages/Edit/5
        public ActionResult Edit(long? id)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDefinePages userdefinepages = db.UserDefinePagess.Find(id);
            if (userdefinepages == null)
            {
                return HttpNotFound();
            }
            return View(userdefinepages);
        }
        // POST: /UserDefinePages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PageName,PageContent")] UserDefinePages userdefinepages)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(userdefinepages).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userdefinepages);
        }
        // GET: /UserDefinePages/Delete/5
        public ActionResult Delete(long? id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDefinePages userdefinepages = db.UserDefinePagess.Find(id);
            if (userdefinepages == null)
            {
                return HttpNotFound();
            }
            return View(userdefinepages);
        }

        // POST: /UserDefinePages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            UserDefinePages userdefinepages = db.UserDefinePagess.Find(id);
            db.UserDefinePagess.Remove(userdefinepages);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
