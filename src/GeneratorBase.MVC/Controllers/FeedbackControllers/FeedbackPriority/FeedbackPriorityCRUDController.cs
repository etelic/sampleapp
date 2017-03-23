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
    public partial class FeedbackPriorityController : BaseController
    {
        // GET: /FeedbackPriority/
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
			var lstFeedbackPriority = from s in db.FeedbackPrioritys
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstFeedbackPriority = searchRecords(lstFeedbackPriority, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstFeedbackPriority = sortRecords(lstFeedbackPriority, sortBy, isAsc);
            }
            else lstFeedbackPriority = lstFeedbackPriority.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackPriority"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackPriority"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackPriority"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackPriority"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _FeedbackPriority = lstFeedbackPriority;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_FeedbackPriority.Count() > 0)
                    pageSize = _FeedbackPriority.Count();
                return View("ExcelExport", _FeedbackPriority.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_FeedbackPriority.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _FeedbackPriority.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _FeedbackPriority.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /FeedbackPriority/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackPriority feedbackpriority = db.FeedbackPrioritys.Find(id);
            if (feedbackpriority == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
            return View(feedbackpriority);
        }
        // GET: /FeedbackPriority/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["FeedbackPriorityParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
          return View();
        }
		// GET: /FeedbackPriority/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
		    ViewData["FeedbackPriorityParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		// POST: /FeedbackPriority/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackPriority feedbackpriority,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackPrioritys.Add(feedbackpriority);
           db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
			LoadViewDataAfterOnCreate(feedbackpriority);	
            return View(feedbackpriority);
        }
		 // GET: /FeedbackPriority/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["FeedbackPriorityParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		  // POST: /FeedbackPriority/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackPriority feedbackpriority,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackPrioritys.Add(feedbackpriority);
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
			LoadViewDataAfterOnCreate(feedbackpriority);
            return View(feedbackpriority);
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
        // POST: /FeedbackPriority/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackPriority feedbackpriority,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.FeedbackPrioritys.Add(feedbackpriority);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = feedbackpriority.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(feedbackpriority);
            return View(feedbackpriority);
        }
        // GET: /FeedbackPriority/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackPriority feedbackpriority = db.FeedbackPrioritys.Find(id);
            if (feedbackpriority == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["FeedbackPriorityParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackPriorityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackPriority")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackPriority/Edit/" + feedbackpriority.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackPriority/Create"))
			ViewData["FeedbackPriorityParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(feedbackpriority);
          return View(feedbackpriority);
        }
        // POST: /FeedbackPriority/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackPriority feedbackpriority,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(feedbackpriority).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = feedbackpriority.Id, UrlReferrer = UrlReferrer });
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
			LoadViewDataAfterOnEdit(feedbackpriority);
            return View(feedbackpriority);
        }
		// GET: /FeedbackPriority/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackPriority feedbackpriority = db.FeedbackPrioritys.Find(id);
            if (feedbackpriority == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["FeedbackPriorityParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackPriorityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackPriority"))
			ViewData["FeedbackPriorityParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(feedbackpriority);
          return View(feedbackpriority);
        }
        // POST: /FeedbackPriority/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,Name,Description")] FeedbackPriority feedbackpriority,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackpriority).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(feedbackpriority);
            return View(feedbackpriority);
        }
        // GET: /FeedbackPriority/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            FeedbackPriority feedbackpriority = db.FeedbackPrioritys.Find(id);
            if (feedbackpriority == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["FeedbackPriorityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackPriority"))
			 ViewData["FeedbackPriorityParentUrl"] = Request.UrlReferrer;
            return View(feedbackpriority);
        }
        // POST: /FeedbackPriority/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(FeedbackPriority feedbackpriority, string UrlReferrer)
        {
			if (!User.CanDelete("FeedbackPriority"))
            {
                return RedirectToAction("Index", "Error");
            }
			db.Entry(feedbackpriority).State = EntityState.Deleted;
            db.FeedbackPrioritys.Remove(feedbackpriority);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["FeedbackPriorityParentUrl"] != null)
            {
                string parentUrl = ViewData["FeedbackPriorityParentUrl"].ToString();
                ViewData["FeedbackPriorityParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 FeedbackPriority feedbackpriority = db.FeedbackPrioritys.Find(id);
                db.Entry(feedbackpriority).State = EntityState.Deleted;
                db.FeedbackPrioritys.Remove(feedbackpriority);
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
