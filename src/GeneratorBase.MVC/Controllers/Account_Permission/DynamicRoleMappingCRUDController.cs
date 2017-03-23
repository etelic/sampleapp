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
    public partial class DynamicRoleMappingController : BaseController
    {
        // GET: /DynamicRoleMapping/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent)
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
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            var lstDynamicRoleMapping = from s in db.DynamicRoleMappings
                                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstDynamicRoleMapping = searchRecords(lstDynamicRoleMapping, searchString.ToUpper(), IsDeepSearch);
            }
            if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstDynamicRoleMapping = sortRecords(lstDynamicRoleMapping, sortBy, isAsc);
            }
            else lstDynamicRoleMapping = lstDynamicRoleMapping.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "DynamicRoleMapping"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
            var _DynamicRoleMapping = lstDynamicRoleMapping;
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_DynamicRoleMapping.Count() > 0)
                    pageSize = _DynamicRoleMapping.Count();
                return View("ExcelExport", _DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
            }
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
            else
            {
                if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                    return PartialView("BulkOperation", _DynamicRoleMapping.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                else
                    return PartialView("IndexPartial", _DynamicRoleMapping.ToPagedList(pageNumber, pageSize));
            }
        }
        // GET: /DynamicRoleMapping/Details/5
        public ActionResult Details(int? id, string HostingEntityName, string AssociatedType)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
            if (dynamicrolemapping == null)
            {
                return HttpNotFound();
            }
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            return View(dynamicrolemapping);
        }
        // GET: /DynamicRoleMapping/Create
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {
            if (!User.CanAdd("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // GET: /DynamicRoleMapping/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
            if (!User.CanAdd("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // POST: /DynamicRoleMapping/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.DynamicRoleMappings.Add(dynamicrolemapping);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            LoadViewDataAfterOnCreate(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // GET: /DynamicRoleMapping/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsAddPop)
        {
            if (!User.CanAdd("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["HostingEntityID"] = HostingEntityID;
            LoadViewDataBeforeOnCreate(HostingEntityName, HostingEntityID, AssociatedType);
            return View();
        }
        // POST: /DynamicRoleMapping/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.DynamicRoleMappings.Add(dynamicrolemapping);
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
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            LoadViewDataAfterOnCreate(dynamicrolemapping);
            return View(dynamicrolemapping);
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
        // POST: /DynamicRoleMapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer, bool? IsDDAdd)
        {
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.DynamicRoleMappings.Add(dynamicrolemapping);
                db.SaveChanges();
                if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = dynamicrolemapping.Id, UrlReferrer = UrlReferrer });
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            LoadViewDataAfterOnCreate(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // GET: /DynamicRoleMapping/Edit/5
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
            if (dynamicrolemapping == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
            if (ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping/Edit/" + dynamicrolemapping.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping/Create"))
                ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["AssociatedType"] = AssociatedType;
            LoadViewDataBeforeOnEdit(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // POST: /DynamicRoleMapping/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.Entry(dynamicrolemapping).State = EntityState.Modified;
                db.SaveChanges();
                if (command == "Save & Continue")
                    return RedirectToAction("Edit", new { Id = dynamicrolemapping.Id, UrlReferrer = UrlReferrer });
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
            LoadViewDataAfterOnEdit(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // GET: /DynamicRoleMapping/EditWizard/5
        public ActionResult EditWizard(int? id, string UrlReferrer)
        {
            if (!User.CanEdit("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
            if (dynamicrolemapping == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["DynamicRoleMappingParentUrl"] = UrlReferrer;
            if (ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping"))
                ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
            LoadViewDataBeforeOnEdit(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // POST: /DynamicRoleMapping/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include = "Id,ConcurrencyKey,EntityName,RoleId,Condition,Value,UserRelation,Other,Flag")] DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dynamicrolemapping).State = EntityState.Modified;
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
            LoadViewDataAfterOnEdit(dynamicrolemapping);
            return View(dynamicrolemapping);
        }
        // GET: /DynamicRoleMapping/Delete/5
        public ActionResult Delete(int id)
        {
            if (!User.CanDelete("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
            if (dynamicrolemapping == null)
            {
                throw (new Exception("Deleted"));
            }
            if (ViewData["DynamicRoleMappingParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/DynamicRoleMapping"))
                ViewData["DynamicRoleMappingParentUrl"] = Request.UrlReferrer;
            return View(dynamicrolemapping);
        }
        // POST: /DynamicRoleMapping/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DynamicRoleMapping dynamicrolemapping, string UrlReferrer)
        {
            if (!User.CanDelete("DynamicRoleMapping"))
            {
                return RedirectToAction("Index", "Error");
            }
            db.Entry(dynamicrolemapping).State = EntityState.Deleted;
            db.DynamicRoleMappings.Remove(dynamicrolemapping);
            db.SaveChanges();
            if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            if (ViewData["DynamicRoleMappingParentUrl"] != null)
            {
                string parentUrl = ViewData["DynamicRoleMappingParentUrl"].ToString();
                ViewData["DynamicRoleMappingParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }
        public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
        {
            foreach (var id in ids.Where(p => p > 0))
            {
                DynamicRoleMapping dynamicrolemapping = db.DynamicRoleMappings.Find(id);
                db.Entry(dynamicrolemapping).State = EntityState.Deleted;
                db.DynamicRoleMappings.Remove(dynamicrolemapping);
                db.SaveChanges();
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
