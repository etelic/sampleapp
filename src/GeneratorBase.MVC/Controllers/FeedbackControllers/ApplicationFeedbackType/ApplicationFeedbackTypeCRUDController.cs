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
    public partial class ApplicationFeedbackTypeController : BaseController
    {
        // GET: /ApplicationFeedbackType/
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
			var lstApplicationFeedbackType = from s in db.ApplicationFeedbackTypes
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstApplicationFeedbackType = searchRecords(lstApplicationFeedbackType, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstApplicationFeedbackType = sortRecords(lstApplicationFeedbackType, sortBy, isAsc);
            }
            else lstApplicationFeedbackType = lstApplicationFeedbackType.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackType"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "ApplicationFeedbackType"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackType"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedbackType"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _ApplicationFeedbackType = lstApplicationFeedbackType;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_ApplicationFeedbackType.Count() > 0)
                    pageSize = _ApplicationFeedbackType.Count();
                return View("ExcelExport", _ApplicationFeedbackType.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_ApplicationFeedbackType.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _ApplicationFeedbackType.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _ApplicationFeedbackType.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /ApplicationFeedbackType/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackType applicationfeedbacktype = db.ApplicationFeedbackTypes.Find(id);
            if (applicationfeedbacktype == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
            return View(applicationfeedbacktype);
        }
        // GET: /ApplicationFeedbackType/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["ApplicationFeedbackTypeParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
          return View();
        }
		// GET: /ApplicationFeedbackType/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
		    ViewData["ApplicationFeedbackTypeParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		// POST: /ApplicationFeedbackType/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackType applicationfeedbacktype,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
           db.ApplicationFeedbackTypes.Add(applicationfeedbacktype);
           db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
			LoadViewDataAfterOnCreate(applicationfeedbacktype);	
            return View(applicationfeedbacktype);
        }
		 // GET: /ApplicationFeedbackType/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["ApplicationFeedbackTypeParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		  // POST: /ApplicationFeedbackType/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackType applicationfeedbacktype,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
           db.ApplicationFeedbackTypes.Add(applicationfeedbacktype);
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
			LoadViewDataAfterOnCreate(applicationfeedbacktype);
            return View(applicationfeedbacktype);
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
        // POST: /ApplicationFeedbackType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackType applicationfeedbacktype,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.ApplicationFeedbackTypes.Add(applicationfeedbacktype);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = applicationfeedbacktype.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(applicationfeedbacktype);
            return View(applicationfeedbacktype);
        }
        // GET: /ApplicationFeedbackType/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackType applicationfeedbacktype = db.ApplicationFeedbackTypes.Find(id);
            if (applicationfeedbacktype == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["ApplicationFeedbackTypeParentUrl"] = UrlReferrer;
		if(ViewData["ApplicationFeedbackTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackType")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackType/Edit/" + applicationfeedbacktype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackType/Create"))
			ViewData["ApplicationFeedbackTypeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(applicationfeedbacktype);
          return View(applicationfeedbacktype);
        }
        // POST: /ApplicationFeedbackType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackType applicationfeedbacktype,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(applicationfeedbacktype).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = applicationfeedbacktype.Id, UrlReferrer = UrlReferrer });
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
			LoadViewDataAfterOnEdit(applicationfeedbacktype);
            return View(applicationfeedbacktype);
        }
		// GET: /ApplicationFeedbackType/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationFeedbackType applicationfeedbacktype = db.ApplicationFeedbackTypes.Find(id);
            if (applicationfeedbacktype == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["ApplicationFeedbackTypeParentUrl"] = UrlReferrer;
		if(ViewData["ApplicationFeedbackTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackType"))
			ViewData["ApplicationFeedbackTypeParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(applicationfeedbacktype);
          return View(applicationfeedbacktype);
        }
        // POST: /ApplicationFeedbackType/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] ApplicationFeedbackType applicationfeedbacktype,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationfeedbacktype).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(applicationfeedbacktype);
            return View(applicationfeedbacktype);
        }
        // GET: /ApplicationFeedbackType/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            ApplicationFeedbackType applicationfeedbacktype = db.ApplicationFeedbackTypes.Find(id);
            if (applicationfeedbacktype == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["ApplicationFeedbackTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/ApplicationFeedbackType"))
			 ViewData["ApplicationFeedbackTypeParentUrl"] = Request.UrlReferrer;
            return View(applicationfeedbacktype);
        }
        // POST: /ApplicationFeedbackType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(ApplicationFeedbackType applicationfeedbacktype, string UrlReferrer)
        {
			if (!User.CanDelete("ApplicationFeedbackType"))
            {
                return RedirectToAction("Index", "Error");
            }
			db.Entry(applicationfeedbacktype).State = EntityState.Deleted;
            db.ApplicationFeedbackTypes.Remove(applicationfeedbacktype);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["ApplicationFeedbackTypeParentUrl"] != null)
            {
                string parentUrl = ViewData["ApplicationFeedbackTypeParentUrl"].ToString();
                ViewData["ApplicationFeedbackTypeParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 ApplicationFeedbackType applicationfeedbacktype = db.ApplicationFeedbackTypes.Find(id);
                db.Entry(applicationfeedbacktype).State = EntityState.Deleted;
                db.ApplicationFeedbackTypes.Remove(applicationfeedbacktype);
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
