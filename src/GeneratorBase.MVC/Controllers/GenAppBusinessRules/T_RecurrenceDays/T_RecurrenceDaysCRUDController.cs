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
    public partial class T_RecurrenceDaysController : BaseController
    {
        // GET: /T_RecurrenceDays/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype)
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
			ViewData["caller"] = caller;
			  if (!string.IsNullOrEmpty(viewtype))
            {
                viewtype = viewtype.Replace("?IsAddPop=true", "");
                ViewBag.TemplatesName = viewtype;
            }
			if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
			var lstT_RecurrenceDays = from s in db.T_RecurrenceDayss
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_RecurrenceDays = searchRecords(lstT_RecurrenceDays, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_RecurrenceDays = sortRecords(lstT_RecurrenceDays, sortBy, isAsc);
            }
            else lstT_RecurrenceDays = lstT_RecurrenceDays.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurrenceDays"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurrenceDays"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurrenceDays"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurrenceDays"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_RecurrenceDays = lstT_RecurrenceDays;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_RecurrenceDays.Count() > 0)
                    pageSize = _T_RecurrenceDays.Count();
                return View("ExcelExport", _T_RecurrenceDays.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
			 {
			  GetTemplatesForList();
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
				return View(_T_RecurrenceDays.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_RecurrenceDays = _fad.FilterDropdown<T_RecurrenceDays>(User,  _T_RecurrenceDays, "T_RecurrenceDays", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_RecurrenceDays.Except(_T_RecurrenceDays),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_RecurrenceDays.Except(_T_RecurrenceDays).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_RecurrenceDays.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_RecurrenceDays.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{
					  if (ViewBag.TemplatesName == null)
                        return PartialView("IndexPartial", _T_RecurrenceDays.ToPagedList(pageNumber, pageSize));
					  else
						return PartialView(ViewBag.TemplatesName, _T_RecurrenceDays.ToPagedList(pageNumber, pageSize));
					}
				}
        }
		 // GET: /T_RecurrenceDays/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
            if (t_recurrencedays == null)
            {
                return HttpNotFound();
            }
			GetTemplatesForDetails();
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_recurrencedays);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_recurrencedays, AssociatedType);
            ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnDetails");
			return View(ViewBag.TemplatesName,t_recurrencedays);

        }
        // GET: /T_RecurrenceDays/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays","OnCreate");
          return View();
        }
		// GET: /T_RecurrenceDays/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnCreate");
            return View();
        }
		// POST: /T_RecurrenceDays/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays, string UrlReferrer)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
           db.T_RecurrenceDayss.Add(t_recurrencedays);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_recurrencedays);	
            return View(t_recurrencedays);
        }
		 // GET: /T_RecurrenceDays/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnCreate");
            return View();
        }
		  // POST: /T_RecurrenceDays/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
           db.T_RecurrenceDayss.Add(t_recurrencedays);
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
			LoadViewDataAfterOnCreate(t_recurrencedays);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_recurrencedays, AssociatedEntity);
			return View(t_recurrencedays);
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
        // POST: /T_RecurrenceDays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_RecurrenceDayss.Add(t_recurrencedays);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_recurrencedays.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_recurrencedays);
            return View(t_recurrencedays);
        }
		// GET: /T_RecurrenceDays/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
            if (t_recurrencedays == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurrenceDaysParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays/Edit/" + t_recurrencedays.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays/Create"))
			ViewData["T_RecurrenceDaysParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurrencedays);
		   ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnEdit");
          return View(t_recurrencedays);
        }
		// POST: /T_RecurrenceDays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurrencedays).State = EntityState.Modified;
               db.SaveChanges();

			   return Json(UrlReferrer, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
			
			LoadViewDataAfterOnEdit(t_recurrencedays);
            return View(t_recurrencedays);
        }


        // GET: /T_RecurrenceDays/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
            if (t_recurrencedays == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurrenceDaysParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays/Edit/" + t_recurrencedays.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays/Create"))
			ViewData["T_RecurrenceDaysParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurrencedays);
		   ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnEdit");
          return View(t_recurrencedays);
        }
        // POST: /T_RecurrenceDays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays,  string UrlReferrer)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurrencedays).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_recurrencedays.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_recurrencedays);
            return View(t_recurrencedays);
        }
		// GET: /T_RecurrenceDays/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
            if (t_recurrencedays == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurrenceDaysParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays"))
			ViewData["T_RecurrenceDaysParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_recurrencedays);
			 ViewBag.T_RecurrenceDaysIsHiddenRule = checkHidden("T_RecurrenceDays", "OnEdit");
          return View(t_recurrencedays);
        }
        // POST: /T_RecurrenceDays/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays,string UrlReferrer)
        {
			CheckBeforeSave(t_recurrencedays);
            if (ModelState.IsValid)
            {
                db.Entry(t_recurrencedays).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_recurrencedays);
            return View(t_recurrencedays);
        }
        // GET: /T_RecurrenceDays/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
            if (t_recurrencedays == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_RecurrenceDaysParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurrenceDays"))
			 ViewData["T_RecurrenceDaysParentUrl"] = Request.UrlReferrer;
            return View(t_recurrencedays);
        }
        // POST: /T_RecurrenceDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_RecurrenceDays t_recurrencedays, string UrlReferrer)
        {
			if (!User.CanDelete("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_recurrencedays))
            {
 //Delete Document
			db.Entry(t_recurrencedays).State = EntityState.Deleted;
            db.T_RecurrenceDayss.Remove(t_recurrencedays);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_RecurrenceDaysParentUrl"] != null)
            {
                string parentUrl = ViewData["T_RecurrenceDaysParentUrl"].ToString();
                ViewData["T_RecurrenceDaysParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_recurrencedays);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 T_RecurrenceDays t_recurrencedays = db.T_RecurrenceDayss.Find(id);
                db.Entry(t_recurrencedays).State = EntityState.Deleted;
                db.T_RecurrenceDayss.Remove(t_recurrencedays);
                try
                {
                    db.SaveChanges();
                }
                catch { }
			}
			return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
        public ActionResult BulkUpdate(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDUpdate)
        {
            if (!User.CanEdit("T_RecurrenceDays"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_RecurrenceDaysParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            string ids = string.Empty;
            if (Request.QueryString["ids"] != null)
                ids = Request.QueryString["ids"];
            ViewBag.BulkUpdate = ids;
            return View();
        }
        [HttpPost]
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurrenceDays t_recurrencedays,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_RecurrenceDays target = db.T_RecurrenceDayss.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_recurrencedays, target, chkUpdate); 
                db.Entry(target).State = EntityState.Modified;
                try
                    {
                        db.SaveChanges();
                    }
                    catch { }
					            }
			}
            if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
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
