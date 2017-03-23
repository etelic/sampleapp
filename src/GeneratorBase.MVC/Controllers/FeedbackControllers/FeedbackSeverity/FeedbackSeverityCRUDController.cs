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
    public partial class FeedbackSeverityController : BaseController
    {
        // GET: /FeedbackSeverity/
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
			var lstFeedbackSeverity = from s in db.FeedbackSeveritys
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstFeedbackSeverity = searchRecords(lstFeedbackSeverity, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstFeedbackSeverity = sortRecords(lstFeedbackSeverity, sortBy, isAsc);
            }
            else lstFeedbackSeverity = lstFeedbackSeverity.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackSeverity"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackSeverity"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackSeverity"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackSeverity"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _FeedbackSeverity = lstFeedbackSeverity;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_FeedbackSeverity.Count() > 0)
                    pageSize = _FeedbackSeverity.Count();
                return View("ExcelExport", _FeedbackSeverity.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_FeedbackSeverity.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _FeedbackSeverity.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _FeedbackSeverity.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /FeedbackSeverity/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackSeverity feedbackseverity = db.FeedbackSeveritys.Find(id);
            if (feedbackseverity == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
            return View(feedbackseverity);
        }
        // GET: /FeedbackSeverity/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["FeedbackSeverityParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
          return View();
        }
		// GET: /FeedbackSeverity/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
		    ViewData["FeedbackSeverityParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		// POST: /FeedbackSeverity/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackSeverity feedbackseverity,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackSeveritys.Add(feedbackseverity);
           db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
			LoadViewDataAfterOnCreate(feedbackseverity);	
            return View(feedbackseverity);
        }
		 // GET: /FeedbackSeverity/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["FeedbackSeverityParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		  // POST: /FeedbackSeverity/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackSeverity feedbackseverity,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackSeveritys.Add(feedbackseverity);
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
			LoadViewDataAfterOnCreate(feedbackseverity);
            return View(feedbackseverity);
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
        // POST: /FeedbackSeverity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackSeverity feedbackseverity,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.FeedbackSeveritys.Add(feedbackseverity);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = feedbackseverity.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(feedbackseverity);
            return View(feedbackseverity);
        }
        // GET: /FeedbackSeverity/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackSeverity feedbackseverity = db.FeedbackSeveritys.Find(id);
            if (feedbackseverity == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["FeedbackSeverityParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackSeverityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackSeverity")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackSeverity/Edit/" + feedbackseverity.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackSeverity/Create"))
			ViewData["FeedbackSeverityParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(feedbackseverity);
          return View(feedbackseverity);
        }
        // POST: /FeedbackSeverity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackSeverity feedbackseverity,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(feedbackseverity).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = feedbackseverity.Id, UrlReferrer = UrlReferrer });
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
			LoadViewDataAfterOnEdit(feedbackseverity);
            return View(feedbackseverity);
        }
		// GET: /FeedbackSeverity/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackSeverity feedbackseverity = db.FeedbackSeveritys.Find(id);
            if (feedbackseverity == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["FeedbackSeverityParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackSeverityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackSeverity"))
			ViewData["FeedbackSeverityParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(feedbackseverity);
          return View(feedbackseverity);
        }
        // POST: /FeedbackSeverity/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackSeverity feedbackseverity,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackseverity).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(feedbackseverity);
            return View(feedbackseverity);
        }
        // GET: /FeedbackSeverity/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            FeedbackSeverity feedbackseverity = db.FeedbackSeveritys.Find(id);
            if (feedbackseverity == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["FeedbackSeverityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackSeverity"))
			 ViewData["FeedbackSeverityParentUrl"] = Request.UrlReferrer;
            return View(feedbackseverity);
        }
        // POST: /FeedbackSeverity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(FeedbackSeverity feedbackseverity, string UrlReferrer)
        {
			if (!User.CanDelete("FeedbackSeverity"))
            {
                return RedirectToAction("Index", "Error");
            }
			db.Entry(feedbackseverity).State = EntityState.Deleted;
            db.FeedbackSeveritys.Remove(feedbackseverity);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["FeedbackSeverityParentUrl"] != null)
            {
                string parentUrl = ViewData["FeedbackSeverityParentUrl"].ToString();
                ViewData["FeedbackSeverityParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 FeedbackSeverity feedbackseverity = db.FeedbackSeveritys.Find(id);
                db.Entry(feedbackseverity).State = EntityState.Deleted;
                db.FeedbackSeveritys.Remove(feedbackseverity);
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
