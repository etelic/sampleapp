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
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            return View(db.UserDefinePagess.ToList());
        }
        // GET: /UserDefinePages/Details/5
        public ActionResult Details(long? id)
        {
            if (!((CustomPrincipal)User).IsAdmin())
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
            if (!((CustomPrincipal)User).IsAdmin())
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
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        // GET: /UserDefinePages/Create
        public ActionResult SaveUserDefinePage(EditUserDefinePageViewModel model)
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                if (model.Roles.Count() > 0)
                {
                    var list = db.UserDefinePagess.FirstOrDefault(q => q.Id == model.Id);
                    UserDefinePages userpages = (list != null ? list : new UserDefinePages());
                    userpages.PageName = model.PageName;
                    userpages.PageContent = model.PageContent;
                    var userdefinepagesrole = new UserDefinePagesRoleContext();
                    userdefinepagesrole.UserDefinePagesRoles.RemoveRange(userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId == model.Id));
                    userdefinepagesrole.SaveChanges();
                    foreach (var ent in model.Roles)
                    {
                        if (ent.Selected)
                        {
                            UserDefinePagesRole objUDPR = new UserDefinePagesRole();
                            objUDPR.PageId = model.Id;
                            objUDPR.RoleName = ent.RoleName;
                            userdefinepagesrole.UserDefinePagesRoles.Add(objUDPR);
                            userdefinepagesrole.SaveChanges();
                        }
                    }
                    if (list == null)
                        db.UserDefinePagess.Add(userpages);
                    db.SaveChanges();
                }
                return RedirectToAction("UserDefinePage", "UserDefinePages", new { page = model.Id });
            }
            return View();
        }
        // GET: /UserDefinePages/Create
        public ActionResult Create()
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            return View();
        }
        // POST: /UserDefinePages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PageName,PageContent")] UserDefinePages userdefinepages)
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.UserDefinePagess.Add(userdefinepages);
                db.SaveChanges();
                Int64 pageId = userdefinepages.Id;
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
        // GET: /UserDefinePages/Edit/5
        public ActionResult Edit(long? id)
        {
            if (!((CustomPrincipal)User).IsAdmin())
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
            if (!((CustomPrincipal)User).IsAdmin())
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
            if (!((CustomPrincipal)User).IsAdmin())
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
            if (!((CustomPrincipal)User).IsAdmin())
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
