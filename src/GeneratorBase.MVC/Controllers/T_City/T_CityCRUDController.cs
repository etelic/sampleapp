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
    public partial class T_CityController : BaseController
    {
        // GET: /T_City/
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
			var lstT_City = from s in db.T_Citys
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_City = searchRecords(lstT_City, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_City = sortRecords(lstT_City, sortBy, isAsc);
            }
            else lstT_City = lstT_City.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_City"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_City"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_City"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_City"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_City = lstT_City.Include(t=>t.t_citycountry).Include(t=>t.t_citystate);
		 if (HostingEntity == "T_Country" && AssociatedType == "T_CityCountry")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_City = _T_City.Where(p => p.T_CityCountryID == hostid);
			 }
			 else
			     _T_City = _T_City.Where(p => p.T_CityCountryID == null);
         }
		 if (HostingEntity == "T_State" && AssociatedType == "T_CityState")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_City = _T_City.Where(p => p.T_CityStateID == hostid);
			 }
			 else
			     _T_City = _T_City.Where(p => p.T_CityStateID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_City.Count() > 0)
                    pageSize = _T_City.Count();
                return View("ExcelExport", _T_City.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_City.Count();
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
				return View(_T_City.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_City = _fad.FilterDropdown<T_City>(User,  _T_City, "T_City", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_City.Except(_T_City),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_City.Except(_T_City).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_City.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_City.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_City.Count() == 0 ? 1 : _T_City.Count();
                                
                            }
							return PartialView("IndexPartial", _T_City.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_City.Count() == 0 ? 1 : _T_City.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_City.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_City/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_City t_city = db.T_Citys.Find(id);
            if (t_city == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_city);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_city, AssociatedType);
            ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnDetails");
			ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnDetails");
			return View(ViewBag.TemplatesName,t_city);

        }



        // GET: /T_City/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype)
        {
            if (!User.CanAdd("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_CityParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_CityIsHiddenRule = checkHidden("T_City","OnCreate");
		  ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnCreate");
          return View();
        }
		// GET: /T_City/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_CityParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnCreate");
			 ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnCreate");
            return View();
        }
		// POST: /T_City/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city, string UrlReferrer)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
           db.T_Citys.Add(t_city);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_city);	
            return View(t_city);
        }
		 // GET: /T_City/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_CityParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnCreate");
			ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnCreate");
            return View();
        }
		  // POST: /T_City/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
           db.T_Citys.Add(t_city);
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
			LoadViewDataAfterOnCreate(t_city);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_city, AssociatedEntity);
			return View(t_city);
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
        // POST: /T_City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_Citys.Add(t_city);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_city.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_city);
            return View(t_city);
        }
		// GET: /T_City/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_City"))
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
            T_City t_city = db.T_Citys.Find(id);
            if (t_city == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_CityParentUrl"] = UrlReferrer;
		if(ViewData["T_CityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_City")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_City/Edit/" + t_city.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_City/Create"))
			ViewData["T_CityParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_city);
		   ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnEdit");
		   ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnEdit");
          return View(t_city);
        }
		// POST: /T_City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_city).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_city);
            return View(t_city);
        }


        // GET: /T_City/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_City t_city = db.T_Citys.Find(id);
            if (t_city == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_CityParentUrl"] = UrlReferrer;
		if(ViewData["T_CityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_City")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_City/Edit/" + t_city.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_City/Create"))
			ViewData["T_CityParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_city);
		   ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnEdit");
		   ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnEdit");
          return View(t_city);
        }
        // POST: /T_City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city,  string UrlReferrer)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_city).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_city.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_city);
            return View(t_city);
        }
		// GET: /T_City/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_City"))
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
			            T_City t_city = db.T_Citys.Find(id);
            if (t_city == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_CityParentUrl"] = UrlReferrer;
		if(ViewData["T_CityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_City"))
			ViewData["T_CityParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_city);
			 ViewBag.T_CityIsHiddenRule = checkHidden("T_City", "OnEdit");
			 ViewBag.T_CityIsSetValueUIRule = checkSetValueUIRule("T_City", "OnEdit");
          return View(t_city);
        }
        // POST: /T_City/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city,string UrlReferrer)
        {
			CheckBeforeSave(t_city);
            if (ModelState.IsValid)
            {
                db.Entry(t_city).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_city);
            return View(t_city);
        }
        // GET: /T_City/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_City t_city = db.T_Citys.Find(id);
            if (t_city == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_CityParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_City"))
			 ViewData["T_CityParentUrl"] = Request.UrlReferrer;
            return View(t_city);
        }
        // POST: /T_City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_City t_city, string UrlReferrer)
        {
			if (!User.CanDelete("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_city))
            {
 //Delete Document
			db.Entry(t_city).State = EntityState.Deleted;
            db.T_Citys.Remove(t_city);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_CityParentUrl"] != null)
            {
                string parentUrl = ViewData["T_CityParentUrl"].ToString();
                ViewData["T_CityParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_city);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_Country" && AssociatedType == "T_CityCountry")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_City obj = db.T_Citys.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_CityCountryID = HostingID;
                    db.SaveChanges();
                }
            }
            if (HostingEntity == "T_State" && AssociatedType == "T_CityState")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_City obj = db.T_Citys.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_CityStateID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
				T_City t_city = db.T_Citys.Find(id);
                db.Entry(t_city).State = EntityState.Deleted;
                db.T_Citys.Remove(t_city);
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
            if (!User.CanEdit("T_City"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_CityParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description,T_CityCountryID,T_CityStateID")] T_City t_city,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_City target = db.T_Citys.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_city, target, chkUpdate); 
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
