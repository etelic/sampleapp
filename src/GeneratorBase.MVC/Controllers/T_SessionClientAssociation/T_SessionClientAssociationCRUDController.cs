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
    public partial class T_SessionClientAssociationController : BaseController
    {
        // GET: /T_SessionClientAssociation/
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
			var lstT_SessionClientAssociation = from s in db.T_SessionClientAssociations
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_SessionClientAssociation = searchRecords(lstT_SessionClientAssociation, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_SessionClientAssociation = sortRecords(lstT_SessionClientAssociation, sortBy, isAsc);
            }
            else lstT_SessionClientAssociation = lstT_SessionClientAssociation.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionClientAssociation"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionClientAssociation"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionClientAssociation"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionClientAssociation"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_SessionClientAssociation = lstT_SessionClientAssociation.Include(t=>t.t_client).Include(t=>t.t_session);
		 if (HostingEntity == "T_Client" && AssociatedType == "T_SessionClientAssociation_T_Client")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionClientAssociation = _T_SessionClientAssociation.Where(p => p.T_ClientID == hostid);
			 }
			 else
			     _T_SessionClientAssociation = _T_SessionClientAssociation.Where(p => p.T_ClientID == null);
         }
		 if (HostingEntity == "T_Session" && AssociatedType == "T_SessionClientAssociation_T_Session")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionClientAssociation = _T_SessionClientAssociation.Where(p => p.T_SessionID == hostid);
			 }
			 else
			     _T_SessionClientAssociation = _T_SessionClientAssociation.Where(p => p.T_SessionID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_SessionClientAssociation.Count() > 0)
                    pageSize = _T_SessionClientAssociation.Count();
                return View("ExcelExport", _T_SessionClientAssociation.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_SessionClientAssociation.Count();
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
				return View(_T_SessionClientAssociation.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_SessionClientAssociation = _fad.FilterDropdown<T_SessionClientAssociation>(User,  _T_SessionClientAssociation, "T_SessionClientAssociation", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_SessionClientAssociation.Except(_T_SessionClientAssociation),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_SessionClientAssociation.Except(_T_SessionClientAssociation).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_SessionClientAssociation.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_SessionClientAssociation.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_SessionClientAssociation.Count() == 0 ? 1 : _T_SessionClientAssociation.Count();
                                
                            }
							return PartialView("IndexPartial", _T_SessionClientAssociation.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_SessionClientAssociation.Count() == 0 ? 1 : _T_SessionClientAssociation.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_SessionClientAssociation.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_SessionClientAssociation/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
            if (t_sessionclientassociation == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_sessionclientassociation);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_sessionclientassociation, AssociatedType);
            ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnDetails", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnDetails", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnDetails");
			return View(ViewBag.TemplatesName,t_sessionclientassociation);

        }

        // GET: /T_SessionClientAssociation/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype )
        {
            if (!User.CanAdd("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
		  ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");
          return View();
        }
		// GET: /T_SessionClientAssociation/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
			 ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");
            return View();
        }
		// POST: /T_SessionClientAssociation/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation, string UrlReferrer)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
           db.T_SessionClientAssociations.Add(t_sessionclientassociation);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");	
            return View(t_sessionclientassociation);
        }
		 // GET: /T_SessionClientAssociation/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");
            return View();
        }
		  // POST: /T_SessionClientAssociation/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation, List<string> T_ClientIDSelected, List<string> T_ClientIDAvailable, List<string> T_SessionIDSelected, List<string> T_SessionIDAvailable,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
					if(T_ClientIDSelected != null || T_ClientIDAvailable != null)
				{
				    t_sessionclientassociation.t_session = db.T_Sessions.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_SessionID);
					var T_ClientIDSelectedlong =T_ClientIDSelected !=null ? T_ClientIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionClientAssociations.Where(p => p.T_SessionID == t_sessionclientassociation.T_SessionID && !T_ClientIDSelectedlong.Contains(p.T_ClientID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_session !=null )
					    db.Entry(item.t_session).State = EntityState.Unchanged;
						if(item.t_client !=null )
                        db.Entry(item.t_client).State = EntityState.Unchanged;
if (item.t_session != null && item.t_session.schedulesession !=null)
                        db.Entry(item.t_session.schedulesession).State = EntityState.Unchanged;
                        db.T_SessionClientAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ClientIDSelected != null)
						foreach (string id in T_ClientIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionClientAssociations.FirstOrDefault(a => a.T_SessionID == t_sessionclientassociation.T_SessionID && a.T_ClientID == longid) == null)
                            {
								var obj = new T_SessionClientAssociation(); 
								obj = t_sessionclientassociation;
								obj.T_ClientID = Convert.ToInt64(id);
								obj.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_ClientID);
								if(obj.t_session !=null )
									db.Entry(obj.t_session).State = EntityState.Unchanged;
								if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								//
if (obj.t_session != null && obj.t_session.schedulesession !=null)
                        db.Entry(obj.t_session.schedulesession).State = EntityState.Unchanged;
								//

								db.T_SessionClientAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_SessionIDSelected != null || T_SessionIDAvailable != null)
				{
				    t_sessionclientassociation.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_ClientID);
					var T_SessionIDSelectedlong =T_SessionIDSelected !=null ? T_SessionIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionClientAssociations.Where(p => p.T_ClientID == t_sessionclientassociation.T_ClientID && !T_SessionIDSelectedlong.Contains(p.T_SessionID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_client !=null )
					    db.Entry(item.t_client).State = EntityState.Unchanged;
						if(item.t_session !=null )
                        db.Entry(item.t_session).State = EntityState.Unchanged;
if (item.t_session != null && item.t_session.schedulesession !=null)
                        db.Entry(item.t_session.schedulesession).State = EntityState.Unchanged;
                        db.T_SessionClientAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_SessionIDSelected != null)
						foreach (string id in T_SessionIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionClientAssociations.FirstOrDefault(a => a.T_ClientID == t_sessionclientassociation.T_ClientID && a.T_SessionID == longid) == null)
                            {
								var obj = new T_SessionClientAssociation(); 
								obj = t_sessionclientassociation;
								obj.T_SessionID = Convert.ToInt64(id);
								obj.t_session = db.T_Sessions.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_SessionID);
								if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								if(obj.t_session !=null )
									db.Entry(obj.t_session).State = EntityState.Unchanged;
								//
if (obj.t_session != null && obj.t_session.schedulesession !=null)
                        db.Entry(obj.t_session.schedulesession).State = EntityState.Unchanged;
								//

								db.T_SessionClientAssociations.Add(obj); db.SaveChanges();
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
			LoadViewDataAfterOnCreate(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_sessionclientassociation, AssociatedEntity);
			return View(t_sessionclientassociation);
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
        // POST: /T_SessionClientAssociation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation, List<string> T_ClientIDSelected, List<string> T_ClientIDAvailable, List<string> T_SessionIDSelected, List<string> T_SessionIDAvailable, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
				if(T_ClientIDSelected != null || T_ClientIDAvailable != null)
				{
					t_sessionclientassociation.t_session = db.T_Sessions.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_SessionID);
					var T_ClientIDSelectedlong =T_ClientIDSelected !=null ? T_ClientIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionClientAssociations.Where(p => p.T_SessionID == t_sessionclientassociation.T_SessionID && !T_ClientIDSelectedlong.Contains(p.T_ClientID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_session !=null )
						db.Entry(item.t_session).State = EntityState.Unchanged;
					if(item.t_client !=null )
                        db.Entry(item.t_client).State = EntityState.Unchanged;
					if (item.t_session != null && item.t_session.schedulesession !=null)
                        db.Entry(item.t_session.schedulesession).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_SessionClientAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ClientIDSelected != null)
						foreach (string id in T_ClientIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionClientAssociations.FirstOrDefault(a => a.T_SessionID == t_sessionclientassociation.T_SessionID && a.T_ClientID == longid) == null)
                            {
								var obj = new T_SessionClientAssociation(); 
								obj = t_sessionclientassociation;
								obj.T_ClientID = Convert.ToInt64(id);
								 obj.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_ClientID);
								 if(obj.t_session !=null )
									db.Entry(obj.t_session).State = EntityState.Unchanged;
								if(obj.t_client !=null )
									 db.Entry(obj.t_client).State = EntityState.Unchanged;
								//
					if (obj.t_session != null && obj.t_session.schedulesession !=null)
                        db.Entry(obj.t_session.schedulesession).State = EntityState.Unchanged;
								//
								db.T_SessionClientAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_SessionIDSelected != null || T_SessionIDAvailable != null)
				{
					t_sessionclientassociation.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_ClientID);
					var T_SessionIDSelectedlong =T_SessionIDSelected !=null ? T_SessionIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionClientAssociations.Where(p => p.T_ClientID == t_sessionclientassociation.T_ClientID && !T_SessionIDSelectedlong.Contains(p.T_SessionID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_client !=null )
						db.Entry(item.t_client).State = EntityState.Unchanged;
					if(item.t_session !=null )
                        db.Entry(item.t_session).State = EntityState.Unchanged;
					
                        if (item.t_session != null && item.t_session.schedulesession !=null)
                        db.Entry(item.t_session.schedulesession).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_SessionClientAssociations.Remove(item);
                        db.SaveChanges();
                    }
					if(T_SessionIDSelected != null)
						foreach (string id in T_SessionIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionClientAssociations.FirstOrDefault(a => a.T_ClientID == t_sessionclientassociation.T_ClientID && a.T_SessionID == longid) == null)
                            {
								var obj = new T_SessionClientAssociation(); 
								obj = t_sessionclientassociation;
								obj.T_SessionID = Convert.ToInt64(id);
								 obj.t_session = db.T_Sessions.FirstOrDefault(p => p.Id == t_sessionclientassociation.T_SessionID);
								 if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								if(obj.t_session !=null )
									 db.Entry(obj.t_session).State = EntityState.Unchanged;
								//
					
                        if (obj.t_session != null && obj.t_session.schedulesession !=null)
                        db.Entry(obj.t_session.schedulesession).State = EntityState.Unchanged;
								//
								db.T_SessionClientAssociations.Add(obj); db.SaveChanges();
							}
						}
				}
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_sessionclientassociation.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnCreate", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnCreate");
            return View(t_sessionclientassociation);
        }
		// GET: /T_SessionClientAssociation/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_SessionClientAssociation"))
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
            T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
            if (t_sessionclientassociation == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionClientAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation/Edit/" + t_sessionclientassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation/Create"))
			ViewData["T_SessionClientAssociationParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessionclientassociation);
		   ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
		   ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
          return View(t_sessionclientassociation);
        }
		// POST: /T_SessionClientAssociation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_sessionclientassociation).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
            return View(t_sessionclientassociation);
        }


        // GET: /T_SessionClientAssociation/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
            if (t_sessionclientassociation == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionClientAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation/Edit/" + t_sessionclientassociation.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation/Create"))
			ViewData["T_SessionClientAssociationParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessionclientassociation);
		   ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
		   ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
          return View(t_sessionclientassociation);
        }
        // POST: /T_SessionClientAssociation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation,  string UrlReferrer)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_sessionclientassociation).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_sessionclientassociation.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
            return View(t_sessionclientassociation);
        }
		// GET: /T_SessionClientAssociation/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_SessionClientAssociation"))
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
			            T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
            if (t_sessionclientassociation == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionClientAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation"))
			ViewData["T_SessionClientAssociationParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_sessionclientassociation);
			 ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
			 ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
          return View(t_sessionclientassociation);
        }
        // POST: /T_SessionClientAssociation/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation,string UrlReferrer)
        {
			CheckBeforeSave(t_sessionclientassociation);
            if (ModelState.IsValid)
            {
                db.Entry(t_sessionclientassociation).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_sessionclientassociation);
			ViewBag.T_SessionClientAssociationIsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", false);
			ViewBag.T_SessionClientAssociationIsGroupsHiddenRule = checkHidden("T_SessionClientAssociation", "OnEdit", true);
			ViewBag.T_SessionClientAssociationIsSetValueUIRule = checkSetValueUIRule("T_SessionClientAssociation", "OnEdit");
            return View(t_sessionclientassociation);
        }
        // GET: /T_SessionClientAssociation/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
            if (t_sessionclientassociation == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_SessionClientAssociationParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionClientAssociation"))
			 ViewData["T_SessionClientAssociationParentUrl"] = Request.UrlReferrer;
            return View(t_sessionclientassociation);
        }
        // POST: /T_SessionClientAssociation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_SessionClientAssociation t_sessionclientassociation, string UrlReferrer)
        {
			if (!User.CanDelete("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_sessionclientassociation))
            {
 //Delete Document
			db.Entry(t_sessionclientassociation).State = EntityState.Deleted;


            db.T_SessionClientAssociations.Remove(t_sessionclientassociation);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_SessionClientAssociationParentUrl"] != null)
            {
                string parentUrl = ViewData["T_SessionClientAssociationParentUrl"].ToString();
                ViewData["T_SessionClientAssociationParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_sessionclientassociation);
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
				T_SessionClientAssociation t_sessionclientassociation = db.T_SessionClientAssociations.Find(id);
                db.Entry(t_sessionclientassociation).State = EntityState.Deleted;
                db.T_SessionClientAssociations.Remove(t_sessionclientassociation);
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
            if (!User.CanEdit("T_SessionClientAssociation"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_SessionClientAssociationParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionID")] T_SessionClientAssociation t_sessionclientassociation,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_SessionClientAssociation target = db.T_SessionClientAssociations.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_sessionclientassociation, target, chkUpdate); 
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
