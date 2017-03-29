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
    public partial class T_TimeSlotsController : BaseController
    {
        // GET: /T_TimeSlots/
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
			var lstT_TimeSlots = from s in db.T_TimeSlotss
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_TimeSlots = searchRecords(lstT_TimeSlots, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_TimeSlots = sortRecords(lstT_TimeSlots, sortBy, isAsc);
            }
            else lstT_TimeSlots = lstT_TimeSlots.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_TimeSlots"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_TimeSlots"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_TimeSlots"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_TimeSlots"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_TimeSlots = lstT_TimeSlots.Include(t=>t.t_learningcentertimeslotsassociation);
		 if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_LearningCenterTimeSlotsAssociation")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_TimeSlots = _T_TimeSlots.Where(p => p.T_LearningCenterTimeSlotsAssociationID == hostid);
			 }
			 else
			     _T_TimeSlots = _T_TimeSlots.Where(p => p.T_LearningCenterTimeSlotsAssociationID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_TimeSlots.Count() > 0)
                    pageSize = _T_TimeSlots.Count();
                return View("ExcelExport", _T_TimeSlots.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_TimeSlots.Count();
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
				return View(_T_TimeSlots.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_TimeSlots = _fad.FilterDropdown<T_TimeSlots>(User,  _T_TimeSlots, "T_TimeSlots", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_TimeSlots.Except(_T_TimeSlots),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_TimeSlots.Except(_T_TimeSlots).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_TimeSlots.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_TimeSlots.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_TimeSlots.Count() == 0 ? 1 : _T_TimeSlots.Count();
                                
                            }
							return PartialView("IndexPartial", _T_TimeSlots.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_TimeSlots.Count() == 0 ? 1 : _T_TimeSlots.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_TimeSlots.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_TimeSlots/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
            if (t_timeslots == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_timeslots);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_timeslots, AssociatedType);
            ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnDetails", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnDetails", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnDetails");
			return View(ViewBag.TemplatesName,t_timeslots);

        }

        // GET: /T_TimeSlots/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype )
        {
            if (!User.CanAdd("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
		  ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");
          return View();
        }
		// GET: /T_TimeSlots/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
			 ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");
            return View();
        }
		// POST: /T_TimeSlots/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots, string UrlReferrer)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
           db.T_TimeSlotss.Add(t_timeslots);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");	
            return View(t_timeslots);
        }
		 // GET: /T_TimeSlots/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");
            return View();
        }
		  // POST: /T_TimeSlots/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
           db.T_TimeSlotss.Add(t_timeslots);
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
			LoadViewDataAfterOnCreate(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_timeslots, AssociatedEntity);
			return View(t_timeslots);
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
        // POST: /T_TimeSlots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_TimeSlotss.Add(t_timeslots);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_timeslots.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnCreate", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnCreate");
            return View(t_timeslots);
        }
		// GET: /T_TimeSlots/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_TimeSlots"))
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
            T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
            if (t_timeslots == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
		if(ViewData["T_TimeSlotsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots/Edit/" + t_timeslots.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots/Create"))
			ViewData["T_TimeSlotsParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_timeslots);
		   ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
		   ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
          return View(t_timeslots);
        }
		// POST: /T_TimeSlots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_timeslots).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
            return View(t_timeslots);
        }


        // GET: /T_TimeSlots/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
            if (t_timeslots == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
		if(ViewData["T_TimeSlotsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots/Edit/" + t_timeslots.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots/Create"))
			ViewData["T_TimeSlotsParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_timeslots);
		   ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
		   ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
          return View(t_timeslots);
        }
        // POST: /T_TimeSlots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots,  string UrlReferrer)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_timeslots).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_timeslots.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
            return View(t_timeslots);
        }
		// GET: /T_TimeSlots/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_TimeSlots"))
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
			            T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
            if (t_timeslots == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
		if(ViewData["T_TimeSlotsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots"))
			ViewData["T_TimeSlotsParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_timeslots);
			 ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
			 ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
          return View(t_timeslots);
        }
        // POST: /T_TimeSlots/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots,string UrlReferrer)
        {
			CheckBeforeSave(t_timeslots);
            if (ModelState.IsValid)
            {
                db.Entry(t_timeslots).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_timeslots);
			ViewBag.T_TimeSlotsIsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", false);
			ViewBag.T_TimeSlotsIsGroupsHiddenRule = checkHidden("T_TimeSlots", "OnEdit", true);
			ViewBag.T_TimeSlotsIsSetValueUIRule = checkSetValueUIRule("T_TimeSlots", "OnEdit");
            return View(t_timeslots);
        }
        // GET: /T_TimeSlots/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
            if (t_timeslots == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_TimeSlotsParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_TimeSlots"))
			 ViewData["T_TimeSlotsParentUrl"] = Request.UrlReferrer;
            return View(t_timeslots);
        }
        // POST: /T_TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_TimeSlots t_timeslots, string UrlReferrer)
        {
			if (!User.CanDelete("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_timeslots))
            {
 //Delete Document
			db.Entry(t_timeslots).State = EntityState.Deleted;


            db.T_TimeSlotss.Remove(t_timeslots);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_TimeSlotsParentUrl"] != null)
            {
                string parentUrl = ViewData["T_TimeSlotsParentUrl"].ToString();
                ViewData["T_TimeSlotsParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_timeslots);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_LearningCenter" && AssociatedType == "T_LearningCenterTimeSlotsAssociation")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_TimeSlots obj = db.T_TimeSlotss.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_LearningCenterTimeSlotsAssociationID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
				T_TimeSlots t_timeslots = db.T_TimeSlotss.Find(id);
                db.Entry(t_timeslots).State = EntityState.Deleted;
                db.T_TimeSlotss.Remove(t_timeslots);
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
            if (!User.CanEdit("T_TimeSlots"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_TimeSlotsParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_SlotNo,T_LearningCenterTimeSlotsAssociationID,T_StartTime,T_EndTime")] T_TimeSlots t_timeslots,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_TimeSlots target = db.T_TimeSlotss.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_timeslots, target, chkUpdate); 
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
