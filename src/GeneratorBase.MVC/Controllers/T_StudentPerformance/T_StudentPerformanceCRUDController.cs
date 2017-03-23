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
    public partial class T_StudentPerformanceController : BaseController
    {
        // GET: /T_StudentPerformance/
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
			var lstT_StudentPerformance = from s in db.T_StudentPerformances
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_StudentPerformance = searchRecords(lstT_StudentPerformance, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_StudentPerformance = sortRecords(lstT_StudentPerformance, sortBy, isAsc);
            }
            else lstT_StudentPerformance = lstT_StudentPerformance.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_StudentPerformance"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_StudentPerformance"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_StudentPerformance"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_StudentPerformance"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_StudentPerformance = lstT_StudentPerformance.Include(t=>t.t_studentcode);
		 if (HostingEntity == "T_Student" && AssociatedType == "T_StudentCode")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_StudentPerformance = _T_StudentPerformance.Where(p => p.T_StudentCodeID == hostid);
			 }
			 else
			     _T_StudentPerformance = _T_StudentPerformance.Where(p => p.T_StudentCodeID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_StudentPerformance.Count() > 0)
                    pageSize = _T_StudentPerformance.Count();
                return View("ExcelExport", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_T_StudentPerformance.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_StudentPerformance = _fad.FilterDropdown<T_StudentPerformance>(User,  _T_StudentPerformance, "T_StudentPerformance", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_StudentPerformance.Except(_T_StudentPerformance),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_StudentPerformance.Except(_T_StudentPerformance).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_StudentPerformance.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
						return PartialView("IndexPartial", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /T_StudentPerformance/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
            if (t_studentperformance == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_studentperformance);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_studentperformance, AssociatedType);
            return View(t_studentperformance);
        }
        // GET: /T_StudentPerformance/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
          return View();
        }
		// GET: /T_StudentPerformance/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
            return View();
        }
		// POST: /T_StudentPerformance/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance, string UrlReferrer)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
           db.T_StudentPerformances.Add(t_studentperformance);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_studentperformance);	
            return View(t_studentperformance);
        }
		 // GET: /T_StudentPerformance/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
            return View();
        }
		  // POST: /T_StudentPerformance/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
           db.T_StudentPerformances.Add(t_studentperformance);
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
			LoadViewDataAfterOnCreate(t_studentperformance);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_studentperformance, AssociatedEntity);
			return View(t_studentperformance);
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
        // POST: /T_StudentPerformance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_StudentPerformances.Add(t_studentperformance);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_studentperformance.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_studentperformance);
            return View(t_studentperformance);
        }
		// GET: /T_StudentPerformance/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
            if (t_studentperformance == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
		if(ViewData["T_StudentPerformanceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance/Edit/" + t_studentperformance.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance/Create"))
			ViewData["T_StudentPerformanceParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_studentperformance);
		   ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
          return View(t_studentperformance);
        }
		// POST: /T_StudentPerformance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_studentperformance).State = EntityState.Modified;

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
			
			LoadViewDataAfterOnEdit(t_studentperformance);
            return View(t_studentperformance);
        }


        // GET: /T_StudentPerformance/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
            if (t_studentperformance == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
		if(ViewData["T_StudentPerformanceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance/Edit/" + t_studentperformance.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance/Create"))
			ViewData["T_StudentPerformanceParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_studentperformance);
		   ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
          return View(t_studentperformance);
        }
        // POST: /T_StudentPerformance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance,  string UrlReferrer)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_studentperformance).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_studentperformance.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_studentperformance);
            return View(t_studentperformance);
        }
		// GET: /T_StudentPerformance/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
            if (t_studentperformance == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
		if(ViewData["T_StudentPerformanceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance"))
			ViewData["T_StudentPerformanceParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_studentperformance);
			 ViewBag.T_StudentPerformanceIsHiddenRule = checkHidden("T_StudentPerformance");
          return View(t_studentperformance);
        }
        // POST: /T_StudentPerformance/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance,string UrlReferrer)
        {
			CheckBeforeSave(t_studentperformance);
            if (ModelState.IsValid)
            {
                db.Entry(t_studentperformance).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_studentperformance);
            return View(t_studentperformance);
        }
        // GET: /T_StudentPerformance/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
            if (t_studentperformance == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_StudentPerformanceParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_StudentPerformance"))
			 ViewData["T_StudentPerformanceParentUrl"] = Request.UrlReferrer;
            return View(t_studentperformance);
        }
        // POST: /T_StudentPerformance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_StudentPerformance t_studentperformance, string UrlReferrer)
        {
			if (!User.CanDelete("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_studentperformance))
            {
 //Delete Document
			db.Entry(t_studentperformance).State = EntityState.Deleted;
            db.T_StudentPerformances.Remove(t_studentperformance);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_StudentPerformanceParentUrl"] != null)
            {
                string parentUrl = ViewData["T_StudentPerformanceParentUrl"].ToString();
                ViewData["T_StudentPerformanceParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_studentperformance);
        }
		public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "T_Student" && AssociatedType == "T_StudentCode")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    T_StudentPerformance obj = db.T_StudentPerformances.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.T_StudentCodeID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
		{
            foreach (var id in ids.Where(p => p > 0))
            {
			 T_StudentPerformance t_studentperformance = db.T_StudentPerformances.Find(id);
				                db.Entry(t_studentperformance).State = EntityState.Deleted;
                db.T_StudentPerformances.Remove(t_studentperformance);
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
            if (!User.CanEdit("T_StudentPerformance"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_StudentPerformanceParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_StudentCodeID,T_Mark1,T_Mark2,T_TotalMarks,T_Grade,T_Remarks")] T_StudentPerformance t_studentperformance,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                T_StudentPerformance target = db.T_StudentPerformances.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_studentperformance, target, chkUpdate); 
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
