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
    public partial class PropertyMappingController : BaseController
    {
        // GET: /PropertyMapping/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate)
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
            var lstPropertyMapping = from s in db.PropertyMappings
                                     select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstPropertyMapping = searchRecords(lstPropertyMapping, searchString.ToUpper(), IsDeepSearch);
            }
            if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstPropertyMapping = sortRecords(lstPropertyMapping, sortBy, isAsc);
            }
            else lstPropertyMapping = lstPropertyMapping.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "PropertyMapping"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
            var _PropertyMapping = lstPropertyMapping.Include(t => t.entitypropertymapping);
            if (HostingEntity == "EntityDataSource" && AssociatedType == "EntityPropertyMapping")
            {
                if (HostingEntityID != null)
                {
                    long hostid = Convert.ToInt64(HostingEntityID);
                    _PropertyMapping = _PropertyMapping.Where(p => p.EntityPropertyMappingID == hostid);
                }
                else
                    _PropertyMapping = _PropertyMapping.Where(p => p.EntityPropertyMappingID == null);
            }

            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_PropertyMapping.Count() > 0)
                    pageSize = _PropertyMapping.Count();
                return View("ExcelExport", _PropertyMapping.ToPagedList(pageNumber, pageSize));
            }
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_PropertyMapping.ToPagedList(pageNumber, pageSize));
            else
            {
                if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                {
                    ViewData["BulkAssociate"] = BulkAssociate;
                    if (caller != "")
                    {
                        FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
                        _PropertyMapping = _fad.FilterDropdown<PropertyMapping>(User, _PropertyMapping, "PropertyMapping", caller);
                    }
                    if (Convert.ToBoolean(BulkAssociate))
                    {
                        if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                            return PartialView("BulkOperation", sortRecords(lstPropertyMapping.Except(_PropertyMapping), sortBy, isAsc).ToPagedList(pageNumber, pageSize));
                        else
                            return PartialView("BulkOperation", lstPropertyMapping.Except(_PropertyMapping).OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
                            return PartialView("BulkOperation", _PropertyMapping.ToPagedList(pageNumber, pageSize));
                        else
                            return PartialView("BulkOperation", _PropertyMapping.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                    }
                }
                else
                    return PartialView("IndexPartial", _PropertyMapping.ToPagedList(pageNumber, pageSize));
            }
        }
        // GET: /PropertyMapping/Details/5
        public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            if (propertymapping == null)
            {
                return HttpNotFound();
            }
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            LoadViewDataBeforeOnEdit(propertymapping);
            if (!string.IsNullOrEmpty(AssociatedType))
                LoadViewDataForCount(propertymapping, AssociatedType);
            return View(propertymapping);
        }
        // GET: /PropertyMapping/Create
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View();
        }
        // GET: /PropertyMapping/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
            if (!User.CanAdd("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }

            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View();
        }
        // POST: /PropertyMapping/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
        {
            CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                db.PropertyMappings.Add(propertymapping);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }

            LoadViewDataAfterOnCreate(propertymapping);
            return View(propertymapping);
        }
        // GET: /PropertyMapping/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;

            var HostingObject = db.EntityDataSources.Find(Convert.ToInt64(HostingEntityID));
            var EntityName= string.Empty;
            if (HostingObject != null)
            { 
                EntityName = HostingObject.EntityName;
                var properymappings = db.PropertyMappings.Where(p => p.EntityPropertyMappingID == HostingObject.Id);

            Dictionary<GeneratorBase.MVC.ModelReflector.Property, string> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, string>();
           // var col = new List<SelectListItem>();
            var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == EntityName);
            if (entList != null)
            {
                foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                {
                    var selectedVal = "";
                    var map = properymappings.FirstOrDefault(p => p.PropertyName == prop.Name);

                    if (map != null)
                        selectedVal = map.DataName;
                    else
                        selectedVal = prop.Name;
                   // objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                    objColMap.Add(prop, selectedVal);
                }
            }
            ViewBag.ColumnMapping = objColMap;
        }
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View();
        }
        // POST: /PropertyMapping/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
            //CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                var properties = propertymapping.PropertyName.Split(",".ToCharArray());
                var jsonproperties = propertymapping.DataName.Split(",".ToCharArray());
                if (properties.Count() == jsonproperties.Count())
                {
                    foreach(var item in db.PropertyMappings.Where(p=>p.EntityPropertyMappingID== propertymapping.EntityPropertyMappingID).ToList())
                    {
                        db.Entry(item).State = EntityState.Deleted;
                        db.PropertyMappings.Remove(item);
                        db.SaveChanges();
                    }

                    for (var i = 0; i < properties.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(properties[i].Trim()))
                        {
                            var obj = new PropertyMapping();
                            obj.EntityPropertyMappingID = propertymapping.EntityPropertyMappingID;
                            obj.PropertyName = properties[i].Trim();
                            obj.DataName = jsonproperties[i].Trim();
                            obj.SourceType = propertymapping.SourceType;
                            obj.MethodType = propertymapping.MethodType;
                            db.PropertyMappings.Add(obj);
                            db.SaveChanges();
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
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            LoadViewDataAfterOnCreate(propertymapping);
            if (!string.IsNullOrEmpty(AssociatedEntity))
                LoadViewDataForCount(propertymapping, AssociatedEntity);
            return View(propertymapping);
        }
        public ActionResult Cancel(string UrlReferrer)
        {
            if (!string.IsNullOrEmpty(UrlReferrer))
            {
                var uri = new Uri(UrlReferrer);
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (Convert.ToBoolean(query.Get("IsFilter")) == true)
                    return RedirectToAction("Index");
                else
                    return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }
        // POST: /PropertyMapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsDDAdd)
        {
            CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.PropertyMappings.Add(propertymapping);
                db.SaveChanges();
                if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = propertymapping.Id, UrlReferrer = UrlReferrer });
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }

            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            LoadViewDataAfterOnCreate(propertymapping);
            return View(propertymapping);
        }
        // GET: /PropertyMapping/Edit/5
        public ActionResult EditQuick(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            if (propertymapping == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            if (ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Edit/" + propertymapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Create"))
                ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["AssociatedType"] = AssociatedType;
            LoadViewDataBeforeOnEdit(propertymapping);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View(propertymapping);
        }
        // POST: /PropertyMapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer, bool? IsAddPop, string AssociatedEntity)
        {
            CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.Entry(propertymapping).State = EntityState.Modified;

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

            LoadViewDataAfterOnEdit(propertymapping);
            return View(propertymapping);
        }


        // GET: /PropertyMapping/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            if (propertymapping == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            if (ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Edit/" + propertymapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping/Create"))
                ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["AssociatedType"] = AssociatedType;
            LoadViewDataBeforeOnEdit(propertymapping);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View(propertymapping);
        }
        // POST: /PropertyMapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
        {
            CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.Entry(propertymapping).State = EntityState.Modified;
                db.SaveChanges();
                if (command != "Save")
                    return RedirectToAction("Edit", new { Id = propertymapping.Id, UrlReferrer = UrlReferrer });
                if (!string.IsNullOrEmpty(UrlReferrer))
                {
                    var uri = new Uri(UrlReferrer);
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    if (Convert.ToBoolean(query.Get("IsFilter")) == true)
                        return RedirectToAction("Index");
                    else
                        return Redirect(UrlReferrer);
                }
                else
                    return RedirectToAction("Index");
            }

            LoadViewDataAfterOnEdit(propertymapping);
            return View(propertymapping);
        }
        // GET: /PropertyMapping/EditWizard/5
        public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            if (propertymapping == null)
            {
                return HttpNotFound();
            }

            if (UrlReferrer != null)
                ViewData["PropertyMappingParentUrl"] = UrlReferrer;
            if (ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping"))
                ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
            LoadViewDataBeforeOnEdit(propertymapping);
            ViewBag.PropertyMappingIsHiddenRule = checkHidden("PropertyMapping");
            return View(propertymapping);
        }
        // POST: /PropertyMapping/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, string UrlReferrer)
        {
            CheckBeforeSave(propertymapping);
            if (ModelState.IsValid)
            {
                db.Entry(propertymapping).State = EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                {
                    var uri = new Uri(UrlReferrer);
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    if (Convert.ToBoolean(query.Get("IsFilter")) == true)
                        return RedirectToAction("Index");
                    else
                        return Redirect(UrlReferrer);
                }
                else
                    return RedirectToAction("Index");
            }
            LoadViewDataAfterOnEdit(propertymapping);
            return View(propertymapping);
        }
        // GET: /PropertyMapping/Delete/5
        public ActionResult Delete(int id)
        {
            if (!User.CanDelete("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PropertyMapping propertymapping = db.PropertyMappings.Find(id);
            if (propertymapping == null)
            {
                throw (new Exception("Deleted"));
            }
            if (ViewData["PropertyMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/PropertyMapping"))
                ViewData["PropertyMappingParentUrl"] = Request.UrlReferrer;
            return View(propertymapping);
        }
        // POST: /PropertyMapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(PropertyMapping propertymapping, string UrlReferrer)
        {
            if (!User.CanDelete("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (CheckBeforeDelete(propertymapping))
            {
                //Delete Document
                db.Entry(propertymapping).State = EntityState.Deleted;
                db.PropertyMappings.Remove(propertymapping);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                if (ViewData["PropertyMappingParentUrl"] != null)
                {
                    string parentUrl = ViewData["PropertyMappingParentUrl"].ToString();
                    ViewData["PropertyMappingParentUrl"] = null;
                    return Redirect(parentUrl);
                }
                else return RedirectToAction("Index");
            }
            return View(propertymapping);
        }
        public ActionResult BulkAssociate(long[] ids, string AssociatedType, string HostingEntity, string HostingEntityID)
        {
            var HostingID = Convert.ToInt64(HostingEntityID);
            if (HostingID == 0)
                return Json("Error", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            if (HostingEntity == "EntityDataSource" && AssociatedType == "EntityPropertyMapping")
            {
                foreach (var id in ids.Where(p => p > 0))
                {
                    PropertyMapping obj = db.PropertyMappings.Find(id);
                    db.Entry(obj).State = EntityState.Modified;
                    obj.EntityPropertyMappingID = HostingID;
                    db.SaveChanges();
                }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
        {
            foreach (var id in ids.Where(p => p > 0))
            {
                PropertyMapping propertymapping = db.PropertyMappings.Find(id);
                db.Entry(propertymapping).State = EntityState.Deleted;
                db.PropertyMappings.Remove(propertymapping);
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
            if (!User.CanEdit("PropertyMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDUpdate != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDUpdate);
            ViewData["PropertyMappingParentUrl"] = UrlReferrer;
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
        public ActionResult BulkUpdate([Bind(Include = "Id,ConcurrencyKey,PropertyName,DataName,DataSource,SourceType,MethodType,Action,EntityPropertyMappingID")] PropertyMapping propertymapping, FormCollection collection, string UrlReferrer)
        {
            var bulkIds = collection["BulkUpdate"].Split(',').ToList();
            var chkUpdate = collection["chkUpdate"];
            if (!string.IsNullOrEmpty(chkUpdate))
            {
                foreach (var id in bulkIds.Where(p => p != string.Empty))
                {
                    long objId = long.Parse(id);
                    PropertyMapping target = db.PropertyMappings.Find(objId);
                    EntityCopy.CopyValuesForSameObjectType(propertymapping, target, chkUpdate);
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
