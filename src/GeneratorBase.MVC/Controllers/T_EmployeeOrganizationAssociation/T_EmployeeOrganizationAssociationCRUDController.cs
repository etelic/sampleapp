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
    public partial class T_EmployeeOrganizationAssociationController : BaseController
    {
        // GET: /T_EmployeeOrganizationAssociation/
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
			var lstT_EmployeeOrganizationAssociation = from s in db.T_EmployeeOrganizationAssociations
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_EmployeeOrganizationAssociation = searchRecords(lstT_EmployeeOrganizationAssociation, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_EmployeeOrganizationAssociation = sortRecords(lstT_EmployeeOrganizationAssociation, sortBy, isAsc);
            }
            else lstT_EmployeeOrganizationAssociation = lstT_EmployeeOrganizationAssociation.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_EmployeeOrganizationAssociation"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_EmployeeOrganizationAssociation"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_EmployeeOrganizationAssociation"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_EmployeeOrganizationAssociation"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_EmployeeOrganizationAssociation = lstT_EmployeeOrganizationAssociation.Include(t=>t.t_employee).Include(t=>t.t_organization);
		 if (HostingEntity == "T_Employee" && AssociatedType == "T_EmployeeOrganizationAssociation_T_Employee")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_EmployeeOrganizationAssociation = _T_EmployeeOrganizationAssociation.Where(p => p.T_EmployeeID == hostid);
			 }
			 else
			     _T_EmployeeOrganizationAssociation = _T_EmployeeOrganizationAssociation.Where(p => p.T_EmployeeID == null);
         }
		 if (HostingEntity == "T_Organization" && AssociatedType == "T_EmployeeOrganizationAssociation_T_Organization")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_EmployeeOrganizationAssociation = _T_EmployeeOrganizationAssociation.Where(p => p.T_OrganizationID == hostid);
			 }
			 else
			     _T_EmployeeOrganizationAssociation = _T_EmployeeOrganizationAssociation.Where(p => p.T_OrganizationID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_EmployeeOrganizationAssociation.Count() > 0)
                    pageSize = _T_EmployeeOrganizationAssociation.Count();
                return View("ExcelExport", _T_EmployeeOrganizationAssociation.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_EmployeeOrganizationAssociation.Count();
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
				return View(_T_EmployeeOrganizationAssociation.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_EmployeeOrganizationAssociation = _fad.FilterDropdown<T_EmployeeOrganizationAssociation>(User,  _T_EmployeeOrganizationAssociation, "T_EmployeeOrganizationAssociation", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_EmployeeOrganizationAssociation.Except(_T_EmployeeOrganizationAssociation),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_EmployeeOrganizationAssociation.Except(_T_EmployeeOrganizationAssociation).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_EmployeeOrganizationAssociation.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_EmployeeOrganizationAssociation.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_EmployeeOrganizationAssociation.Count() == 0 ? 1 : _T_EmployeeOrganizationAssociation.Count();
                                
                            }
							return PartialView("IndexPartial", _T_EmployeeOrganizationAssociation.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_EmployeeOrganizationAssociation.Count() == 0 ? 1 : _T_EmployeeOrganizationAssociation.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_EmployeeOrganizationAssociation.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_EmployeeOrganizationAssociation/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
            if (t_employeeorganizationassociation == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_employeeorganizationassociation);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_employeeorganizationassociation, AssociatedType);
            ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnDetails");
			ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnDetails");
			return View(ViewBag.TemplatesName,t_employeeorganizationassociation);

        }



        // GET: /T_EmployeeOrganizationAssociation/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype)
        {
            if (!User.CanAdd("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation","OnCreate");
		  ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnCreate");
          return View();
        }
		// GET: /T_EmployeeOrganizationAssociation/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnCreate");
			 ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnCreate");
            return View();
        }
		// POST: /T_EmployeeOrganizationAssociation/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation, string UrlReferrer)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
           db.T_EmployeeOrganizationAssociations.Add(t_employeeorganizationassociation);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_employeeorganizationassociation);	
            return View(t_employeeorganizationassociation);
        }
		 // GET: /T_EmployeeOrganizationAssociation/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnCreate");
			ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnCreate");
            return View();
        }
		  // POST: /T_EmployeeOrganizationAssociation/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation, List<string> T_EmployeeIDSelected, List<string> T_EmployeeIDAvailable, List<string> T_OrganizationIDSelected, List<string> T_OrganizationIDAvailable,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
					if(T_EmployeeIDSelected != null || T_EmployeeIDAvailable != null)
				{
					var T_EmployeeIDSelectedlong =T_EmployeeIDSelected !=null ? T_EmployeeIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_EmployeeOrganizationAssociations.Where(p => p.T_OrganizationID == t_employeeorganizationassociation.T_OrganizationID && !T_EmployeeIDSelectedlong.Contains(p.T_EmployeeID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_organization !=null )
					    db.Entry(item.t_organization).State = EntityState.Unchanged;
						if(item.t_employee !=null )
                        db.Entry(item.t_employee).State = EntityState.Unchanged;
                        db.T_EmployeeOrganizationAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_EmployeeIDSelected != null)
						foreach (string id in T_EmployeeIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_EmployeeOrganizationAssociations.FirstOrDefault(a => a.T_OrganizationID == t_employeeorganizationassociation.T_OrganizationID && a.T_EmployeeID == longid) == null)
                            {
								var obj = new T_EmployeeOrganizationAssociation(); 
								obj = t_employeeorganizationassociation;
								obj.T_EmployeeID = Convert.ToInt64(id);
								db.T_EmployeeOrganizationAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_OrganizationIDSelected != null || T_OrganizationIDAvailable != null)
				{
					var T_OrganizationIDSelectedlong =T_OrganizationIDSelected !=null ? T_OrganizationIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_EmployeeOrganizationAssociations.Where(p => p.T_EmployeeID == t_employeeorganizationassociation.T_EmployeeID && !T_OrganizationIDSelectedlong.Contains(p.T_OrganizationID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_employee !=null )
					    db.Entry(item.t_employee).State = EntityState.Unchanged;
						if(item.t_organization !=null )
                        db.Entry(item.t_organization).State = EntityState.Unchanged;
                        db.T_EmployeeOrganizationAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_OrganizationIDSelected != null)
						foreach (string id in T_OrganizationIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_EmployeeOrganizationAssociations.FirstOrDefault(a => a.T_EmployeeID == t_employeeorganizationassociation.T_EmployeeID && a.T_OrganizationID == longid) == null)
                            {
								var obj = new T_EmployeeOrganizationAssociation(); 
								obj = t_employeeorganizationassociation;
								obj.T_OrganizationID = Convert.ToInt64(id);
								db.T_EmployeeOrganizationAssociations.Add(obj); db.SaveChanges();
							}
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
			LoadViewDataAfterOnCreate(t_employeeorganizationassociation);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_employeeorganizationassociation, AssociatedEntity);
			return View(t_employeeorganizationassociation);
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
        // POST: /T_EmployeeOrganizationAssociation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation, List<string> T_EmployeeIDSelected, List<string> T_EmployeeIDAvailable, List<string> T_OrganizationIDSelected, List<string> T_OrganizationIDAvailable, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
				if(T_EmployeeIDSelected != null || T_EmployeeIDAvailable != null)
				{
					var T_EmployeeIDSelectedlong =T_EmployeeIDSelected !=null ? T_EmployeeIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_EmployeeOrganizationAssociations.Where(p => p.T_OrganizationID == t_employeeorganizationassociation.T_OrganizationID && !T_EmployeeIDSelectedlong.Contains(p.T_EmployeeID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_organization !=null )
						db.Entry(item.t_organization).State = EntityState.Unchanged;
					if(item.t_employee !=null )
                        db.Entry(item.t_employee).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_EmployeeOrganizationAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_EmployeeIDSelected != null)
						foreach (string id in T_EmployeeIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_EmployeeOrganizationAssociations.FirstOrDefault(a => a.T_OrganizationID == t_employeeorganizationassociation.T_OrganizationID && a.T_EmployeeID == longid) == null)
                            {
								var obj = new T_EmployeeOrganizationAssociation(); 
								obj = t_employeeorganizationassociation;
								obj.T_EmployeeID = Convert.ToInt64(id);
								db.T_EmployeeOrganizationAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_OrganizationIDSelected != null || T_OrganizationIDAvailable != null)
				{
					var T_OrganizationIDSelectedlong =T_OrganizationIDSelected !=null ? T_OrganizationIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_EmployeeOrganizationAssociations.Where(p => p.T_EmployeeID == t_employeeorganizationassociation.T_EmployeeID && !T_OrganizationIDSelectedlong.Contains(p.T_OrganizationID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_employee !=null )
						db.Entry(item.t_employee).State = EntityState.Unchanged;
					if(item.t_organization !=null )
                        db.Entry(item.t_organization).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_EmployeeOrganizationAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_OrganizationIDSelected != null)
						foreach (string id in T_OrganizationIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_EmployeeOrganizationAssociations.FirstOrDefault(a => a.T_EmployeeID == t_employeeorganizationassociation.T_EmployeeID && a.T_OrganizationID == longid) == null)
                            {
								var obj = new T_EmployeeOrganizationAssociation(); 
								obj = t_employeeorganizationassociation;
								obj.T_OrganizationID = Convert.ToInt64(id);
								db.T_EmployeeOrganizationAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_employeeorganizationassociation.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_employeeorganizationassociation);
            return View(t_employeeorganizationassociation);
        }
		// GET: /T_EmployeeOrganizationAssociation/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_EmployeeOrganizationAssociation"))
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
            T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
            if (t_employeeorganizationassociation == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeOrganizationAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation/Edit/" + t_employeeorganizationassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation/Create"))
			ViewData["T_EmployeeOrganizationAssociationParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_employeeorganizationassociation);
		   ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnEdit");
		   ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnEdit");
          return View(t_employeeorganizationassociation);
        }
		// POST: /T_EmployeeOrganizationAssociation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_employeeorganizationassociation).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_employeeorganizationassociation);
            return View(t_employeeorganizationassociation);
        }


        // GET: /T_EmployeeOrganizationAssociation/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
            if (t_employeeorganizationassociation == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeOrganizationAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation/Edit/" + t_employeeorganizationassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation/Create"))
			ViewData["T_EmployeeOrganizationAssociationParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_employeeorganizationassociation);
		   ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnEdit");
		   ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnEdit");
          return View(t_employeeorganizationassociation);
        }
        // POST: /T_EmployeeOrganizationAssociation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation,  string UrlReferrer)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_employeeorganizationassociation).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_employeeorganizationassociation.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_employeeorganizationassociation);
            return View(t_employeeorganizationassociation);
        }
		// GET: /T_EmployeeOrganizationAssociation/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_EmployeeOrganizationAssociation"))
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
			            T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
            if (t_employeeorganizationassociation == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeOrganizationAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation"))
			ViewData["T_EmployeeOrganizationAssociationParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_employeeorganizationassociation);
			 ViewBag.T_EmployeeOrganizationAssociationIsHiddenRule = checkHidden("T_EmployeeOrganizationAssociation", "OnEdit");
			 ViewBag.T_EmployeeOrganizationAssociationIsSetValueUIRule = checkSetValueUIRule("T_EmployeeOrganizationAssociation", "OnEdit");
          return View(t_employeeorganizationassociation);
        }
        // POST: /T_EmployeeOrganizationAssociation/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation,string UrlReferrer)
        {
			CheckBeforeSave(t_employeeorganizationassociation);
            if (ModelState.IsValid)
            {
                db.Entry(t_employeeorganizationassociation).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_employeeorganizationassociation);
            return View(t_employeeorganizationassociation);
        }
        // GET: /T_EmployeeOrganizationAssociation/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
            if (t_employeeorganizationassociation == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_EmployeeOrganizationAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_EmployeeOrganizationAssociation"))
			 ViewData["T_EmployeeOrganizationAssociationParentUrl"] = Request.UrlReferrer;
            return View(t_employeeorganizationassociation);
        }
        // POST: /T_EmployeeOrganizationAssociation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_EmployeeOrganizationAssociation t_employeeorganizationassociation, string UrlReferrer)
        {
			if (!User.CanDelete("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_employeeorganizationassociation))
            {
 //Delete Document
			db.Entry(t_employeeorganizationassociation).State = EntityState.Deleted;
            db.T_EmployeeOrganizationAssociations.Remove(t_employeeorganizationassociation);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_EmployeeOrganizationAssociationParentUrl"] != null)
            {
                string parentUrl = ViewData["T_EmployeeOrganizationAssociationParentUrl"].ToString();
                ViewData["T_EmployeeOrganizationAssociationParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_employeeorganizationassociation);
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
				T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);
                db.Entry(t_employeeorganizationassociation).State = EntityState.Deleted;
                db.T_EmployeeOrganizationAssociations.Remove(t_employeeorganizationassociation);
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
            if (!User.CanEdit("T_EmployeeOrganizationAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_EmployeeOrganizationAssociationParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_EmployeeID,T_OrganizationID")] T_EmployeeOrganizationAssociation t_employeeorganizationassociation,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_EmployeeOrganizationAssociation target = db.T_EmployeeOrganizationAssociations.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_employeeorganizationassociation, target, chkUpdate); 
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
