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
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
    public class JournalEntryController : BaseController
    {
        private JournalEntryContext db = new JournalEntryContext();
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsDeepSearch, bool? IsMobileRequest, bool? IsFilter, bool? RenderPartial)
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
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            var lstJournal = from s in db.JournalEntries
                             select s;
            if ((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "EntityName")
            {
                lstJournal = lstJournal.Where(p => p.EntityName == AssociatedType).OrderByDescending(p => p.Id);
            }
            if ((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "Type")
            {
                lstJournal = lstJournal.Where(p => p.Type == AssociatedType).OrderByDescending(p => p.Id);
            }
            if ((IsFilter == null ? false : IsFilter.Value) && HostingEntity == "UserName")
            {
                lstJournal = lstJournal.Where(p => p.UserName == AssociatedType).OrderByDescending(p => p.Id);
            }
            if (HostingEntity != null && HostingEntityID != null)
                lstJournal = lstJournal.Where(p => p.EntityName == HostingEntity && p.RecordId == HostingEntityID).OrderByDescending(p => p.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                lstJournal = searchRecords(lstJournal, searchString.ToUpper(), IsDeepSearch);
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstJournal = sortRecords(lstJournal, sortBy, isAsc);
            }
            else lstJournal = lstJournal.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            var _JournalEntry = lstJournal;
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_JournalEntry.Count() > 0)
                    pageSize = _JournalEntry.Count();
                return View("ExcelExport", _JournalEntry.ToPagedList(pageNumber, pageSize));
            }
            if (IsMobileRequest != true)
            {
                if (Request.AcceptTypes.Contains("text/html"))
                {
                    if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                        return View(_JournalEntry.ToPagedList(pageNumber, pageSize));
                    else
                        return PartialView("IndexPartial", _JournalEntry.ToPagedList(pageNumber, pageSize));
                }
                else if (Request.AcceptTypes.Contains("application/json"))
                {
                    var Result = getJournalEntryList(_JournalEntry);
                    return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var Result = getJournalEntryList(_JournalEntry);
                return Json(Result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_JournalEntry.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _JournalEntry.ToPagedList(pageNumber, pageSize));
        }
        private Object getJournalEntryList(IQueryable<JournalEntry> journalEntry)
        {
            var _journalEntry = from obj in journalEntry
                                select new
                                {
                                    DateTimeOfEntry = obj.DateTimeOfEntry,
                                    EntityName = obj.EntityName,
                                    NewValue = obj.NewValue,
                                    OldValue = obj.OldValue,
                                    PropertyName = obj.PropertyName,
                                    RecordId = obj.RecordId,
                                    RecordInfo = obj.RecordInfo,
                                    Type = obj.Type,
                                    UserName = obj.UserName,
                                    ID = obj.Id
                                };
            return journalEntry;
        }
        private Object getJournalItem(JournalEntry journalEntry)
        {
            return new
            {
                DateTimeOfEntry = journalEntry.DateTimeOfEntry,
                EntityName = journalEntry.EntityName,
                NewValue = journalEntry.NewValue,
                OldValue = journalEntry.OldValue,
                PropertyName = journalEntry.PropertyName,
                RecordId = journalEntry.RecordId,
                RecordInfo = journalEntry.RecordInfo,
                Type = journalEntry.Type,
                UserName = journalEntry.UserName,
                ID = journalEntry.Id
            };
        }
        private IQueryable<JournalEntry> searchRecords(IQueryable<JournalEntry> lstjournalEntry, string searchString, bool? IsDeepSearch)
        {
            if (Convert.ToBoolean(IsDeepSearch))
                lstjournalEntry = lstjournalEntry.Where(s => (s.RecordId != null && SqlFunctions.StringConvert((double)s.RecordId).Contains(searchString))
                    || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.NewValue) && s.NewValue.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.OldValue) && s.OldValue.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.RecordInfo) && s.RecordInfo.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.Type) && s.Type.ToUpper().Contains(searchString))
                    || (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
            else
                lstjournalEntry = lstjournalEntry.Where(s => (s.RecordId != null && SqlFunctions.StringConvert((double)s.RecordId).Contains(searchString))
                     || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.NewValue) && s.NewValue.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.OldValue) && s.OldValue.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.RecordInfo) && s.RecordInfo.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.Type) && s.Type.ToUpper().Contains(searchString))
                     || (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
            try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstjournalEntry = lstjournalEntry.Union(db.JournalEntries.Where(s => (s.DateTimeOfEntry == datevalue)));
            }
            catch { }
            return lstjournalEntry;
        }
        private IQueryable<JournalEntry> sortRecords(IQueryable<JournalEntry> lstJournalEntry, string sortBy, string isAsc)
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
            ParameterExpression paramExpression = Expression.Parameter(typeof(JournalEntry), "journalentry");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<JournalEntry>)lstJournalEntry.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstJournalEntry.ElementType, lambda.Body.Type },
                    lstJournalEntry.Expression,
                    lambda));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string HostingEntityName)
        {
            IQueryable<JournalEntry> list = db.JournalEntries;
            if(HostingEntityName == "EntityName")
            {
                var data = from x in list.Select(p=>p.EntityName).Distinct().ToList()
                           select new { Id = x, Name = x };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if(HostingEntityName == "Type")
            {
                var data = from x in list.Select(p => p.Type).Distinct().ToList()
                           select new { Id = x, Name = x };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if(HostingEntityName == "UserName")
            {
                var data = from x in list.Select(p => p.UserName).Distinct().ToList()
                           select new { Id = x, Name = x };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
