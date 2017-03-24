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
    public partial class T_RecurringScheduleDetailstypeController : BaseController
    {
        // GET: /T_RecurringScheduleDetailstype/
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
			var lstT_RecurringScheduleDetailstype = from s in db.T_RecurringScheduleDetailstypes
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_RecurringScheduleDetailstype = searchRecords(lstT_RecurringScheduleDetailstype, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_RecurringScheduleDetailstype = sortRecords(lstT_RecurringScheduleDetailstype, sortBy, isAsc);
            }
            else lstT_RecurringScheduleDetailstype = lstT_RecurringScheduleDetailstype.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurringScheduleDetailstype"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurringScheduleDetailstype"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurringScheduleDetailstype"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurringScheduleDetailstype"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_RecurringScheduleDetailstype = lstT_RecurringScheduleDetailstype;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_RecurringScheduleDetailstype.Count() > 0)
                    pageSize = _T_RecurringScheduleDetailstype.Count();
                return View("ExcelExport", _T_RecurringScheduleDetailstype.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
			 {
			  GetTemplatesForList();
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
				return View(_T_RecurringScheduleDetailstype.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_RecurringScheduleDetailstype = _fad.FilterDropdown<T_RecurringScheduleDetailstype>(User,  _T_RecurringScheduleDetailstype, "T_RecurringScheduleDetailstype", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_RecurringScheduleDetailstype.Except(_T_RecurringScheduleDetailstype),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_RecurringScheduleDetailstype.Except(_T_RecurringScheduleDetailstype).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_RecurringScheduleDetailstype.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_RecurringScheduleDetailstype.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{
					  if (ViewBag.TemplatesName == null)
                        return PartialView("IndexPartial", _T_RecurringScheduleDetailstype.ToPagedList(pageNumber, pageSize));
					  else
						return PartialView(ViewBag.TemplatesName, _T_RecurringScheduleDetailstype.ToPagedList(pageNumber, pageSize));
					}
				}
        }
		 // GET: /T_RecurringScheduleDetailstype/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
            if (t_recurringscheduledetailstype == null)
            {
                return HttpNotFound();
            }
			GetTemplatesForDetails();
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_recurringscheduledetailstype);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_recurringscheduledetailstype, AssociatedType);
            ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnDetails");
			return View(ViewBag.TemplatesName,t_recurringscheduledetailstype);

        }
        // GET: /T_RecurringScheduleDetailstype/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype","OnCreate");
          return View();
        }
		// GET: /T_RecurringScheduleDetailstype/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnCreate");
            return View();
        }
		// POST: /T_RecurringScheduleDetailstype/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype, string UrlReferrer)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
           db.T_RecurringScheduleDetailstypes.Add(t_recurringscheduledetailstype);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_recurringscheduledetailstype);	
            return View(t_recurringscheduledetailstype);
        }
		 // GET: /T_RecurringScheduleDetailstype/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnCreate");
            return View();
        }
		  // POST: /T_RecurringScheduleDetailstype/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
           db.T_RecurringScheduleDetailstypes.Add(t_recurringscheduledetailstype);
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
			LoadViewDataAfterOnCreate(t_recurringscheduledetailstype);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_recurringscheduledetailstype, AssociatedEntity);
			return View(t_recurringscheduledetailstype);
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
        // POST: /T_RecurringScheduleDetailstype/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_RecurringScheduleDetailstypes.Add(t_recurringscheduledetailstype);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_recurringscheduledetailstype.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_recurringscheduledetailstype);
            return View(t_recurringscheduledetailstype);
        }
		// GET: /T_RecurringScheduleDetailstype/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
            if (t_recurringscheduledetailstype == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringScheduleDetailstypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype/Edit/" + t_recurringscheduledetailstype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype/Create"))
			ViewData["T_RecurringScheduleDetailstypeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurringscheduledetailstype);
		   ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnEdit");
          return View(t_recurringscheduledetailstype);
        }
		// POST: /T_RecurringScheduleDetailstype/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurringscheduledetailstype).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_recurringscheduledetailstype);
            return View(t_recurringscheduledetailstype);
        }


        // GET: /T_RecurringScheduleDetailstype/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
            if (t_recurringscheduledetailstype == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringScheduleDetailstypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype/Edit/" + t_recurringscheduledetailstype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype/Create"))
			ViewData["T_RecurringScheduleDetailstypeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurringscheduledetailstype);
		   ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnEdit");
          return View(t_recurringscheduledetailstype);
        }
        // POST: /T_RecurringScheduleDetailstype/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype,  string UrlReferrer)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurringscheduledetailstype).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_recurringscheduledetailstype.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_recurringscheduledetailstype);
            return View(t_recurringscheduledetailstype);
        }
		// GET: /T_RecurringScheduleDetailstype/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
            if (t_recurringscheduledetailstype == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringScheduleDetailstypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype"))
			ViewData["T_RecurringScheduleDetailstypeParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_recurringscheduledetailstype);
			 ViewBag.T_RecurringScheduleDetailstypeIsHiddenRule = checkHidden("T_RecurringScheduleDetailstype", "OnEdit");
          return View(t_recurringscheduledetailstype);
        }
        // POST: /T_RecurringScheduleDetailstype/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype,string UrlReferrer)
        {
			CheckBeforeSave(t_recurringscheduledetailstype);
            if (ModelState.IsValid)
            {
                db.Entry(t_recurringscheduledetailstype).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_recurringscheduledetailstype);
            return View(t_recurringscheduledetailstype);
        }
        // GET: /T_RecurringScheduleDetailstype/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
            if (t_recurringscheduledetailstype == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_RecurringScheduleDetailstypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringScheduleDetailstype"))
			 ViewData["T_RecurringScheduleDetailstypeParentUrl"] = Request.UrlReferrer;
            return View(t_recurringscheduledetailstype);
        }
        // POST: /T_RecurringScheduleDetailstype/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_RecurringScheduleDetailstype t_recurringscheduledetailstype, string UrlReferrer)
        {
			if (!User.CanDelete("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_recurringscheduledetailstype))
            {
 //Delete Document
			db.Entry(t_recurringscheduledetailstype).State = EntityState.Deleted;
            db.T_RecurringScheduleDetailstypes.Remove(t_recurringscheduledetailstype);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_RecurringScheduleDetailstypeParentUrl"] != null)
            {
                string parentUrl = ViewData["T_RecurringScheduleDetailstypeParentUrl"].ToString();
                ViewData["T_RecurringScheduleDetailstypeParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_recurringscheduledetailstype);
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
			 T_RecurringScheduleDetailstype t_recurringscheduledetailstype = db.T_RecurringScheduleDetailstypes.Find(id);
                db.Entry(t_recurringscheduledetailstype).State = EntityState.Deleted;
                db.T_RecurringScheduleDetailstypes.Remove(t_recurringscheduledetailstype);
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
            if (!User.CanEdit("T_RecurringScheduleDetailstype"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_RecurringScheduleDetailstypeParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringScheduleDetailstype t_recurringscheduledetailstype,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_RecurringScheduleDetailstype target = db.T_RecurringScheduleDetailstypes.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_recurringscheduledetailstype, target, chkUpdate); 
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
