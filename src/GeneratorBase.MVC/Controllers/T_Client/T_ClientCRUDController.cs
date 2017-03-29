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
    public partial class T_ClientController : BaseController
    {
        // GET: /T_Client/
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
			var lstT_Client = from s in db.T_Clients
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_Client = searchRecords(lstT_Client, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_Client = sortRecords(lstT_Client, sortBy, isAsc);
            }
            else lstT_Client = lstT_Client.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Client"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_Client"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Client"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_Client"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_Client = lstT_Client;
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_Client.Count() > 0)
                    pageSize = _T_Client.Count();
                return View("ExcelExport", _T_Client.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_Client.Count();
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
				return View(_T_Client.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_Client = _fad.FilterDropdown<T_Client>(User,  _T_Client, "T_Client", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_Client.Except(_T_Client),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_Client.Except(_T_Client).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_Client.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_Client.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_Client.Count() == 0 ? 1 : _T_Client.Count();
                                
                            }
							return PartialView("IndexPartial", _T_Client.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_Client.Count() == 0 ? 1 : _T_Client.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_Client.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_Client/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Client t_client = db.T_Clients.Find(id);
            if (t_client == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_client);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_client, AssociatedType);
            ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnDetails", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnDetails", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnDetails");
			return View(ViewBag.TemplatesName,t_client);

        }

        // GET: /T_Client/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype )
        {
            if (!User.CanAdd("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_ClientParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
		  ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");
          return View();
        }
		// GET: /T_Client/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_ClientParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
			 ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");
            return View();
        }
		// POST: /T_Client/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client, string UrlReferrer)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
           db.T_Clients.Add(t_client);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");	
            return View(t_client);
        }
		 // GET: /T_Client/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_ClientParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");
            return View();
        }
		  // POST: /T_Client/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
           db.T_Clients.Add(t_client);
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
			LoadViewDataAfterOnCreate(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_client, AssociatedEntity);
			return View(t_client);
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
        // POST: /T_Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
           db.T_Clients.Add(t_client);
           db.SaveChanges();
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_client.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnCreate", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnCreate", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnCreate");
            return View(t_client);
        }
		// GET: /T_Client/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_Client"))
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
            T_Client t_client = db.T_Clients.Find(id);
            if (t_client == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_ClientParentUrl"] = UrlReferrer;
		if(ViewData["T_ClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client/Edit/" + t_client.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client/Create"))
			ViewData["T_ClientParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_client);
		   ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
		   ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
          return View(t_client);
        }
		// POST: /T_Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_client).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
            return View(t_client);
        }


        // GET: /T_Client/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Client t_client = db.T_Clients.Find(id);
            if (t_client == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_ClientParentUrl"] = UrlReferrer;
		if(ViewData["T_ClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client/Edit/" + t_client.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client/Create"))
			ViewData["T_ClientParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_client);
		   ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
		   ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
          return View(t_client);
        }
        // POST: /T_Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client,  string UrlReferrer)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
                db.Entry(t_client).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_client.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
            return View(t_client);
        }
		// GET: /T_Client/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_Client"))
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
			            T_Client t_client = db.T_Clients.Find(id);
            if (t_client == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_ClientParentUrl"] = UrlReferrer;
		if(ViewData["T_ClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client"))
			ViewData["T_ClientParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_client);
			 ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
			 ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
          return View(t_client);
        }
        // POST: /T_Client/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client,string UrlReferrer)
        {
			CheckBeforeSave(t_client);
            if (ModelState.IsValid)
            {
                db.Entry(t_client).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_client);
			ViewBag.T_ClientIsHiddenRule = checkHidden("T_Client", "OnEdit", false);
			ViewBag.T_ClientIsGroupsHiddenRule = checkHidden("T_Client", "OnEdit", true);
			ViewBag.T_ClientIsSetValueUIRule = checkSetValueUIRule("T_Client", "OnEdit");
            return View(t_client);
        }
        // GET: /T_Client/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_Client t_client = db.T_Clients.Find(id);
            if (t_client == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_ClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_Client"))
			 ViewData["T_ClientParentUrl"] = Request.UrlReferrer;
            return View(t_client);
        }
        // POST: /T_Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_Client t_client, string UrlReferrer)
        {
			if (!User.CanDelete("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_client))
            {
 //Delete Document
			db.Entry(t_client).State = EntityState.Deleted;


            db.T_Clients.Remove(t_client);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_ClientParentUrl"] != null)
            {
                string parentUrl = ViewData["T_ClientParentUrl"].ToString();
                ViewData["T_ClientParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_client);
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
				T_Client t_client = db.T_Clients.Find(id);
                db.Entry(t_client).State = EntityState.Deleted;
                db.T_Clients.Remove(t_client);
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
            if (!User.CanEdit("T_Client"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_ClientParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_Name,T_Description")] T_Client t_client,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_Client target = db.T_Clients.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_client, target, chkUpdate); 
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
