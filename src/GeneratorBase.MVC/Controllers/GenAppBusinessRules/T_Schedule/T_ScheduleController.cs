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
using RecurrenceGenerator;
using System.Linq.Dynamic;
namespace GeneratorBase.MVC.Controllers
{
    public partial class T_ScheduleController : BaseController
    { //mahesh
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            if (!string.IsNullOrEmpty(propNameBR))
            {
                var testresult = list.Select("new(Id," + propNameBR + " as value)");
                return Json(testresult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        private void LoadViewDataForCount(T_Schedule t_schedule, string AssocType)
        {
        }
        private void LoadViewDataAfterOnEdit(T_Schedule t_schedule)
        {
            ViewBag.T_AssociatedScheduleTypeID = new SelectList(db.T_Scheduletypes, "ID", "DisplayValue", t_schedule.T_AssociatedScheduleTypeID);
            ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(db.T_RecurringScheduleDetailstypes, "ID", "DisplayValue", t_schedule.T_AssociatedRecurringScheduleDetailsTypeID);
            ViewBag.T_RecurringRepeatFrequencyID = new SelectList(db.T_RecurringFrequencys, "ID", "DisplayValue", t_schedule.T_RecurringRepeatFrequencyID);
            ViewBag.T_RepeatByID = new SelectList(db.T_MonthlyRepeatTypes, "ID", "DisplayValue", t_schedule.T_RepeatByID);
            ViewBag.T_RecurringTaskEndTypeID = new SelectList(db.T_RecurringEndTypes, "ID", "DisplayValue", t_schedule.T_RecurringTaskEndTypeID);
        }
        private void LoadViewDataBeforeOnEdit(T_Schedule t_schedule)
        {
            var T_RecurrenceDays_T_RepeatOnlist = db.T_RecurrenceDayss.OrderBy(p => p.DisplayValue).Take(10).Distinct();
            if (t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                var list = T_RecurrenceDays_T_RepeatOnlist.Union(db.T_RecurrenceDayss.Where(p => t_schedule.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(p.Id))).Distinct();
                ViewBag.SelectedT_RecurrenceDays_T_RepeatOn = new MultiSelectList(list, "ID", "DisplayValue", t_schedule.SelectedT_RecurrenceDays_T_RepeatOn);
            }
            else
            {
                ViewBag.SelectedT_RecurrenceDays_T_RepeatOn = new MultiSelectList(T_RecurrenceDays_T_RepeatOnlist, "ID", "DisplayValue");
            }

            var _objT_AssociatedScheduleType = new List<T_Scheduletype>();
            _objT_AssociatedScheduleType.Add(t_schedule.t_associatedscheduletype);
            if (t_schedule.t_associatedscheduletype != null)
                ViewBag.T_AssociatedScheduleTypeID = new SelectList(db.T_Scheduletypes.ToList(), "ID", "DisplayValue", t_schedule.T_AssociatedScheduleTypeID);
            else
                ViewBag.T_AssociatedScheduleTypeID = new SelectList(db.T_Scheduletypes.ToList(), "ID", "DisplayValue");
            var _objT_AssociatedRecurringScheduleDetailsType = new List<T_RecurringScheduleDetailstype>();
            _objT_AssociatedRecurringScheduleDetailsType.Add(t_schedule.t_associatedrecurringscheduledetailstype);
            if (t_schedule.t_associatedrecurringscheduledetailstype != null)
                ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(db.T_RecurringScheduleDetailstypes.ToList(), "ID", "DisplayValue", t_schedule.T_AssociatedRecurringScheduleDetailsTypeID);
            else
                ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(db.T_RecurringScheduleDetailstypes.ToList(), "ID", "DisplayValue");
            var _objT_RecurringRepeatFrequency = new List<T_RecurringFrequency>();
            _objT_RecurringRepeatFrequency.Add(t_schedule.t_recurringrepeatfrequency);
            ViewBag.T_RecurringRepeatFrequencyID = new SelectList(_objT_RecurringRepeatFrequency, "ID", "DisplayValue", t_schedule.T_RecurringRepeatFrequencyID);
            var _objT_RepeatBy = new List<T_MonthlyRepeatType>();
            _objT_RepeatBy.Add(t_schedule.t_repeatby);
            if (t_schedule.t_repeatby != null)
                ViewBag.T_RepeatByID = new SelectList(db.T_MonthlyRepeatTypes.ToList(), "ID", "DisplayValue", t_schedule.T_RepeatByID);
            else
                ViewBag.T_RepeatByID = new SelectList(db.T_MonthlyRepeatTypes.ToList(), "ID", "DisplayValue");
            var _objT_RecurringTaskEndType = new List<T_RecurringEndType>();
            _objT_RecurringTaskEndType.Add(t_schedule.t_recurringtaskendtype);
            if (t_schedule.t_recurringtaskendtype != null)
                ViewBag.T_RecurringTaskEndTypeID = new SelectList(db.T_RecurringEndTypes.ToList(), "ID", "DisplayValue", t_schedule.T_RecurringTaskEndTypeID);
            else
                ViewBag.T_RecurringTaskEndTypeID = new SelectList(db.T_RecurringEndTypes.ToList(), "ID", "DisplayValue");
        }
        private void LoadViewDataAfterOnCreate(T_Schedule t_schedule)
        {
            ViewBag.T_AssociatedScheduleTypeID = new SelectList(db.T_Scheduletypes, "ID", "DisplayValue", t_schedule.T_AssociatedScheduleTypeID);
            ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(db.T_RecurringScheduleDetailstypes, "ID", "DisplayValue", t_schedule.T_AssociatedRecurringScheduleDetailsTypeID);
            ViewBag.T_RecurringRepeatFrequencyID = new SelectList(db.T_RecurringFrequencys, "ID", "DisplayValue", t_schedule.T_RecurringRepeatFrequencyID);
            ViewBag.T_RepeatByID = new SelectList(db.T_MonthlyRepeatTypes, "ID", "DisplayValue", t_schedule.T_RepeatByID);
            ViewBag.T_RecurringTaskEndTypeID = new SelectList(db.T_RecurringEndTypes, "ID", "DisplayValue", t_schedule.T_RecurringTaskEndTypeID);
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
            var T_RecurrenceDays_T_RepeatOnlist = db.T_RecurrenceDayss.OrderBy(p => p.DisplayValue).Take(10).Distinct();
            ViewBag.SelectedT_RecurrenceDays_T_RepeatOn = new MultiSelectList(T_RecurrenceDays_T_RepeatOnlist, "ID", "DisplayValue");
            if (HostingEntityName == "T_Scheduletype" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "T_AssociatedScheduleType")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.T_AssociatedScheduleTypeID = new SelectList(db.T_Scheduletypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
                ViewBag.DisplayVal = db.T_Scheduletypes.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
            }
            else
            {
                var objT_AssociatedScheduleType = db.T_Scheduletypes.ToList();
                ViewBag.T_AssociatedScheduleTypeID = new SelectList(objT_AssociatedScheduleType, "ID", "DisplayValue");
            }
            if (HostingEntityName == "T_RecurringScheduleDetailstype" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "T_AssociatedRecurringScheduleDetailsType")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(db.T_RecurringScheduleDetailstypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
                ViewBag.DisplayVal = db.T_RecurringScheduleDetailstypes.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
            }
            else
            {
                var objT_AssociatedRecurringScheduleDetailsType = db.T_RecurringScheduleDetailstypes.ToList();
                ViewBag.T_AssociatedRecurringScheduleDetailsTypeID = new SelectList(objT_AssociatedRecurringScheduleDetailsType, "ID", "DisplayValue");
            }
            if (HostingEntityName == "T_RecurringFrequency" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "T_RecurringRepeatFrequency")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.T_RecurringRepeatFrequencyID = new SelectList(db.T_RecurringFrequencys.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
                ViewBag.DisplayVal = db.T_RecurringFrequencys.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
            }
            else
            {
                var objT_RecurringRepeatFrequency = new List<T_RecurringFrequency>();
                ViewBag.T_RecurringRepeatFrequencyID = new SelectList(objT_RecurringRepeatFrequency, "ID", "DisplayValue");
            }
            if (HostingEntityName == "T_MonthlyRepeatType" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "T_RepeatBy")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.T_RepeatByID = new SelectList(db.T_MonthlyRepeatTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
                ViewBag.DisplayVal = db.T_MonthlyRepeatTypes.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
            }
            else
            {
                var objT_RepeatBy = db.T_MonthlyRepeatTypes.ToList();
                ViewBag.T_RepeatByID = new SelectList(objT_RepeatBy, "ID", "DisplayValue");
            }
            if (HostingEntityName == "T_RecurringEndType" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType == "T_RecurringTaskEndType")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.T_RecurringTaskEndTypeID = new SelectList(db.T_RecurringEndTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
                ViewBag.DisplayVal = db.T_RecurringEndTypes.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
            }
            else
            {
                var objT_RecurringTaskEndType = db.T_RecurringEndTypes.ToList();
                ViewBag.T_RecurringTaskEndTypeID = new SelectList(objT_RecurringTaskEndType, "ID", "DisplayValue");
            }
        }
        private IQueryable<T_Schedule> searchRecords(IQueryable<T_Schedule> lstT_Schedule, string searchString, bool? IsDeepSearch)
        {
            searchString = searchString.Trim();
            if (Convert.ToBoolean(IsDeepSearch))
                lstT_Schedule = lstT_Schedule.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (s.t_associatedscheduletype != null && (s.t_associatedscheduletype.DisplayValue.ToUpper().Contains(searchString))) || (s.t_associatedrecurringscheduledetailstype != null && (s.t_associatedrecurringscheduledetailstype.DisplayValue.ToUpper().Contains(searchString))) || (s.t_recurringrepeatfrequency != null && (s.t_recurringrepeatfrequency.DisplayValue.ToUpper().Contains(searchString))) || (s.t_repeatby != null && (s.t_repeatby.DisplayValue.ToUpper().Contains(searchString))) || (s.t_recurringtaskendtype != null && (s.t_recurringtaskendtype.DisplayValue.ToUpper().Contains(searchString))) || (s.T_OccurrenceLimitCount != null && SqlFunctions.StringConvert((double)s.T_OccurrenceLimitCount).Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Summary) && s.T_Summary.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            else
                lstT_Schedule = lstT_Schedule.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) || (s.t_associatedscheduletype != null && (s.t_associatedscheduletype.DisplayValue.ToUpper().Contains(searchString))) || (s.t_associatedrecurringscheduledetailstype != null && (s.t_associatedrecurringscheduledetailstype.DisplayValue.ToUpper().Contains(searchString))) || (s.t_recurringrepeatfrequency != null && (s.t_recurringrepeatfrequency.DisplayValue.ToUpper().Contains(searchString))) || (s.t_repeatby != null && (s.t_repeatby.DisplayValue.ToUpper().Contains(searchString))) || (s.t_recurringtaskendtype != null && (s.t_recurringtaskendtype.DisplayValue.ToUpper().Contains(searchString))) || (s.T_OccurrenceLimitCount != null && SqlFunctions.StringConvert((double)s.T_OccurrenceLimitCount).Contains(searchString)) || (!String.IsNullOrEmpty(s.T_Summary) && s.T_Summary.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstT_Schedule = lstT_Schedule.Union(db.T_Schedules.Where(s => (s.T_StartDateTime == datevalue) || (s.T_EndDate == datevalue)));
            }
            catch { }
            return lstT_Schedule;
        }
        private IQueryable<T_Schedule> sortRecords(IQueryable<T_Schedule> lstT_Schedule, string sortBy, string isAsc)
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

            if (sortBy == "T_AssociatedScheduleTypeID")
                return isAsc.ToLower() == "asc" ? lstT_Schedule.OrderBy(p => p.t_associatedscheduletype.DisplayValue) : lstT_Schedule.OrderByDescending(p => p.t_associatedscheduletype.DisplayValue);
            if (sortBy == "T_AssociatedRecurringScheduleDetailsTypeID")
                return isAsc.ToLower() == "asc" ? lstT_Schedule.OrderBy(p => p.t_associatedrecurringscheduledetailstype.DisplayValue) : lstT_Schedule.OrderByDescending(p => p.t_associatedrecurringscheduledetailstype.DisplayValue);
            if (sortBy == "T_RecurringRepeatFrequencyID")
                return isAsc.ToLower() == "asc" ? lstT_Schedule.OrderBy(p => p.t_recurringrepeatfrequency.DisplayValue) : lstT_Schedule.OrderByDescending(p => p.t_recurringrepeatfrequency.DisplayValue);
            if (sortBy == "T_RepeatByID")
                return isAsc.ToLower() == "asc" ? lstT_Schedule.OrderBy(p => p.t_repeatby.DisplayValue) : lstT_Schedule.OrderByDescending(p => p.t_repeatby.DisplayValue);
            if (sortBy == "T_RecurringTaskEndTypeID")
                return isAsc.ToLower() == "asc" ? lstT_Schedule.OrderBy(p => p.t_recurringtaskendtype.DisplayValue) : lstT_Schedule.OrderByDescending(p => p.t_recurringtaskendtype.DisplayValue);
            ParameterExpression paramExpression = Expression.Parameter(typeof(T_Schedule), "t_schedule");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<T_Schedule>)lstT_Schedule.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstT_Schedule.ElementType, lambda.Body.Type },
                    lstT_Schedule.Expression,
                    lambda));
        }
        public ActionResult FindFSearch(string id, string sourceEntity)
        {
            return null;
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity, string t_associatedscheduletype, string t_associatedrecurringscheduledetailstype, string t_recurringrepeatfrequency, string t_repeatby, string t_recurringtaskendtype)
        {
            int Qcount = Request.UrlReferrer.Query.Count();
            ViewBag.CurrentFilter = searchString;
            if (Qcount > 0)
            {
                var T_AssociatedScheduleTypelist = db.T_Scheduletypes.OrderBy(p => p.DisplayValue).Take(10).Distinct();
                if (t_associatedscheduletype != null)
                {
                    var ids = t_associatedscheduletype.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    ViewBag.SearchResult += "\r\n Schedule Type= ";

                    foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                                ViewBag.SearchResult += "";
                            }
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                ViewBag.SearchResult += db.T_Scheduletypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                            }
                        }
                    }

                    ids1 = ids1.ToList();
                    var list = T_AssociatedScheduleTypelist.Union(db.T_Scheduletypes.Where(p => ids1.Contains(p.Id))).Distinct();
                    ViewBag.t_associatedscheduletype = new SelectList(list, "ID", "DisplayValue");
                }
                else
                {
                    ViewBag.t_associatedscheduletype = new SelectList(T_AssociatedScheduleTypelist, "ID", "DisplayValue");
                }
                var T_AssociatedRecurringScheduleDetailsTypelist = db.T_RecurringScheduleDetailstypes.OrderBy(p => p.DisplayValue).Take(10).Distinct();
                if (t_associatedrecurringscheduledetailstype != null)
                {
                    var ids = t_associatedrecurringscheduledetailstype.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    ViewBag.SearchResult += "\r\n Repeat Type= ";

                    foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                                ViewBag.SearchResult += "";
                            }
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                ViewBag.SearchResult += db.T_RecurringScheduleDetailstypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                            }
                        }
                    }

                    ids1 = ids1.ToList();
                    var list = T_AssociatedRecurringScheduleDetailsTypelist.Union(db.T_RecurringScheduleDetailstypes.Where(p => ids1.Contains(p.Id))).Distinct();
                    ViewBag.t_associatedrecurringscheduledetailstype = new SelectList(list, "ID", "DisplayValue");
                }
                else
                {
                    ViewBag.t_associatedrecurringscheduledetailstype = new SelectList(T_AssociatedRecurringScheduleDetailsTypelist, "ID", "DisplayValue");
                }
                var T_RecurringRepeatFrequencylist = db.T_RecurringFrequencys.OrderBy(p => p.DisplayValue).Take(10).Distinct();
                if (t_recurringrepeatfrequency != null)
                {
                    var ids = t_recurringrepeatfrequency.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    ViewBag.SearchResult += "\r\n Repeat Every= ";

                    foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                                ViewBag.SearchResult += "";
                            }
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                ViewBag.SearchResult += db.T_RecurringFrequencys.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                            }
                        }
                    }

                    ids1 = ids1.ToList();
                    var list = T_RecurringRepeatFrequencylist.Union(db.T_RecurringFrequencys.Where(p => ids1.Contains(p.Id))).Distinct();
                    ViewBag.t_recurringrepeatfrequency = new SelectList(list, "ID", "DisplayValue");
                }
                else
                {
                    ViewBag.t_recurringrepeatfrequency = new SelectList(T_RecurringRepeatFrequencylist, "ID", "DisplayValue");
                }
                var T_RepeatBylist = db.T_MonthlyRepeatTypes.OrderBy(p => p.DisplayValue).Take(10).Distinct();
                if (t_repeatby != null)
                {
                    var ids = t_repeatby.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    ViewBag.SearchResult += "\r\n Repeat By= ";

                    foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                                ViewBag.SearchResult += "";
                            }
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                ViewBag.SearchResult += db.T_MonthlyRepeatTypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                            }
                        }
                    }

                    ids1 = ids1.ToList();
                    var list = T_RepeatBylist.Union(db.T_MonthlyRepeatTypes.Where(p => ids1.Contains(p.Id))).Distinct();
                    ViewBag.t_repeatby = new SelectList(list, "ID", "DisplayValue");
                }
                else
                {
                    ViewBag.t_repeatby = new SelectList(T_RepeatBylist, "ID", "DisplayValue");
                }
                var T_RecurringTaskEndTypelist = db.T_RecurringEndTypes.OrderBy(p => p.DisplayValue).Take(10).Distinct();
                if (t_recurringtaskendtype != null)
                {
                    var ids = t_recurringtaskendtype.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    ViewBag.SearchResult += "\r\n Ends= ";

                    foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                                ViewBag.SearchResult += "";
                            }
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                ViewBag.SearchResult += db.T_RecurringEndTypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                            }
                        }
                    }

                    ids1 = ids1.ToList();
                    var list = T_RecurringTaskEndTypelist.Union(db.T_RecurringEndTypes.Where(p => ids1.Contains(p.Id))).Distinct();
                    ViewBag.t_recurringtaskendtype = new SelectList(list, "ID", "DisplayValue");
                }
                else
                {
                    ViewBag.t_recurringtaskendtype = new SelectList(T_RecurringTaskEndTypelist, "ID", "DisplayValue");
                }
            }
            else
            {
                var objT_AssociatedScheduleType = new List<T_Scheduletype>();
                ViewBag.t_associatedscheduletype = new SelectList(objT_AssociatedScheduleType, "ID", "DisplayValue");
                var objT_AssociatedRecurringScheduleDetailsType = new List<T_RecurringScheduleDetailstype>();
                ViewBag.t_associatedrecurringscheduledetailstype = new SelectList(objT_AssociatedRecurringScheduleDetailsType, "ID", "DisplayValue");
                var objT_RecurringRepeatFrequency = new List<T_RecurringFrequency>();
                ViewBag.t_recurringrepeatfrequency = new SelectList(objT_RecurringRepeatFrequency, "ID", "DisplayValue");
                var objT_RepeatBy = new List<T_MonthlyRepeatType>();
                ViewBag.t_repeatby = new SelectList(objT_RepeatBy, "ID", "DisplayValue");
                var objT_RecurringTaskEndType = new List<T_RecurringEndType>();
                ViewBag.t_recurringtaskendtype = new SelectList(objT_RecurringTaskEndType, "ID", "DisplayValue");
            }
            var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
            if (lstFavoriteItem.Count() > 0)
                ViewBag.FavoriteItem = lstFavoriteItem;
            return View();
        }
        // GET: /T_Schedule/FSearch/
        public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string t_associatedscheduletype, string t_associatedrecurringscheduledetailstype, string t_recurringrepeatfrequency, string t_repeatby, string t_recurringtaskendtype, string T_StartDateTimeFrom, string T_StartDateTimeTo, string T_EndDateFrom, string T_EndDateTo, string T_OccurrenceLimitCountFrom, string T_OccurrenceLimitCountTo, string HostingEntity, string AssociatedType, string HostingEntityID, string viewtype)//, string HostingEntity
        {
            ViewData["HostingEntity"] = HostingEntity;
            ViewData["AssociatedType"] = AssociatedType;
            ViewData["HostingEntityID"] = HostingEntityID;
            ViewBag.SearchResult = "";
            if (!string.IsNullOrEmpty(viewtype))
                ViewBag.TemplatesName = viewtype.Replace("?IsAddPop=true", "");

            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            if (!string.IsNullOrEmpty(searchString) && FSFilter == null)
                ViewBag.FSFilter = "Fsearch";
            var lstT_Schedule = from s in db.T_Schedules
                                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_Schedule = searchRecords(lstT_Schedule, searchString.ToUpper(), true);
            }
            if (!string.IsNullOrEmpty(search))
                search = search.Replace("?IsAddPop=true", "");
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.SearchResult += "\r\n General Criterial= " + search;
                lstT_Schedule = searchRecords(lstT_Schedule, search, true);
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_Schedule = sortRecords(lstT_Schedule, sortBy, isAsc);
            }
            else lstT_Schedule = lstT_Schedule.OrderByDescending(c => c.Id);
            lstT_Schedule = lstT_Schedule.Include(t => t.t_associatedscheduletype).Include(t => t.t_associatedrecurringscheduledetailstype).Include(t => t.t_recurringrepeatfrequency).Include(t => t.t_repeatby).Include(t => t.t_recurringtaskendtype);
            var _T_Schedule = lstT_Schedule;
            if (T_StartDateTimeFrom != null || T_StartDateTimeTo != null)
            {
                try
                {
                    DateTime from = T_StartDateTimeFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(T_StartDateTimeFrom);
                    DateTime to = T_StartDateTimeTo == null ? DateTime.MaxValue : Convert.ToDateTime(T_StartDateTimeTo);
                    _T_Schedule = _T_Schedule.Where(o => o.T_StartDateTime >= from && o.T_StartDateTime <= to);
                    ViewBag.SearchResult += "\r\n Start Date Time= " + from.ToShortDateString() + "-" + to.ToShortDateString();
                }
                catch { }
            }
            if (T_EndDateFrom != null || T_EndDateTo != null)
            {
                try
                {
                    DateTime from = T_EndDateFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(T_EndDateFrom);
                    DateTime to = T_EndDateTo == null ? DateTime.MaxValue : Convert.ToDateTime(T_EndDateTo);
                    _T_Schedule = _T_Schedule.Where(o => o.T_EndDate >= from && o.T_EndDate <= to);
                    ViewBag.SearchResult += "\r\n End Date= " + from.ToShortDateString() + "-" + to.ToShortDateString();
                }
                catch { }
            }
            if (T_OccurrenceLimitCountFrom != null || T_OccurrenceLimitCountTo != null)
            {
                try
                {
                    int from = T_OccurrenceLimitCountFrom == null ? 0 : Convert.ToInt32(T_OccurrenceLimitCountFrom);
                    int to = T_OccurrenceLimitCountTo == null ? int.MaxValue : Convert.ToInt32(T_OccurrenceLimitCountTo);
                    _T_Schedule = _T_Schedule.Where(o => o.T_OccurrenceLimitCount >= from && o.T_OccurrenceLimitCount <= to);
                    ViewBag.SearchResult += "\r\n Occurrence Limit Count= " + from + "-" + to;
                }
                catch { }
            }
            //if (lstT_Schedule.Where(p => p.t_associatedscheduletype != null).Count() <= 50)
            //ViewBag.t_associatedscheduletype = new SelectList(lstT_Schedule.Where(p => p.t_associatedscheduletype != null).Select(P => P.t_associatedscheduletype).Distinct(), "ID", "DisplayValue");
            if (t_associatedscheduletype != null)
            {
                var ids = t_associatedscheduletype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Schedule Type= ";

                foreach (var str in ids)
                {
                    //Null Search
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            var obj = db.T_Scheduletypes.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_Schedule = _T_Schedule.Where(p => ids1.Contains(p.T_AssociatedScheduleTypeID));
            }
            //if (lstT_Schedule.Where(p => p.t_associatedrecurringscheduledetailstype != null).Count() <= 50)
            //ViewBag.t_associatedrecurringscheduledetailstype = new SelectList(lstT_Schedule.Where(p => p.t_associatedrecurringscheduledetailstype != null).Select(P => P.t_associatedrecurringscheduledetailstype).Distinct(), "ID", "DisplayValue");
            if (t_associatedrecurringscheduledetailstype != null)
            {
                var ids = t_associatedrecurringscheduledetailstype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Repeat Type= ";

                foreach (var str in ids)
                {
                    //Null Search
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            var obj = db.T_RecurringScheduleDetailstypes.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_Schedule = _T_Schedule.Where(p => ids1.Contains(p.T_AssociatedRecurringScheduleDetailsTypeID));
            }
            //if (lstT_Schedule.Where(p => p.t_recurringrepeatfrequency != null).Count() <= 50)
            //ViewBag.t_recurringrepeatfrequency = new SelectList(lstT_Schedule.Where(p => p.t_recurringrepeatfrequency != null).Select(P => P.t_recurringrepeatfrequency).Distinct(), "ID", "DisplayValue");
            if (t_recurringrepeatfrequency != null)
            {
                var ids = t_recurringrepeatfrequency.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Repeat Every= ";

                foreach (var str in ids)
                {
                    //Null Search
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            var obj = db.T_RecurringFrequencys.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_Schedule = _T_Schedule.Where(p => ids1.Contains(p.T_RecurringRepeatFrequencyID));
            }
            //if (lstT_Schedule.Where(p => p.t_repeatby != null).Count() <= 50)
            //ViewBag.t_repeatby = new SelectList(lstT_Schedule.Where(p => p.t_repeatby != null).Select(P => P.t_repeatby).Distinct(), "ID", "DisplayValue");
            if (t_repeatby != null)
            {
                var ids = t_repeatby.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Repeat By= ";

                foreach (var str in ids)
                {
                    //Null Search
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            var obj = db.T_MonthlyRepeatTypes.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_Schedule = _T_Schedule.Where(p => ids1.Contains(p.T_RepeatByID));
            }
            //if (lstT_Schedule.Where(p => p.t_recurringtaskendtype != null).Count() <= 50)
            //ViewBag.t_recurringtaskendtype = new SelectList(lstT_Schedule.Where(p => p.t_recurringtaskendtype != null).Select(P => P.t_recurringtaskendtype).Distinct(), "ID", "DisplayValue");
            if (t_recurringtaskendtype != null)
            {
                var ids = t_recurringtaskendtype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Ends= ";

                foreach (var str in ids)
                {
                    //Null Search
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (str == "NULL")
                        {
                            ids1.Add(null);
                            ViewBag.SearchResult += "";
                        }
                        else
                        {
                            ids1.Add(Convert.ToInt64(str));
                            var obj = db.T_RecurringEndTypes.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_Schedule = _T_Schedule.Where(p => ids1.Contains(p.T_RecurringTaskEndTypeID));
            }
            ViewBag.SearchResult = ((string)ViewBag.SearchResult).TrimStart("\r\n".ToCharArray());
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_Schedule"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_Schedule.Count() > 0)
                    pageSize = _T_Schedule.Count();
                return View("ExcelExport", _T_Schedule.ToPagedList(pageNumber, pageSize));
            }
            var PagedListT_Schedule = _T_Schedule.ToList();

            PagedListT_Schedule.ForEach(p => p.T_RecurrenceDays_T_RepeatOn = db.T_RecurrenceDayss.OrderBy(x => x.DisplayValue).ToList());
            PagedListT_Schedule.ForEach(p => p.SelectedT_RecurrenceDays_T_RepeatOn = db.T_RepeatOns.Where(a => a.T_ScheduleID == p.Id).Select(m => m.T_RecurrenceDaysID).ToList());
            //return View("Index", PagedListT_Schedule.ToPagedList(pageNumber, pageSize));
            if (!Request.IsAjaxRequest())
            {
                GetTemplatesForList();
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
                return View("Index", PagedListT_Schedule.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                if (ViewBag.TemplatesName == null)
                    return PartialView("IndexPartial", PagedListT_Schedule.ToPagedList(pageNumber, pageSize));
                else
                    return PartialView(ViewBag.TemplatesName, PagedListT_Schedule.ToPagedList(pageNumber, pageSize));
            }
        }
        public string ShowGraph()
        {
            string result = "";
            {
                var gd = db.T_Schedules.GroupBy(p => p.t_associatedscheduletype.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Schedule Type")
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
                var _yvalT_OccurrenceLimitCount = gd.Select(k => k.Sum(p => p.T_OccurrenceLimitCount)).ToArray();
                var bytesT_OccurrenceLimitCount = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                    .AddSeries(
                    chartType: "Column",
                     xValue: _xval,
                     yValues: _yvalT_OccurrenceLimitCount).AddTitle("Total of Occurrence Limit Count by Schedule Type")
                    .GetBytes("png");
                string imgT_OccurrenceLimitCount = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_OccurrenceLimitCount = Convert.ToBase64String(bytesT_OccurrenceLimitCount.ToArray());
                result += String.Format(imgT_OccurrenceLimitCount, encodedT_OccurrenceLimitCount);
            }
            {
                var gd = db.T_Schedules.GroupBy(p => p.t_associatedrecurringscheduledetailstype.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.Pie)
                .AddSeries(
                chartType: "Pie",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Repeat Type").AddLegend()
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
                var _yvalT_OccurrenceLimitCount = gd.Select(k => k.Sum(p => p.T_OccurrenceLimitCount)).ToArray();
                var bytesT_OccurrenceLimitCount = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.Pie)
                    .AddSeries(
                    chartType: "Column",
                     xValue: _xval,
                     yValues: _yvalT_OccurrenceLimitCount).AddTitle("Total of Occurrence Limit Count by Repeat Type").AddLegend()
                    .GetBytes("png");
                string imgT_OccurrenceLimitCount = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_OccurrenceLimitCount = Convert.ToBase64String(bytesT_OccurrenceLimitCount.ToArray());
                result += String.Format(imgT_OccurrenceLimitCount, encodedT_OccurrenceLimitCount);
            }
            {
                var gd = db.T_Schedules.GroupBy(p => p.t_recurringrepeatfrequency.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Repeat Every")
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
                var _yvalT_OccurrenceLimitCount = gd.Select(k => k.Sum(p => p.T_OccurrenceLimitCount)).ToArray();
                var bytesT_OccurrenceLimitCount = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                    .AddSeries(
                    chartType: "Column",
                     xValue: _xval,
                     yValues: _yvalT_OccurrenceLimitCount).AddTitle("Total of Occurrence Limit Count by Repeat Every")
                    .GetBytes("png");
                string imgT_OccurrenceLimitCount = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_OccurrenceLimitCount = Convert.ToBase64String(bytesT_OccurrenceLimitCount.ToArray());
                result += String.Format(imgT_OccurrenceLimitCount, encodedT_OccurrenceLimitCount);
            }
            {
                var gd = db.T_Schedules.GroupBy(p => p.t_repeatby.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.Pie)
                .AddSeries(
                chartType: "Pie",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Repeat By").AddLegend()
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
                var _yvalT_OccurrenceLimitCount = gd.Select(k => k.Sum(p => p.T_OccurrenceLimitCount)).ToArray();
                var bytesT_OccurrenceLimitCount = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.Pie)
                    .AddSeries(
                    chartType: "Column",
                     xValue: _xval,
                     yValues: _yvalT_OccurrenceLimitCount).AddTitle("Total of Occurrence Limit Count by Repeat By").AddLegend()
                    .GetBytes("png");
                string imgT_OccurrenceLimitCount = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_OccurrenceLimitCount = Convert.ToBase64String(bytesT_OccurrenceLimitCount.ToArray());
                result += String.Format(imgT_OccurrenceLimitCount, encodedT_OccurrenceLimitCount);
            }
            {
                var gd = db.T_Schedules.GroupBy(p => p.t_recurringtaskendtype.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Ends")
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
                var _yvalT_OccurrenceLimitCount = gd.Select(k => k.Sum(p => p.T_OccurrenceLimitCount)).ToArray();
                var bytesT_OccurrenceLimitCount = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                    .AddSeries(
                    chartType: "Column",
                     xValue: _xval,
                     yValues: _yvalT_OccurrenceLimitCount).AddTitle("Total of Occurrence Limit Count by Ends")
                    .GetBytes("png");
                string imgT_OccurrenceLimitCount = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_OccurrenceLimitCount = Convert.ToBase64String(bytesT_OccurrenceLimitCount.ToArray());
                result += String.Format(imgT_OccurrenceLimitCount, encodedT_OccurrenceLimitCount);
            }
            return result;
        }
        public string GetDisplayValue(string id)
        {
            if (string.IsNullOrEmpty(id)) return "";
            long idvalue = Convert.ToInt64(id);
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var _Obj = db1.T_Schedules.FirstOrDefault(p => p.Id == idvalue);
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
        public object GetRecordById(string id)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            if (string.IsNullOrEmpty(id)) return "";
            var _Obj = db1.T_Schedules.Find(Convert.ToInt64(id));
            return _Obj;
        }
        public string GetRecordById_Reflection(string id)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            if (string.IsNullOrEmpty(id)) return "";
            var _Obj = db1.T_Schedules.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_Schedule>(_Obj, "T_Schedule", new string[] { "" }); ;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_Schedules;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_Schedule), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_Schedule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_Schedule>)q);
                }
                else if (AssoID == 0)
                {
                    IQueryable query = db.T_Schedules;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_Schedule), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_Schedule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_Schedule>)q);

                }
            }
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_Schedule>(User, list, "T_Schedule", caller);
            if (key != null && key.Length > 0)
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_Schedules;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_Schedule), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_Schedule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_Schedule>)q);
                }
            }
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                try
                {
                    Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                    if (AssoID != null && AssoID > 0)
                    {
                        IQueryable query = db.T_Schedules;
                        Type[] exprArgTypes = { query.ElementType };
                        string propToWhere = AssoNameWithParent;
                        ParameterExpression p = Expression.Parameter(typeof(T_Schedule), "p");
                        MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                        LambdaExpression lambda = Expression.Lambda<Func<T_Schedule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                        IQueryable q = query.Provider.CreateQuery(methodCall);
                        list = ((IQueryable<T_Schedule>)q).Take(20);
                    }
                }
                catch
                {
                    var data = from x in list.Take(20).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_Schedule>(User, list, "T_Schedule", caller);
            if (key != null && key.Length > 0)
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(20).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(20).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.Id != val).Take(20).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Take(20).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllMultiSelectValue(string key, string AssoNameWithParent, string AssociationID)
        {
            IQueryable<T_Schedule> list = db.T_Schedules;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.T_Schedules;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(T_Schedule), "p"));
                MemberExpression member = Expression.PropertyOrField(paramList[0], propToWhere);
                List<LambdaExpression> lexList = new List<LambdaExpression>();
                Expression ex = null;
                for (int i = 0; i < AssoIDs.Count; i++)
                {
                    if (string.IsNullOrEmpty(AssoIDs[i].ToString()))
                        continue;
                    Nullable<long> AssoID = Convert.ToInt64(AssoIDs[i].ToString());
                    if (i == 0)
                    {
                        Expression bodyInner = Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type));
                        lexList.Add(Expression.Lambda(bodyInner, paramList[0]));
                    }
                    else
                    {
                        ex = Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type));
                        Expression bodyOuter = Expression.Or(lexList[lexList.Count - 1].Body, ex);
                        lexList.Add(Expression.Lambda(bodyOuter, paramList[0]));
                        ex = null;
                    }
                }
                LambdaExpression lambda = (lexList[lexList.Count - 1]);
                MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                IQueryable q = query.Provider.CreateQuery(methodCall);
                list = ((IQueryable<T_Schedule>)q);
            }
            if (key != null && key.Length > 0)
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.Take(10).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        private DataSet DataImport(string fileExtension, string fileLocation)
        {
            string excelConnectionString = string.Empty;
            if (fileExtension == ".xls")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            }
            else if (fileExtension == ".csv")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.IO.Path.GetDirectoryName(fileLocation) + ";Extended Properties=\"Text;HDR=YES;FMT=Delimited\"";
            }
            else if (fileExtension == ".xlsx")
            {
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            }
            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
            excelConnection.Open();
            DataSet objDataSet = new DataSet();
            DataTable dt = null;
            string query = string.Empty;
            String[] excelSheets = null;
            if (fileExtension == ".csv")
            {
                query = "SELECT * FROM [" + System.IO.Path.GetFileName(fileLocation) + "]";
            }
            else
            {
                dt = new DataTable();
                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }
                excelSheets = new String[dt.Rows.Count];
                int t = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                query = string.Format("Select * from [{0}]", excelSheets[0]);
            }
            excelConnection.Close();
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
            {
                dataAdapter.Fill(objDataSet);
            }
            excelConnection1.Close();
            return WithoutBlankRow(objDataSet);
        }
        private DataSet WithoutBlankRow(DataSet ds)
        {
            DataSet dsnew = new DataSet();
            DataTable dt = ds.Tables[0].Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field).ToString().Trim(), string.Empty) == 0)).CopyToDataTable();
            dsnew.Tables.Add(dt);
            return dsnew;
        }
        [HttpGet]
        public ActionResult Upload()
        {
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_Schedule")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_Schedule").ToList();
            var distinctMapping = lstMappings.GroupBy(p => p.MappingName).Distinct();
            List<ImportConfiguration> ddlMappingList = new List<ImportConfiguration>();
            foreach (var elem in distinctMapping)
            {
                ddlMappingList.Add(elem.FirstOrDefault());
            }
            var DefaultMapping = lstMappings.Where(p => p.IsDefaultMapping).FirstOrDefault();
            var mappingID = DefaultMapping == null ? 0 : DefaultMapping.Id;
            ViewBag.IsDefaultMapping = DefaultMapping != null ? true : false;
            ViewBag.ListOfMappings = new SelectList(ddlMappingList, "ID", "MappingName", mappingID);
            ViewBag.Title = "Upload File";
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult Upload([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload, FormCollection collection)
        {
            if (FileUpload != null)
            {
                string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv" || fileExtension == ".all")
                {
                    string rename = string.Empty;
                    if (fileExtension == ".all")
                    {
                        rename = System.IO.Path.GetFileName(FileUpload.FileName.ToLower().Replace(fileExtension, ".csv"));
                        fileExtension = ".csv";
                    }
                    else
                        rename = System.IO.Path.GetFileName(FileUpload.FileName);
                    string fileLocation = string.Format("{0}\\{1}", Server.MapPath("~/ExcelFiles"), rename);
                    if (System.IO.File.Exists(fileLocation))
                        System.IO.File.Delete(fileLocation);
                    FileUpload.SaveAs(fileLocation);
                    DataSet objDataSet = DataImport(fileExtension, fileLocation);
                    var col = new List<SelectListItem>();
                    if (objDataSet.Tables.Count > 0)
                    {
                        int iCols = objDataSet.Tables[0].Columns.Count;
                        if (iCols > 0)
                        {
                            for (int i = 0; i < iCols; i++)
                            {
                                col.Add(new SelectListItem { Value = (i + 1).ToString(), Text = objDataSet.Tables[0].Columns[i].Caption });
                            }
                        }
                    }
                    col.Insert(0, new SelectListItem { Value = "", Text = "Select Column" });
                    Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList> objColMapAssocProperties = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, SelectList>();
                    Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>();
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_Schedule");
                    if (entList != null)
                    {
                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                        {
                            long selectedVal = 0;
                            var colSelected = col.FirstOrDefault(p => p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                            if (colSelected != null)
                                selectedVal = long.Parse(colSelected.Value);
                            objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                        }
                        List<GeneratorBase.MVC.ModelReflector.Association> assocList = entList.Associations;
                        if (assocList != null)
                        {
                            foreach (var assoc in assocList)
                            {
                                if (assoc.Target == "IdentityUser")
                                    continue;
                                Dictionary<string, string> lstProperty = new Dictionary<string, string>();
                                var assocEntity = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == assoc.Target);
                                var assocProperties = assocEntity.Properties.Where(p => p.Name != "DisplayValue");
                                lstProperty.Add("DisplayValue", "DisplayValue-" + assoc.AssociationProperty);
                                foreach (var prop in assocProperties)
                                {
                                    lstProperty.Add(prop.DisplayName, prop.DisplayName + "-" + assoc.AssociationProperty);
                                }
                                var dispValue = lstProperty.Keys.FirstOrDefault();
                                objColMapAssocProperties.Add(assoc, new SelectList(lstProperty.AsEnumerable(), "Value", "Key", "Key"));
                            }
                        }
                    }
                    ViewBag.AssociatedProperties = objColMapAssocProperties;
                    ViewBag.ColumnMapping = objColMap;
                    ViewBag.FilePath = fileLocation;
                    if (!string.IsNullOrEmpty(collection["ListOfMappings"]))
                    {
                        string typeName = "";
                        string colKey = "";
                        string colDisKey = "";
                        string colListInx = "";
                        typeName = "T_Schedule";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_Schedule" && p.IsDefaultMapping);
                            if (lstMapping.Count() > 0)
                            {
                                foreach (var mapping in lstMapping)
                                {
                                    mapping.IsDefaultMapping = false;
                                    db.Entry(mapping).State = EntityState.Modified;
                                }
                            }
                            foreach (var defaultMapping in DefaultMapping)
                            {
                                defaultMapping.IsDefaultMapping = true;
                                db.Entry(defaultMapping).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        foreach (var defcol in ViewBag.ColumnMapping as Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>)
                        {
                            colDisKey = colDisKey + defcol.Key.DisplayName + ",";
                            colKey = colKey + defcol.Key.Name + ",";
                            string colSelected = (DefaultMapping.ToList().Where(p => p.TableColumn == defcol.Key.DisplayName).Count() > 0 ? DefaultMapping.ToList().Where(p => p.TableColumn == defcol.Key.DisplayName).FirstOrDefault().SheetColumn : null);
                            int colExist = 0;
                            if (!string.IsNullOrEmpty(colSelected))
                            {
                                colExist = defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).Count();
                                if (colExist == 0)
                                    ExistsColumnMappingName += defcol.Key.DisplayName + " - " + colSelected + ", ";
                                colListInx = colListInx + (colExist > 0 ? defcol.Value.Where(s => s.Text.Trim() == colSelected.Trim()).First().Value.ToString() : "") + ",";
                            }
                            else
                                colListInx = colListInx + "" + ",";
                        }
                        if (colKey != "")
                            colKey = colKey.Substring(0, colKey.Length - 1);
                        if (colDisKey != "")
                            colDisKey = colDisKey.Substring(0, colDisKey.Length - 1);
                        if (colListInx != "")
                            colListInx = colListInx.Substring(0, colListInx.Length - 1);
                        if (!string.IsNullOrEmpty(ExistsColumnMappingName))
                            ExistsColumnMappingName = ExistsColumnMappingName.Trim().Substring(0, ExistsColumnMappingName.Trim().Length - 1);
                        string FilePath = ViewBag.FilePath;
                        var columnlist = colKey;
                        var columndisplaynamelist = colDisKey;
                        var selectedlist = colListInx;
                        string DefaultColumnMappingName = string.Empty;
                        if (DefaultMapping.Count > 0)
                            DefaultColumnMappingName = String.Join(", ", DefaultMapping.OrderByDescending(p => p.Id).Select(p => p.TableColumn));
                        ViewBag.DefaultMappingMsg = null;
                        if (DefaultMapping.Count() != colListInx.Split(',').Where(p => p.Trim() != string.Empty).Count())
                        {
                            ViewBag.DefaultMappingMsg += "There was an ERROR in file being uploaded: It does not contain all the required columns.";
                            ViewBag.DefaultMappingMsg += "<br /><br /> Error Details: <br /> The following columns are missing : " + ExistsColumnMappingName;
                            ViewBag.DefaultMappingMsg += "<br /><br /> Please verify the file and upload again. No data has currently been imported and NO change has been made.";
                        }
                        string DetailMessage = "";
                        string excelConnectionString = string.Empty;
                        DataTable tempdt = new DataTable();
                        if (selectedlist != null && columnlist != null)
                        {
                            var dtsheetColumns = selectedlist.Split(',').ToList();
                            var dttblColumns = columndisplaynamelist.Split(',').ToList();
                            for (int j = 0; j < dtsheetColumns.Count; j++)
                            {
                                string columntable = dttblColumns[j];
                                int columnSheet = 0;
                                if (string.IsNullOrEmpty(dtsheetColumns[j]))
                                    continue;
                                else
                                    columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                                tempdt.Columns.Add(columntable);
                            }
                            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                            {
                                var sheetColumns = selectedlist.Split(',').ToList();
                                if (AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                                    continue;
                                var tblColumns = columndisplaynamelist.Split(',').ToList();
                                DataRow objdr = tempdt.NewRow();
                                for (int j = 0; j < sheetColumns.Count; j++)
                                {
                                    string columntable = tblColumns[j];
                                    int columnSheet = 0;
                                    if (string.IsNullOrEmpty(sheetColumns[j]))
                                        continue;
                                    else
                                        columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                                    string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                                    if (string.IsNullOrEmpty(columnValue))
                                        continue;
                                    objdr[columntable] = columnValue;
                                }
                                tempdt.Rows.Add(objdr);
                            }
                        }
                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Schedules";
                        Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
                        if (entList != null)
                        {
                            DataTable uniqueCols = new DataTable();
                            foreach (var association in entList.Associations)
                            {
                                if (!tempdt.Columns.Contains(association.DisplayName))
                                    continue;
                                uniqueCols = tempdt.DefaultView.ToTable(true, association.DisplayName);
                                List<String> uniqueassoValues = new List<String>();
                                for (int i = 0; i < uniqueCols.Rows.Count; i++)
                                {
                                    string assovalue = "";
                                    if (string.IsNullOrEmpty(uniqueCols.Rows[i][0].ToString().Trim()))
                                        continue;
                                    else
                                        assovalue = uniqueCols.Rows[i][0].ToString();
                                    #region Association Values
                                    switch (association.AssociationProperty)
                                    {
                                        case "T_AssociatedScheduleTypeID":
                                            var t_associatedscheduletypeId = db.T_Scheduletypes.FirstOrDefault(p => p.DisplayValue == assovalue);
                                            if (t_associatedscheduletypeId == null)
                                                uniqueassoValues.Add(assovalue);
                                            break;
                                        case "T_AssociatedRecurringScheduleDetailsTypeID":
                                            var t_associatedrecurringscheduledetailstypeId = db.T_RecurringScheduleDetailstypes.FirstOrDefault(p => p.DisplayValue == assovalue);
                                            if (t_associatedrecurringscheduledetailstypeId == null)
                                                uniqueassoValues.Add(assovalue);
                                            break;
                                        case "T_RecurringRepeatFrequencyID":
                                            var t_recurringrepeatfrequencyId = db.T_RecurringFrequencys.FirstOrDefault(p => p.DisplayValue == assovalue);
                                            if (t_recurringrepeatfrequencyId == null)
                                                uniqueassoValues.Add(assovalue);
                                            break;
                                        case "T_RepeatByID":
                                            var t_repeatbyId = db.T_MonthlyRepeatTypes.FirstOrDefault(p => p.DisplayValue == assovalue);
                                            if (t_repeatbyId == null)
                                                uniqueassoValues.Add(assovalue);
                                            break;
                                        case "T_RecurringTaskEndTypeID":
                                            var t_recurringtaskendtypeId = db.T_RecurringEndTypes.FirstOrDefault(p => p.DisplayValue == assovalue);
                                            if (t_recurringtaskendtypeId == null)
                                                uniqueassoValues.Add(assovalue);
                                            break;
                                        default:
                                            break;
                                    }
                                    #endregion
                                }
                                if (uniqueassoValues.Count > 0)
                                {
                                    DetailMessage += ", " + uniqueassoValues.Count() + " <b>new " + (association.DisplayName.EndsWith("s") ? association.DisplayName + "</b>" : association.DisplayName + "s</b>");
                                    objAssoUnique.Add(association, uniqueassoValues.ToList());
                                    if (!User.CanAdd(association.Target) && ViewBag.Confirm == null)
                                        ViewBag.Confirm = true;
                                }
                            }
                            if (objAssoUnique.Count > 0)
                                ViewBag.AssoUnique = objAssoUnique;
                            if (!string.IsNullOrEmpty(DetailMessage))
                                ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
                            ViewBag.ColumnMapping = null;
                            ViewBag.FilePath = FilePath;
                            ViewBag.ColumnList = columnlist;
                            ViewBag.SelectedList = selectedlist;
                            ViewBag.ConfirmImportData = tempdt;
                            if (ViewBag.ConfirmImportData != null)
                            {
                                ViewBag.Title = "Data Preview";
                                return View("Upload");
                            }
                            else
                                return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Plese select Excel File.");
                }
            }
            ViewBag.Title = "Column Mapping";
            return View("Upload");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult ConfirmImportData(FormCollection collection)
        {
            string FilePath = collection["hdnFilePath"];
            var columnlist = collection["lblColumn"];
            var columndisplaynamelist = collection["lblColumnDisplayName"];
            var selectedlist = collection["colList"];
            var selectedAssocPropList = collection["colAssocPropList"];
            bool SaveMapping = collection["SaveMapping"] == "on" ? true : false;
            string mappingName = collection["MappingName"];
            string DetailMessage = "";
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string typename = "T_Schedule";
            string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
            if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
            {
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
                if (!String.IsNullOrEmpty(mappingName))
                {
                    if (SaveMapping)
                    {
                        var lstMapping = db.ImportConfigurations.Where(p => p.Name == typename && p.IsDefaultMapping);
                        if (lstMapping.Count() > 0)
                        {
                            foreach (var mapping in lstMapping)
                            {
                                mapping.IsDefaultMapping = false;
                                db.Entry(mapping).State = EntityState.Modified;
                            }
                        }
                    }
                    var tblcols = columndisplaynamelist.Split(',').ToList();
                    var shtcols = selectedlist.Split(',').ToList();
                    var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typename).ToList();
                    for (int i = 0; i < tblcols.Count(); i++)
                    {
                        ImportConfiguration objImtConfig = null;
                        string shtcolName = string.IsNullOrEmpty(shtcols[i]) ? "" : objDataSet.Tables[0].Columns[int.Parse(shtcols[i]) - 1].Caption;
                        objImtConfig = new ImportConfiguration();
                        objImtConfig.Name = typename;
                        objImtConfig.MappingName = mappingName;
                        objImtConfig.IsDefaultMapping = SaveMapping;
                        objImtConfig.TableColumn = tblcols[i];
                        objImtConfig.SheetColumn = shtcolName;
                        db.ImportConfigurations.Add(objImtConfig);
                    }
                    db.SaveChanges();
                }
                DataTable tempdt = new DataTable();
                if (selectedlist != null && columnlist != null)
                {
                    var dtsheetColumns = selectedlist.Split(',').ToList();
                    var dttblColumns = columndisplaynamelist.Split(',').ToList();
                    for (int j = 0; j < dtsheetColumns.Count; j++)
                    {
                        string columntable = dttblColumns[j];
                        int columnSheet = 0;
                        if (string.IsNullOrEmpty(dtsheetColumns[j]))
                            continue;
                        else
                            columnSheet = Convert.ToInt32(dtsheetColumns[j]) - 1;
                        tempdt.Columns.Add(columntable);
                    }
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        var sheetColumns = selectedlist.Split(',').ToList();
                        if (AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                            continue;
                        var tblColumns = columndisplaynamelist.Split(',').ToList();
                        DataRow objdr = tempdt.NewRow();
                        for (int j = 0; j < sheetColumns.Count; j++)
                        {
                            string columntable = tblColumns[j];
                            int columnSheet = 0;
                            if (string.IsNullOrEmpty(sheetColumns[j]))
                                continue;
                            else
                                columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                            string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                            if (string.IsNullOrEmpty(columnValue))
                                continue;
                            objdr[columntable] = columnValue;
                        }
                        tempdt.Rows.Add(objdr);
                    }
                }
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Schedules";
                Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(selectedAssocPropList))
                {
                    var entitypropList = selectedAssocPropList.Split(',');
                    foreach (var prop in entitypropList)
                    {
                        var lst = prop.Split('-');
                        lstEntityProp.Add(lst[1], lst[0]);
                    }
                }
                Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_Schedule");
                if (entList != null)
                {
                    DataTable uniqueCols = new DataTable();
                    foreach (var association in entList.Associations)
                    {
                        if (!tempdt.Columns.Contains(association.DisplayName))
                            continue;
                        uniqueCols = tempdt.DefaultView.ToTable(true, association.DisplayName);
                        List<String> uniqueassoValues = new List<String>();
                        for (int i = 0; i < uniqueCols.Rows.Count; i++)
                        {
                            string assovalue = "";
                            if (string.IsNullOrEmpty(uniqueCols.Rows[i][0].ToString().Trim()))
                                continue;
                            else
                                assovalue = uniqueCols.Rows[i][0].ToString();
                            #region Association Values
                            switch (association.AssociationProperty)
                            {
                                case "T_AssociatedScheduleTypeID":
                                    var strPropertyT_AssociatedScheduleType = lstEntityProp.FirstOrDefault(p => p.Key == "T_AssociatedScheduleTypeID").Value;
                                    ModelReflector.Property propT_AssociatedScheduleType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Scheduletype").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_AssociatedScheduleType);
                                    var elementTypeT_AssociatedScheduleType = db.T_Scheduletypes.ElementType;
                                    var propertyInfoT_AssociatedScheduleType = elementTypeT_AssociatedScheduleType.GetProperty(propT_AssociatedScheduleType.Name);
                                    var t_associatedscheduletypeId = db.T_Scheduletypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_AssociatedScheduleType.GetValue(p, null)) == assovalue);
                                    if (t_associatedscheduletypeId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                case "T_AssociatedRecurringScheduleDetailsTypeID":
                                    var strPropertyT_AssociatedRecurringScheduleDetailsType = lstEntityProp.FirstOrDefault(p => p.Key == "T_AssociatedRecurringScheduleDetailsTypeID").Value;
                                    ModelReflector.Property propT_AssociatedRecurringScheduleDetailsType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringScheduleDetailstype").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_AssociatedRecurringScheduleDetailsType);
                                    var elementTypeT_AssociatedRecurringScheduleDetailsType = db.T_RecurringScheduleDetailstypes.ElementType;
                                    var propertyInfoT_AssociatedRecurringScheduleDetailsType = elementTypeT_AssociatedRecurringScheduleDetailsType.GetProperty(propT_AssociatedRecurringScheduleDetailsType.Name);
                                    var t_associatedrecurringscheduledetailstypeId = db.T_RecurringScheduleDetailstypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_AssociatedRecurringScheduleDetailsType.GetValue(p, null)) == assovalue);
                                    if (t_associatedrecurringscheduledetailstypeId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                case "T_RecurringRepeatFrequencyID":
                                    var strPropertyT_RecurringRepeatFrequency = lstEntityProp.FirstOrDefault(p => p.Key == "T_RecurringRepeatFrequencyID").Value;
                                    ModelReflector.Property propT_RecurringRepeatFrequency = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringFrequency").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RecurringRepeatFrequency);
                                    var elementTypeT_RecurringRepeatFrequency = db.T_RecurringFrequencys.ElementType;
                                    var propertyInfoT_RecurringRepeatFrequency = elementTypeT_RecurringRepeatFrequency.GetProperty(propT_RecurringRepeatFrequency.Name);
                                    var t_recurringrepeatfrequencyId = db.T_RecurringFrequencys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RecurringRepeatFrequency.GetValue(p, null)) == assovalue);
                                    if (t_recurringrepeatfrequencyId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                case "T_RepeatByID":
                                    var strPropertyT_RepeatBy = lstEntityProp.FirstOrDefault(p => p.Key == "T_RepeatByID").Value;
                                    ModelReflector.Property propT_RepeatBy = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MonthlyRepeatType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RepeatBy);
                                    var elementTypeT_RepeatBy = db.T_MonthlyRepeatTypes.ElementType;
                                    var propertyInfoT_RepeatBy = elementTypeT_RepeatBy.GetProperty(propT_RepeatBy.Name);
                                    var t_repeatbyId = db.T_MonthlyRepeatTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RepeatBy.GetValue(p, null)) == assovalue);
                                    if (t_repeatbyId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                case "T_RecurringTaskEndTypeID":
                                    var strPropertyT_RecurringTaskEndType = lstEntityProp.FirstOrDefault(p => p.Key == "T_RecurringTaskEndTypeID").Value;
                                    ModelReflector.Property propT_RecurringTaskEndType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringEndType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RecurringTaskEndType);
                                    var elementTypeT_RecurringTaskEndType = db.T_RecurringEndTypes.ElementType;
                                    var propertyInfoT_RecurringTaskEndType = elementTypeT_RecurringTaskEndType.GetProperty(propT_RecurringTaskEndType.Name);
                                    var t_recurringtaskendtypeId = db.T_RecurringEndTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RecurringTaskEndType.GetValue(p, null)) == assovalue);
                                    if (t_recurringtaskendtypeId == null)
                                        uniqueassoValues.Add(assovalue);
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                        }
                        if (uniqueassoValues.Count > 0)
                        {
                            DetailMessage += ", " + uniqueassoValues.Count() + " <b>new " + (association.DisplayName.EndsWith("s") ? association.DisplayName + "</b>" : association.DisplayName + "s</b>");
                            objAssoUnique.Add(association, uniqueassoValues.ToList());
                            if (!User.CanAdd(association.Target) && ViewBag.Confirm == null)
                                ViewBag.Confirm = true;
                        }
                    }
                }
                if (objAssoUnique.Count > 0)
                    ViewBag.AssoUnique = objAssoUnique;
                if (!string.IsNullOrEmpty(DetailMessage))
                    ViewBag.DetailMessage = DetailMessage + " in the Excel file. Please review the data below, before we import it into the system.";
                ViewBag.FilePath = FilePath;
                ViewBag.ColumnList = columnlist;
                ViewBag.SelectedList = selectedlist;
                ViewBag.ConfirmImportData = tempdt;
                ViewBag.colAssocPropList = selectedAssocPropList;
                if (ViewBag.ConfirmImportData != null)
                {
                    ViewBag.Title = "Data Preview";
                    return View("Upload");
                }
                else
                    return RedirectToAction("Index");
            }
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult ImportData(FormCollection collection)
        {
            string FilePath = collection["hdnFilePath"];
            var columnlist = collection["hdnColumnList"];
            var selectedlist = collection["hdnSelectedList"];
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
            var selectedAssocPropList = collection["hdnSelectedAssocPropList"];
            Dictionary<string, string> lstEntityProp = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(selectedAssocPropList))
            {
                var entitypropList = selectedAssocPropList.Split(',');
                foreach (var prop in entitypropList)
                {
                    var lst = prop.Split('-');
                    lstEntityProp.Add(lst[1], lst[0]);
                }
            }
            if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
            {
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
                string error = string.Empty;
                if (selectedlist != null && columnlist != null)
                {
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        var sheetColumns = selectedlist.Split(',').ToList();
                        if (AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                            continue;
                        T_Schedule model = new T_Schedule();
                        var tblColumns = columnlist.Split(',').ToList();
                        for (int j = 0; j < sheetColumns.Count; j++)
                        {
                            string columntable = tblColumns[j];
                            int columnSheet = 0;
                            if (string.IsNullOrEmpty(sheetColumns[j]))
                                continue;
                            else
                                columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                            string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString().Trim();
                            if (string.IsNullOrEmpty(columnValue))
                                continue;
                            switch (columntable)
                            {
                                case "T_Name":
                                    model.T_Name = columnValue;
                                    break;
                                case "T_Description":
                                    model.T_Description = columnValue;
                                    break;
                                case "T_StartDateTime":
                                    model.T_StartDateTime = DateTime.Parse(columnValue);
                                    break;
                                case "T_EndDate":
                                    model.T_EndDate = DateTime.Parse(columnValue);
                                    break;
                                case "T_OccurrenceLimitCount":
                                    model.T_OccurrenceLimitCount = Int32.Parse(columnValue);
                                    break;
                                case "T_Summary":
                                    model.T_Summary = columnValue;
                                    break;
                                case "T_AssociatedScheduleTypeID":
                                    dynamic t_associatedscheduletypeId = null;
                                    if (lstEntityProp.Count > 0)
                                    {
                                        var strPropertyT_AssociatedScheduleType = lstEntityProp.FirstOrDefault(p => p.Key == "T_AssociatedScheduleTypeID").Value;
                                        ModelReflector.Property propT_AssociatedScheduleType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Scheduletype").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_AssociatedScheduleType);
                                        var elementTypeT_AssociatedScheduleType = db.T_Scheduletypes.ElementType;
                                        var propertyInfoT_AssociatedScheduleType = elementTypeT_AssociatedScheduleType.GetProperty(propT_AssociatedScheduleType.Name);
                                        t_associatedscheduletypeId = db.T_Scheduletypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_AssociatedScheduleType.GetValue(p, null)) == columnValue);
                                    }
                                    else
                                        t_associatedscheduletypeId = db.T_Scheduletypes.FirstOrDefault(p => p.DisplayValue == columnValue);
                                    if (t_associatedscheduletypeId != null)
                                        model.T_AssociatedScheduleTypeID = t_associatedscheduletypeId.Id;
                                    else
                                    {
                                        if ((collection["T_AssociatedScheduleTypeID"].ToString() == "true,false"))
                                        {
                                            try
                                            {
                                                T_Scheduletype objT_Scheduletype = new T_Scheduletype();
                                                objT_Scheduletype.T_Name = (columnValue);
                                                db.T_Scheduletypes.Add(objT_Scheduletype);
                                                db.SaveChanges();
                                                model.T_AssociatedScheduleTypeID = objT_Scheduletype.Id;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    break;
                                case "T_AssociatedRecurringScheduleDetailsTypeID":
                                    dynamic t_associatedrecurringscheduledetailstypeId = null;
                                    if (lstEntityProp.Count > 0)
                                    {
                                        var strPropertyT_AssociatedRecurringScheduleDetailsType = lstEntityProp.FirstOrDefault(p => p.Key == "T_AssociatedRecurringScheduleDetailsTypeID").Value;
                                        ModelReflector.Property propT_AssociatedRecurringScheduleDetailsType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringScheduleDetailstype").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_AssociatedRecurringScheduleDetailsType);
                                        var elementTypeT_AssociatedRecurringScheduleDetailsType = db.T_RecurringScheduleDetailstypes.ElementType;
                                        var propertyInfoT_AssociatedRecurringScheduleDetailsType = elementTypeT_AssociatedRecurringScheduleDetailsType.GetProperty(propT_AssociatedRecurringScheduleDetailsType.Name);
                                        t_associatedrecurringscheduledetailstypeId = db.T_RecurringScheduleDetailstypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_AssociatedRecurringScheduleDetailsType.GetValue(p, null)) == columnValue);
                                    }
                                    else
                                        t_associatedrecurringscheduledetailstypeId = db.T_RecurringScheduleDetailstypes.FirstOrDefault(p => p.DisplayValue == columnValue);
                                    if (t_associatedrecurringscheduledetailstypeId != null)
                                        model.T_AssociatedRecurringScheduleDetailsTypeID = t_associatedrecurringscheduledetailstypeId.Id;
                                    else
                                    {
                                        if ((collection["T_AssociatedRecurringScheduleDetailsTypeID"].ToString() == "true,false"))
                                        {
                                            try
                                            {
                                                T_RecurringScheduleDetailstype objT_RecurringScheduleDetailstype = new T_RecurringScheduleDetailstype();
                                                objT_RecurringScheduleDetailstype.T_Name = (columnValue);
                                                db.T_RecurringScheduleDetailstypes.Add(objT_RecurringScheduleDetailstype);
                                                db.SaveChanges();
                                                model.T_AssociatedRecurringScheduleDetailsTypeID = objT_RecurringScheduleDetailstype.Id;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    break;
                                case "T_RecurringRepeatFrequencyID":
                                    dynamic t_recurringrepeatfrequencyId = null;
                                    if (lstEntityProp.Count > 0)
                                    {
                                        var strPropertyT_RecurringRepeatFrequency = lstEntityProp.FirstOrDefault(p => p.Key == "T_RecurringRepeatFrequencyID").Value;
                                        ModelReflector.Property propT_RecurringRepeatFrequency = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringFrequency").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RecurringRepeatFrequency);
                                        var elementTypeT_RecurringRepeatFrequency = db.T_RecurringFrequencys.ElementType;
                                        var propertyInfoT_RecurringRepeatFrequency = elementTypeT_RecurringRepeatFrequency.GetProperty(propT_RecurringRepeatFrequency.Name);
                                        t_recurringrepeatfrequencyId = db.T_RecurringFrequencys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RecurringRepeatFrequency.GetValue(p, null)) == columnValue);
                                    }
                                    else
                                        t_recurringrepeatfrequencyId = db.T_RecurringFrequencys.FirstOrDefault(p => p.DisplayValue == columnValue);
                                    if (t_recurringrepeatfrequencyId != null)
                                        model.T_RecurringRepeatFrequencyID = t_recurringrepeatfrequencyId.Id;
                                    else
                                    {
                                        if ((collection["T_RecurringRepeatFrequencyID"].ToString() == "true,false"))
                                        {
                                            try
                                            {
                                                T_RecurringFrequency objT_RecurringFrequency = new T_RecurringFrequency();
                                                objT_RecurringFrequency.T_Name = Convert.ToInt32(columnValue);
                                                db.T_RecurringFrequencys.Add(objT_RecurringFrequency);
                                                db.SaveChanges();
                                                model.T_RecurringRepeatFrequencyID = objT_RecurringFrequency.Id;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    break;
                                case "T_RepeatByID":
                                    dynamic t_repeatbyId = null;
                                    if (lstEntityProp.Count > 0)
                                    {
                                        var strPropertyT_RepeatBy = lstEntityProp.FirstOrDefault(p => p.Key == "T_RepeatByID").Value;
                                        ModelReflector.Property propT_RepeatBy = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_MonthlyRepeatType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RepeatBy);
                                        var elementTypeT_RepeatBy = db.T_MonthlyRepeatTypes.ElementType;
                                        var propertyInfoT_RepeatBy = elementTypeT_RepeatBy.GetProperty(propT_RepeatBy.Name);
                                        t_repeatbyId = db.T_MonthlyRepeatTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RepeatBy.GetValue(p, null)) == columnValue);
                                    }
                                    else
                                        t_repeatbyId = db.T_MonthlyRepeatTypes.FirstOrDefault(p => p.DisplayValue == columnValue);
                                    if (t_repeatbyId != null)
                                        model.T_RepeatByID = t_repeatbyId.Id;
                                    else
                                    {
                                        if ((collection["T_RepeatByID"].ToString() == "true,false"))
                                        {
                                            try
                                            {
                                                T_MonthlyRepeatType objT_MonthlyRepeatType = new T_MonthlyRepeatType();
                                                objT_MonthlyRepeatType.T_Name = (columnValue);
                                                db.T_MonthlyRepeatTypes.Add(objT_MonthlyRepeatType);
                                                db.SaveChanges();
                                                model.T_RepeatByID = objT_MonthlyRepeatType.Id;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    break;
                                case "T_RecurringTaskEndTypeID":
                                    dynamic t_recurringtaskendtypeId = null;
                                    if (lstEntityProp.Count > 0)
                                    {
                                        var strPropertyT_RecurringTaskEndType = lstEntityProp.FirstOrDefault(p => p.Key == "T_RecurringTaskEndTypeID").Value;
                                        ModelReflector.Property propT_RecurringTaskEndType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_RecurringEndType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_RecurringTaskEndType);
                                        var elementTypeT_RecurringTaskEndType = db.T_RecurringEndTypes.ElementType;
                                        var propertyInfoT_RecurringTaskEndType = elementTypeT_RecurringTaskEndType.GetProperty(propT_RecurringTaskEndType.Name);
                                        t_recurringtaskendtypeId = db.T_RecurringEndTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_RecurringTaskEndType.GetValue(p, null)) == columnValue);
                                    }
                                    else
                                        t_recurringtaskendtypeId = db.T_RecurringEndTypes.FirstOrDefault(p => p.DisplayValue == columnValue);
                                    if (t_recurringtaskendtypeId != null)
                                        model.T_RecurringTaskEndTypeID = t_recurringtaskendtypeId.Id;
                                    else
                                    {
                                        if ((collection["T_RecurringTaskEndTypeID"].ToString() == "true,false"))
                                        {
                                            try
                                            {
                                                T_RecurringEndType objT_RecurringEndType = new T_RecurringEndType();
                                                objT_RecurringEndType.T_Name = (columnValue);
                                                db.T_RecurringEndTypes.Add(objT_RecurringEndType);
                                                db.SaveChanges();
                                                model.T_RecurringTaskEndTypeID = objT_RecurringEndType.Id;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (model.T_StartDateTime == DateTime.MinValue)
                            model.T_StartDateTime = DateTime.Now;
                        if (ValidateModel(model) && string.IsNullOrEmpty(CheckBeforeSave(model)))
                        {
                            var result = CheckMandatoryProperties(model);
                            if (result == null || result.Count == 0)
                            {
                                db.T_Schedules.Add(model);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (ViewBag.ImportError == null)
                                    ViewBag.ImportError = "Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                else
                                    ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " " + string.Join(", ", result.ToArray()) + " Required Value Missing";
                                error += ((i + 1).ToString()) + ",";
                            }
                        }
                        else
                        {
                            if (ViewBag.ImportError == null)
                                ViewBag.ImportError = "Row No : " + (i + 1) + " Some Required Value Missing or Before save validation failed.";
                            else
                                ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " Some Required Value Missing or Before save validation failed";
                            error += ((i + 1).ToString()) + ",";
                        }
                    }
                }
                if (ViewBag.ImportError != null)
                {
                    ViewBag.FilePath = FilePath;
                    ViewBag.ErrorList = error.Substring(0, error.Length - 1);
                    ViewBag.Title = "Error List";
                    return View("Upload");
                }
                else
                {
                    if (System.IO.File.Exists(fileLocation))
                        System.IO.File.Delete(fileLocation);
                    if (ViewBag.ImportError == null)
                    {
                        ViewBag.ImportError = "success";
                        ViewBag.Title = "Upload List";
                        return View("Upload");
                    }
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public ActionResult DownloadSheet(FormCollection collection)
        {
            string FilePath = collection["hdnFilePath"];
            var columnlist = collection["hdnErrorList"];
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string fileExtension = System.IO.Path.GetExtension(fileLocation).ToLower();
            if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".csv")
            {
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
                if (System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                (new DataToExcel()).ExportDetails(objDataSet.Tables[0], fileExtension == ".csv" ? "CSV" : "Excel", "DownloadError" + (fileExtension == ".csv" ? ".csv" : ".xls"), columnlist.Split(',').ToList());
            }
            return View();
        }
        public bool ValidateModel(T_Schedule validate)
        {
            return Validator.TryValidateObject(validate, new ValidationContext(validate, null, null), null);
        }
        bool AreAllColumnsEmpty(DataRow dr, List<string> sheetColumns)
        {
            if (dr == null)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < sheetColumns.Count(); i++)
                {
                    if (string.IsNullOrEmpty(sheetColumns[i]))
                        continue;
                    if (dr[Convert.ToInt32(sheetColumns[i]) - 1] != null && dr[Convert.ToInt32(sheetColumns[i]) - 1].ToString() != "")
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMapping(string typename)
        {
            bool isMapping = (db.ImportConfigurations.Where(p => p.LastUpdateUser == User.Name && p.Name == typename)).Count() > 0 ? true : false;
            return Json(isMapping, JsonRequestBehavior.AllowGet);
        }
        public object GetFieldValueByEntityId(long Id, string PropName)
        {
            try
            {
                ApplicationContext db1 = new ApplicationContext(new SystemUser());
                var obj1 = db1.T_Schedules.Find(Id);
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                return PropValue;
            }
            catch { return null; }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReadOnlyProperties(T_Schedule OModel)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_Schedule").ToList();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_Schedule");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Schedule").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            if (!RequiredProperties.ContainsKey(paramName))
                                RequiredProperties.Add(paramName, dispName);
                        }
                    }
                }
            }
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(T_Schedule OModel, string ruleType)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_Schedule").ToList();
                if (BR != null && BR.Count > 0)
                {
                    if (ruleType == "OnCreate")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                    else if (ruleType == "OnEdit")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_Schedule");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Schedule").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            if (!RequiredProperties.ContainsKey(paramName))
                                RequiredProperties.Add(paramName, dispName);
                        }
                    }
                }
            }
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(T_Schedule OModel)
        {
            string RulesApplied = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_Schedule").ToList();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "T_Schedule");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
                    {
                        RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
                    }
                }
            }
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
        private List<string> CheckMandatoryProperties(T_Schedule OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_Schedule").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_Schedule");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var type = OModel.GetType();
                            if (type.GetProperty(paramName) == null) continue;
                            var propertyvalue = type.GetProperty(paramName).GetValue(OModel, null);
                            if (propertyvalue == null)
                            {
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Schedule").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                                result.Add(dispName);
                            }
                        }
                    }
                }
            }
            return result;
        }
        public long? GetIdFromDisplayValue(string displayvalue)
        {
            ApplicationContext db1 = new ApplicationContext(User);
            if (string.IsNullOrEmpty(displayvalue)) return 0;
            var _Obj = db1.T_Schedules.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if (_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var obj1 = db1.T_Schedules.Find(Id);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            PropValue = PropValue == null ? 0 : PropValue;
            return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);

        }
        public string checkHidden(string entityName, string brType)
        {
            System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
            System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
            RuleActionContext objRuleAction = new RuleActionContext();
            ConditionContext objCondition = new ConditionContext();
            ActionArgsContext objActionArgs = new ActionArgsContext();
            var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            string[] rbList = new string[] { "T_AssociatedScheduleTypeID", "T_AssociatedRecurringScheduleDetailsTypeID", "T_RepeatByID", "T_RecurringTaskEndTypeID" };
            if (businessRules != null && businessRules.Count > 0)
            {
                foreach (BusinessRule objBR in businessRules)
                {
                    long ActionTypeId = 6;
                    var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                    if (objRuleActionList.Count() > 0)
                    {
                        if (objBR.AssociatedBusinessRuleTypeID == 1 && brType != "OnCreate")
                            continue;
                        else if (objBR.AssociatedBusinessRuleTypeID == 2 && brType != "OnEdit")
                            continue;
                        foreach (RuleAction objRA in objRuleActionList)
                        {
                            var objConditionList = objCondition.Conditions.Where(con => con.RuleConditionsID == objRA.RuleActionID);
                            if (objConditionList.Count() > 0)
                            {
                                string fnCondition = string.Empty;
                                string fnConditionValue = string.Empty;
                                foreach (Condition objCon in objConditionList)
                                {
                                    if (string.IsNullOrEmpty(chkHidden.ToString()))
                                    {
                                        chkHidden.Append("<script type='text/javascript'>$(document).ready(function () {");
                                        fnCondition = "hiddenCondition" + objCon.Id.ToString() + "()";
                                        chkHidden.Append(fnCondition + ";");
                                    }
                                    string datatype = checkPropType(entityName, objCon.PropertyName);
                                    string operand = checkOperator(objCon.Operator);
                                    //check if today is used for datetime property
                                    string condValue = "";
                                    if (datatype == "DateTime" && objCon.Value.ToLower() == "today")
                                        condValue = DateTime.Now.Date.ToString("MM/dd/yyyy");
                                    else
                                        condValue = objCon.Value;
                                    var rbcheck = false;
                                    if (rbList != null && rbList.Contains(objCon.PropertyName))
                                        rbcheck = true;
                                    chkHidden.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                    if (datatype == "Association")
                                    {
                                        if (operand.Length > 2)
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (operand.Length > 2)
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                if (objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnEdit")
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val() " + operand + " '" + objCon.Value + "')";
                                                else if (objCon.PropertyName == "Id" && objCon.Operator == ">" && objCon.Value == "0" && brType == "OnCreate")
                                                    fnConditionValue = "('true')";
                                                else
                                                    fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(chkHidden.ToString()))
                                {
                                    chkHidden.Append(" });");
                                    var objActionArgsList = objActionArgs.ActionArgss.Where(aa => aa.ActionArgumentsID == objRA.Id);
                                    if (objActionArgsList.Count() > 0)
                                    {
                                        string fnName = string.Empty;
                                        string fnProp = string.Empty;
                                        string fn = string.Empty;
                                        foreach (ActionArgs objaa in objActionArgsList)
                                        {
                                            fn += objaa.Id.ToString();
                                            fnProp += "$('#dv" + objaa.ParameterName + "').css('display', type);";
                                        }
                                        if (!string.IsNullOrEmpty(fn))
                                            fnName = "hiddenProp" + fn;
                                        if (!string.IsNullOrEmpty(fnName))
                                        {
                                            chkHidden.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) {" + fnName + "('none'); } else { " + fnName + "('block');  } }");
                                            chkFnHidden.Append("function " + fnName + "(type) { " + fnProp + " }");
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(chkFnHidden.ToString()))
                                {
                                    chkHidden.Append(chkFnHidden.ToString());
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(chkHidden.ToString()))
                        {
                            chkHidden.Append("</script> ");
                        }
                    }
                }
            }
            return chkHidden.ToString();
        }
        public string checkOperator(string condition)
        {
            string opr = string.Empty;
            switch (condition)
            {
                case "=":
                    opr = "==";
                    break;
                case "Contains":
                    opr = "Contains";
                    break;
                default:
                    opr = condition;
                    break;
            }
            return opr;
        }
        public string checkPropType(string EntityName, string PropName)
        {
            if (PropName == "Id")
                return "long";
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
            var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
            string DataType = PropInfo.DataType;
            if (AssociationInfo != null)
            {
                DataType = "Association";
            }
            return DataType;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Check1MThresholdValue(T_Schedule t_schedule)
        {
            Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
            return Json(msgThreshold, JsonRequestBehavior.AllowGet);
        }
        public bool CheckBeforeDelete(T_Schedule t_schedule)
        {
            var result = true;
            // Write your logic here

            if (!result)
            {
                var AlertMsg = "Validation Alert - Before Delete !! Information not deleted.";
                ModelState.AddModelError("CustomError", AlertMsg);
                ViewBag.ApplicationError = AlertMsg;
            }
            return result;
        }
        public string CheckBeforeSave(T_Schedule t_schedule)
        {
            var result = true;
            var AlertMsg = "";
            // Write your logic here

            if (!result)
            {
                AlertMsg = "Validation Alert - Before Save !! Information not saved.";
                ModelState.AddModelError("CustomError", AlertMsg);
                ViewBag.ApplicationError = AlertMsg;
            }
            return AlertMsg;
        }
        public void OnDeleting(T_Schedule t_schedule)
        {
            // Write your logic here

        }
        public void OnSaving(T_Schedule t_schedule, ApplicationContext db)
        {
            // Write your logic here

        }
        
        //code for verb action security
        public string[] getVerbsName()
        {
            string[] Verbsarr = new string[] { "BulkUpdate", "BulkDelete" };
            return Verbsarr;
        }
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCalculationValues(T_Schedule t_schedule)
        {
            t_schedule.setCalculation();
            Dictionary<string, string> Calculations = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_schedule.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        //get Templates 
        // select templates 
        public void GetTemplatesForList()
        {
            string pageNameList = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_Schedule"
                                       select s;
            bool IsInRoles = false;
            if (lstDefaultEntityPage.Count() > 0)
            {
                string role = lstDefaultEntityPage.Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach (var item in rolesArr)
                {
                    if (item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());

                }
            }
            if (lstDefaultEntityPage.Count() > 0)
                pageNameList = lstDefaultEntityPage.Select(p => p.ListEntityPage).FirstOrDefault().ToString();
            if (!String.IsNullOrEmpty(pageNameList) && IsInRoles)
                ViewBag.TemplatesName = pageNameList;
            else
                ViewBag.TemplatesName = "IndexPartial";



        }
        public void GetTemplatesForDetails()
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_Schedule"
                                       select s;
            bool IsInRoles = false;
            if (lstDefaultEntityPage.Count() > 0)
            {
                string role = lstDefaultEntityPage.Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach (var item in rolesArr)
                {
                    if (item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());

                }
            }
            if (lstDefaultEntityPage.Count() > 0)
            {
                pageDetails = lstDefaultEntityPage.Select(p => p.ViewEntityPage).FirstOrDefault().ToString();
            }
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = "Details";

        }
        public void GetTemplatesForEdit()
        {
            string pageEdit = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_Schedule"
                                       select s;
            bool IsInRoles = false;
            if (lstDefaultEntityPage.Count() > 0)
            {
                string role = lstDefaultEntityPage.Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach (var item in rolesArr)
                {
                    if (item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());

                }
            }
            if (lstDefaultEntityPage.Count() > 0)
            {
                pageEdit = lstDefaultEntityPage.Select(p => p.EditEntityPage).FirstOrDefault().ToString();
            }
            if (!String.IsNullOrEmpty(pageEdit) && IsInRoles)
                ViewBag.TemplatesName = pageEdit;
            else
                ViewBag.TemplatesName = "Edit";

        }
        public void GetTemplatesForSearch()
        {
            string pageSearch = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_Schedule"
                                       select s;
            bool IsInRoles = false;
            if (lstDefaultEntityPage.Count() > 0)
            {
                string role = lstDefaultEntityPage.Select(p => p.Roles).FirstOrDefault().ToString();
                var rolesArr = role.Split(',');
                foreach (var item in rolesArr)
                {
                    if (item.ToString() == "All")
                    {
                        IsInRoles = true;
                        break;
                    }
                    else
                        IsInRoles = User.IsInRole(item.ToString());

                }
            }
            if (lstDefaultEntityPage.Count() > 0)
                pageSearch = lstDefaultEntityPage.Select(p => p.SearchEntityPage).FirstOrDefault().ToString();

            if (!String.IsNullOrEmpty(pageSearch) && IsInRoles)
                ViewBag.TemplatesName = pageSearch;
            else
                ViewBag.TemplatesName = "SetFSearch";
        }

        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDerivedDetails(T_Schedule t_schedule, string IgnoreEditable)
        {
            Dictionary<string, string> derivedlist = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_schedule.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
     
        public ActionResult ShowFullCalendar()
        {
            var obj = db.T_Schedules.ToList();
            return View(obj);
        }

    }
}