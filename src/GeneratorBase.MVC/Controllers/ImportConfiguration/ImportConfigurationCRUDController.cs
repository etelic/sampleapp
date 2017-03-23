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
using System.Drawing;
namespace GeneratorBase.MVC.Controllers
{
    public partial class ImportConfigurationController : BaseController
    {
        //private ApplicationContext db = new ApplicationContext(User);
        // GET: /ImportConfiguration/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsMobileRequest, bool? IsFilter, bool? RenderPartial, string BulkOperation)
        {
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
			var lstImportConfiguration = from s in db.ImportConfigurations
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstImportConfiguration = searchRecords(lstImportConfiguration, searchString.ToUpper(), IsDeepSearch);
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstImportConfiguration = sortRecords(lstImportConfiguration, sortBy, isAsc);
            }
            else lstImportConfiguration = lstImportConfiguration.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.JavaScriptStringEncode(User.Name)  + "ImportConfiguration"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.JavaScriptStringEncode(User.Name)  + "ImportConfiguration"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
			var _ImportConfiguration = lstImportConfiguration;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_ImportConfiguration.Count() > 0)
                    pageSize = _ImportConfiguration.Count();
                return View("ExcelExport", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
            }
           if (IsMobileRequest != true)
            {
			if (Request.AcceptTypes.Contains("text/html"))
            {
				 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
					return View(_ImportConfiguration.ToPagedList(pageNumber, pageSize));
				 else
				 {
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _ImportConfiguration.ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
				 }
            }
            else if (Request.AcceptTypes.Contains("application/json"))
            {
				 var Result = getImportConfigurationList(_ImportConfiguration);
				 return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			}
			}
			else
            {
                var Result = getImportConfigurationList(_ImportConfiguration);
				 return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_ImportConfiguration.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _ImportConfiguration.ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /ImportConfiguration/Details/5
		 		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
            if (importconfiguration == null)
            {
                return HttpNotFound();
            }
            if (Request.AcceptTypes.Contains("text/html"))
            {
                return View(importconfiguration);
            }
            else if (Request.AcceptTypes.Contains("application/json"))
            {
                var Result = getImportConfigurationItem(importconfiguration);
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View(importconfiguration);
        }
        // GET: /ImportConfiguration/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
            if (Request.AcceptTypes.Contains("text/html"))
            {
                return View();
            }
            else if (Request.AcceptTypes.Contains("application/json"))
            {
                var Result = getImportConfigurationItem(new ImportConfiguration());
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
		// GET: /ImportConfiguration/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
		  ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
            return View();
        }
		// POST: /ImportConfiguration/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name")] ImportConfiguration importconfiguration,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
			importconfiguration.LastUpdate = DateTime.Now;
            importconfiguration.LastUpdateUser = User.Name;
                db.ImportConfigurations.Add(importconfiguration);
                db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            return View(importconfiguration);
        }
		 // GET: /ImportConfiguration/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		  ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
            return View();
        }
		  // POST: /ImportConfiguration/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,Name")] ImportConfiguration importconfiguration,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
			importconfiguration.LastUpdate = DateTime.Now;
            importconfiguration.LastUpdateUser = User.Name;
                db.ImportConfigurations.Add(importconfiguration);
                db.SaveChanges();
				//if (IsAddPop != null && IsAddPop == true)
                  //  return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                //else
				   //return Redirect(Request.UrlReferrer.ToString());
				return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
		            return View(importconfiguration);
        }
		public ActionResult Cancel(string UrlReferrer)
        {
                 if (!string.IsNullOrEmpty(UrlReferrer))
                 {
                     var uri = new Uri(UrlReferrer);
                     var query = HttpUtility.ParseQueryString(uri.Query);
                     if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                         return RedirectToAction("Index");
                     else
                        return Redirect(UrlReferrer);
                 }
                 else
                     return RedirectToAction("Index");
        }
        // POST: /ImportConfiguration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name")] ImportConfiguration importconfiguration,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
			importconfiguration.LastUpdate = DateTime.Now;
            importconfiguration.LastUpdateUser = User.Name;
                db.ImportConfigurations.Add(importconfiguration);
                db.SaveChanges();
								 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = importconfiguration.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            return View(importconfiguration);
        }
        // GET: /ImportConfiguration/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
            if (importconfiguration == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
		if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration/Edit/" + importconfiguration.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration/Create"))
			ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
            if (Request.AcceptTypes.Contains("text/html"))
            {
                return View(importconfiguration);
            }
            else if (Request.AcceptTypes.Contains("application/json"))
            {
                var Result = getImportConfigurationItem(importconfiguration);
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
          return View(importconfiguration);
        }
        // POST: /ImportConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name")] ImportConfiguration importconfiguration,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
			importconfiguration.LastUpdate = DateTime.Now;
            importconfiguration.LastUpdateUser = User.Name;
                db.Entry(importconfiguration).State = EntityState.Modified;
                    db.SaveChanges();
				 if (command == "Save & Continue")
                    return RedirectToAction("Edit", new { Id = importconfiguration.Id, UrlReferrer = UrlReferrer });
                 if (!string.IsNullOrEmpty(UrlReferrer))
                 {
                     var uri = new Uri(UrlReferrer);
                     var query = HttpUtility.ParseQueryString(uri.Query);
                     if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                         return RedirectToAction("Index");
                     else
                        return Redirect(UrlReferrer);
                 }
                 else
                     return RedirectToAction("Index");
            }
            return View(importconfiguration);
        }
		// GET: /ImportConfiguration/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
			importconfiguration.LastUpdate = DateTime.Now;
            importconfiguration.LastUpdateUser = User.Name;
            if (importconfiguration == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["ImportConfigurationParentUrl"] = UrlReferrer;
		if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration"))
			ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
          return View(importconfiguration);
        }
        // POST: /ImportConfiguration/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,TableColumn,SheetColumn,UniqueColumn,LastUpdate,LastUpdateUser,Name")] ImportConfiguration importconfiguration,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(importconfiguration).State = EntityState.Modified;
                    db.SaveChanges();
                 if (!string.IsNullOrEmpty(UrlReferrer))
                 {
                     var uri = new Uri(UrlReferrer);
                     var query = HttpUtility.ParseQueryString(uri.Query);
                     if(Convert.ToBoolean(query.Get("IsFilter")) == true)
                         return RedirectToAction("Index");
                     else
                        return Redirect(UrlReferrer);
                 }
                 else
                     return RedirectToAction("Index");
            }
            return View(importconfiguration);
        }
        // GET: /ImportConfiguration/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
            if (importconfiguration == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["ImportConfigurationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ImportConfiguration"))
			 ViewData["ImportConfigurationParentUrl"] = Request.UrlReferrer;
            return View(importconfiguration);
        }
        // POST: /ImportConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(ImportConfiguration importconfiguration, string UrlReferrer)
        {
			if (!User.CanDelete("ImportConfiguration"))
            {
                return RedirectToAction("Index", "Error");
            }
            //ImportConfiguration importconfiguration = db.ImportConfigurations.Find(id);
			db.Entry(importconfiguration).State = EntityState.Deleted;
            db.ImportConfigurations.Remove(importconfiguration);
                    db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["ImportConfigurationParentUrl"] != null)
            {
                string parentUrl = ViewData["ImportConfigurationParentUrl"].ToString();
                ViewData["ImportConfigurationParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
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

