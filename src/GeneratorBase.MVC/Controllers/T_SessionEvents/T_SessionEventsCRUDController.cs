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
    public partial class T_SessionEventsController : BaseController
    {
        // GET: /T_SessionEvents/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate, string viewtype,string isMobileHome)
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
			var lstT_SessionEvents = from s in db.T_SessionEventss
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_SessionEvents = searchRecords(lstT_SessionEvents, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_SessionEvents = sortRecords(lstT_SessionEvents, sortBy, isAsc);
            }
            else lstT_SessionEvents = lstT_SessionEvents.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionEvents"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionEvents"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionEvents"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionEvents"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_SessionEvents = lstT_SessionEvents.Include(t=>t.t_sessioneventslearningcenter).Include(t=>t.schedule).Include(t=>t.t_sessioneventstimeslots);
		 if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_SessionEventsLearningCenter")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionEvents = _T_SessionEvents.Where(p => p.T_SessionEventsLearningCenterID == hostid);
			 }
			 else
			     _T_SessionEvents = _T_SessionEvents.Where(p => p.T_SessionEventsLearningCenterID == null);
         }
		 if (HostingEntity == "T_Schedule" && AssociatedType == "Schedule")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionEvents = _T_SessionEvents.Where(p => p.ScheduleID == hostid);
			 }
			 else
			     _T_SessionEvents = _T_SessionEvents.Where(p => p.ScheduleID == null);
         }
		 if (HostingEntity == "T_TimeSlots" && AssociatedType == "T_SessionEventsTimeSlots")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionEvents = _T_SessionEvents.Where(p => p.T_SessionEventsTimeSlotsID == hostid);
			 }
			 else
			     _T_SessionEvents = _T_SessionEvents.Where(p => p.T_SessionEventsTimeSlotsID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_SessionEvents.Count() > 0)
                    pageSize = _T_SessionEvents.Count();
                return View("ExcelExport", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_SessionEvents.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if (pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
			 }
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
			 {
				if (string.IsNullOrEmpty(viewtype))
                    viewtype = "IndexPartial";
                GetTemplatesForList(viewtype);
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
				return View(_T_SessionEvents.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_SessionEvents = _fad.FilterDropdown<T_SessionEvents>(User,  _T_SessionEvents, "T_SessionEvents", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_SessionEvents.Except(_T_SessionEvents),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_SessionEvents.Except(_T_SessionEvents).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_SessionEvents.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_SessionEvents.Count() == 0 ? 1 : _T_SessionEvents.Count();
                                
                            }
							return PartialView("IndexPartial", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_SessionEvents.Count() == 0 ? 1 : _T_SessionEvents.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_SessionEvents.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_SessionEvents/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
            if (t_sessionevents == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_sessionevents);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_sessionevents, AssociatedType);
            ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnDetails", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnDetails", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnDetails");
			return View(ViewBag.TemplatesName,t_sessionevents);

        }

        // GET: /T_SessionEvents/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype )
        {
            if (!User.CanAdd("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
		  ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");
          return View();
        }
		// GET: /T_SessionEvents/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
			 ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");
            return View();
        }
		// POST: /T_SessionEvents/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents, string UrlReferrer)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
           db.T_SessionEventss.Add(t_sessionevents);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");	
            return View(t_sessionevents);
        }
		 // GET: /T_SessionEvents/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");
            return View();
        }
		  // POST: /T_SessionEvents/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
           db.T_SessionEventss.Add(t_sessionevents);
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
			LoadViewDataAfterOnCreate(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_sessionevents, AssociatedEntity);
			return View(t_sessionevents);
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
        // POST: /T_SessionEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_SessionEventss.Add(t_sessionevents);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_sessionevents.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnCreate", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnCreate");
            return View(t_sessionevents);
        }
		// GET: /T_SessionEvents/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "EditQuick";
            GetTemplatesForEditQuick(viewtype);
            T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
            if (t_sessionevents == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents/Edit/" + t_sessionevents.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents/Create"))
			ViewData["T_SessionEventsParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessionevents);
		   ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
		   ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
          return View(t_sessionevents);
        }
		// POST: /T_SessionEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_sessionevents).State = EntityState.Modified;
               db.SaveChanges();

			  var result = new { Result = "Succeed", UrlRefr =UrlReferrer };
               return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
				var result = new { Result = "fail", UrlRefr = errors };
                return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
			
			LoadViewDataAfterOnEdit(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
            return View(t_sessionevents);
        }


        // GET: /T_SessionEvents/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
            if (t_sessionevents == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents/Edit/" + t_sessionevents.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents/Create"))
			ViewData["T_SessionEventsParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessionevents);
		   ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
		   ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
          return View(t_sessionevents);
        }
        // POST: /T_SessionEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents,  string UrlReferrer)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_sessionevents).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_sessionevents.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
            return View(t_sessionevents);
        }
		// GET: /T_SessionEvents/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "EditWizard";
            GetTemplatesForEditWizard(viewtype);
			            T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
            if (t_sessionevents == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents"))
			ViewData["T_SessionEventsParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_sessionevents);
			 ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
			 ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
          return View(t_sessionevents);
        }
        // POST: /T_SessionEvents/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents,string UrlReferrer)
        {
			CheckBeforeSave(t_sessionevents);
            if (ModelState.IsValid)
            {
                db.Entry(t_sessionevents).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_sessionevents);
			ViewBag.T_SessionEventsIsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", false);
			ViewBag.T_SessionEventsIsGroupsHiddenRule = checkHidden("T_SessionEvents", "OnEdit", true);
			ViewBag.T_SessionEventsIsSetValueUIRule = checkSetValueUIRule("T_SessionEvents", "OnEdit");
            return View(t_sessionevents);
        }
        // GET: /T_SessionEvents/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
            if (t_sessionevents == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_SessionEventsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEvents"))
			 ViewData["T_SessionEventsParentUrl"] = Request.UrlReferrer;
            return View(t_sessionevents);
        }
        // POST: /T_SessionEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_SessionEvents t_sessionevents, string UrlReferrer)
        {
			if (!User.CanDelete("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_sessionevents))
            {
 //Delete Document
			db.Entry(t_sessionevents).State = EntityState.Deleted;


            db.T_SessionEventss.Remove(t_sessionevents);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_SessionEventsParentUrl"] != null)
            {
                string parentUrl = ViewData["T_SessionEventsParentUrl"].ToString();
                ViewData["T_SessionEventsParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_sessionevents);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_SessionEventsLearningCenter")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_SessionEvents obj = db.T_SessionEventss.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_SessionEventsLearningCenterID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_Schedule" && AssociatedType == "Schedule")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_SessionEvents obj = db.T_SessionEventss.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.ScheduleID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_TimeSlots" && AssociatedType == "T_SessionEventsTimeSlots")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_SessionEvents obj = db.T_SessionEventss.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_SessionEventsTimeSlotsID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
				T_SessionEvents t_sessionevents = db.T_SessionEventss.Find(id);
                db.Entry(t_sessionevents).State = EntityState.Deleted;
                db.T_SessionEventss.Remove(t_sessionevents);
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
            if (!User.CanEdit("T_SessionEvents"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_SessionEventsParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Title,T_EventDate,T_SessionEventsLearningCenterID,T_StartTime,T_EndTime,T_IsCancelled,T_Description,ScheduleID,T_SessionEventsTimeSlotsID")] T_SessionEvents t_sessionevents,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_SessionEvents target = db.T_SessionEventss.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_sessionevents, target, chkUpdate); 
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
