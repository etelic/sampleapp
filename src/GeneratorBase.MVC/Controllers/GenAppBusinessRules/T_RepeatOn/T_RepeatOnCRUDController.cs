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
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
    public partial class T_RepeatOnController : BaseController
    {
        // GET: /T_RepeatOn/
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
			var lstT_RepeatOn = from s in db.T_RepeatOns
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_RepeatOn = searchRecords(lstT_RepeatOn, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_RepeatOn = sortRecords(lstT_RepeatOn, sortBy, isAsc);
            }
            else lstT_RepeatOn = lstT_RepeatOn.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RepeatOn"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RepeatOn"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RepeatOn"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RepeatOn"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_RepeatOn = lstT_RepeatOn.Include(t=>t.t_schedule).Include(t=>t.t_recurrencedays);
		 if (HostingEntity == "T_Schedule" && AssociatedType == "T_RepeatOn_T_Schedule")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_RepeatOn = _T_RepeatOn.Where(p => p.T_ScheduleID == hostid);
			 }
			 else
			     _T_RepeatOn = _T_RepeatOn.Where(p => p.T_ScheduleID == null);
         }
		 if (HostingEntity == "T_RecurrenceDays" && AssociatedType == "T_RepeatOn_T_RecurrenceDays")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_RepeatOn = _T_RepeatOn.Where(p => p.T_RecurrenceDaysID == hostid);
			 }
			 else
			     _T_RepeatOn = _T_RepeatOn.Where(p => p.T_RecurrenceDaysID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_RepeatOn.Count() > 0)
                    pageSize = _T_RepeatOn.Count();
                return View("ExcelExport", _T_RepeatOn.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
			 {
			  GetTemplatesForList();
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
				return View(_T_RepeatOn.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_RepeatOn = _fad.FilterDropdown<T_RepeatOn>(User,  _T_RepeatOn, "T_RepeatOn", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_RepeatOn.Except(_T_RepeatOn),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_RepeatOn.Except(_T_RepeatOn).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_RepeatOn.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_RepeatOn.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{
					  if (ViewBag.TemplatesName == null)
                        return PartialView("IndexPartial", _T_RepeatOn.ToPagedList(pageNumber, pageSize));
					  else
						return PartialView(ViewBag.TemplatesName, _T_RepeatOn.ToPagedList(pageNumber, pageSize));
					}
				}
        }
		 // GET: /T_RepeatOn/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
            if (t_repeaton == null)
            {
                return HttpNotFound();
            }
			GetTemplatesForDetails();
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_repeaton);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_repeaton, AssociatedType);
            ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnDetails");
			return View(ViewBag.TemplatesName,t_repeaton);

        }
        // GET: /T_RepeatOn/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn","OnCreate");
          return View();
        }
		// GET: /T_RepeatOn/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnCreate");
            return View();
        }
		// POST: /T_RepeatOn/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton, string UrlReferrer)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
           db.T_RepeatOns.Add(t_repeaton);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_repeaton);	
            return View(t_repeaton);
        }
		 // GET: /T_RepeatOn/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnCreate");
            return View();
        }
		  // POST: /T_RepeatOn/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton, List<string> T_ScheduleIDSelected, List<string> T_ScheduleIDAvailable, List<string> T_RecurrenceDaysIDSelected, List<string> T_RecurrenceDaysIDAvailable,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
					if(T_ScheduleIDSelected != null || T_ScheduleIDAvailable != null)
				{
					var deletedItems = db.T_RepeatOns.Where(p => p.T_RecurrenceDaysID == t_repeaton.T_RecurrenceDaysID).ToList();
                    foreach (var item in deletedItems)
                    {
                        db.T_RepeatOns.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ScheduleIDSelected != null)
						foreach (string id in T_ScheduleIDSelected)
						{
							var obj = new T_RepeatOn(); 
							obj = t_repeaton;
							obj.T_ScheduleID = Convert.ToInt64(id);
							db.T_RepeatOns.Add(obj); db.SaveChanges();
						}
				}
				if(T_RecurrenceDaysIDSelected != null || T_RecurrenceDaysIDAvailable != null)
				{
					var deletedItems = db.T_RepeatOns.Where(p => p.T_ScheduleID == t_repeaton.T_ScheduleID).ToList();
                    foreach (var item in deletedItems)
                    {
                        db.T_RepeatOns.Remove(item);
                        db.SaveChanges();
                    }
					if(T_RecurrenceDaysIDSelected != null)
						foreach (string id in T_RecurrenceDaysIDSelected)
						{
							var obj = new T_RepeatOn(); 
							obj = t_repeaton;
							obj.T_RecurrenceDaysID = Convert.ToInt64(id);
							db.T_RepeatOns.Add(obj); db.SaveChanges();
						}
				}
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
			LoadViewDataAfterOnCreate(t_repeaton);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_repeaton, AssociatedEntity);
			return View(t_repeaton);
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
        // POST: /T_RepeatOn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton, List<string> T_ScheduleIDSelected, List<string> T_ScheduleIDAvailable, List<string> T_RecurrenceDaysIDSelected, List<string> T_RecurrenceDaysIDAvailable, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
				if(T_ScheduleIDSelected != null || T_ScheduleIDAvailable != null)
				{
					var deletedItems = db.T_RepeatOns.Where(p => p.T_RecurrenceDaysID == t_repeaton.T_RecurrenceDaysID).ToList();
                    foreach (var item in deletedItems)
                    {
                        db.T_RepeatOns.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ScheduleIDSelected != null)
						foreach (string id in T_ScheduleIDSelected)
						{
							var obj = new T_RepeatOn(); 
							obj = t_repeaton;
							obj.T_ScheduleID = Convert.ToInt64(id);
							db.T_RepeatOns.Add(obj); db.SaveChanges();
						}
				}
				if(T_RecurrenceDaysIDSelected != null || T_RecurrenceDaysIDAvailable != null)
				{
					var deletedItems = db.T_RepeatOns.Where(p => p.T_ScheduleID == t_repeaton.T_ScheduleID).ToList();
                    foreach (var item in deletedItems)
                    {
                        db.T_RepeatOns.Remove(item);
                        db.SaveChanges();
                    }
					if(T_RecurrenceDaysIDSelected != null)
						foreach (string id in T_RecurrenceDaysIDSelected)
						{
							var obj = new T_RepeatOn(); 
							obj = t_repeaton;
							obj.T_RecurrenceDaysID = Convert.ToInt64(id);
							db.T_RepeatOns.Add(obj); db.SaveChanges();
						}
				}
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_repeaton.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_repeaton);
            return View(t_repeaton);
        }
		// GET: /T_RepeatOn/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
            if (t_repeaton == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
		if(ViewData["T_RepeatOnParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn/Edit/" + t_repeaton.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn/Create"))
			ViewData["T_RepeatOnParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_repeaton);
		   ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnEdit");
          return View(t_repeaton);
        }
		// POST: /T_RepeatOn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_repeaton).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_repeaton);
            return View(t_repeaton);
        }


        // GET: /T_RepeatOn/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
            if (t_repeaton == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
		if(ViewData["T_RepeatOnParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn/Edit/" + t_repeaton.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn/Create"))
			ViewData["T_RepeatOnParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_repeaton);
		   ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnEdit");
          return View(t_repeaton);
        }
        // POST: /T_RepeatOn/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton,  string UrlReferrer)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_repeaton).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_repeaton.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_repeaton);
            return View(t_repeaton);
        }
		// GET: /T_RepeatOn/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
            if (t_repeaton == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
		if(ViewData["T_RepeatOnParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn"))
			ViewData["T_RepeatOnParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_repeaton);
			 ViewBag.T_RepeatOnIsHiddenRule = checkHidden("T_RepeatOn", "OnEdit");
          return View(t_repeaton);
        }
        // POST: /T_RepeatOn/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton,string UrlReferrer)
        {
			CheckBeforeSave(t_repeaton);
            if (ModelState.IsValid)
            {
                db.Entry(t_repeaton).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_repeaton);
            return View(t_repeaton);
        }
        // GET: /T_RepeatOn/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
            if (t_repeaton == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_RepeatOnParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RepeatOn"))
			 ViewData["T_RepeatOnParentUrl"] = Request.UrlReferrer;
            return View(t_repeaton);
        }
        // POST: /T_RepeatOn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_RepeatOn t_repeaton, string UrlReferrer)
        {
			if (!User.CanDelete("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_repeaton))
            {
 //Delete Document
			db.Entry(t_repeaton).State = EntityState.Deleted;
            db.T_RepeatOns.Remove(t_repeaton);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_RepeatOnParentUrl"] != null)
            {
                string parentUrl = ViewData["T_RepeatOnParentUrl"].ToString();
                ViewData["T_RepeatOnParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_repeaton);
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
			 T_RepeatOn t_repeaton = db.T_RepeatOns.Find(id);
                db.Entry(t_repeaton).State = EntityState.Deleted;
                db.T_RepeatOns.Remove(t_repeaton);
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
            if (!User.CanEdit("T_RepeatOn"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_RepeatOnParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_ScheduleID,T_RecurrenceDaysID")] T_RepeatOn t_repeaton,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_RepeatOn target = db.T_RepeatOns.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_repeaton, target, chkUpdate); 
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
