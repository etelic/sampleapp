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
    public partial class T_SessionEventsClientController : BaseController
    {
        // GET: /T_SessionEventsClient/
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
			var lstT_SessionEventsClient = from s in db.T_SessionEventsClients
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_SessionEventsClient = searchRecords(lstT_SessionEventsClient, searchString.ToUpper(), IsDeepSearch);
            }
	   if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_SessionEventsClient = sortRecords(lstT_SessionEventsClient, sortBy, isAsc);
            }
            else lstT_SessionEventsClient = lstT_SessionEventsClient.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
			ViewBag.Pages = page;
			if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
			 //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionEventsClient"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name)  + "T_SessionEventsClient"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
			//Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionEventsClient"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "T_SessionEventsClient"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
			var _T_SessionEventsClient = lstT_SessionEventsClient.Include(t=>t.t_client).Include(t=>t.t_sessionevents);
		 if (HostingEntity == "T_Client" && AssociatedType == "T_SessionEventsClient_T_Client")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionEventsClient = _T_SessionEventsClient.Where(p => p.T_ClientID == hostid);
			 }
			 else
			     _T_SessionEventsClient = _T_SessionEventsClient.Where(p => p.T_ClientID == null);
         }
		 if (HostingEntity == "T_SessionEvents" && AssociatedType == "T_SessionEventsClient_T_SessionEvents")
         {
			 if (HostingEntityID != null)
             {
                long hostid = Convert.ToInt64(HostingEntityID);
                _T_SessionEventsClient = _T_SessionEventsClient.Where(p => p.T_SessionEventsID == hostid);
			 }
			 else
			     _T_SessionEventsClient = _T_SessionEventsClient.Where(p => p.T_SessionEventsID == null);
         }
	
			 if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_SessionEventsClient.Count() > 0)
                    pageSize = _T_SessionEventsClient.Count();
                return View("ExcelExport", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_SessionEventsClient.Count();
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
				return View(_T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
			 }
			 else
				{
					if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
					{
						ViewData["BulkAssociate"] = BulkAssociate;
						if (!string.IsNullOrEmpty(caller))
						{
							FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
							_T_SessionEventsClient = _fad.FilterDropdown<T_SessionEventsClient>(User,  _T_SessionEventsClient, "T_SessionEventsClient", caller);
						}
						if (Convert.ToBoolean(BulkAssociate))
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation",sortRecords(lstT_SessionEventsClient.Except(_T_SessionEventsClient),sortBy,isAsc).ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", lstT_SessionEventsClient.Except(_T_SessionEventsClient).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
						}
						else
						{
							if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
								return PartialView("BulkOperation", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
							else
								return PartialView("BulkOperation", _T_SessionEventsClient.OrderBy(q=>q.DisplayValue).ToPagedList(pageNumber, pageSize)); 
						}
					}
					else
					{

					  if (ViewBag.TemplatesName == null)
					  {
						if (!string.IsNullOrEmpty(isMobileHome))
                            {
								 pageSize = _T_SessionEventsClient.Count() == 0 ? 1 : _T_SessionEventsClient.Count();
                                
                            }
							return PartialView("IndexPartial", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
					  }
					  else
					  {
							if (!string.IsNullOrEmpty(isMobileHome))
                            {
                                pageSize = _T_SessionEventsClient.Count() == 0 ? 1 : _T_SessionEventsClient.Count();
                            }
							return PartialView(ViewBag.TemplatesName, _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
					  }
					}
				}
        }
		 // GET: /T_SessionEventsClient/Details/5
		 		public ActionResult Details(int? id,string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
            if (t_sessioneventsclient == null)
            {
                return HttpNotFound();
            }
			
			if (string.IsNullOrEmpty(defaultview))
                defaultview = "Details";
            GetTemplatesForDetails(defaultview);
			ViewData["AssociatedType"] = AssociatedType;
		    ViewData["HostingEntityName"] = HostingEntityName;
			LoadViewDataBeforeOnEdit(t_sessioneventsclient);	
			if (!string.IsNullOrEmpty(AssociatedType))
                    LoadViewDataForCount(t_sessioneventsclient, AssociatedType);
            ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnDetails", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnDetails", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnDetails");
			return View(ViewBag.TemplatesName,t_sessioneventsclient);

        }

        // GET: /T_SessionEventsClient/Create
        public ActionResult Create(string UrlReferrer,string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd, string viewtype )
        {
            if (!User.CanAdd("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
		  if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		   if (string.IsNullOrEmpty(viewtype))
                viewtype = "Create";
            GetTemplatesForCreate(viewtype);
		  ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
		  ViewData["AssociatedType"] = AssociatedType;
		  ViewData["HostingEntityName"] = HostingEntityName;
		  ViewData["HostingEntityID"] = HostingEntityID;
		  LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		  ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
		  ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");
          return View();
        }
		// GET: /T_SessionEventsClient/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer,string HostingEntityName, string HostingEntityID,string AssociatedType, string viewtype)
        {
            if (!User.CanAdd("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
	
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateWizard";
            GetTemplatesForCreateWizard(viewtype);
		    ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
			LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
			 ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
			 ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");
            return View();
        }
		// POST: /T_SessionEventsClient/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient, string UrlReferrer)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
           db.T_SessionEventsClients.Add(t_sessioneventsclient);
           db.SaveChanges();
	
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
			LoadViewDataAfterOnCreate(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");	
            return View(t_sessioneventsclient);
        }
		 // GET: /T_SessionEventsClient/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop, string viewtype)
        {
            if (!User.CanAdd("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
			if (string.IsNullOrEmpty(viewtype))
                viewtype = "CreateQuick";
            GetTemplatesForCreateQuick(viewtype);
			 ViewBag.IsAddPop = IsAddPop;
		   ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
		   ViewData["AssociatedType"] = AssociatedType;
           ViewData["HostingEntityName"] = HostingEntityName;
		   ViewData["HostingEntityID"] = HostingEntityID;
		   LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
		    ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");
            return View();
        }
		  // POST: /T_SessionEventsClient/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		        public ActionResult CreateQuick([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient, List<string> T_ClientIDSelected, List<string> T_ClientIDAvailable, List<string> T_SessionEventsIDSelected, List<string> T_SessionEventsIDAvailable,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
					if(T_ClientIDSelected != null || T_ClientIDAvailable != null)
				{
				    t_sessioneventsclient.t_sessionevents = db.T_SessionEventss.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_SessionEventsID);
					var T_ClientIDSelectedlong =T_ClientIDSelected !=null ? T_ClientIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionEventsClients.Where(p => p.T_SessionEventsID == t_sessioneventsclient.T_SessionEventsID && !T_ClientIDSelectedlong.Contains(p.T_ClientID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_sessionevents !=null )
					    db.Entry(item.t_sessionevents).State = EntityState.Unchanged;
						if(item.t_client !=null )
                        db.Entry(item.t_client).State = EntityState.Unchanged;
                        db.T_SessionEventsClients.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ClientIDSelected != null)
						foreach (string id in T_ClientIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionEventsClients.FirstOrDefault(a => a.T_SessionEventsID == t_sessioneventsclient.T_SessionEventsID && a.T_ClientID == longid) == null)
                            {
								var obj = new T_SessionEventsClient(); 
								obj = t_sessioneventsclient;
								obj.T_ClientID = Convert.ToInt64(id);
								obj.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_ClientID);
								if(obj.t_sessionevents !=null )
									db.Entry(obj.t_sessionevents).State = EntityState.Unchanged;
								if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								//
								//

								db.T_SessionEventsClients.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_SessionEventsIDSelected != null || T_SessionEventsIDAvailable != null)
				{
				    t_sessioneventsclient.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_ClientID);
					var T_SessionEventsIDSelectedlong =T_SessionEventsIDSelected !=null ? T_SessionEventsIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionEventsClients.Where(p => p.T_ClientID == t_sessioneventsclient.T_ClientID && !T_SessionEventsIDSelectedlong.Contains(p.T_SessionEventsID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_client !=null )
					    db.Entry(item.t_client).State = EntityState.Unchanged;
						if(item.t_sessionevents !=null )
                        db.Entry(item.t_sessionevents).State = EntityState.Unchanged;
                        db.T_SessionEventsClients.Remove(item);
                        db.SaveChanges();
                    }
					if(T_SessionEventsIDSelected != null)
						foreach (string id in T_SessionEventsIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionEventsClients.FirstOrDefault(a => a.T_ClientID == t_sessioneventsclient.T_ClientID && a.T_SessionEventsID == longid) == null)
                            {
								var obj = new T_SessionEventsClient(); 
								obj = t_sessioneventsclient;
								obj.T_SessionEventsID = Convert.ToInt64(id);
								obj.t_sessionevents = db.T_SessionEventss.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_SessionEventsID);
								if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								if(obj.t_sessionevents !=null )
									db.Entry(obj.t_sessionevents).State = EntityState.Unchanged;
								//
								//

								db.T_SessionEventsClients.Add(obj); db.SaveChanges();
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
			LoadViewDataAfterOnCreate(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");
            if (!string.IsNullOrEmpty(AssociatedEntity))
                    LoadViewDataForCount(t_sessioneventsclient, AssociatedEntity);
			return View(t_sessioneventsclient);
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
        // POST: /T_SessionEventsClient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient, List<string> T_ClientIDSelected, List<string> T_ClientIDAvailable, List<string> T_SessionEventsIDSelected, List<string> T_SessionEventsIDAvailable, string UrlReferrer, bool? IsDDAdd)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
			 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
				if(T_ClientIDSelected != null || T_ClientIDAvailable != null)
				{
					t_sessioneventsclient.t_sessionevents = db.T_SessionEventss.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_SessionEventsID);
					var T_ClientIDSelectedlong =T_ClientIDSelected !=null ? T_ClientIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionEventsClients.Where(p => p.T_SessionEventsID == t_sessioneventsclient.T_SessionEventsID && !T_ClientIDSelectedlong.Contains(p.T_ClientID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_sessionevents !=null )
						db.Entry(item.t_sessionevents).State = EntityState.Unchanged;
					if(item.t_client !=null )
                        db.Entry(item.t_client).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_SessionEventsClients.Remove(item);
                        db.SaveChanges();
                    }
					if(T_ClientIDSelected != null)
						foreach (string id in T_ClientIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionEventsClients.FirstOrDefault(a => a.T_SessionEventsID == t_sessioneventsclient.T_SessionEventsID && a.T_ClientID == longid) == null)
                            {
								var obj = new T_SessionEventsClient(); 
								obj = t_sessioneventsclient;
								obj.T_ClientID = Convert.ToInt64(id);
								 obj.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_ClientID);
								 if(obj.t_sessionevents !=null )
									db.Entry(obj.t_sessionevents).State = EntityState.Unchanged;
								if(obj.t_client !=null )
									 db.Entry(obj.t_client).State = EntityState.Unchanged;
								//
								//
								db.T_SessionEventsClients.Add(obj); db.SaveChanges();
							}
						}
				}
				if(T_SessionEventsIDSelected != null || T_SessionEventsIDAvailable != null)
				{
					t_sessioneventsclient.t_client = db.T_Clients.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_ClientID);
					var T_SessionEventsIDSelectedlong =T_SessionEventsIDSelected !=null ? T_SessionEventsIDSelected.Select(long.Parse).ToList():new List<long>();
					var deletedItems = db.T_SessionEventsClients.Where(p => p.T_ClientID == t_sessioneventsclient.T_ClientID && !T_SessionEventsIDSelectedlong.Contains(p.T_SessionEventsID.Value)).ToList();
                    foreach (var item in deletedItems)
                    {
					if(item.t_client !=null )
						db.Entry(item.t_client).State = EntityState.Unchanged;
					if(item.t_sessionevents !=null )
                        db.Entry(item.t_sessionevents).State = EntityState.Unchanged;
						db.Entry(item).State = EntityState.Deleted;
                        db.T_SessionEventsClients.Remove(item);
                        db.SaveChanges();
                    }
					if(T_SessionEventsIDSelected != null)
						foreach (string id in T_SessionEventsIDSelected)
						{
							long longid = Convert.ToInt64(id);
                            if (db.T_SessionEventsClients.FirstOrDefault(a => a.T_ClientID == t_sessioneventsclient.T_ClientID && a.T_SessionEventsID == longid) == null)
                            {
								var obj = new T_SessionEventsClient(); 
								obj = t_sessioneventsclient;
								obj.T_SessionEventsID = Convert.ToInt64(id);
								 obj.t_sessionevents = db.T_SessionEventss.FirstOrDefault(p => p.Id == t_sessioneventsclient.T_SessionEventsID);
								 if(obj.t_client !=null )
									db.Entry(obj.t_client).State = EntityState.Unchanged;
								if(obj.t_sessionevents !=null )
									 db.Entry(obj.t_sessionevents).State = EntityState.Unchanged;
								//
								//
								db.T_SessionEventsClients.Add(obj); db.SaveChanges();
							}
						}
				}
				 if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = t_sessioneventsclient.Id, UrlReferrer = UrlReferrer });
				if(!string.IsNullOrEmpty(UrlReferrer))
                   return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
	
		 if (IsDDAdd != null)
              ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
		LoadViewDataAfterOnCreate(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnCreate", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnCreate");
            return View(t_sessioneventsclient);
        }
		// GET: /T_SessionEventsClient/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string viewtype)
        {
            if (!User.CanEdit("T_SessionEventsClient"))
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
            T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
            if (t_sessioneventsclient == null)
            {
                return HttpNotFound();
            }
		if (UrlReferrer != null)
                ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient/Edit/" + t_sessioneventsclient.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient/Create"))
			ViewData["T_SessionEventsClientParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessioneventsclient);
		   ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
		   ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
          return View(t_sessioneventsclient);
        }
		// POST: /T_SessionEventsClient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient,  string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_sessioneventsclient).State = EntityState.Modified;
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
			
			LoadViewDataAfterOnEdit(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
            return View(t_sessioneventsclient);
        }


        // GET: /T_SessionEventsClient/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (!User.CanEdit("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
            if (t_sessioneventsclient == null)
            {
                return HttpNotFound();
            }
		if (string.IsNullOrEmpty(defaultview))
                defaultview = "Edit";
            GetTemplatesForEdit(defaultview);

		if (UrlReferrer != null)
                ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient")  && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient/Edit/" + t_sessioneventsclient.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient/Create"))
			ViewData["T_SessionEventsClientParentUrl"] = Request.UrlReferrer;
		 ViewData["HostingEntityName"] = HostingEntityName;
         ViewData["AssociatedType"] = AssociatedType;
		  LoadViewDataBeforeOnEdit(t_sessioneventsclient);
		   ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
		   ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
          return View(t_sessioneventsclient);
        }
        // POST: /T_SessionEventsClient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient,  string UrlReferrer)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
				 string command = Request.Form["hdncommand"];
				string path = Server.MapPath("~/Files/");
				string ticks = DateTime.Now.Ticks.ToString();
                db.Entry(t_sessioneventsclient).State = EntityState.Modified;
                db.SaveChanges();
				 if (command != "Save")
                    return RedirectToAction("Edit", new { Id = t_sessioneventsclient.Id, UrlReferrer = UrlReferrer });
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
			
			LoadViewDataAfterOnEdit(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
            return View(t_sessioneventsclient);
        }
		// GET: /T_SessionEventsClient/EditWizard/5
         public ActionResult EditWizard(int? id, string UrlReferrer,string viewtype)
        {
            if (!User.CanEdit("T_SessionEventsClient"))
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
			            T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
            if (t_sessioneventsclient == null)
            {
                return HttpNotFound();
            }
	
		 if (UrlReferrer != null)
                ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
		if(ViewData["T_SessionEventsClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient"))
			ViewData["T_SessionEventsClientParentUrl"] = Request.UrlReferrer;
			 LoadViewDataBeforeOnEdit(t_sessioneventsclient);
			 ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
			 ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
          return View(t_sessioneventsclient);
        }
        // POST: /T_SessionEventsClient/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient,string UrlReferrer)
        {
			CheckBeforeSave(t_sessioneventsclient);
            if (ModelState.IsValid)
            {
                db.Entry(t_sessioneventsclient).State = EntityState.Modified;
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
			LoadViewDataAfterOnEdit(t_sessioneventsclient);
			ViewBag.T_SessionEventsClientIsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", false);
			ViewBag.T_SessionEventsClientIsGroupsHiddenRule = checkHidden("T_SessionEventsClient", "OnEdit", true);
			ViewBag.T_SessionEventsClientIsSetValueUIRule = checkSetValueUIRule("T_SessionEventsClient", "OnEdit");
            return View(t_sessioneventsclient);
        }
        // GET: /T_SessionEventsClient/Delete/5
        public ActionResult Delete(int id)
        {
			if (!User.CanDelete("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			            T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
            if (t_sessioneventsclient == null)
            {
                throw(new Exception("Deleted"));
            }
			if(ViewData["T_SessionEventsClientParentUrl"] == null  && Request.UrlReferrer !=null && ! Request.UrlReferrer.AbsolutePath.EndsWith("/T_SessionEventsClient"))
			 ViewData["T_SessionEventsClientParentUrl"] = Request.UrlReferrer;
            return View(t_sessioneventsclient);
        }
        // POST: /T_SessionEventsClient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(T_SessionEventsClient t_sessioneventsclient, string UrlReferrer)
        {
			if (!User.CanDelete("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
			 if (CheckBeforeDelete(t_sessioneventsclient))
            {
 //Delete Document
			db.Entry(t_sessioneventsclient).State = EntityState.Deleted;


            db.T_SessionEventsClients.Remove(t_sessioneventsclient);
            db.SaveChanges();
			if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["T_SessionEventsClientParentUrl"] != null)
            {
                string parentUrl = ViewData["T_SessionEventsClientParentUrl"].ToString();
                ViewData["T_SessionEventsClientParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
			 }
            return View(t_sessioneventsclient);
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
				T_SessionEventsClient t_sessioneventsclient = db.T_SessionEventsClients.Find(id);
                db.Entry(t_sessioneventsclient).State = EntityState.Deleted;
                db.T_SessionEventsClients.Remove(t_sessioneventsclient);
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
            if (!User.CanEdit("T_SessionEventsClient"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["T_SessionEventsClientParentUrl"] = UrlReferrer;
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
		public ActionResult BulkUpdate([Bind(Include="Id,ConcurrencyKey,T_ClientID,T_SessionEventsID")] T_SessionEventsClient t_sessioneventsclient,FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
			if (!string.IsNullOrEmpty(chkUpdate))
            {
            foreach (var id in bulkIds.Where(p => p != string.Empty))
            {
			                long objId = long.Parse(id);
                T_SessionEventsClient target = db.T_SessionEventsClients.Find(objId);
                EntityCopy.CopyValuesForSameObjectType(t_sessioneventsclient, target, chkUpdate); 
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
