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
    public partial class ApplicationFeedbackStatusController : BaseController
    {
        // GET: /ApplicationFeedbackStatus/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent)
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
			var lstApplicationFeedbackStatus = from s in db.ApplicationFeedbackStatuss
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstApplicationFeedbackStatus = searchRecords(lstApplicationFeedbackStatus, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstApplicationFeedbackStatus = sortRecords(lstApplicationFeedbackStatus, sortBy, isAsc);
            }
            else lstApplicationFeedbackStatus = lstApplicationFeedbackStatus.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackStatus"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackStatus"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackStatus"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _ApplicationFeedbackStatus = lstApplicationFeedbackStatus;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_ApplicationFeedbackStatus.Count() > 0)
                    pageSize = _ApplicationFeedbackStatus.Count();
                return View("ExcelExport", _ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _ApplicationFeedbackStatus.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _ApplicationFeedbackStatus.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /ApplicationFeedbackStatus/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
            if (applicationfeedbackstatus == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
            return View(applicationfeedbackstatus);
        }
        // GET: /ApplicationFeedbackStatus/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
          return View();
        }
		// GET: /ApplicationFeedbackStatus/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
		    ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		// POST: /ApplicationFeedbackStatus/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
           db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
           db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
			LoadViewDataAfterOnCreate(applicationfeedbackstatus);	
            return View(applicationfeedbackstatus);
        }
		 // GET: /ApplicationFeedbackStatus/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		  // POST: /ApplicationFeedbackStatus/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
           db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
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
						errors+=error.ErrorMessage+".  ";
					}
				}
				return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			}
			LoadViewDataAfterOnCreate(applicationfeedbackstatus);
            return View(applicationfeedbackstatus);
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
        // POST: /ApplicationFeedbackStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.ApplicationFeedbackStatuss.Add(applicationfeedbackstatus);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = applicationfeedbackstatus.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(applicationfeedbackstatus);
            return View(applicationfeedbackstatus);
        }
        // GET: /ApplicationFeedbackStatus/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
            if (applicationfeedbackstatus == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
		if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus/Edit/" + applicationfeedbackstatus.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus/Create"))
			ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(applicationfeedbackstatus);
          return View(applicationfeedbackstatus);
        }
        // POST: /ApplicationFeedbackStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(applicationfeedbackstatus).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = applicationfeedbackstatus.Id, UrlReferrer = UrlReferrer });
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
			LoadViewDataAfterOnEdit(applicationfeedbackstatus);
            return View(applicationfeedbackstatus);
        }
		// GET: /ApplicationFeedbackStatus/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
            if (applicationfeedbackstatus == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["ApplicationFeedbackStatusParentUrl"] = UrlReferrer;
		if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus"))
			ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(applicationfeedbackstatus);
          return View(applicationfeedbackstatus);
        }
        // POST: /ApplicationFeedbackStatus/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackStatus applicationfeedbackstatus,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationfeedbackstatus).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(applicationfeedbackstatus);
            return View(applicationfeedbackstatus);
        }
        // GET: /ApplicationFeedbackStatus/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
            if (applicationfeedbackstatus == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["ApplicationFeedbackStatusParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackStatus"))
			 ViewData["ApplicationFeedbackStatusParentUrl"] = Request.UrlReferrer;
            return View(applicationfeedbackstatus);
        }
        // POST: /ApplicationFeedbackStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(ApplicationFeedbackStatus applicationfeedbackstatus, string UrlReferrer)
        {
			if (!User.CanDelete("ApplicationFeedbackStatus"))
            {
                return RedirectToAction("Index", "Error");
            }
			db.Entry(applicationfeedbackstatus).State = EntityState.Deleted;
            db.ApplicationFeedbackStatuss.Remove(applicationfeedbackstatus);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["ApplicationFeedbackStatusParentUrl"] != null)
            {
                string parentUrl = ViewData["ApplicationFeedbackStatusParentUrl"].ToString();
                ViewData["ApplicationFeedbackStatusParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 ApplicationFeedbackStatus applicationfeedbackstatus = db.ApplicationFeedbackStatuss.Find(id);
                db.Entry(applicationfeedbackstatus).State = EntityState.Deleted;
                db.ApplicationFeedbackStatuss.Remove(applicationfeedbackstatus);
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
    }
}
