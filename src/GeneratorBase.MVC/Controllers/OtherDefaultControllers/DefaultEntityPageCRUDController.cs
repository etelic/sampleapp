using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.ComponentModel.DataAnnotations;
namespace GeneratorBase.MVC.Controllers
{
    public partial class DefaultEntityPageController : BaseController
    {
        // GET: /DefaultEntityPage/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Error");
            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            ViewData["HostingEntity"] = HostingEntity;
            ViewData["HostingEntityID"] = HostingEntityID;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["IsFilter"] = IsFilter;
            ViewData["BulkOperation"] = BulkOperation;
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstDefaultEntityPage = searchRecords(lstDefaultEntityPage, searchString.ToUpper(), IsDeepSearch);
            }
            if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstDefaultEntityPage = sortRecords(lstDefaultEntityPage, sortBy, isAsc);
            }
            else lstDefaultEntityPage = lstDefaultEntityPage.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
            var _DefaultEntityPage = lstDefaultEntityPage;
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_DefaultEntityPage.Count() > 0)
                    pageSize = _DefaultEntityPage.Count();
                return View("ExcelExport", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            }
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            else
            {
                if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                    return PartialView("BulkOperation", _DefaultEntityPage.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                else
                    return PartialView("IndexPartial", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            }
        }
        // GET: /DefaultEntityPage/Details/5
        public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
            if (defaultentitypage == null)
            {
                return HttpNotFound();
            }
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            return View(defaultentitypage);
        }
        // GET: /DefaultEntityPage/Create
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // GET: /DefaultEntityPage/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanAdd("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // POST: /DefaultEntityPage/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (ModelState.IsValid)
            {
                db.DefaultEntityPages.Add(defaultentitypage);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            LoadViewDataAfterOnCreate(defaultentitypage);
            return View(defaultentitypage);
        }
        // GET: /DefaultEntityPage/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanAdd("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // POST: /DefaultEntityPage/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag,ViewEntityPage,ListEntityPage,EditEntityPage,SearchEntityPage,CreateEntityPage")] DefaultEntityPage defaultentitypage, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (ModelState.IsValid)
            {
                //var page = db.DefaultEntityPages.FirstOrDefault(p => p.EntityName == defaultentitypage.EntityName);
                //if (page == null)
                //{
                db.DefaultEntityPages.Add(defaultentitypage);
                //}
                //else
                //{
                //    page.EntityName = defaultentitypage.EntityName;
                //    page.PageType = defaultentitypage.PageType;
                //    page.PageUrl = defaultentitypage.PageUrl;
                //    page.Roles = defaultentitypage.Roles;
                //    page.Other = defaultentitypage.Other;
                //    page.Flag = defaultentitypage.Flag;
                //    db.Entry(defaultentitypage).State = EntityState.Modified;
                //}
                db.SaveChanges();
                return RedirectToAction("Index");
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
            LoadViewDataAfterOnCreate(defaultentitypage);
            return View(defaultentitypage);
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
        // POST: /DefaultEntityPage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer, bool? IsDDAdd)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.DefaultEntityPages.Add(defaultentitypage);
                db.SaveChanges();
                if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = defaultentitypage.Id, UrlReferrer = UrlReferrer });
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            LoadViewDataAfterOnCreate(defaultentitypage);
            return View(defaultentitypage);
        }
        // GET: /DefaultEntityPage/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanEdit("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
            if (defaultentitypage == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
            if (ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage/Edit/" + defaultentitypage.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage/Create"))
                ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["AssociatedType"] = AssociatedType;
            LoadViewDataBeforeOnEdit(defaultentitypage);
            return View(defaultentitypage);
        }
        // POST: /DefaultEntityPage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag,ViewEntityPage,ListEntityPage,EditEntityPage,SearchEntityPage,CreateEntityPage")] DefaultEntityPage defaultentitypage, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.Entry(defaultentitypage).State = EntityState.Modified;
                db.SaveChanges();
                if (command == "Save & Continue")
                    return RedirectToAction("Edit", new { Id = defaultentitypage.Id, UrlReferrer = UrlReferrer });
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
            LoadViewDataAfterOnEdit(defaultentitypage);
            return View(defaultentitypage);
        }
        // GET: /DefaultEntityPage/EditWizard/5
        public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanEdit("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
            if (defaultentitypage == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["DefaultEntityPageParentUrl"] = UrlReferrer;
            if (ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage"))
                ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
            LoadViewDataBeforeOnEdit(defaultentitypage);
            return View(defaultentitypage);
        }
        // POST: /DefaultEntityPage/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,Roles,PageType,PageUrl,Other,Flag")] DefaultEntityPage defaultentitypage, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (ModelState.IsValid)
            {
                db.Entry(defaultentitypage).State = EntityState.Modified;
                db.SaveChanges();
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
            LoadViewDataAfterOnEdit(defaultentitypage);
            return View(defaultentitypage);
        }
        // GET: /DefaultEntityPage/Delete/5
        public ActionResult Delete(int id)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanDelete("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
            if (defaultentitypage == null)
            {
                throw (new Exception("Deleted"));
            }
            if (ViewData["DefaultEntityPageParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DefaultEntityPage"))
                ViewData["DefaultEntityPageParentUrl"] = Request.UrlReferrer;
            return View(defaultentitypage);
        }
        // POST: /DefaultEntityPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DefaultEntityPage defaultentitypage, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            if (!User.CanDelete("DefaultEntityPage"))
            {
                return RedirectToAction("Index", "Error");
            }
            db.Entry(defaultentitypage).State = EntityState.Deleted;
            db.DefaultEntityPages.Remove(defaultentitypage);
            db.SaveChanges();
            if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["DefaultEntityPageParentUrl"] != null)
            {
                string parentUrl = ViewData["DefaultEntityPageParentUrl"].ToString();
                ViewData["DefaultEntityPageParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
        public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
        {
            if (!User.IsAdmin)
                return RedirectToAction("Index", "Error");
            foreach (var id in ids.Where(p => p > 0))
            {
                DefaultEntityPage defaultentitypage = db.DefaultEntityPages.Find(id);
                db.Entry(defaultentitypage).State = EntityState.Deleted;
                db.DefaultEntityPages.Remove(defaultentitypage);
                db.SaveChanges();
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult BindtemplatesIndex(string EntityName)
        {
            //Dictionary object for all templates 
            Dictionary<string, string> dicIndexFile = new Dictionary<string, string>();
            Dictionary<string, string> dicDetailsFile = new Dictionary<string, string>();
            Dictionary<string, string> dicEditFile = new Dictionary<string, string>();
            Dictionary<string, string> dicSearchFile = new Dictionary<string, string>();
            Dictionary<string, string> dicCreateFile = new Dictionary<string, string>();
            //
            //get all cshtml page from view dirctory for an entity.
            var viewsName = System.IO.Directory.EnumerateFiles(Server.MapPath(@"~/Views/" + EntityName)).ToList();
            //bind file name of an entity page in Dictionary
            foreach (var item in viewsName)
            {
                string fileName = Path.GetFileName(item).Replace(".cshtml", "");
                string dispalyName = "";
                if (fileName.ToLower().Contains(".mobile"))
                    continue;
                // getting index page
                if (fileName.ToLower().Contains("indexpartial"))
                {
                    if (fileName.ToLower() == "indexpartial")
                        dispalyName = "Table(Default)";
                    else
                        dispalyName = fileName.Replace("IndexPartial", "");
                    dicIndexFile.Add(dispalyName, fileName);
                }
                //
                //getting ditails page
                if (fileName.ToLower().Contains("details"))
                {
                    if (fileName.ToLower() == "details")
                    {
                        dispalyName = "(Detail)Default";
                        dicDetailsFile.Add(dispalyName, fileName);
                    }
                    else
                        dispalyName = fileName.Replace("Details", "");
                    //dicDetailsFile.Add(dispalyName, fileName);
                }
                //

                //getting Edit page
                if (fileName.ToLower().Contains("edit"))
                {
                    if (fileName.ToLower() == "edit")
                        dispalyName = "(Edit)Default";
                    else
                        dispalyName = fileName.Replace("Edit", "");
                    dicEditFile.Add(dispalyName, fileName);
                }
                //
                //getting Search page
                if (fileName.ToLower().Contains("search"))
                {
                    if (fileName.ToLower() == "setfsearch")
                        dispalyName = "Faceted Search(Default)";
                    else
                        dispalyName = fileName.Replace("Search", "");
                    dicSearchFile.Add(dispalyName, fileName);
                }
                //
                //getting Search page
                if (fileName.ToLower().Contains("create"))
                {
                    if (fileName.ToLower() == "create")
                        dispalyName = "(Create)Default";
                    else
                        dispalyName = fileName.Replace("Create", "");
                    dicCreateFile.Add(dispalyName, fileName);
                }
                //


            }
            //
            return Json(new
            {
                IndexPages = dicIndexFile,
                DetailsPage = dicDetailsFile,
                EditPage = dicEditFile,
                SearchPage = dicSearchFile,
                CreatePage = dicCreateFile
            }, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        //public string GetIndexTemplates(string EntityName)
        //{
        //    string pageName = "";
        //    var lstDefaultEntityPage = from s in db.DefaultEntityPages
        //                               where s.EntityName == EntityName
        //                               select s;
        //    if (lstDefaultEntityPage.Count() > 0)
        //        pageName = lstDefaultEntityPage.Select(p => p.ListEntityPage).FirstOrDefault().ToString();
        //    return pageName;

        //}
        //public string GetViewTemplates(string EntityName)
        //{
        //    string pageName = "";
        //    return pageName;

        //}
        //public string GetEditTemplates(string EntityName)
        //{
        //    string pageName = "";
        //    return pageName;

        //}
        //public string GetSearchTemplates(string EntityName)
        //{
        //    string pageName = "";
        //    return pageName;

        //}
    }
}
