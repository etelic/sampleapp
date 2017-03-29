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
    public partial class T_SessionController : BaseController
    {
        // GET: /T_Session/
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
			var lstT_Session = from s in db.T_Sessions
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_Session = searchRecords(lstT_Session, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_Session = sortRecords(lstT_Session, sortBy, isAsc);
            }
            else lstT_Session = lstT_Session.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Session"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Session"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Session"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Session"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_Session = lstT_Session.Include(t=>t.t_sessionlearningcenterassociation).Include(t=>t.t_sessiontimeslotassociaton).Include(t=>t.schedulesession);
		 if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_SessionLearningCenterAssociation")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Session = _T_Session.Where(p => p.T_SessionLearningCenterAssociationID == hostid);
			 }
			 else
			     _T_Session = _T_Session.Where(p => p.T_SessionLearningCenterAssociationID == null);
         }
		 if (HostingEntity == "T_TimeSlots" && AssociatedType == "T_SessionTimeSlotAssociaton")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Session = _T_Session.Where(p => p.T_SessionTimeSlotAssociatonID == hostid);
			 }
			 else
			     _T_Session = _T_Session.Where(p => p.T_SessionTimeSlotAssociatonID == null);
         }
		 if (HostingEntity == "T_Schedule" && AssociatedType == "ScheduleSession")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Session = _T_Session.Where(p => p.ScheduleSessionID == hostid);
			 }
			 else
			     _T_Session = _T_Session.Where(p => p.ScheduleSessionID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_Session.Count() > 0)
                    pageSize = _T_Session.Count();
                return View("ExcelExport", _T_Session.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_Session.Count();
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
				return View(_T_Session.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_Session = _fad.FilterDropdown<T_Session>(User,  _T_Session, "T_Session", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_Session.Except(_T_Session),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_Session.Except(_T_Session).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_Session.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_Session.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_Session.Count() == 0 ? 1 : _T_Session.Count();
                                
                            }
							return PartialView("IndexPartial", _T_Session.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_Session.Count() == 0 ? 1 : _T_Session.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_Session.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_Session/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Session t_session = db.T_Sessions.Find(id);
            if (t_session == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_session);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_session, AssociatedType);
            ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnDetails", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnDetails", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnDetails");
			return View(ViewBag.TemplatesName,t_session);

        }

        // GET: /T_Session/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype , string slotID)
        {
            if (!User.CanAdd("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (!string.IsNullOrEmpty(slotID))
            {
                HostingEntityName = "T_TimeSlots";
                HostingEntityID = slotID;
                AssociatedType = "T_SessionTimeSlotAssociaton";
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_SessionParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
		  ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");
          return View();
        }
		// GET: /T_Session/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_SessionParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
			 ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");
            return View();
        }
		// POST: /T_Session/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session, string UrlReferrer)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
           db.T_Sessions.Add(t_session);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");	
            return View(t_session);
        }
		 // GET: /T_Session/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_SessionParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");
            return View();
        }
		  // POST: /T_Session/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
           db.T_Sessions.Add(t_session);
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
			LoadViewDataAfterOnCreate(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_session, AssociatedEntity);
			return View(t_session);
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
        // POST: /T_Session/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_Sessions.Add(t_session);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_session.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnCreate", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnCreate", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnCreate");
            return View(t_session);
        }
		// GET: /T_Session/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_Session"))
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
            T_Session t_session = db.T_Sessions.Find(id);
            if (t_session == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_SessionParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session/Edit/" + t_session.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session/Create"))
			ViewData["T_SessionParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_session);
		   ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
		   ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
          return View(t_session);
        }
		// POST: /T_Session/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
			 if(t_session.schedulesession != null && t_session.schedulesession.Id == 0)
                   db.Entry(t_session.schedulesession).State = EntityState.Added;
             else if(t_session.schedulesession != null && t_session.schedulesession.Id > 0)
                  db.Entry(t_session.schedulesession).State = EntityState.Modified;
                db.Entry(t_session).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
            return View(t_session);
        }


        // GET: /T_Session/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Session t_session = db.T_Sessions.Find(id);
            if (t_session == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_SessionParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session/Edit/" + t_session.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session/Create"))
			ViewData["T_SessionParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_session);
		   ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
		   ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
          return View(t_session);
        }
        // POST: /T_Session/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session,  string UrlReferrer)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
			 if(t_session.schedulesession != null && t_session.schedulesession.Id == 0)
                   db.Entry(t_session.schedulesession).State = EntityState.Added;
             else if(t_session.schedulesession != null && t_session.schedulesession.Id > 0)
                  db.Entry(t_session.schedulesession).State = EntityState.Modified;
                db.Entry(t_session).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_session.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
            return View(t_session);
        }
		// GET: /T_Session/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_Session"))
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
			            T_Session t_session = db.T_Sessions.Find(id);
            if (t_session == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_SessionParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session"))
			ViewData["T_SessionParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_session);
			 ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
			 ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
          return View(t_session);
        }
        // POST: /T_Session/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session,string UrlReferrer)
        {
			CheckBeforeSave(t_session);
            if (ModelState.IsValid)
            {
			 if(t_session.schedulesession !=null && t_session.schedulesession.Id == 0)
                   db.Entry(t_session.schedulesession).State = EntityState.Added;
             else if(t_session.schedulesession !=null && t_session.schedulesession.Id > 0)
                  db.Entry(t_session.schedulesession).State = EntityState.Modified;
                db.Entry(t_session).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_session);
			ViewBag.T_SessionIsHiddenRule = checkHidden("T_Session", "OnEdit", false);
			ViewBag.T_SessionIsGroupsHiddenRule = checkHidden("T_Session", "OnEdit", true);
			ViewBag.T_SessionIsSetValueUIRule = checkSetValueUIRule("T_Session", "OnEdit");
            return View(t_session);
        }
        // GET: /T_Session/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_Session t_session = db.T_Sessions.Find(id);
            if (t_session == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_SessionParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Session"))
			 ViewData["T_SessionParentUrl"] = Request.UrlReferrer;
            return View(t_session);
        }
        // POST: /T_Session/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_Session t_session, string UrlReferrer)
        {
			if (!User.CanDelete("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_session))
            {
 //Delete Document
				try
                {
                    foreach (var evt in db.T_SessionEventss.Where(p => p.ScheduleID == t_session.ScheduleSessionID))
                        db.T_SessionEventss.Remove(evt);
                    db.T_Schedules.Remove(db.T_Schedules.Find(t_session.ScheduleSessionID));
                    db.SaveChanges();
                }
                catch { 
                }
			db.Entry(t_session).State = EntityState.Deleted;


            db.T_Sessions.Remove(t_session);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_SessionParentUrl"] != null)
            {
                string parentUrl = ViewData["T_SessionParentUrl"].ToString();
                ViewData["T_SessionParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_session);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_SessionLearningCenterAssociation")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Session obj = db.T_Sessions.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_SessionLearningCenterAssociationID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_TimeSlots" && AssociatedType == "T_SessionTimeSlotAssociaton")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Session obj = db.T_Sessions.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_SessionTimeSlotAssociatonID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_Schedule" && AssociatedType == "ScheduleSession")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Session obj = db.T_Sessions.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.ScheduleSessionID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
				T_Session t_session = db.T_Sessions.Find(id);
                db.Entry(t_session).State = EntityState.Deleted;
                db.T_Sessions.Remove(t_session);
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
            if (!User.CanEdit("T_Session"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_SessionParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_SessionLearningCenterAssociationID,T_Name,T_Description,T_SessionTimeSlotAssociatonID,T_IsOpen,ScheduleSessionID,schedulesession")] T_Session t_session,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_Session target = db.T_Sessions.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_session, target, chkUpdate); 
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
