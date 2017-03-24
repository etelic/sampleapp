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
    public partial class T_RecurringEndTypeController : BaseController
    {
        // GET: /T_RecurringEndType/
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
			var lstT_RecurringEndType = from s in db.T_RecurringEndTypes
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_RecurringEndType = searchRecords(lstT_RecurringEndType, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_RecurringEndType = sortRecords(lstT_RecurringEndType, sortBy, isAsc);
            }
            else lstT_RecurringEndType = lstT_RecurringEndType.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurringEndType"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_RecurringEndType"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurringEndType"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_RecurringEndType"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_RecurringEndType = lstT_RecurringEndType;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_RecurringEndType.Count() > 0)
                    pageSize = _T_RecurringEndType.Count();
                return View("ExcelExport", _T_RecurringEndType.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
			 {
			  GetTemplatesForList();
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
				return View(_T_RecurringEndType.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_RecurringEndType = _fad.FilterDropdown<T_RecurringEndType>(User,  _T_RecurringEndType, "T_RecurringEndType", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_RecurringEndType.Except(_T_RecurringEndType),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_RecurringEndType.Except(_T_RecurringEndType).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_RecurringEndType.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_RecurringEndType.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{
					  if (ViewBag.TemplatesName == null)
                        return PartialView("IndexPartial", _T_RecurringEndType.ToPagedList(pageNumber, pageSize));
					  else
						return PartialView(ViewBag.TemplatesName, _T_RecurringEndType.ToPagedList(pageNumber, pageSize));
					}
				}
        }
		 // GET: /T_RecurringEndType/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
            if (t_recurringendtype == null)
            {
                return HttpNotFound();
            }
			GetTemplatesForDetails();
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_recurringendtype);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_recurringendtype, AssociatedType);
            ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnDetails");
			return View(ViewBag.TemplatesName,t_recurringendtype);

        }
        // GET: /T_RecurringEndType/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType","OnCreate");
          return View();
        }
		// GET: /T_RecurringEndType/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnCreate");
            return View();
        }
		// POST: /T_RecurringEndType/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype, string UrlReferrer)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
           db.T_RecurringEndTypes.Add(t_recurringendtype);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_recurringendtype);	
            return View(t_recurringendtype);
        }
		 // GET: /T_RecurringEndType/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnCreate");
            return View();
        }
		  // POST: /T_RecurringEndType/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
           db.T_RecurringEndTypes.Add(t_recurringendtype);
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
			LoadViewDataAfterOnCreate(t_recurringendtype);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_recurringendtype, AssociatedEntity);
			return View(t_recurringendtype);
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
        // POST: /T_RecurringEndType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_RecurringEndTypes.Add(t_recurringendtype);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_recurringendtype.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_recurringendtype);
            return View(t_recurringendtype);
        }
		// GET: /T_RecurringEndType/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
            if (t_recurringendtype == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringEndTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType/Edit/" + t_recurringendtype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType/Create"))
			ViewData["T_RecurringEndTypeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurringendtype);
		   ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnEdit");
          return View(t_recurringendtype);
        }
		// POST: /T_RecurringEndType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurringendtype).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_recurringendtype);
            return View(t_recurringendtype);
        }


        // GET: /T_RecurringEndType/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
            if (t_recurringendtype == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringEndTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType/Edit/" + t_recurringendtype.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType/Create"))
			ViewData["T_RecurringEndTypeParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_recurringendtype);
		   ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnEdit");
          return View(t_recurringendtype);
        }
        // POST: /T_RecurringEndType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype,  string UrlReferrer)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_recurringendtype).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_recurringendtype.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_recurringendtype);
            return View(t_recurringendtype);
        }
		// GET: /T_RecurringEndType/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
            if (t_recurringendtype == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
		if(ViewData["T_RecurringEndTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType"))
			ViewData["T_RecurringEndTypeParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_recurringendtype);
			 ViewBag.T_RecurringEndTypeIsHiddenRule = checkHidden("T_RecurringEndType", "OnEdit");
          return View(t_recurringendtype);
        }
        // POST: /T_RecurringEndType/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype,string UrlReferrer)
        {
			CheckBeforeSave(t_recurringendtype);
            if (ModelState.IsValid)
            {
                db.Entry(t_recurringendtype).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_recurringendtype);
            return View(t_recurringendtype);
        }
        // GET: /T_RecurringEndType/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
            if (t_recurringendtype == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_RecurringEndTypeParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_RecurringEndType"))
			 ViewData["T_RecurringEndTypeParentUrl"] = Request.UrlReferrer;
            return View(t_recurringendtype);
        }
        // POST: /T_RecurringEndType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_RecurringEndType t_recurringendtype, string UrlReferrer)
        {
			if (!User.CanDelete("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_recurringendtype))
            {
 //Delete Document
			db.Entry(t_recurringendtype).State = EntityState.Deleted;
            db.T_RecurringEndTypes.Remove(t_recurringendtype);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_RecurringEndTypeParentUrl"] != null)
            {
                string parentUrl = ViewData["T_RecurringEndTypeParentUrl"].ToString();
                ViewData["T_RecurringEndTypeParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_recurringendtype);
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
			 T_RecurringEndType t_recurringendtype = db.T_RecurringEndTypes.Find(id);
                db.Entry(t_recurringendtype).State = EntityState.Deleted;
                db.T_RecurringEndTypes.Remove(t_recurringendtype);
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
            if (!User.CanEdit("T_RecurringEndType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_RecurringEndTypeParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_RecurringEndType t_recurringendtype,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_RecurringEndType target = db.T_RecurringEndTypes.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_recurringendtype, target, chkUpdate); 
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
