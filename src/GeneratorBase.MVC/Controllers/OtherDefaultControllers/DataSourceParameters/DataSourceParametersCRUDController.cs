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
    public partial class DataSourceParametersController : BaseController
    {
        // GET: /DataSourceParameters/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation,string parent,string Wfsearch,string caller, bool? BulkAssociate)
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
			if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
			var lstDataSourceParameters = from s in db.DataSourceParameterss
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstDataSourceParameters = searchRecords(lstDataSourceParameters, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstDataSourceParameters = sortRecords(lstDataSourceParameters, sortBy, isAsc);
            }
            else lstDataSourceParameters = lstDataSourceParameters.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "DataSourceParameters"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "DataSourceParameters"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DataSourceParameters"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DataSourceParameters"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _DataSourceParameters = lstDataSourceParameters.Include(t=>t.entitydatasourceparameters);
		 if (HostingEntity == "EntityDataSource" && AssociatedType == "EntityDataSourceParameters")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _DataSourceParameters = _DataSourceParameters.Where(p => p.EntityDataSourceParametersID == hostid);
			 }
			 else
			     _DataSourceParameters = _DataSourceParameters.Where(p => p.EntityDataSourceParametersID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_DataSourceParameters.Count() > 0)
                    pageSize = _DataSourceParameters.Count();
                return View("ExcelExport", _DataSourceParameters.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_DataSourceParameters.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_DataSourceParameters = _fad.FilterDropdown<DataSourceParameters>(User,  _DataSourceParameters, "DataSourceParameters", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstDataSourceParameters.Except(_DataSourceParameters),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstDataSourceParameters.Except(_DataSourceParameters).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _DataSourceParameters.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _DataSourceParameters.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
						return PartialView("IndexPartial", _DataSourceParameters.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /DataSourceParameters/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
            if (datasourceparameters == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(datasourceparameters);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(datasourceparameters, AssociatedType);
            return View(datasourceparameters);
        }
        // GET: /DataSourceParameters/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
          return View();
        }
		// GET: /DataSourceParameters/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
            return View();
        }
		// POST: /DataSourceParameters/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters, string UrlReferrer)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
           db.DataSourceParameterss.Add(datasourceparameters);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(datasourceparameters);	
            return View(datasourceparameters);
        }
		 // GET: /DataSourceParameters/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
            return View();
        }
		  // POST: /DataSourceParameters/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
           db.DataSourceParameterss.Add(datasourceparameters);
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
			LoadViewDataAfterOnCreate(datasourceparameters);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(datasourceparameters, AssociatedEntity);
			return View(datasourceparameters);
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
        // POST: /DataSourceParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.DataSourceParameterss.Add(datasourceparameters);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = datasourceparameters.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(datasourceparameters);
            return View(datasourceparameters);
        }
		// GET: /DataSourceParameters/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
            if (datasourceparameters == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
		if(ViewData["DataSourceParametersParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters/Edit/" + datasourceparameters.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters/Create"))
			ViewData["DataSourceParametersParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(datasourceparameters);
		   ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
          return View(datasourceparameters);
        }
		// POST: /DataSourceParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(datasourceparameters).State = EntityState.Modified;

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
			
			LoadViewDataAfterOnEdit(datasourceparameters);
            return View(datasourceparameters);
        }


        // GET: /DataSourceParameters/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
            if (datasourceparameters == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
		if(ViewData["DataSourceParametersParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters/Edit/" + datasourceparameters.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters/Create"))
			ViewData["DataSourceParametersParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(datasourceparameters);
		   ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
          return View(datasourceparameters);
        }
        // POST: /DataSourceParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters,  string UrlReferrer)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(datasourceparameters).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = datasourceparameters.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(datasourceparameters);
            return View(datasourceparameters);
        }
		// GET: /DataSourceParameters/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
            if (datasourceparameters == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
		if(ViewData["DataSourceParametersParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters"))
			ViewData["DataSourceParametersParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(datasourceparameters);
			 ViewBag.DataSourceParametersIsHiddenRule = checkHidden("DataSourceParameters");
          return View(datasourceparameters);
        }
        // POST: /DataSourceParameters/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters,string UrlReferrer)
        {
			CheckBeforeSave(datasourceparameters);
            if (ModelState.IsValid)
            {
                db.Entry(datasourceparameters).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(datasourceparameters);
            return View(datasourceparameters);
        }
        // GET: /DataSourceParameters/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
            if (datasourceparameters == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["DataSourceParametersParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/DataSourceParameters"))
			 ViewData["DataSourceParametersParentUrl"] = Request.UrlReferrer;
            return View(datasourceparameters);
        }
        // POST: /DataSourceParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DataSourceParameters datasourceparameters, string UrlReferrer)
        {
			if (!User.CanDelete("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(datasourceparameters))
            {
 //Delete Document
			db.Entry(datasourceparameters).State = EntityState.Deleted;
            db.DataSourceParameterss.Remove(datasourceparameters);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["DataSourceParametersParentUrl"] != null)
            {
                string parentUrl = ViewData["DataSourceParametersParentUrl"].ToString();
                ViewData["DataSourceParametersParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(datasourceparameters);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "EntityDataSource" && AssociatedType == "EntityDataSourceParameters")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    DataSourceParameters obj = db.DataSourceParameterss.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.EntityDataSourceParametersID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 DataSourceParameters datasourceparameters = db.DataSourceParameterss.Find(id);
				                db.Entry(datasourceparameters).State = EntityState.Deleted;
                db.DataSourceParameterss.Remove(datasourceparameters);
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
            if (!User.CanEdit("DataSourceParameters"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["DataSourceParametersParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,ArgumentName,ArgumentValue,HostingEntity,flag,other,EntityDataSourceParametersID")] DataSourceParameters datasourceparameters,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                DataSourceParameters target = db.DataSourceParameterss.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(datasourceparameters, target, chkUpdate); 
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
