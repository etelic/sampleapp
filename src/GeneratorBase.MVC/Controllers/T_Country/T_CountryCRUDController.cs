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
    public partial class T_CountryController : BaseController
    {
        // GET: /T_Country/
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
			var lstT_Country = from s in db.T_Countrys
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_Country = searchRecords(lstT_Country, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_Country = sortRecords(lstT_Country, sortBy, isAsc);
            }
            else lstT_Country = lstT_Country.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Country"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Country"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Country"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Country"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_Country = lstT_Country;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_Country.Count() > 0)
                    pageSize = _T_Country.Count();
                return View("ExcelExport", _T_Country.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_Country.Count();
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
				return View(_T_Country.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_Country = _fad.FilterDropdown<T_Country>(User,  _T_Country, "T_Country", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_Country.Except(_T_Country),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_Country.Except(_T_Country).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_Country.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_Country.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_Country.Count() == 0 ? 1 : _T_Country.Count();
                                
                            }
							return PartialView("IndexPartial", _T_Country.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_Country.Count() == 0 ? 1 : _T_Country.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_Country.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_Country/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Country t_country = db.T_Countrys.Find(id);
            if (t_country == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_country);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_country, AssociatedType);
            ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnDetails");
			ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnDetails");
			return View(ViewBag.TemplatesName,t_country);

        }



        // GET: /T_Country/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype)
        {
            if (!User.CanAdd("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_CountryParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country","OnCreate");
		  ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnCreate");
          return View();
        }
		// GET: /T_Country/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_CountryParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnCreate");
			 ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnCreate");
            return View();
        }
		// POST: /T_Country/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country, string UrlReferrer)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
           db.T_Countrys.Add(t_country);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_country);	
            return View(t_country);
        }
		 // GET: /T_Country/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_CountryParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnCreate");
			ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnCreate");
            return View();
        }
		  // POST: /T_Country/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
           db.T_Countrys.Add(t_country);
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
			LoadViewDataAfterOnCreate(t_country);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_country, AssociatedEntity);
			return View(t_country);
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
        // POST: /T_Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_Countrys.Add(t_country);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_country.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_country);
            return View(t_country);
        }
		// GET: /T_Country/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_Country"))
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
            T_Country t_country = db.T_Countrys.Find(id);
            if (t_country == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_CountryParentUrl"] = UrlReferrer;
		if(ViewData["T_CountryParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country/Edit/" + t_country.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country/Create"))
			ViewData["T_CountryParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_country);
		   ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnEdit");
		   ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnEdit");
          return View(t_country);
        }
		// POST: /T_Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_country).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_country);
            return View(t_country);
        }


        // GET: /T_Country/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Country t_country = db.T_Countrys.Find(id);
            if (t_country == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_CountryParentUrl"] = UrlReferrer;
		if(ViewData["T_CountryParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country/Edit/" + t_country.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country/Create"))
			ViewData["T_CountryParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_country);
		   ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnEdit");
		   ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnEdit");
          return View(t_country);
        }
        // POST: /T_Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country,  string UrlReferrer)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_country).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_country.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_country);
            return View(t_country);
        }
		// GET: /T_Country/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_Country"))
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
			            T_Country t_country = db.T_Countrys.Find(id);
            if (t_country == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_CountryParentUrl"] = UrlReferrer;
		if(ViewData["T_CountryParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country"))
			ViewData["T_CountryParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_country);
			 ViewBag.T_CountryIsHiddenRule = checkHidden("T_Country", "OnEdit");
			 ViewBag.T_CountryIsSetValueUIRule = checkSetValueUIRule("T_Country", "OnEdit");
          return View(t_country);
        }
        // POST: /T_Country/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country,string UrlReferrer)
        {
			CheckBeforeSave(t_country);
            if (ModelState.IsValid)
            {
                db.Entry(t_country).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_country);
            return View(t_country);
        }
        // GET: /T_Country/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_Country t_country = db.T_Countrys.Find(id);
            if (t_country == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_CountryParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Country"))
			 ViewData["T_CountryParentUrl"] = Request.UrlReferrer;
            return View(t_country);
        }
        // POST: /T_Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_Country t_country, string UrlReferrer)
        {
			if (!User.CanDelete("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_country))
            {
 //Delete Document
			db.Entry(t_country).State = EntityState.Deleted;
            db.T_Countrys.Remove(t_country);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_CountryParentUrl"] != null)
            {
                string parentUrl = ViewData["T_CountryParentUrl"].ToString();
                ViewData["T_CountryParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_country);
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
				T_Country t_country = db.T_Countrys.Find(id);
                db.Entry(t_country).State = EntityState.Deleted;
                db.T_Countrys.Remove(t_country);
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
            if (!User.CanEdit("T_Country"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_CountryParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Country t_country,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_Country target = db.T_Countrys.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_country, target, chkUpdate); 
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
