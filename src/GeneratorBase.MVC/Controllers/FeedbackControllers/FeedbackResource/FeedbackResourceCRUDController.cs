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
    public partial class FeedbackResourceController : BaseController
    {
        // GET: /FeedbackResource/
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
			var lstFeedbackResource = from s in db.FeedbackResources
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstFeedbackResource = searchRecords(lstFeedbackResource, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstFeedbackResource = sortRecords(lstFeedbackResource, sortBy, isAsc);
            }
            else lstFeedbackResource = lstFeedbackResource.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackResource"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FeedbackResource"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackResource"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FeedbackResource"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _FeedbackResource = lstFeedbackResource;
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_FeedbackResource.Count() > 0)
                    pageSize = _FeedbackResource.Count();
                return View("ExcelExport", _FeedbackResource.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_FeedbackResource.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
						return PartialView("BulkOperation", _FeedbackResource.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
					else
						return PartialView("IndexPartial", _FeedbackResource.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /FeedbackResource/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackResource feedbackresource = db.FeedbackResources.Find(id);
            if (feedbackresource == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
            return View(feedbackresource);
        }
        // GET: /FeedbackResource/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["FeedbackResourceParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
          return View();
        }
		// GET: /FeedbackResource/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
		    ViewData["FeedbackResourceParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		// POST: /FeedbackResource/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,ResourceId,FirstName,LastName,Email,PhoneNo")] FeedbackResource feedbackresource,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackResources.Add(feedbackresource);
           db.SaveChanges();
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
			LoadViewDataAfterOnCreate(feedbackresource);	
            return View(feedbackresource);
        }
		 // GET: /FeedbackResource/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["FeedbackResourceParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
		  // POST: /FeedbackResource/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,ResourceId,FirstName,LastName,Email,PhoneNo")] FeedbackResource feedbackresource,string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
           db.FeedbackResources.Add(feedbackresource);
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
			LoadViewDataAfterOnCreate(feedbackresource);
            return View(feedbackresource);
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
        // POST: /FeedbackResource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,ResourceId,FirstName,LastName,Email,PhoneNo")] FeedbackResource feedbackresource,string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.FeedbackResources.Add(feedbackresource);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = feedbackresource.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(feedbackresource);
            return View(feedbackresource);
        }
        // GET: /FeedbackResource/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackResource feedbackresource = db.FeedbackResources.Find(id);
            if (feedbackresource == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["FeedbackResourceParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackResourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackResource")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackResource/Edit/" + feedbackresource.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackResource/Create"))
			ViewData["FeedbackResourceParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(feedbackresource);
          return View(feedbackresource);
        }
        // POST: /FeedbackResource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,ResourceId,FirstName,LastName,Email,PhoneNo")] FeedbackResource feedbackresource,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(feedbackresource).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = feedbackresource.Id, UrlReferrer = UrlReferrer });
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
			LoadViewDataAfterOnEdit(feedbackresource);
            return View(feedbackresource);
        }
		// GET: /FeedbackResource/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackResource feedbackresource = db.FeedbackResources.Find(id);
            if (feedbackresource == null)
            {
                return HttpNotFound();
            }
		 if (UrlReferrer != null)
                ViewData["FeedbackResourceParentUrl"] = UrlReferrer;
		if(ViewData["FeedbackResourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackResource"))
			ViewData["FeedbackResourceParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(feedbackresource);
          return View(feedbackresource);
        }
        // POST: /FeedbackResource/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,ResourceId,FirstName,LastName,Email,PhoneNo")] FeedbackResource feedbackresource,string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackresource).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(feedbackresource);
            return View(feedbackresource);
        }
        // GET: /FeedbackResource/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            FeedbackResource feedbackresource = db.FeedbackResources.Find(id);
            if (feedbackresource == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["FeedbackResourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FeedbackResource"))
			 ViewData["FeedbackResourceParentUrl"] = Request.UrlReferrer;
            return View(feedbackresource);
        }
        // POST: /FeedbackResource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
         public ActionResult DeleteConfirmed(FeedbackResource feedbackresource, string UrlReferrer)
        {
			if (!User.CanDelete("FeedbackResource"))
            {
                return RedirectToAction("Index", "Error");
            }
			db.Entry(feedbackresource).State = EntityState.Deleted;
            db.FeedbackResources.Remove(feedbackresource);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["FeedbackResourceParentUrl"] != null)
            {
                string parentUrl = ViewData["FeedbackResourceParentUrl"].ToString();
                ViewData["FeedbackResourceParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 FeedbackResource feedbackresource = db.FeedbackResources.Find(id);
                db.Entry(feedbackresource).State = EntityState.Deleted;
                db.FeedbackResources.Remove(feedbackresource);
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
