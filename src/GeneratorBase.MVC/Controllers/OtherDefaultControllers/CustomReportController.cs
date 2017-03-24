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
    public class CustomReportController : BaseController
    {
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsFilter, bool? RenderPartial, string BulkOperation, string parent, string Wfsearch, string caller, bool? BulkAssociate, string viewtype, string isMobileHome)
        {
            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            ViewData["HostingEntity"] = HostingEntity;
            ViewData["HostingEntityID"] = HostingEntityID;
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            var lstCustomReport = from s in db.CustomReports
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstCustomReport = searchRecords(lstCustomReport, searchString.ToUpper(), IsDeepSearch);
            }
            if (parent != null && parent == "Home")
            {
                ViewBag.Homeval = searchString;
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstCustomReport = sortRecords(lstCustomReport, sortBy, isAsc);
            }
            else lstCustomReport = lstCustomReport.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "CustomReport"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "CustomReport"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "CustomReport"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "CustomReport"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //
            var _CustomReport = lstCustomReport;

            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_CustomReport.Count() > 0)
                    pageSize = _CustomReport.Count();
                return View("ExcelExport", _CustomReport.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                if (pageNumber > 1)
                {
                    var totalListCount = _CustomReport.Count();
                    int quotient = totalListCount / pageSize;
                    var remainder = totalListCount % pageSize;
                    var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                    if (pageNumber > maxpagenumber)
                    {
                        pageNumber = 1;
                    }
                }
            }
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
            {
                return View(_CustomReport.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                if (!string.IsNullOrEmpty(isMobileHome))
                {
                    pageSize = _CustomReport.Count() == 0 ? 1 : _CustomReport.Count();
                }
                return PartialView("IndexPartial", _CustomReport.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Details(int? id, string HostingEntityName, string AssociatedType, string defaultview)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomReport customreport = db.CustomReports.Find(id);
            if (customreport == null)
            {
                return HttpNotFound();
            }
            return View("Details", customreport);

        }
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID, string AssociatedType, bool? IsDDAdd)
        {

            ViewBag.EntityNameDD = new SelectList(EnityDictionary(), "Key", "Value");
            if (!User.CanAdd("CustomReport"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            ViewData["CustomReportParentUrl"] = UrlReferrer;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,ReportName,EntityName,PeportType,Description,OtherProperty,ViewReport")] CustomReport customreport, string UrlReferrer, bool? IsDDAdd)
        {
            ViewBag.EntityNameDD = new SelectList(EnityDictionary(), "Key", "Value");
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.CustomReports.Add(customreport);
                db.SaveChanges();
                if (command == "Create & Continue")
                    return RedirectToAction("Edit", new { Id = customreport.Id, UrlReferrer = UrlReferrer });
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }

            if (IsDDAdd != null)
                ViewBag.IsDDAdd = Convert.ToBoolean(IsDDAdd);
            return View(customreport);
        }
        public ActionResult Edit(int? id, string UrlReferrer, string HostingEntityName, string AssociatedType)
        {
            if (!User.CanEdit("CustomReport"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomReport customreport = db.CustomReports.Find(id);
            ViewBag.EntityNameDD = new SelectList(EnityDictionary(), "Key", "Value", customreport.EntityName);
            if (customreport == null)
            {
                return HttpNotFound();
            }
            if (UrlReferrer != null)
                ViewData["CustomReportParentUrl"] = UrlReferrer;
            if (ViewData["CustomReportParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/CustomReport") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CustomReport/Edit/" + customreport.Id + "") && !Request.UrlReferrer.AbsolutePath.EndsWith("/CustomReport/Create"))
                ViewData["CustomReportParentUrl"] = Request.UrlReferrer;
            return View(customreport);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,ReportName,EntityName,PeportType,Description,OtherProperty,ViewReport")] CustomReport customreport, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                string command = Request.Form["hdncommand"];
                db.Entry(customreport).State = EntityState.Modified;
                db.SaveChanges();
                if (command != "Save")
                    return RedirectToAction("Edit", new { Id = customreport.Id, UrlReferrer = UrlReferrer });
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
            return View(customreport);
        }
        public ActionResult DeleteBulk(long[] ids, string UrlReferrer)
        {
            foreach (var id in ids.Where(p => p > 0))
            {
                CustomReport customreport = db.CustomReports.Find(id);
                db.Entry(customreport).State = EntityState.Deleted;
                db.CustomReports.Remove(customreport);
                try
                {
                    db.SaveChanges();
                }
                catch { }
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
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
        // Sorting,Searching
        private IQueryable<CustomReport> searchRecords(IQueryable<CustomReport> lstCustomReport, string searchString, bool? IsDeepSearch)
        {
            searchString = searchString.Trim();
            if (Convert.ToBoolean(IsDeepSearch))
                lstCustomReport = lstCustomReport.Where(s => (!String.IsNullOrEmpty(s.ReportName) && s.ReportName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PeportType) && s.PeportType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.OtherProperty) && s.OtherProperty.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ViewReport) && s.ViewReport.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            else
                lstCustomReport = lstCustomReport.Where(s => (!String.IsNullOrEmpty(s.ReportName) && s.ReportName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PeportType) && s.PeportType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.OtherProperty) && s.OtherProperty.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.ViewReport) && s.ViewReport.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstCustomReport;
        }
        private IQueryable<CustomReport> sortRecords(IQueryable<CustomReport> lstCustomReport, string sortBy, string isAsc)
        {
            string methodName = "";
            switch (isAsc.ToLower())
            {
                case "asc":
                    methodName = "OrderBy";
                    break;
                case "desc":
                    methodName = "OrderByDescending";
                    break;
            }

            ParameterExpression paramExpression = Expression.Parameter(typeof(CustomReport), "customreport");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<CustomReport>)lstCustomReport.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstCustomReport.ElementType, lambda.Body.Type },
                    lstCustomReport.Expression,
                    lambda));
        }
        //code for verb action security
        public string[] getVerbsName()
        {
            string[] Verbsarr = new string[] { "BulkDelete" };
            return Verbsarr;
        }
        //Entity List
        public Dictionary<string, string> EnityDictionary()
        {
            Dictionary<string, string> entList = new Dictionary<string, string>();
            var entitys = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name != "MultiTenantExtraAccess" && p.Name != "FileDocument" && p.Name != "MultiTenantLoginSelected" && p.Name != "ApiToken");

            foreach (var item in entitys)
            {
                var lstitem = item.Associations;
                var getDateType = item.Properties;
                int nonstringcount = 0;
                foreach (var proItem in getDateType)
                    if (proItem.DataType.ToUpper() != "STRING")
                        nonstringcount = nonstringcount + 1;

                entList.Add(item.Name, item.DisplayName);
            }
            return entList;
        }
        //
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
