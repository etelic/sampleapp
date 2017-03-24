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
    public partial class EntityDataSourceController : BaseController
    {
        // GET: /EntityDataSource/
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
			var lstEntityDataSource = from s in db.EntityDataSources
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstEntityDataSource = searchRecords(lstEntityDataSource, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstEntityDataSource = sortRecords(lstEntityDataSource, sortBy, isAsc);
            }
            else lstEntityDataSource = lstEntityDataSource.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "EntityDataSource"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "EntityDataSource"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "EntityDataSource"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "EntityDataSource"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _EntityDataSource = lstEntityDataSource;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_EntityDataSource.Count() > 0)
                    pageSize = _EntityDataSource.Count();
                return View("ExcelExport", _EntityDataSource.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_EntityDataSource.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_EntityDataSource = _fad.FilterDropdown<EntityDataSource>(User,  _EntityDataSource, "EntityDataSource", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstEntityDataSource.Except(_EntityDataSource),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstEntityDataSource.Except(_EntityDataSource).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _EntityDataSource.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _EntityDataSource.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
						return PartialView("IndexPartial", _EntityDataSource.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /EntityDataSource/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
            if (entitydatasource == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(entitydatasource);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(entitydatasource, AssociatedType);
            return View(entitydatasource);
        }
        // GET: /EntityDataSource/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
          ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.IsExternalEntity), "Name", "DisplayName");
          return View();
        }
		// GET: /EntityDataSource/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
            return View();
        }
		// POST: /EntityDataSource/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource, string UrlReferrer)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
           db.EntityDataSources.Add(entitydatasource);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(entitydatasource);	
            return View(entitydatasource);
        }
		 // GET: /EntityDataSource/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
            ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.IsExternalEntity), "Name", "DisplayName");
            return View();
        }
		  // POST: /EntityDataSource/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
           db.EntityDataSources.Add(entitydatasource);
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
			LoadViewDataAfterOnCreate(entitydatasource);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(entitydatasource, AssociatedEntity);
			return View(entitydatasource);
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
        // POST: /EntityDataSource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.EntityDataSources.Add(entitydatasource);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = entitydatasource.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(entitydatasource);
            return View(entitydatasource);
        }
		// GET: /EntityDataSource/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
            if (entitydatasource == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
		if(ViewData["EntityDataSourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource/Edit/" + entitydatasource.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource/Create"))
			ViewData["EntityDataSourceParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(entitydatasource);
		   ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
           ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.IsExternalEntity), "Name", "DisplayName", entitydatasource.EntityName);
          return View(entitydatasource);
        }
		// POST: /EntityDataSource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(entitydatasource).State = EntityState.Modified;

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
			
			LoadViewDataAfterOnEdit(entitydatasource);
            return View(entitydatasource);
        }


        // GET: /EntityDataSource/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
            if (entitydatasource == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
		if(ViewData["EntityDataSourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource/Edit/" + entitydatasource.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource/Create"))
			ViewData["EntityDataSourceParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(entitydatasource);
		   ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
           ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.IsExternalEntity), "Name", "DisplayName", entitydatasource.EntityName);
          return View(entitydatasource);
        }
        // POST: /EntityDataSource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource,  string UrlReferrer)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(entitydatasource).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = entitydatasource.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(entitydatasource);
            return View(entitydatasource);
        }
		// GET: /EntityDataSource/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
            if (entitydatasource == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
		if(ViewData["EntityDataSourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource"))
			ViewData["EntityDataSourceParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(entitydatasource);
			 ViewBag.EntityDataSourceIsHiddenRule = checkHidden("EntityDataSource");
          return View(entitydatasource);
        }
        // POST: /EntityDataSource/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource,string UrlReferrer)
        {
			CheckBeforeSave(entitydatasource);
            if (ModelState.IsValid)
            {
                db.Entry(entitydatasource).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(entitydatasource);
            return View(entitydatasource);
        }
        // GET: /EntityDataSource/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
            if (entitydatasource == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["EntityDataSourceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/EntityDataSource"))
			 ViewData["EntityDataSourceParentUrl"] = Request.UrlReferrer;
            return View(entitydatasource);
        }
        // POST: /EntityDataSource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(EntityDataSource entitydatasource, string UrlReferrer)
        {
			if (!User.CanDelete("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(entitydatasource))
            {
 //Delete Document
			db.Entry(entitydatasource).State = EntityState.Deleted;
            db.EntityDataSources.Remove(entitydatasource);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["EntityDataSourceParentUrl"] != null)
            {
                string parentUrl = ViewData["EntityDataSourceParentUrl"].ToString();
                ViewData["EntityDataSourceParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(entitydatasource);
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
			 EntityDataSource entitydatasource = db.EntityDataSources.Find(id);
				                db.Entry(entitydatasource).State = EntityState.Deleted;
                db.EntityDataSources.Remove(entitydatasource);
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
            if (!User.CanEdit("EntityDataSource"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["EntityDataSourceParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,EntityName,DataSource,SourceType,MethodType,Action,flag,other")] EntityDataSource entitydatasource,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                EntityDataSource target = db.EntityDataSources.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(entitydatasource, target, chkUpdate); 
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
