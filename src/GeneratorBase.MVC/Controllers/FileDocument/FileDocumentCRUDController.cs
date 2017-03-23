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
    public partial class FileDocumentController : BaseController
    {
        // GET: /FileDocument/
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
			var lstFileDocument = from s in db.FileDocuments
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstFileDocument = searchRecords(lstFileDocument, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstFileDocument = sortRecords(lstFileDocument, sortBy, isAsc);
            }
            else lstFileDocument = lstFileDocument.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FileDocument"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "FileDocument"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FileDocument"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "FileDocument"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _FileDocument = lstFileDocument;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_FileDocument.Count() > 0)
                    pageSize = _FileDocument.Count();
                return View("ExcelExport", _FileDocument.ToPagedList(pageNumber, pageSize));
            }
			 if (!(RenderPartial==null?false:RenderPartial.Value) && !Request.IsAjaxRequest())
				return View(_FileDocument.ToPagedList(pageNumber, pageSize));
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (caller != "")
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_FileDocument = _fad.FilterDropdown<FileDocument>(User,  _FileDocument, "FileDocument", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstFileDocument.Except(_FileDocument),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstFileDocument.Except(_FileDocument).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _FileDocument.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _FileDocument.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
						return PartialView("IndexPartial", _FileDocument.ToPagedList(pageNumber, pageSize));
				}
        }
		 // GET: /FileDocument/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileDocument filedocument = db.FileDocuments.Find(id);
            if (filedocument == null)
            {
                return HttpNotFound();
            }
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(filedocument);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(filedocument, AssociatedType);
            return View(filedocument);
        }
        // GET: /FileDocument/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
		 if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		  ViewData["FileDocumentParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
          return View();
        }
		// GET: /FileDocument/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType)
        {
            if (!User.CanAdd("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
	
		    ViewData["FileDocumentParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
            return View();
        }
		// POST: /FileDocument/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase AttachDocument, string UrlReferrer)
        {
			CheckBeforeSave(filedocument);
		if (AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(AttachDocument);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (AttachDocument != null)
                { 
									AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(AttachDocument.FileName);
                }
           db.FileDocuments.Add(filedocument);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(filedocument);	
            return View(filedocument);
        }
		 // GET: /FileDocument/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["FileDocumentParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
            return View();
        }
		  // POST: /FileDocument/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase AttachDocument , String CamerafileUploadAttachDocument,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(filedocument);
		if (AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(AttachDocument);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (AttachDocument != null)
                { 
					AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(AttachDocument.FileName);
                }
				if (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"]  != "")
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    filedocument.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                }
           db.FileDocuments.Add(filedocument);
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
			LoadViewDataAfterOnCreate(filedocument);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(filedocument, AssociatedEntity);
			return View(filedocument);
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
        // POST: /FileDocument/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase AttachDocument , String CamerafileUploadAttachDocument, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(filedocument);
		if (AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(AttachDocument);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (AttachDocument != null)
                { 
					AttachDocument.SaveAs(path + ticks + System.IO.Path.GetFileName(AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(AttachDocument.FileName);
                }
			    if (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"]  != "")
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"])));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    filedocument.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                }
           db.FileDocuments.Add(filedocument);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = filedocument.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(filedocument);
            return View(filedocument);
        }
		// GET: /FileDocument/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileDocument filedocument = db.FileDocuments.Find(id);
            if (filedocument == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["FileDocumentParentUrl"] = UrlReferrer;
		if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Edit/" + filedocument.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Create"))
			ViewData["FileDocumentParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(filedocument);
		   ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
          return View(filedocument);
        }
		// POST: /FileDocument/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument , String CamerafileUploadAttachDocument,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(filedocument);
		if (File_AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(File_AttachDocument);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (File_AttachDocument != null)
                { 
					File_AttachDocument.SaveAs(path  + ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName);
                }
				if (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"] )));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    filedocument.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                }
                db.Entry(filedocument).State = EntityState.Modified;

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
			
			LoadViewDataAfterOnEdit(filedocument);
            return View(filedocument);
        }


        // GET: /FileDocument/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileDocument filedocument = db.FileDocuments.Find(id);
            if (filedocument == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["FileDocumentParentUrl"] = UrlReferrer;
		if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Edit/" + filedocument.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument/Create"))
			ViewData["FileDocumentParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(filedocument);
		   ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
          return View(filedocument);
        }
        // POST: /FileDocument/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument , String CamerafileUploadAttachDocument,  string UrlReferrer)
        {
			CheckBeforeSave(filedocument);
		if (File_AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(File_AttachDocument);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                if (File_AttachDocument != null)
                { 
					File_AttachDocument.SaveAs(path  + ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName);
                }
				if (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != "")
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Request.Form["CamerafileUploadAttachDocument"] )));
                    image.Save(path + ticks + "Camera-" + ticks + "-" + User.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    filedocument.AttachDocument = ticks + "Camera-" + ticks + "-" + User.Name + ".jpg";
                }
                db.Entry(filedocument).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = filedocument.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(filedocument);
            return View(filedocument);
        }
		// GET: /FileDocument/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileDocument filedocument = db.FileDocuments.Find(id);
            if (filedocument == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["FileDocumentParentUrl"] = UrlReferrer;
		if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument"))
			ViewData["FileDocumentParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(filedocument);
			 ViewBag.FileDocumentIsHiddenRule = checkHidden("FileDocument");
          return View(filedocument);
        }
        // POST: /FileDocument/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument , String CamerafileUploadAttachDocument,string UrlReferrer)
        {
			CheckBeforeSave(filedocument);
		if (File_AttachDocument != null || (Request.Form["CamerafileUploadAttachDocument"] != null && Request.Form["CamerafileUploadAttachDocument"] != ""))
                IsFileTypeAndSizeAllowed(File_AttachDocument);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
	                if (File_AttachDocument != null)
                { 
					File_AttachDocument.SaveAs(path  + ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName));
                    filedocument.AttachDocument = ticks + System.IO.Path.GetFileName(File_AttachDocument.FileName);
                }
                db.Entry(filedocument).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(filedocument);
            return View(filedocument);
        }
        // GET: /FileDocument/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            FileDocument filedocument = db.FileDocuments.Find(id);
            if (filedocument == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["FileDocumentParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/FileDocument"))
			 ViewData["FileDocumentParentUrl"] = Request.UrlReferrer;
            return View(filedocument);
        }
        // POST: /FileDocument/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(FileDocument filedocument, string UrlReferrer)
        {
			if (!User.CanDelete("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(filedocument))
            {
 //Delete Document
			db.Entry(filedocument).State = EntityState.Deleted;
            db.FileDocuments.Remove(filedocument);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["FileDocumentParentUrl"] != null)
            {
                string parentUrl = ViewData["FileDocumentParentUrl"].ToString();
                ViewData["FileDocumentParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(filedocument);
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
			 FileDocument filedocument = db.FileDocuments.Find(id);
				                db.Entry(filedocument).State = EntityState.Deleted;
                db.FileDocuments.Remove(filedocument);
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
            if (!User.CanEdit("FileDocument"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["FileDocumentParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,DocumentName,Description,AttachDocument,DateCreated,DateLastUpdated")] FileDocument filedocument, HttpPostedFileBase File_AttachDocument , String CamerafileUploadAttachDocument,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
                long objId = long.Parse(id);
                FileDocument target = db.FileDocuments.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(filedocument, target, chkUpdate); 
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
