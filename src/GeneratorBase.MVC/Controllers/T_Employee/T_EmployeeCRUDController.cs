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
    public partial class T_EmployeeController : BaseController
    {
			private ApplicationDbContext UserContext = new ApplicationDbContext();
        // GET: /T_Employee/
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
			var lstT_Employee = from s in db.T_Employees
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_Employee = searchRecords(lstT_Employee, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_Employee = sortRecords(lstT_Employee, sortBy, isAsc);
            }
            else lstT_Employee = lstT_Employee.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Employee"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Employee"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Employee"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Employee"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_Employee = lstT_Employee.Include(t=>t.t_associatedemployeetype).Include(t=>t.t_associatedemployeestatus).Include(t=>t.t_employeeuserlogin).Include(t=>t.t_employeeaddress);
		 if (HostingEntity == "T_Employeetype" && AssociatedType == "T_AssociatedEmployeeType")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Employee = _T_Employee.Where(p => p.T_AssociatedEmployeeTypeID == hostid);
			 }
			 else
			     _T_Employee = _T_Employee.Where(p => p.T_AssociatedEmployeeTypeID == null);
         }
		 if (HostingEntity == "T_Employeestatus" && AssociatedType == "T_AssociatedEmployeeStatus")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Employee = _T_Employee.Where(p => p.T_AssociatedEmployeeStatusID == hostid);
			 }
			 else
			     _T_Employee = _T_Employee.Where(p => p.T_AssociatedEmployeeStatusID == null);
         }
		 if (HostingEntity == "T_Address" && AssociatedType == "T_EmployeeAddress")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_Employee = _T_Employee.Where(p => p.T_EmployeeAddressID == hostid);
			 }
			 else
			     _T_Employee = _T_Employee.Where(p => p.T_EmployeeAddressID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_Employee.Count() > 0)
                    pageSize = _T_Employee.Count();
                return View("ExcelExport", _T_Employee.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_Employee.Count();
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
				return View(_T_Employee.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_Employee = _fad.FilterDropdown<T_Employee>(User,  _T_Employee, "T_Employee", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_Employee.Except(_T_Employee),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_Employee.Except(_T_Employee).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_Employee.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_Employee.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_Employee.Count() == 0 ? 1 : _T_Employee.Count();
                                
                            }
							return PartialView("IndexPartial", _T_Employee.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_Employee.Count() == 0 ? 1 : _T_Employee.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_Employee.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_Employee/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Employee t_employee = db.T_Employees.Find(id);
            if (t_employee == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_employee);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_employee, AssociatedType);
            ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnDetails");
			ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnDetails");
			return View(ViewBag.TemplatesName,t_employee);

        }



        // GET: /T_Employee/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype)
        {
            if (!User.CanAdd("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_EmployeeParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee","OnCreate");
		  ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnCreate");
          return View();
        }
		// GET: /T_Employee/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_EmployeeParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnCreate");
			 ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnCreate");
            return View();
        }
		// POST: /T_Employee/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase T_Picture, string UrlReferrer)
        {
			CheckBeforeSave(t_employee);
		if (T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(T_Picture);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (T_Picture != null)
                { 
									long documentID = SaveDocument(T_Picture);
                    t_employee.T_Picture = documentID ;
                }
           db.T_Employees.Add(t_employee);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_employee);	
            return View(t_employee);
        }
		 // GET: /T_Employee/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_EmployeeParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnCreate");
			ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnCreate");
            return View();
        }
		  // POST: /T_Employee/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase T_Picture , String CamerafileUploadT_Picture,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_employee);
		if (T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(T_Picture);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (T_Picture != null)
                { 
					long documentID = SaveDocument(T_Picture);
                    t_employee.T_Picture = documentID;
                }
				if (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"]  != "")
                {
                 System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_employee.T_Picture = documentIDCamara;
                }
           db.T_Employees.Add(t_employee);
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
			LoadViewDataAfterOnCreate(t_employee);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_employee, AssociatedEntity);
			return View(t_employee);
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
        // POST: /T_Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase T_Picture , String CamerafileUploadT_Picture, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_employee);
		if (T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(T_Picture);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (T_Picture != null)
                { 
					long documentID = SaveDocument(T_Picture);
					 t_employee.T_Picture = documentID;
                }
			    if (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"]  != "")
                {
					System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_employee.T_Picture = documentIDCamara;
                }
           db.T_Employees.Add(t_employee);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_employee.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_employee);
            return View(t_employee);
        }
		// GET: /T_Employee/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_Employee"))
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
            T_Employee t_employee = db.T_Employees.Find(id);
            if (t_employee == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_EmployeeParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee/Edit/" + t_employee.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee/Create"))
			ViewData["T_EmployeeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_employee);
		   ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnEdit");
		   ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnEdit");
          return View(t_employee);
        }
		// POST: /T_Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_Picture,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase File_T_Picture , String CamerafileUploadT_Picture,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_employee);
		if (File_T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(File_T_Picture);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (File_T_Picture != null)
                { 
					long documentID = 0;
					if (t_employee.T_Picture != null)
                        documentID = UpdateDocument(File_T_Picture, t_employee.T_Picture);
                    else
                        documentID = SaveDocument(File_T_Picture);
					t_employee.T_Picture = documentID;
                }
				if (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != "")
                {
					 System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if (t_employee.T_Picture != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, t_employee.T_Picture);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_employee.T_Picture = documentIDCamara;
                }
			 if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id == 0)
                   db.Entry(t_employee.t_employeeaddress).State = EntityState.Added;
             else if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id > 0)
                  db.Entry(t_employee.t_employeeaddress).State = EntityState.Modified;
                db.Entry(t_employee).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_employee);
            return View(t_employee);
        }


        // GET: /T_Employee/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Employee t_employee = db.T_Employees.Find(id);
            if (t_employee == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_EmployeeParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee/Edit/" + t_employee.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee/Create"))
			ViewData["T_EmployeeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_employee);
		   ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnEdit");
		   ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnEdit");
          return View(t_employee);
        }
        // POST: /T_Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_Picture,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase File_T_Picture , String CamerafileUploadT_Picture,  string UrlReferrer)
        {
			CheckBeforeSave(t_employee);
		if (File_T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(File_T_Picture);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (File_T_Picture != null)
                { 
					long documentID = 0;
					if (t_employee.T_Picture != null)
                        documentID = UpdateDocument(File_T_Picture, t_employee.T_Picture);
                    else
                        documentID = SaveDocument(File_T_Picture);
					t_employee.T_Picture = documentID;
                }
				if (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != "")
                {
					 System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    string fileext = ".jpeg";
                    string fileName = ticks + "Camera-" + ticks + "-" + User.Name;
                    byte[] bytes = Convert.FromBase64String(Request.Form["CamerafileUploadT_Picture"]);
                    long _contentLength = bytes.Length;
                    int Imglen = bytes.Length;
                    long documentIDCamara = 0;
                    if (t_employee.T_Picture != null)
                        documentIDCamara = UpdateDocumentCamera(fileext, fileName, bytes, _contentLength, Imglen, t_employee.T_Picture);
                    else
                        documentIDCamara = SaveDocumentCameraImage(fileext, fileName, bytes, _contentLength, Imglen);
                    t_employee.T_Picture = documentIDCamara;
                }
			 if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id == 0)
                   db.Entry(t_employee.t_employeeaddress).State = EntityState.Added;
             else if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id > 0)
                  db.Entry(t_employee.t_employeeaddress).State = EntityState.Modified;
                db.Entry(t_employee).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_employee.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_employee);
            return View(t_employee);
        }
		// GET: /T_Employee/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_Employee"))
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
			            T_Employee t_employee = db.T_Employees.Find(id);
            if (t_employee == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_EmployeeParentUrl"] = UrlReferrer;
		if(ViewData["T_EmployeeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee"))
			ViewData["T_EmployeeParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_employee);
			 ViewBag.T_EmployeeIsHiddenRule = checkHidden("T_Employee", "OnEdit");
			 ViewBag.T_EmployeeIsSetValueUIRule = checkSetValueUIRule("T_Employee", "OnEdit");
          return View(t_employee);
        }
        // POST: /T_Employee/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_Picture,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase File_T_Picture , String CamerafileUploadT_Picture,string UrlReferrer)
        {
			CheckBeforeSave(t_employee);
		if (File_T_Picture != null || (Request.Form["CamerafileUploadT_Picture"] != null && Request.Form["CamerafileUploadT_Picture"] != ""))
                IsFileTypeAndSizeAllowed(File_T_Picture);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (File_T_Picture != null)
                { 
					long documentID = 0;
					if (t_employee.T_Picture != null)
                        documentID = UpdateDocument(File_T_Picture, t_employee.T_Picture);
                    else
                        documentID = SaveDocument(File_T_Picture);
                }
			 if(t_employee.t_employeeaddress !=null && t_employee.t_employeeaddress.Id == 0)
                   db.Entry(t_employee.t_employeeaddress).State = EntityState.Added;
             else if(t_employee.t_employeeaddress !=null && t_employee.t_employeeaddress.Id > 0)
                  db.Entry(t_employee.t_employeeaddress).State = EntityState.Modified;
                db.Entry(t_employee).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_employee);
            return View(t_employee);
        }
        // GET: /T_Employee/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_Employee t_employee = db.T_Employees.Find(id);
            if (t_employee == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_EmployeeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Employee"))
			 ViewData["T_EmployeeParentUrl"] = Request.UrlReferrer;
            return View(t_employee);
        }
        // POST: /T_Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_Employee t_employee, string UrlReferrer)
        {
			if (!User.CanDelete("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_employee))
            {
 //Delete Document
			if (t_employee.T_Picture != null)
            {
                DeleteDocument(t_employee.T_Picture);
            }
			db.Entry(t_employee).State = EntityState.Deleted;
            db.T_Employees.Remove(t_employee);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_EmployeeParentUrl"] != null)
            {
                string parentUrl = ViewData["T_EmployeeParentUrl"].ToString();
                ViewData["T_EmployeeParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_employee);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_Employeetype" && AssociatedType == "T_AssociatedEmployeeType")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Employee obj = db.T_Employees.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_AssociatedEmployeeTypeID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_Employeestatus" && AssociatedType == "T_AssociatedEmployeeStatus")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Employee obj = db.T_Employees.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_AssociatedEmployeeStatusID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_Address" && AssociatedType == "T_EmployeeAddress")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_Employee obj = db.T_Employees.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_EmployeeAddressID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
				T_Employee t_employee = db.T_Employees.Find(id);
                db.Entry(t_employee).State = EntityState.Deleted;
                db.T_Employees.Remove(t_employee);
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
            if (!User.CanEdit("T_Employee"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_EmployeeParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Title,T_FirstName,T_MiddleName,T_LastName,T_Email,T_PhoneNo,T_IdentificationNo,T_Picture,T_AssociatedEmployeeTypeID,T_AssociatedEmployeeStatusID,T_EmployeeUserLoginID,T_EmployeeAddressID,t_employeeaddress")] T_Employee t_employee, HttpPostedFileBase File_T_Picture , String CamerafileUploadT_Picture,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_Employee target = db.T_Employees.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_employee, target, chkUpdate); 
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
