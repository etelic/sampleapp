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
using GeneratorBase.MVC.DynamicQueryable;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{	
    public partial class T_SessionEventsController : BaseController
    {
		private void LoadViewDataForCount(T_SessionEvents t_sessionevents, string AssocType)
        {
        }
		private void LoadViewDataAfterOnEdit(T_SessionEvents t_sessionevents)
        {
					 ViewBag.T_SessionEventsLearningCenterID = new SelectList(db.T_LearningCenters, "ID", "DisplayValue", t_sessionevents.T_SessionEventsLearningCenterID);
			 ViewBag.ScheduleID = new SelectList(db.T_Schedules, "ID", "DisplayValue", t_sessionevents.ScheduleID);
			 ViewBag.T_SessionEventsTimeSlotsID = new SelectList(db.T_TimeSlotss, "ID", "DisplayValue", t_sessionevents.T_SessionEventsTimeSlotsID);
        }
        private void LoadViewDataBeforeOnEdit(T_SessionEvents t_sessionevents)
        {
		
               var _objT_SessionEventsLearningCenter = new List<T_LearningCenter>();
               _objT_SessionEventsLearningCenter.Add(t_sessionevents.t_sessioneventslearningcenter);
			   			   ViewBag.T_SessionEventsLearningCenterID = new SelectList(_objT_SessionEventsLearningCenter, "ID", "DisplayValue", t_sessionevents.T_SessionEventsLearningCenterID);
			               var _objSchedule = new List<T_Schedule>();
               _objSchedule.Add(t_sessionevents.schedule);
			   			   ViewBag.ScheduleID = new SelectList(_objSchedule, "ID", "DisplayValue", t_sessionevents.ScheduleID);
			               var _objT_SessionEventsTimeSlots = new List<T_TimeSlots>();
               _objT_SessionEventsTimeSlots.Add(t_sessionevents.t_sessioneventstimeslots);
			   			   ViewBag.T_SessionEventsTimeSlotsID = new SelectList(_objT_SessionEventsTimeSlots, "ID", "DisplayValue", t_sessionevents.T_SessionEventsTimeSlotsID);
			        }
        private void LoadViewDataAfterOnCreate(T_SessionEvents t_sessionevents)
        {
			 ViewBag.T_SessionEventsLearningCenterID = new SelectList(db.T_LearningCenters, "ID", "DisplayValue", t_sessionevents.T_SessionEventsLearningCenterID);
			 ViewBag.ScheduleID = new SelectList(db.T_Schedules, "ID", "DisplayValue", t_sessionevents.ScheduleID);
			 ViewBag.T_SessionEventsTimeSlotsID = new SelectList(db.T_TimeSlotss, "ID", "DisplayValue", t_sessionevents.T_SessionEventsTimeSlotsID);
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
	
			if(HostingEntityName == "T_LearningCenter" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_SessionEventsLearningCenter")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.T_SessionEventsLearningCenterID = new SelectList(db.T_LearningCenters.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_LearningCenters.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
			var objT_SessionEventsLearningCenter = new List<T_LearningCenter>();
			 ViewBag.T_SessionEventsLearningCenterID = new SelectList(objT_SessionEventsLearningCenter , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "T_Schedule" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="Schedule")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.ScheduleID = new SelectList(db.T_Schedules.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_Schedules.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
			var objSchedule = new List<T_Schedule>();
			 ViewBag.ScheduleID = new SelectList(objSchedule , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "T_TimeSlots" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_SessionEventsTimeSlots")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.T_SessionEventsTimeSlotsID = new SelectList(db.T_TimeSlotss.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_TimeSlotss.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
			var objT_SessionEventsTimeSlots = new List<T_TimeSlots>();
			 ViewBag.T_SessionEventsTimeSlotsID = new SelectList(objT_SessionEventsTimeSlots , "ID", "DisplayValue");
		    }
        }
		private IQueryable<T_SessionEvents> searchRecords(IQueryable<T_SessionEvents> lstT_SessionEvents, string searchString, bool? IsDeepSearch)
        {
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_SessionEvents = lstT_SessionEvents.Where(s => (!String.IsNullOrEmpty(s.T_Title) && s.T_Title.ToUpper().Contains(searchString)) ||(s.t_sessioneventslearningcenter!= null && (s.t_sessioneventslearningcenter.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.schedule!= null && (s.schedule.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_sessioneventstimeslots!= null && (s.t_sessioneventstimeslots.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_SessionEvents = lstT_SessionEvents.Where(s => (!String.IsNullOrEmpty(s.T_Title) && s.T_Title.ToUpper().Contains(searchString)) ||(s.t_sessioneventslearningcenter!= null && (s.t_sessioneventslearningcenter.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.t_sessioneventstimeslots!= null && (s.t_sessioneventstimeslots.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
			try
            {
                var boolvalue = Convert.ToBoolean(searchString);
                lstT_SessionEvents = lstT_SessionEvents.Union(db.T_SessionEventss.Where(s => (s.T_IsCancelled == boolvalue) ));
            }
            catch { }
			try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstT_SessionEvents = lstT_SessionEvents.Union(db.T_SessionEventss.Where(s => (s.T_EventDate == datevalue) ||(s.T_StartTime == datevalue) ||(s.T_EndTime == datevalue) ));
            }
            catch { }
            return lstT_SessionEvents;
        }
		private IQueryable<T_SessionEvents> sortRecords(IQueryable<T_SessionEvents> lstT_SessionEvents, string sortBy, string isAsc)
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

	 if(sortBy == "T_SessionEventsLearningCenterID")
                return isAsc.ToLower() == "asc" ? lstT_SessionEvents.OrderBy(p => p.t_sessioneventslearningcenter.DisplayValue) : lstT_SessionEvents.OrderByDescending(p => p.t_sessioneventslearningcenter.DisplayValue);
	 if(sortBy == "T_SessionEventsTimeSlotsID")
                return isAsc.ToLower() == "asc" ? lstT_SessionEvents.OrderBy(p => p.t_sessioneventstimeslots.DisplayValue) : lstT_SessionEvents.OrderByDescending(p => p.t_sessioneventstimeslots.DisplayValue);
            ParameterExpression paramExpression = Expression.Parameter(typeof(T_SessionEvents), "t_sessionevents");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<T_SessionEvents>)lstT_SessionEvents.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstT_SessionEvents.ElementType, lambda.Body.Type },
                    lstT_SessionEvents.Expression,
                    lambda));
        }
		public ActionResult FindFSearch(string id, string sourceEntity)
        {
            if (sourceEntity == "T_Session")
            {
                var url = (Url.Action("FSearch"));
				var _Object = db.T_Sessions.Find(Convert.ToInt64(id));
                url += "?search=";
						var T_SessionEventsLearningCenter  = _Object.T_SessionLearningCenterAssociationID;									
						var T_SessionEventsTimeSlots  = _Object.T_SessionTimeSlotAssociatonID;									
							var T_SessionClientAssociation_t_session_t_sessioneventsclient  = _Object.T_SessionClientAssociation_t_session.Select(p => p.T_ClientID);
							if (T_SessionClientAssociation_t_session_t_sessioneventsclient != null)
							{
								var str = String.Join(",", T_SessionClientAssociation_t_session_t_sessioneventsclient);
								url += "&t_sessioneventsclient=" + str;
							}
                Response.Redirect(url.ToString());
            }
            if (sourceEntity == "T_TimeSlots")
            {
                var url = (Url.Action("FSearch"));
				var _Object = db.T_TimeSlotss.Find(Convert.ToInt64(id));
                url += "?search=";
						var T_SessionEventsLearningCenter  = _Object.T_LearningCenterTimeSlotsAssociationID;									
                Response.Redirect(url.ToString());
            }
            return null;
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity ,string t_sessioneventslearningcenter,string schedule,string t_sessioneventstimeslots,string t_sessioneventsclient, bool? RenderPartial)
        {
			int Qcount = Request.UrlReferrer.Query.Count();
			//For Reports
			 if ((RenderPartial == null ? false : RenderPartial.Value))
                Qcount = Request.QueryString.AllKeys.Count();

			ViewBag.CurrentFilter = searchString;
			if(Qcount > 0)
			{
			var T_SessionEventsLearningCenterlist = db.T_LearningCenters.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessioneventslearningcenter != null)
            {
                var ids = t_sessioneventslearningcenter.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Learning Center= ";

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
							   ViewBag.SearchResult += db.T_LearningCenters.Find(Convert.ToInt64(str)).DisplayValue+", ";
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = T_SessionEventsLearningCenterlist.Union(db.T_LearningCenters.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessioneventslearningcenter = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_sessioneventslearningcenter = new SelectList(T_SessionEventsLearningCenterlist, "ID", "DisplayValue");
			}
			var Schedulelist = db.T_Schedules.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (schedule != null)
            {
                var ids = schedule.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Schedule= ";

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
							   ViewBag.SearchResult += db.T_Schedules.Find(Convert.ToInt64(str)).DisplayValue+", ";
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = Schedulelist.Union(db.T_Schedules.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.schedule = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.schedule = new SelectList(Schedulelist, "ID", "DisplayValue");
			}
			var T_SessionEventsTimeSlotslist = db.T_TimeSlotss.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessioneventstimeslots != null)
            {
                var ids = t_sessioneventstimeslots.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Time Slot= ";

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
							   ViewBag.SearchResult += db.T_TimeSlotss.Find(Convert.ToInt64(str)).DisplayValue+", ";
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = T_SessionEventsTimeSlotslist.Union(db.T_TimeSlotss.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessioneventstimeslots = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_sessioneventstimeslots = new SelectList(T_SessionEventsTimeSlotslist, "ID", "DisplayValue");
			}
			var T_SessionEventsClientlist = db.T_Clients.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessioneventsclient != null)
            {
                var ids = t_sessioneventsclient.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Client= ";

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
							   ViewBag.SearchResult += db.T_Clients.Find(Convert.ToInt64(str)).DisplayValue+", ";
                            }
                        }
                    }
					ids1 = ids1.ToList();
					var list = T_SessionEventsClientlist.Union(db.T_Clients.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessioneventsclient = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_sessioneventsclient = new SelectList(T_SessionEventsClientlist, "ID", "DisplayValue");
			}
			}
			else
			{
			var objT_SessionEventsLearningCenter = new List<T_LearningCenter>();
		    ViewBag.t_sessioneventslearningcenter = new SelectList(objT_SessionEventsLearningCenter, "ID", "DisplayValue");
			var objSchedule = new List<T_Schedule>();
		    ViewBag.schedule = new SelectList(objSchedule, "ID", "DisplayValue");
			var objT_SessionEventsTimeSlots = new List<T_TimeSlots>();
		    ViewBag.t_sessioneventstimeslots = new SelectList(objT_SessionEventsTimeSlots, "ID", "DisplayValue");
			var objT_SessionEventsClient = new List<T_Client>();
		    ViewBag.t_sessioneventsclient = new SelectList(objT_SessionEventsClient, "ID", "DisplayValue");
			}
				var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
				if (lstFavoriteItem.Count() > 0)
                    ViewBag.FavoriteItem = lstFavoriteItem;

				ViewBag.EntityName = "T_SessionEvents";
				var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == ViewBag.EntityName);
				var Prop = Entity.Properties.Select(z => new { z.DisplayName, z.Name });
				var sortlist = Prop;

				ViewBag.PropertyList = new SelectList(Prop, "Name", "DisplayName");
				ViewBag.SuggestedDynamicValueInCondition7 = new SelectList(Prop, "Name", "DisplayName");
				Dictionary<string, string> Dumplist = new Dictionary<string, string>();
				ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7
               = ViewBag.SuggestedPropertyValue = ViewBag.AssociationPropertyList = ViewBag.SuggestedDynamicValueInCondition71 = new SelectList(Dumplist, "key", "value");

				ViewBag.SortOrder1 = new SelectList(sortlist, "Name", "DisplayName");
				ViewBag.GroupByColumn = new SelectList(sortlist, "Name", "DisplayName");

				Dictionary<string, string> columns = new Dictionary<string, string>();
			columns.Add("2", "Title");
			columns.Add("3", "Event Date");
			columns.Add("4", "Learning Center");
			columns.Add("5", "Start Time");
			columns.Add("6", "End Time");
			columns.Add("7", "Cancel this Event");
			columns.Add("8", "Description");
			columns.Add("9", "Time Slot");
				ViewBag.HideColumns = new MultiSelectList(columns, "Key", "Value");
				if ((RenderPartial == null ? false : RenderPartial.Value))
					return PartialView("~/Views/T_SessionEvents/CustomReportSearch.cshtml", new T_SessionEvents());
				return View(new T_SessionEvents());
            }

		public string conditionFSearch(string property, string operators, string value)
        {
            string expression = string.Empty;
            var PrpertyType = GetDataTypeOfProperty("T_SessionEvents", property);
            var dataType = PrpertyType[0];
            property = PrpertyType[1];
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var ValueType = GetDataTypeOfProperty("T_SessionEvents", value, true);
                if (ValueType != null && ValueType[0] == "dynamic")
                {
                    dataType = ValueType[0];
                    value = ValueType[1];
                }
            }

            switch (dataType)
            {
                case "Int32":
                case "Int64":
                case "Double":
                case "Boolean":
                case "Decimal":
                    {
                        expression = string.Format(" " + property + " " + operators + " {0}", value);
                        break;
                    }
                case "DateTime":
                    {
                        DateTime val = Convert.ToDateTime(value);
                        expression = string.Format(" " + property + " " + operators + " (\"{0}\")", val);
                        break;
                    }
                case "Text":
                case "String":
                    {
                        if (operators.ToLower() == "contains")
                            expression = string.Format(" " + property + "." + operators + "(\"{0}\")", value);
                        else
                            expression = string.Format(" " + property + " " + operators + " \"{0}\"", value);
                        break;
                    }
                default:
                    {
                        expression = string.Format(" " + property + " " + operators + " {0}", value);
                        break;
                    }
            }

            return expression;
        }

        public List<string> GetDataTypeOfProperty(string entityName, string propertyName, bool valueType = false)
        {
            var listString = new List<string>();

            System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == propertyName);

            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
            var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
            ModelReflector.Property targetPropInfo = null;
            var associatedprop = string.Empty;
            if (propertyName.StartsWith("[") && propertyName.EndsWith("]"))
            {
                propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (propertyName.Contains("."))
                {
                    var targetProperties = propertyName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            associatedprop = AssociationInfo.Name.ToLower() + "." + PropInfo.Name;
                        }
                    }
                }
            }
            string DataType = string.Empty;
            if (valueType)
                DataType = "dynamic";
            else
                DataType = PropInfo.DataType;
            listString.Add(DataType);
            if (AssociationInfo != null)
                listString.Add(associatedprop);
            else
                listString.Add(propertyName);
            return listString;
        }

        public string GetPropertyDP(string entityName, string propertyName)
        {
            System.Reflection.PropertyInfo[] properties = (propertyName.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == propertyName);

            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == entityName);
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == propertyName);
            var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propertyName);
            ModelReflector.Property targetPropInfo = null;
            var associatedprop = string.Empty;
            if (propertyName.StartsWith("[") && propertyName.EndsWith("]"))
            {
                propertyName = propertyName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (propertyName.Contains("."))
                {
                    var targetProperties = propertyName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            associatedprop = "[" + AssociationInfo.DisplayName + "." + PropInfo.DisplayName + "]";
                        }
                    }
                }
            }
            string PropertyName = string.Empty;
            if (AssociationInfo != null)
                PropertyName = associatedprop;
            else
                PropertyName = PropInfo.DisplayName;
            return PropertyName;
        }





		// GET: /T_SessionEvents/FSearch/
		public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport ,string t_sessioneventslearningcenter,string schedule,string t_sessioneventstimeslots,string t_sessioneventsclient ,string T_EventDateFrom,string T_EventDateTo,string T_StartTimeFrom,string T_StartTimeTo,string T_EndTimeFrom,string T_EndTimeTo,string T_IsCancelled, string FilterCondition, string HostingEntity, string AssociatedType,string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn,bool? IsReports)
        {
			ViewData["HostingEntity"] = HostingEntity;
            ViewData["AssociatedType"] = AssociatedType;
			ViewData["HostingEntityID"] = HostingEntityID;
			ViewBag.SearchResult = "";
			ViewData["HideColumns"] = HideColumns;
			ViewBag.GroupByColumn = GroupByColumn;
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
				 var lstT_SessionEvents  = from s in db.T_SessionEventss
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_SessionEvents  = searchRecords(lstT_SessionEvents, searchString.ToUpper(),true);
            }
			  if(!string.IsNullOrEmpty(search))
                	search=search.Replace("?IsAddPop=true", "");
			if(!string.IsNullOrEmpty(search)){
				ViewBag.SearchResult += "\r\n General Criterial= "+search;
				lstT_SessionEvents = searchRecords(lstT_SessionEvents, search,true);
			}
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_SessionEvents  = sortRecords(lstT_SessionEvents, sortBy, isAsc);
            }
            else   lstT_SessionEvents  = lstT_SessionEvents.OrderByDescending(c => c.Id);
			lstT_SessionEvents = lstT_SessionEvents.Include(t=>t.t_sessioneventslearningcenter).Include(t=>t.schedule).Include(t=>t.t_sessioneventstimeslots);
			lstT_SessionEvents = Sorting.Sort<T_SessionEvents>(lstT_SessionEvents, SortOrder);

			if (!string.IsNullOrEmpty(FilterCondition))
            {
                StringBuilder whereCondition = new StringBuilder();
                var conditions = FilterCondition.Split("?".ToCharArray()).Where(lrc => lrc != "");
                int iCnt = 1;
                foreach (var cond in conditions)
                {
                    if (string.IsNullOrEmpty(cond)) continue;
                    var param = cond.Split(",".ToCharArray());
                    var PropertyName = param[0];
                    var Operator = param[1];
                    var Value = string.Empty;
                    var LogicalConnector = string.Empty;

                    Value = param[2];
                    LogicalConnector = (param[3] == "AND" ? " And" : " Or");
                    if (iCnt == conditions.Count())
                        LogicalConnector = "";
                    whereCondition.Append(conditionFSearch(PropertyName, Operator, Value) + LogicalConnector);
                    ViewBag.SearchResult += " " + GetPropertyDP("T_SessionEvents", PropertyName) + " " + Operator + " " + Value + " " + LogicalConnector;
                    iCnt++;
                }
                if (!string.IsNullOrEmpty(whereCondition.ToString()))
                    lstT_SessionEvents = lstT_SessionEvents.Where(whereCondition.ToString());

                ViewBag.FilterCondition = FilterCondition;
            }

			 if (!string.IsNullOrEmpty(GroupByColumn))
            {
                string[] props = GroupByColumn.Split(',');
                var columns = new List<string>();
                var firstTarget = string.Empty;
                var SecondTarget = string.Empty;
                var iCnt = 1;
                foreach (string prop in props)
                {
                    if (string.IsNullOrEmpty(prop)) continue;
                    var asso = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents").Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                    if (asso != null)
                    {
                        columns.Add(prop);
                        if (iCnt == 1)
                            firstTarget = asso.Target;
                        if (iCnt == 2)
                            SecondTarget = asso.Target;
                    }
                    else
                    {
                        columns.Add(prop);
                    }
                    iCnt++;
                }
                ViewBag.GroupByColumn = GroupByColumn;
                string[] groupcolumn = columns.ToArray();
                
                var grp_T_SessionEvents = lstT_SessionEvents;
                var grp_T_SessionEventsList = grp_T_SessionEvents.GroupByMany(groupcolumn);
                var objT_SessionEventsList = new List<T_SessionEvents>();

                foreach (var grp in grp_T_SessionEventsList)
                {
                    var firstKey = grp.Key == "" ? "None" : grp.Key;
                    if (!string.IsNullOrEmpty(firstTarget) && !string.IsNullOrEmpty(grp.Key))
                        firstKey = EntityComparer.GetDisplayValueForAssociation(firstTarget, grp.Key);
                    
                    var Subgroup = grp.SubGroups;
                    if (Subgroup != null && Subgroup.Count() > 0)
                    {
                        foreach (var itemsub in Subgroup)
                        {
                            var SecondKey = firstKey + " - " + (itemsub.Key == "" ? "None" : itemsub.Key);
                            if (!string.IsNullOrEmpty(SecondTarget) && !string.IsNullOrEmpty(itemsub.Key))
                                SecondKey = firstKey + " - " + EntityComparer.GetDisplayValueForAssociation(SecondTarget, itemsub.Key);

                            var Subgroup1 = itemsub.Items;
                            foreach (var item1 in Subgroup1)
                            {
                                var objT_SessionEvents = new T_SessionEvents();
                                objT_SessionEvents = (T_SessionEvents)item1;
                                objT_SessionEvents.m_DisplayValue = SecondKey;
                                objT_SessionEventsList.Add(objT_SessionEvents);
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemgrp in grp.Items)
                        {
                            var objT_SessionEvents = new T_SessionEvents();
                            objT_SessionEvents = (T_SessionEvents)itemgrp;
                            objT_SessionEvents.m_DisplayValue = firstKey.ToString();
                            objT_SessionEventsList.Add(objT_SessionEvents);
                        }
                    }
                }
				if (objT_SessionEventsList.Count() > 0)
                lstT_SessionEvents = objT_SessionEventsList.AsQueryable();
                ViewBag.IsGroupBy = true;
            }





            var _T_SessionEvents = lstT_SessionEvents;
				if(T_EventDateFrom!=null || T_EventDateTo !=null)
				{
						try
						{
							DateTime from = T_EventDateFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(T_EventDateFrom);
							DateTime to = T_EventDateTo == null ? DateTime.MaxValue : Convert.ToDateTime(T_EventDateTo);
							_T_SessionEvents =  _T_SessionEvents.Where(o => o.T_EventDate >= from && o.T_EventDate <= to);
							ViewBag.SearchResult += "\r\n Event Date= "+from.ToShortDateString()+"-"+to.ToShortDateString();
						}
						catch { }
				}
				if(T_StartTimeFrom!=null || T_StartTimeTo !=null)
				{
						try
						{
							DateTime from = T_StartTimeFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(T_StartTimeFrom);
							DateTime to = T_StartTimeTo == null ? DateTime.MaxValue : Convert.ToDateTime(T_StartTimeTo);
							_T_SessionEvents =  _T_SessionEvents.Where(o => o.T_StartTime >= from && o.T_StartTime <= to);
							ViewBag.SearchResult += "\r\n Start Time= "+from.ToShortDateString()+"-"+to.ToShortDateString();
						}
						catch { }
				}
				if(T_EndTimeFrom!=null || T_EndTimeTo !=null)
				{
						try
						{
							DateTime from = T_EndTimeFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(T_EndTimeFrom);
							DateTime to = T_EndTimeTo == null ? DateTime.MaxValue : Convert.ToDateTime(T_EndTimeTo);
							_T_SessionEvents =  _T_SessionEvents.Where(o => o.T_EndTime >= from && o.T_EndTime <= to);
							ViewBag.SearchResult += "\r\n End Time= "+from.ToShortDateString()+"-"+to.ToShortDateString();
						}
						catch { }
				}
		 if(T_IsCancelled!=null)
            {
                try
                {
                    bool boolvalue = Convert.ToBoolean(T_IsCancelled);
                    _T_SessionEvents =  _T_SessionEvents.Where(o => o.T_IsCancelled == boolvalue);
					ViewBag.SearchResult += "\r\n Cancel this Event= "+T_IsCancelled;
                }
                catch { }
            }
			//if (lstT_SessionEvents.Where(p => p.t_sessioneventslearningcenter != null).Count() <= 50)
		    //ViewBag.t_sessioneventslearningcenter = new SelectList(lstT_SessionEvents.Where(p => p.t_sessioneventslearningcenter != null).Select(P => P.t_sessioneventslearningcenter).Distinct(), "ID", "DisplayValue");
            if (t_sessioneventslearningcenter != null)
            {
                var ids = t_sessioneventslearningcenter.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Learning Center= ";

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
                            var obj = db.T_LearningCenters.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEvents = _T_SessionEvents.Where(p => ids1.Contains(p.T_SessionEventsLearningCenterID));
            }
				//if (lstT_SessionEvents.Where(p => p.schedule != null).Count() <= 50)
		    //ViewBag.schedule = new SelectList(lstT_SessionEvents.Where(p => p.schedule != null).Select(P => P.schedule).Distinct(), "ID", "DisplayValue");
            if (schedule != null)
            {
                var ids = schedule.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Schedule= ";

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
                            var obj = db.T_Schedules.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEvents = _T_SessionEvents.Where(p => ids1.Contains(p.ScheduleID));
            }
				//if (lstT_SessionEvents.Where(p => p.t_sessioneventstimeslots != null).Count() <= 50)
		    //ViewBag.t_sessioneventstimeslots = new SelectList(lstT_SessionEvents.Where(p => p.t_sessioneventstimeslots != null).Select(P => P.t_sessioneventstimeslots).Distinct(), "ID", "DisplayValue");
            if (t_sessioneventstimeslots != null)
            {
                var ids = t_sessioneventstimeslots.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Time Slot= ";

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
                            var obj = db.T_TimeSlotss.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEvents = _T_SessionEvents.Where(p => ids1.Contains(p.T_SessionEventsTimeSlotsID));
            }
				if (t_sessioneventsclient != null)
            {
                var ids = t_sessioneventsclient.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n  Client= ";

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
                        var idvalue= Convert.ToInt64(str);
                        ViewBag.SearchResult += db.T_Clients.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                        ids1.AddRange(db.T_SessionEventsClients.Where(p=>p.T_ClientID ==idvalue).Select(p=>p.T_SessionEventsID));
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEvents = _T_SessionEvents.Where(p => ids1.Contains(p.Id));
            }
			if (!string.IsNullOrEmpty(SortOrder))
            {
                ViewBag.SearchResult += " \r\n Sort Order = ";
                var sortString = "";
                var sortProps = SortOrder.Split(",".ToCharArray());
                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents");
                foreach(var prop in sortProps)
                {
                    if (string.IsNullOrEmpty(prop)) continue;
                    var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                    if (asso != null)
                    {
                        sortString += asso.DisplayName + ">";
                    }
                    else
                    {
                        var propInfo = Entity.Properties.FirstOrDefault(p=>p.Name == prop);
                        sortString += propInfo.DisplayName + ">";
                    }
                }
                ViewBag.SearchResult += sortString.TrimEnd(">".ToCharArray());
            }

			if (!string.IsNullOrEmpty(GroupByColumn))
            {
                ViewBag.SearchResult += " \r\n Group By = ";
                var groupbyString = "";
                var GroupProps = GroupByColumn.Split(",".ToCharArray());
                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents");
                foreach (var prop in GroupProps)
                {
                    if (string.IsNullOrEmpty(prop)) continue;
                    var asso = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == prop);
                    if (asso != null)
                    {
                        groupbyString += asso.DisplayName + " > ";
                    }
                    else
                    {
                        var propInfo = Entity.Properties.FirstOrDefault(p => p.Name == prop);
                        groupbyString += propInfo.DisplayName + " > ";
                    }
                }
                ViewBag.SearchResult += groupbyString.TrimEnd(" > ".ToCharArray());
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
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_SessionEvents"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_SessionEvents"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_SessionEvents.Count() > 0)
                    pageSize = _T_SessionEvents.Count();
                return View("ExcelExport", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
            }
			else
            {
			 if (pageNumber > 1)
			 {
                var totalListCount = _T_SessionEvents.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if (pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
			 }
            }
           // return View("Index", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
		    if (IsReports != null)
            {
			  var lstCustReportItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
                if (lstCustReportItem.Count() > 0)
                    ViewBag.CustReportItem= lstCustReportItem;
                return PartialView("~/Views/T_SessionEvents/CustomReportResult.cshtml", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
            }
			if (!Request.IsAjaxRequest())
			{
				if (string.IsNullOrEmpty(viewtype))
                    viewtype = "IndexPartial";
				GetTemplatesForList(viewtype); 
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
                return View("Index",_T_SessionEvents.ToPagedList(pageNumber, pageSize));
			}
            else
			{
				if (ViewBag.TemplatesName == null)
                    return PartialView("IndexPartial", _T_SessionEvents.ToPagedList(pageNumber, pageSize));
                else
                 return PartialView(ViewBag.TemplatesName, _T_SessionEvents.ToPagedList(pageNumber, pageSize));
			}
        }
		public string ShowGraph(string type, int? inlarge)
        {
		string result = "";
		var entity = "T_SessionEvents";
           var chartlist = db.Charts.Where(chrt => chrt.EntityName == entity && chrt.ShowInDashBoard).ToList();
           if (type != "all")
               chartlist = chartlist.Where(chrt => chrt.Id == Convert.ToInt64(type)).ToList();

           var entitylist = db.T_SessionEventss;

           foreach (var charts in chartlist)
           {
		    try
               {
                   var xaxis = charts.XAxis;
                   var yaxis = charts.YAxis;
                   var xaxisTitle = charts.XAxis;
                   var yaxisTitle = charts.YAxis;
                   var charttitle = charts.ChartTitle;

                   var entInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == charts.EntityName);

                   if (yaxis == null || yaxis == "0" || yaxis.ToLower() == "displayvalue")
                       yaxis = "id";
                   if (yaxis.ToLower() == "id")
                       yaxisTitle = entInfo.DisplayName;

                   var asso = entInfo.Associations.FirstOrDefault(p => p.AssociationProperty == xaxis);
                   if (asso != null)
                   {
                       xaxis = asso.Name.ToLower() + "." + "DisplayValue";
                       xaxisTitle = asso.DisplayName;
                   }

                   var gd = entitylist.AsQueryable().GroupBy(xaxis, "it");
                   var cntgrt10 = false;
                   if (gd.Count() > 10 && inlarge == null)
                   {
                       gd = gd.Take(10);
                       cntgrt10 = true;
                   }

                   if (yaxis.ToLower() == "id")
                       gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Count() as " + yaxis + ")");
                   else
                       gd = gd.Select("new (it.Key as " + xaxisTitle.Replace(" ", "") + ", it.Sum(" + yaxis + ") as " + yaxis + ")");

                   var chart = new System.Web.UI.DataVisualization.Charting.Chart
                   {
                       BorderlineDashStyle = ChartDashStyle.Solid,
                       BackSecondaryColor = System.Drawing.Color.White,
                       BackGradientStyle = GradientStyle.TopBottom,
                       BorderlineWidth = 1,
                       BorderlineColor = System.Drawing.Color.FromArgb(26, 59, 105),

                       RenderType = RenderType.ImageTag,
                       AntiAliasing = AntiAliasingStyles.All,
                       TextAntiAliasingQuality = TextAntiAliasingQuality.High
                   };
                   chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

                   if (cntgrt10)
                       chart.Titles.Add(CreateTitle(charttitle + " (Top 10)"));
                   else
                       chart.Titles.Add(CreateTitle(charttitle));

                   chart.ChartAreas.Add(CreateChartArea((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), "", xaxisTitle, yaxisTitle));
                   chart.Series.Add(CreateSeries((SeriesChartType)Enum.Parse(typeof(SeriesChartType), charts.ChartType), ""));
                   chart.Series[0].Points.DataBindXY(gd, xaxisTitle.Replace(" ", ""), gd, yaxis);
                   chart.Legends.Add(CreateLegend(""));

                   if (charts.ChartType.ToLower() == "pie")
                   {
                       chart.Series[0].LegendText = "#VALX";
                       chart.Series[0].Label = "#PERCENT{P2}";
                   }

                   if (inlarge != null)
                   {
                       chart.Width = 800;
                       chart.Height = 800;
                   }

                   byte[] bytes;
                   using (var chartimage = new MemoryStream())
                   {
                       chart.SaveImage(chartimage, ChartImageFormat.Png);
                       bytes = chartimage.GetBuffer();
                   }

                   if (cntgrt10)
                   {
                       string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", entity, new { type = charts.Id, inlarge = 1 }) + "')\"" : "") + "/></div>";
                       string encoded = Convert.ToBase64String(bytes.ToArray());
                       result += String.Format(img, encoded);
                   }
                   else
                   {
                       string img = "<div class=" + (inlarge == null ? "col-sm-4" : "col-sm-12") + " style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;' /></div>";
                       string encoded = Convert.ToBase64String(bytes.ToArray());
                       result += String.Format(img, encoded);
                   }
               }
			    catch
                { continue; }
           }
		    return result;
	    }


		public string ShowGraph1(string type, int? inlarge)
        {
            string result = "";
           {
                var gd = db.T_SessionEventss.GroupBy(p => p.t_sessioneventslearningcenter.DisplayValue).ToList();
                var _xval = gd.Select(k => Convert.ToString(k.Key)).ToList();

				if (type == "1" || type == "all")
                {
                    var cntgrt10 = false;
                    var _yval = gd.Select(k => k.Count().ToString()).ToList();
                    if (_yval.Count > 10 && inlarge == null)
                    {
                        _xval = gd.Select(k => Convert.ToString(k.Key)).Take(10).ToList();
                        _yval = gd.Select(k => k.Count().ToString()).Take(10).ToList();
                        cntgrt10 = true;
                    }
                
               
			   var chart = new System.Web.UI.DataVisualization.Charting.Chart
                {
                    //BackColor = System.Drawing.Color.FromArgb(211, 223, 240),
                    BorderlineDashStyle = ChartDashStyle.Solid,
                    BackSecondaryColor = System.Drawing.Color.White,
                    BackGradientStyle = GradientStyle.TopBottom,
                    BorderlineWidth = 1,
                    //Palette = ChartColorPalette.BrightPastel,
                    BorderlineColor = System.Drawing.Color.Black,

                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };
                chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
				if (cntgrt10)
                chart.Titles.Add(CreateTitle("Count by Learning Center (Top 10)"));
				else
				 chart.Titles.Add(CreateTitle("Count by Learning Center"));
                chart.ChartAreas.Add(CreateChartArea(SeriesChartType.Column, "", "Learning Center", "Session Events"));
                chart.Series.Add(CreateSeries(SeriesChartType.Column, ""));
                chart.Series[0].Points.DataBindXY(_xval, _yval);
                chart.Legends.Add(CreateLegend(""));

				 if (inlarge != null)
                    {
                        chart.Width = 800;
                        chart.Height = 800;
                    }

				                byte[] bytes;
                using (var chartimage = new MemoryStream())
                {
                    chart.SaveImage(chartimage, ChartImageFormat.Png);
                    bytes = chartimage.GetBuffer();
                }
                if (cntgrt10)
                    {
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", "T_SessionEvents", new { type = "1", inlarge = 1 }) + "')\"" : "") + "/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                    else
                    {
                        string img = "<div class=" + (inlarge == null ? "col-sm-4" : "col-sm-12") + " style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;'/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
				}
            }
           {
                var gd = db.T_SessionEventss.GroupBy(p => p.schedule.DisplayValue).ToList();
                var _xval = gd.Select(k => Convert.ToString(k.Key)).ToList();

				if (type == "2" || type == "all")
                {
                    var cntgrt10 = false;
                    var _yval = gd.Select(k => k.Count().ToString()).ToList();
                    if (_yval.Count > 10 && inlarge == null)
                    {
                        _xval = gd.Select(k => Convert.ToString(k.Key)).Take(10).ToList();
                        _yval = gd.Select(k => k.Count().ToString()).Take(10).ToList();
                        cntgrt10 = true;
                    }
                
               
			   var chart = new System.Web.UI.DataVisualization.Charting.Chart
                {
                    //BackColor = System.Drawing.Color.FromArgb(211, 223, 240),
                    BorderlineDashStyle = ChartDashStyle.Solid,
                    BackSecondaryColor = System.Drawing.Color.White,
                    BackGradientStyle = GradientStyle.TopBottom,
                    BorderlineWidth = 1,
                    //Palette = ChartColorPalette.BrightPastel,
                    BorderlineColor = System.Drawing.Color.Black,

                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };
                chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
				if (cntgrt10)
                chart.Titles.Add(CreateTitle("Count by Schedule (Top 10)"));
				else
				 chart.Titles.Add(CreateTitle("Count by Schedule"));
                chart.ChartAreas.Add(CreateChartArea(SeriesChartType.Pie, "", "Schedule", "Session Events"));
                chart.Series.Add(CreateSeries(SeriesChartType.Pie, ""));
                chart.Series[0].Points.DataBindXY(_xval, _yval);
                chart.Legends.Add(CreateLegend(""));

				 if (inlarge != null)
                    {
                        chart.Width = 800;
                        chart.Height = 800;
                    }

								chart.Series[0].LegendText = "#VALX";
				chart.Series[0].Label = "#PERCENT{P2}";
				                byte[] bytes;
                using (var chartimage = new MemoryStream())
                {
                    chart.SaveImage(chartimage, ChartImageFormat.Png);
                    bytes = chartimage.GetBuffer();
                }
                if (cntgrt10)
                    {
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", "T_SessionEvents", new { type = "2", inlarge = 1 }) + "')\"" : "") + "/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                    else
                    {
                        string img = "<div class=" + (inlarge == null ? "col-sm-4" : "col-sm-12") + " style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;'/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
				}
            }
           {
                var gd = db.T_SessionEventss.GroupBy(p => p.t_sessioneventstimeslots.DisplayValue).ToList();
                var _xval = gd.Select(k => Convert.ToString(k.Key)).ToList();

				if (type == "3" || type == "all")
                {
                    var cntgrt10 = false;
                    var _yval = gd.Select(k => k.Count().ToString()).ToList();
                    if (_yval.Count > 10 && inlarge == null)
                    {
                        _xval = gd.Select(k => Convert.ToString(k.Key)).Take(10).ToList();
                        _yval = gd.Select(k => k.Count().ToString()).Take(10).ToList();
                        cntgrt10 = true;
                    }
                
               
			   var chart = new System.Web.UI.DataVisualization.Charting.Chart
                {
                    //BackColor = System.Drawing.Color.FromArgb(211, 223, 240),
                    BorderlineDashStyle = ChartDashStyle.Solid,
                    BackSecondaryColor = System.Drawing.Color.White,
                    BackGradientStyle = GradientStyle.TopBottom,
                    BorderlineWidth = 1,
                    //Palette = ChartColorPalette.BrightPastel,
                    BorderlineColor = System.Drawing.Color.Black,

                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };
                chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
				if (cntgrt10)
                chart.Titles.Add(CreateTitle("Count by Time Slot (Top 10)"));
				else
				 chart.Titles.Add(CreateTitle("Count by Time Slot"));
                chart.ChartAreas.Add(CreateChartArea(SeriesChartType.Column, "", "Time Slot", "Session Events"));
                chart.Series.Add(CreateSeries(SeriesChartType.Column, ""));
                chart.Series[0].Points.DataBindXY(_xval, _yval);
                chart.Legends.Add(CreateLegend(""));

				 if (inlarge != null)
                    {
                        chart.Width = 800;
                        chart.Height = 800;
                    }

				                byte[] bytes;
                using (var chartimage = new MemoryStream())
                {
                    chart.SaveImage(chartimage, ChartImageFormat.Png);
                    bytes = chartimage.GetBuffer();
                }
                if (cntgrt10)
                    {
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", "T_SessionEvents", new { type = "3", inlarge = 1 }) + "')\"" : "") + "/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
                    else
                    {
                        string img = "<div class=" + (inlarge == null ? "col-sm-4" : "col-sm-12") + " style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%;'/></div>";
                        string encoded = Convert.ToBase64String(bytes.ToArray());
                        result += String.Format(img, encoded);
                    }
				}
            }
			 return result;
	    }
		#region Chart Methods
        public Title CreateTitle(string charttitle)
        {
            Title title = new Title();
            title.Text = charttitle;
            title.Font = new System.Drawing.Font("Helvetica", 10F, System.Drawing.FontStyle.Regular);
            title.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
            return title;
        }
        public Legend CreateLegend(string chartlegend)
        {
            Legend legend = new Legend();
            legend.Title = chartlegend;
            legend.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
            legend.ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
            legend.Docking = Docking.Bottom;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            return legend;
        }
        public Series CreateSeries(SeriesChartType chartType, string chartseries)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = chartseries;
            seriesDetail.IsValueShownAsLabel = false;
            if (chartType == SeriesChartType.Column)
                seriesDetail.IsVisibleInLegend = false;
            seriesDetail.ChartType = chartType;
            seriesDetail.BorderWidth = 2;
            seriesDetail.ChartArea = chartseries;
            return seriesDetail;
        }
        public ChartArea CreateChartArea(SeriesChartType chartType, string chartarea, string xtitle, string ytitle)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = chartarea;
            chartArea.BackColor = System.Drawing.Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Helvetica", 8F, System.Drawing.FontStyle.Regular);
            chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            if (chartType == SeriesChartType.Column)
                chartArea.AxisX.LabelStyle.Angle = -90;
            chartArea.AxisX.Title = xtitle;
            chartArea.AxisY.Title = ytitle;
            return chartArea;
        }
        #endregion



        public string GetDisplayValue(string id)
        {
			if (string.IsNullOrEmpty(id)) return "";
			long idvalue = Convert.ToInt64(id);
			ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var _Obj = db1.T_SessionEventss.FirstOrDefault(p => p.Id == idvalue);
            return  _Obj==null?"":_Obj.DisplayValue;
        }
		 public object GetRecordById(string id)
        {
		            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            if (string.IsNullOrEmpty(id)) return "";
            var _Obj = db1.T_SessionEventss.Find(Convert.ToInt64(id));
            return _Obj;
        }
		public string GetRecordById_Reflection(string id)
        {
							ApplicationContext db1 = new ApplicationContext(new SystemUser());
			if (string.IsNullOrEmpty(id)) return "";
			var _Obj = db1.T_SessionEventss.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_SessionEvents>(_Obj, "T_SessionEvents", new string[] { ""  }); ;
			        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
		            IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
			        }
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_SessionEventss;
					string lambda = "";
                    foreach (var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambda += asso + "=" + AssociationID + " OR ";
                    }
                    lambda = lambda.TrimEnd(" OR ".ToCharArray());
                    query = query.Where(lambda);

                   //Type[] exprArgTypes = { query.ElementType };
                   // string propToWhere = AssoNameWithParent;
                   // ParameterExpression p = Expression.Parameter(typeof(T_SessionEvents), "p");
                   // MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                   // LambdaExpression lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                   // MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                   // IQueryable q = query.Provider.CreateQuery(methodCall);
                   //list = ((IQueryable<T_SessionEvents>)q);
				   list = ((IQueryable<T_SessionEvents>)query);
                }
				else if (AssoID == 0)
                {
                   IQueryable query = db.T_SessionEventss;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_SessionEvents), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_SessionEvents>)q);

                }
            }
			FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_SessionEvents>(User,list, "T_SessionEvents",caller);
		   if (key != null && key.Length > 0)
            {
			    if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p=>p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(q => q.DisplayValue).Take(10).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
               if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                     var data = from x in list.Where(p=>p.Id != val).Take(9).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }                
                else
                {
                    var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
        }
		
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_SessionEventss;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_SessionEvents), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_SessionEvents>)q);
                }
            }
			var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
        }
[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
				try
                {
					Nullable<long> AssoID = Convert.ToInt64(AssociationID);
					if (AssoID != null && AssoID > 0)
					{
						IQueryable query = db.T_SessionEventss;
						Type[] exprArgTypes = { query.ElementType };
						string propToWhere = AssoNameWithParent;
						ParameterExpression p = Expression.Parameter(typeof(T_SessionEvents), "p");
						MemberExpression member = Expression.PropertyOrField(p, propToWhere);
						LambdaExpression lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
						MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
						IQueryable q = query.Provider.CreateQuery(methodCall);
						list = ((IQueryable<T_SessionEvents>)q).Take(20);
					}
				}
				catch
                {
                    var data = from x in list.OrderBy(q => q.DisplayValue).Take(20).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
		   FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
           list = _fad.FilterDropdown<T_SessionEvents>(User,list, "T_SessionEvents",caller);
		   if (key != null && key.Length > 0)
            {
			    if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(20).Union(list.Where(p=>p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(q => q.DisplayValue).Take(20).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
               if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                     var data = from x in list.Where(p=>p.Id != val).Take(20).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }                
                else
                {
                    var data = from x in list.OrderBy(q => q.DisplayValue).Take(20).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllMultiSelectValueForBR(string propNameBR)
        {
            IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            if (!string.IsNullOrEmpty(propNameBR))
            {
				//added new code (Remove old code if everything works)
				var result = list.Select("new(Id," + propNameBR + " as value)");
                return Json(result, JsonRequestBehavior.AllowGet);
				//

                ParameterExpression param = Expression.Parameter(typeof(T_SessionEvents), "d");
                //bulid expression tree:data.Field1
                var Property = typeof(T_SessionEvents).GetProperty(propNameBR);
                Expression selector = Expression.Property(param, Property);
                Expression pred = Expression.Lambda(selector, param);
                //bulid expression tree:Select(d=>d.Field1)

                var targetType = Property.PropertyType;
                if (targetType.GetGenericArguments().Count() > 0)
                {
                    if (targetType.GetGenericArguments()[0].Name == "DateTime")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select", 
                        new Type[] { typeof(T_SessionEvents), typeof(DateTime?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<DateTime?> query = list.Provider.CreateQuery<DateTime?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                }
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (targetType.GetGenericArguments()[0].Name == "Decimal")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEvents), typeof(decimal?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<decimal?> query = list.Provider.CreateQuery<decimal?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Boolean")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEvents), typeof(bool?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<bool?> query = list.Provider.CreateQuery<bool?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Int32")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEvents), typeof(Int32?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<Int32?> query = list.Provider.CreateQuery<Int32?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Int64")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEvents), typeof(Int64?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<Int64?> query = list.Provider.CreateQuery<Int64?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Double")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEvents), typeof(double?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<double?> query = list.Provider.CreateQuery<double?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                            new Type[] { typeof(T_SessionEvents), typeof(string) }, Expression.Constant(list.AsQueryable()), pred);
                    //var result = query.AsEnumerable().Take(10);
                    IQueryable<string> query = list.Provider.CreateQuery<string>(expr).Distinct();
                    return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                }
                return Json(list.AsEnumerable(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                           select new { Id = x.Id, Name = x.DisplayValue };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllMultiSelectValue(string key, string AssoNameWithParent, string AssociationID)
        {
			IQueryable<T_SessionEvents> list = db.T_SessionEventss;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.T_SessionEventss;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(T_SessionEvents), "p"));
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
                list = ((IQueryable<T_SessionEvents>)q);
            }
			if (key != null && key.Length > 0)
            {
			   var data = from x in list.Where(p=>p.DisplayValue.Contains(key)).OrderBy(q=>q.DisplayValue).Take(10).ToList()
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
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_SessionEvents")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_SessionEvents").ToList();
            var distinctMapping = lstMappings.GroupBy(p => p.MappingName).Distinct();
            List<ImportConfiguration> ddlMappingList = new List<ImportConfiguration>();
            foreach (var elem in distinctMapping)
            {
                ddlMappingList.Add(elem.FirstOrDefault());
            }
            var DefaultMapping = lstMappings.Where(p => p.IsDefaultMapping).FirstOrDefault();
            var mappingID = DefaultMapping == null ? "" : DefaultMapping.MappingName;
            ViewBag.IsDefaultMapping = DefaultMapping != null ? true : false;
            //ViewBag.ListOfMappings = new SelectList(ddlMappingList, "ID", "MappingName", mappingID);
			ViewBag.ListOfMappings = new SelectList(ddlMappingList, "MappingName", "MappingName", mappingID);
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
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_SessionEvents");
                    if (entList != null)
                    {
                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                        {
                            long selectedVal = 0;
                            var colSelected = col.FirstOrDefault(p=> p.Text.Trim().ToLower() == prop.DisplayName.Trim().ToLower());
                            if (colSelected != null)
                                selectedVal = long.Parse(colSelected.Value);
                            objColMap.Add(prop, new SelectList(col,"Value", "Text", selectedVal));
                        }
						List<GeneratorBase.MVC.ModelReflector.Association> assocList = entList.Associations;
                        if (assocList != null)
                        {
                            foreach (var assoc in assocList)
                            {
								if(assoc.Target == "IdentityUser")
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
                        typeName = "T_SessionEvents";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
						string idMapping = collection["ListOfMappings"];
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_SessionEvents" && p.IsDefaultMapping);
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
                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Session Eventss";
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
                                										 case "T_SessionEventsLearningCenterID":
										 var t_sessioneventslearningcenterId = db.T_LearningCenters.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (t_sessioneventslearningcenterId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ScheduleID":
										 var scheduleId = db.T_Schedules.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (scheduleId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "T_SessionEventsTimeSlotsID":
										 var t_sessioneventstimeslotsId = db.T_TimeSlotss.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (t_sessioneventstimeslotsId == null)
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
			string typename = "T_SessionEvents";
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
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Session Eventss";
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_SessionEvents");
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
                                										 case "T_SessionEventsLearningCenterID":
										 var strPropertyT_SessionEventsLearningCenter = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsLearningCenterID").Value;
										 ModelReflector.Property propT_SessionEventsLearningCenter = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_LearningCenter").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEventsLearningCenter);
										 //var elementTypeT_SessionEventsLearningCenter = db.T_LearningCenters.ElementType;
										 //var propertyInfoT_SessionEventsLearningCenter = elementTypeT_SessionEventsLearningCenter.GetProperty(propT_SessionEventsLearningCenter.Name);
										 // var t_sessioneventslearningcenterId = db.T_LearningCenters.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEventsLearningCenter.GetValue(p, null)) == assovalue);
										 var t_sessioneventslearningcenterId = db.T_LearningCenters.Where(propT_SessionEventsLearningCenter.Name+"=\""+assovalue+"\"").FirstOrDefault();
										 if (t_sessioneventslearningcenterId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ScheduleID":
										 var strPropertySchedule = lstEntityProp.FirstOrDefault(p => p.Key == "ScheduleID").Value;
										 ModelReflector.Property propSchedule = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Schedule").Properties.FirstOrDefault(p => p.DisplayName == strPropertySchedule);
										 //var elementTypeSchedule = db.T_Schedules.ElementType;
										 //var propertyInfoSchedule = elementTypeSchedule.GetProperty(propSchedule.Name);
										 // var scheduleId = db.T_Schedules.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoSchedule.GetValue(p, null)) == assovalue);
										 var scheduleId = db.T_Schedules.Where(propSchedule.Name+"=\""+assovalue+"\"").FirstOrDefault();
										 if (scheduleId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "T_SessionEventsTimeSlotsID":
										 var strPropertyT_SessionEventsTimeSlots = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsTimeSlotsID").Value;
										 ModelReflector.Property propT_SessionEventsTimeSlots = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_TimeSlots").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEventsTimeSlots);
										 //var elementTypeT_SessionEventsTimeSlots = db.T_TimeSlotss.ElementType;
										 //var propertyInfoT_SessionEventsTimeSlots = elementTypeT_SessionEventsTimeSlots.GetProperty(propT_SessionEventsTimeSlots.Name);
										 // var t_sessioneventstimeslotsId = db.T_TimeSlotss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEventsTimeSlots.GetValue(p, null)) == assovalue);
										 var t_sessioneventstimeslotsId = db.T_TimeSlotss.Where(propT_SessionEventsTimeSlots.Name+"=\""+assovalue+"\"").FirstOrDefault();
										 if (t_sessioneventstimeslotsId == null)
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
                if (ViewBag.ConfirmImportData != null){
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
                        T_SessionEvents model = new T_SessionEvents();
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
    case "T_Title":
    model.T_Title = columnValue;
	break;
    case "T_EventDate":
    model.T_EventDate = DateTime.Parse(columnValue);
	break;
    case "T_StartTime":
    model.T_StartTime = DateTime.Parse(columnValue);
	break;
    case "T_EndTime":
    model.T_EndTime = DateTime.Parse(columnValue);
	break;
    case "T_IsCancelled":
    model.T_IsCancelled = Boolean.Parse(columnValue);
	break;
    case "T_Description":
    model.T_Description = columnValue;
	break;
					 case "T_SessionEventsLearningCenterID":
		 dynamic t_sessioneventslearningcenterId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertyT_SessionEventsLearningCenter = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsLearningCenterID").Value;
			 ModelReflector.Property propT_SessionEventsLearningCenter = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_LearningCenter").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEventsLearningCenter);
			 //var elementTypeT_SessionEventsLearningCenter = db.T_LearningCenters.ElementType;
			 //var propertyInfoT_SessionEventsLearningCenter = elementTypeT_SessionEventsLearningCenter.GetProperty(propT_SessionEventsLearningCenter.Name);			 
			 //t_sessioneventslearningcenterId = db.T_LearningCenters.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEventsLearningCenter.GetValue(p, null)) == columnValue);
			 t_sessioneventslearningcenterId = db.T_LearningCenters.Where(propT_SessionEventsLearningCenter.Name + "=\"" + columnValue + "\"").FirstOrDefault();
		 }
		 else
			t_sessioneventslearningcenterId = db.T_LearningCenters.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (t_sessioneventslearningcenterId != null)
			model.T_SessionEventsLearningCenterID = t_sessioneventslearningcenterId.Id;
		  else
			{
				if ((collection["T_SessionEventsLearningCenterID"].ToString() == "true,false"))
				{
					try
					{
						T_LearningCenter objT_LearningCenter = new T_LearningCenter();
						objT_LearningCenter.T_Name  = (columnValue);
						db.T_LearningCenters.Add(objT_LearningCenter);
						db.SaveChanges();
						model.T_SessionEventsLearningCenterID = objT_LearningCenter.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "ScheduleID":
		 dynamic scheduleId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertySchedule = lstEntityProp.FirstOrDefault(p => p.Key == "ScheduleID").Value;
			 ModelReflector.Property propSchedule = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Schedule").Properties.FirstOrDefault(p => p.DisplayName == strPropertySchedule);
			 //var elementTypeSchedule = db.T_Schedules.ElementType;
			 //var propertyInfoSchedule = elementTypeSchedule.GetProperty(propSchedule.Name);			 
			 //scheduleId = db.T_Schedules.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoSchedule.GetValue(p, null)) == columnValue);
			 scheduleId = db.T_Schedules.Where(propSchedule.Name + "=\"" + columnValue + "\"").FirstOrDefault();
		 }
		 else
			scheduleId = db.T_Schedules.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (scheduleId != null)
			model.ScheduleID = scheduleId.Id;
		  else
			{
				if ((collection["ScheduleID"].ToString() == "true,false"))
				{
					try
					{
						T_Schedule objT_Schedule = new T_Schedule();
						objT_Schedule.T_Name  = (columnValue);
				 try { objT_Schedule.T_Description =(columnValue); }
                 catch { objT_Schedule.T_Description = default(string); }
						db.T_Schedules.Add(objT_Schedule);
						db.SaveChanges();
						model.ScheduleID = objT_Schedule.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "T_SessionEventsTimeSlotsID":
		 dynamic t_sessioneventstimeslotsId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertyT_SessionEventsTimeSlots = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsTimeSlotsID").Value;
			 ModelReflector.Property propT_SessionEventsTimeSlots = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_TimeSlots").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEventsTimeSlots);
			 //var elementTypeT_SessionEventsTimeSlots = db.T_TimeSlotss.ElementType;
			 //var propertyInfoT_SessionEventsTimeSlots = elementTypeT_SessionEventsTimeSlots.GetProperty(propT_SessionEventsTimeSlots.Name);			 
			 //t_sessioneventstimeslotsId = db.T_TimeSlotss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEventsTimeSlots.GetValue(p, null)) == columnValue);
			 t_sessioneventstimeslotsId = db.T_TimeSlotss.Where(propT_SessionEventsTimeSlots.Name + "=\"" + columnValue + "\"").FirstOrDefault();
		 }
		 else
			t_sessioneventstimeslotsId = db.T_TimeSlotss.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (t_sessioneventstimeslotsId != null)
			model.T_SessionEventsTimeSlotsID = t_sessioneventstimeslotsId.Id;
         break;
            default:
                break;
        }
    }
					    if (ValidateModel(model) && string.IsNullOrEmpty(CheckBeforeSave(model)))
                        {
							var result = CheckMandatoryProperties(model);
                            if (result == null || result.Count == 0)
                            {
								db.T_SessionEventss.Add(model);
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
		public bool ValidateModel(T_SessionEvents validate)
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
                for (int i = 0; i < sheetColumns.Count(); i++ )
                {
                    if (string.IsNullOrEmpty(sheetColumns[i]))
                        continue;
                    if (dr[ Convert.ToInt32(sheetColumns[i]) - 1] != null && dr[ Convert.ToInt32(sheetColumns[i]) - 1].ToString() != "")
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
                var obj1 = db1.T_SessionEventss.Find(Id);
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                return PropValue;
				            }
            catch { return null; }
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReadOnlyProperties(T_SessionEvents OModel)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEvents").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_SessionEvents");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 4);
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                        var listArgs = Args;
                        foreach (var parametersArgs in Args)
                        {
                            var dispName = "";
                            var paramName = parametersArgs.ParameterName;
                            var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents");
                            var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                            if (property != null)
                                dispName = property.DisplayName;
                            else
                            {
                                if (paramName.Contains("."))
                                {
                                    var arrparamName = paramName.Split('.');
                                    var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);

                                    var targetPropName = ModelReflector.Entities.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                    paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                    dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                                }
                            }
                            if (!RulesApplied.ContainsKey(paramName))
                            {
                                RulesApplied.Add(paramName, dispName);
                                var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
								if (!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
									RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                            }
                        }
                    }
                    if (BRFail != null && BRFail.Count() > 0)
                    {
                        foreach (var objBR in BRFail)
                        {
							if (!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
								RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                        }
                    }
                }
            }
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(T_SessionEvents OModel, string ruleType)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEvents").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    if (ruleType == "OnCreate")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                    else if (ruleType == "OnEdit")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_SessionEvents");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();

                    var ruleActions = new RuleActionContext().RuleActions.Where(p => p.AssociatedActionTypeID == 2).Select(p => p.RuleActionID).ToList();
                    var BRFail = BRAll.Except(BR);
                    BRFail = BRFail.Where(p => ruleActions.Contains(p.Id)).ToList();

                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                        var listArgs = Args;
                        foreach (var parametersArgs in Args)
                        {
							var dispName = "";
                            var paramName = parametersArgs.ParameterName;
                            var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents");
                            var property = entity.Properties.FirstOrDefault(p => p.Name == paramName);
                            if (property != null)
                                dispName = property.DisplayName;
                            else
                            {
                                if (paramName.Contains("."))
                                {
                                    var arrparamName = paramName.Split('.');
                                    var assocName = entity.Associations.FirstOrDefault(p => p.AssociationProperty == arrparamName[0]);

                                    var targetPropName = ModelReflector.Entities.FirstOrDefault(p => p.Name == assocName.Target).Properties.FirstOrDefault(q => q.Name == arrparamName[1]);
                                    paramName = arrparamName[0].Replace("ID", "").ToLower().Trim() + "_" + arrparamName[1];
                                    dispName = assocName.DisplayName + "." + targetPropName.DisplayName;
                                }
                            }
							if (!RequiredProperties.ContainsKey(paramName))
                            {
                                RequiredProperties.Add(paramName, dispName);
                                var objBR = BR.Find(p => p.Id == parametersArgs.actionarguments.RuleActionID);
								if (!RequiredProperties.ContainsKey("FailureMessage-" + objBR.Id))
									RequiredProperties.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                            }
                        }
                    }
                    if (BRFail != null && BRFail.Count() > 0)
                    {
                        foreach (var objBR in BRFail)
                        {
							if (!RequiredProperties.ContainsKey("InformationMessage-" + objBR.Id))
								RequiredProperties.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                        }
                    }
                }
            }
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetValidateBeforeSaveProperties(T_SessionEvents OModel, string ruleType)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            var conditions = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEvents").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ValidateBeforeSavePropertiesRule(OModel, BR, "T_SessionEvents");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    var BRFail = BRAll.Except(BR).Where(p => p.AssociatedBusinessRuleTypeID == 10);
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(10))
                    {
                        foreach (var rules in ResultOfBusinessRules)
                        {
                            if (rules.Key.TypeNo == 10)
                            {
                                var ruleconditionsdb = new ConditionContext().Conditions.Where(p => p.RuleConditionsID == rules.Value.BRID);
                                foreach (var condition in ruleconditionsdb)
                                {
                                    string conditionPropertyName = condition.PropertyName;
                                    var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents");
                                    var PropInfo = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName);
                                    var AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == conditionPropertyName);
                                    var propDispName = "";
                                    if (conditionPropertyName.StartsWith("[") && conditionPropertyName.EndsWith("]"))
                                    {
                                        conditionPropertyName = conditionPropertyName.TrimStart('[').TrimEnd(']').Trim();
                                        if (conditionPropertyName.Contains("."))
                                        {
                                            var targetProperties = conditionPropertyName.Split(".".ToCharArray());
                                            if (targetProperties.Length > 1)
                                            {
                                                AssociationInfo = Entity.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                                if (AssociationInfo != null)
                                                {
                                                    var EntityInfo1 = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                                    conditionPropertyName = EntityInfo1.Properties.FirstOrDefault(p => p.Name == targetProperties[1]).DisplayName;
                                                }
                                            }
                                            propDispName = AssociationInfo.DisplayName + "." + conditionPropertyName;
                                        }
                                    }
                                    else
                                    {
                                        propDispName = Entity.Properties.FirstOrDefault(p => p.Name == conditionPropertyName).DisplayName;
                                    }
                                    conditions += propDispName + " " + condition.Operator + " " + condition.Value + ", ";
                                }   
                            }
                            RulesApplied.Add("Business Rule #" + rules.Value.BRID + " applied : ", conditions.Trim().TrimEnd(','));
                            var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                            foreach (var objBR in BRList)
                            {
								if (!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
									RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                            }
                        }
                    }
                    if (BRFail != null && BRFail.Count() > 0)
                    {
                        foreach (var objBR in BRFail)
                        {
							if (!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
								RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                        }
                    }
                }
            }
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(T_SessionEvents OModel)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            //string RulesApplied = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEvents").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "T_SessionEvents");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();

                    var BRFail = BRAll.Except(BR).Where(p=>p.AssociatedBusinessRuleTypeID==2);
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1)||ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
                    {
                        var BRList = BR.Where(q => ResultOfBusinessRules.Values.Select(p => p.BRID).Contains(q.Id));
                        foreach(var objBR in BRList)
                        {
                            RulesApplied.Add("Rule #" + objBR.Id + " is Applied", objBR.RuleName);
							if (!RulesApplied.ContainsKey("FailureMessage-" + objBR.Id))
								RulesApplied.Add("FailureMessage-" + objBR.Id, objBR.FailureMessage);
                        }
                    }
                    if (BRFail != null && BRFail.Count() > 0)
                    {
                        foreach (var objBR in BRFail)
                        {
							if (!RulesApplied.ContainsKey("InformationMessage-" + objBR.Id))
								RulesApplied.Add("InformationMessage-" + objBR.Id, objBR.InformationMessage);
                        }
                    }
                }
            }
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
		private List<string> CheckMandatoryProperties(T_SessionEvents OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEvents").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_SessionEvents");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(2))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var type = OModel.GetType();
                            if (type.GetProperty(paramName) == null) continue;
                            var propertyvalue = type.GetProperty(paramName).GetValue(OModel, null);
                            if (propertyvalue == null)
                            {
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
            var _Obj = db1.T_SessionEventss.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if (_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
				public long? GetIdFromPropertyValue(string PropName, string PropValue)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            IQueryable query = db1.T_SessionEventss;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(T_SessionEvents), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if (PropValue.ToLower().Trim() != "null")
            lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
            else
                lambda = Expression.Lambda<Func<T_SessionEvents, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<T_SessionEvents>)q);
            if (list1 != null && list1.Count() > 0)
                return Int64.TryParse(list1.FirstOrDefault().Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
				[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
        {
								ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var obj1 = db1.T_SessionEventss.Find(Id);
			 if (obj1 == null)
                return Json("0", JsonRequestBehavior.AllowGet);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            PropValue = PropValue == null ? 0 : PropValue;
			return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
			           
        }
		public string checkHidden(string entityName, string brType, bool isHiddenGroup)
        {
            System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
            System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
            RuleActionContext objRuleAction = new RuleActionContext();
            ConditionContext objCondition = new ConditionContext();
            ActionArgsContext objActionArgs = new ActionArgsContext();
            var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
				string[] rbList = null;
            if (businessRules != null && businessRules.Count > 0)
            {
                foreach (BusinessRule objBR in businessRules)
                {
                    long ActionTypeId = 6;
					if (isHiddenGroup)
                        ActionTypeId = 12;
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
                                    
                                    if (datatype == "Association")
                                    {
                                        var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                        chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                        if (operand.Length > 2)
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                    else
                                    {
										chkHidden.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + ".change(function() { " + fnCondition + "; });");
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
                                            //change for inline association
                                            if (isHiddenGroup)
												fnProp += "$('#dvGroup" + objaa.ParameterName.Remove(objaa.ParameterName.IndexOf('|')) + "').css('display', type);";
                                            else
                                                fnProp += "$('#dv" + objaa.ParameterName.Replace('.', '_') + "').css('display', type);";
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
		public string checkSetValueUIRule(string entityName, string brType)
        {
            System.Text.StringBuilder chkHidden = new System.Text.StringBuilder();
            System.Text.StringBuilder chkFnHidden = new System.Text.StringBuilder();
            RuleActionContext objRuleAction = new RuleActionContext();
            ConditionContext objCondition = new ConditionContext();
            ActionArgsContext objActionArgs = new ActionArgsContext();
            var businessRules = User.businessrules.Where(p => p.EntityName == entityName).ToList();
            string[] rbList = null;
            if (businessRules != null && businessRules.Count > 0)
            {
                foreach (BusinessRule objBR in businessRules)
                {
                    long ActionTypeId = 7;
                    var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                    if (objRuleActionList.Count() > 0)
                    {
                        if (objBR.AssociatedBusinessRuleTypeID != 6)
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
                                        fnCondition = "setvalueUIRule" + objCon.Id.ToString() + "()";
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
                                    if (datatype == "Association")
                                    {
                                        var propertyName = objCon.PropertyName.Replace('[', ' ').Remove(objCon.PropertyName.IndexOf('.')).Trim();
                                        chkHidden.Append((rbcheck ? " $('input:radio[name=" + propertyName + "]')" : " $('#" + propertyName + "')") + ".change(function() { " + fnCondition + "; });");
                                        if (operand.Length > 2)
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase().indexOf('" + condValue + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + propertyName + "') ") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= " + propertyName + "]:checked').next('span:first')" : " && ($('option:selected', '#" + propertyName + "')") + ".text().toLowerCase() " + operand + " '" + condValue + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                    else
                                    {
										chkHidden.Append((rbcheck ? " $('input:radio[name=" + objCon.PropertyName + "]')" : " $('#" + objCon.PropertyName + "')") + ".change(function() { " + fnCondition + "; });");
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
                                            string paramValue = objaa.ParameterValue;
											string paramType = checkPropType(entityName, objaa.ParameterName);
                                            if (paramValue.ToLower().Trim().Contains("today"))
                                            {
                                                paramValue = ApplyRule.EvaluateDateForActionInTarget(paramValue);
                                                fnProp += "$('#" + objaa.ParameterName + "').val('" + paramValue + "');";
                                            }
                                            else if (paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                                            {
                                                paramValue = paramValue.TrimStart('[').TrimEnd(']').Trim();
                                                paramValue = "$('#" + paramValue + "').val()";
                                                fnProp += "$('#" + objaa.ParameterName + "').val(" + paramValue + ");";
                                            }
                                            else
												 if (paramType == "Association")
                                                {
                                                    fnProp += "$('#" + objaa.ParameterName + "').trigger('chosen:open');";
                                                    fnProp += "$('#" + objaa.ParameterName + " option').map(function () { if ($(this).text() == '" + paramValue + "') return this; }).attr('selected', 'selected');";
                                                    fnProp += "$('#" + objaa.ParameterName + "').trigger('chosen:updated');";
                                                    fnProp += "$('#" + objaa.ParameterName + "').trigger('click.chosen');";
                                                }
                                                else
													fnProp += "$('#" + objaa.ParameterName + "').val('" + paramValue + "');";

                                            fn += objaa.Id.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(fn))
                                            fnName = "setvalueUIRuleProp" + fn;
                                        if (!string.IsNullOrEmpty(fnName))
                                        {
                                            chkHidden.Append("function " + fnCondition + " { if ( " + fnConditionValue + " ) { " + fnProp + " } }");
                                            //chkFnHidden.Append("function " + fnName + "(value) { " + fnProp + " }");
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
            if (PropName.StartsWith("[") && PropName.EndsWith("]"))
            {
                PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (PropName.Contains("."))
                {
                    var targetProperties = PropName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                        }
                    }
                }
            }
            string DataType = PropInfo.DataType;
            if (AssociationInfo != null)
            {
                DataType = "Association";
            }
            return DataType;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Check1MThresholdValue(T_SessionEvents t_sessionevents)
        {
            Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
            return Json(msgThreshold, JsonRequestBehavior.AllowGet);
        }
		public bool CheckBeforeDelete(T_SessionEvents t_sessionevents)
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
		public string CheckBeforeSave(T_SessionEvents t_sessionevents)
        {
			var AlertMsg = "";
            // Write your logic here
 
			   //Make sure to assign AlertMsg with proper message
			   	
			   //AlertMsg = "Validation Alert - Before Save !! Information not saved.";
               //ModelState.AddModelError("CustomError", AlertMsg);
			   //ViewBag.ApplicationError = AlertMsg;
			
            return AlertMsg;
        }
		public void OnDeleting(T_SessionEvents t_sessionevents)
        {
            // Write your logic here
 
		}
        public void OnSaving(T_SessionEvents t_sessionevents, ApplicationContext db)
        {
			if (t_sessionevents.Id == 0)
            {
                var scheduleid = t_sessionevents.ScheduleID;
                var parent = db.T_Sessions.FirstOrDefault(p => p.ScheduleSessionID == scheduleid);
                t_sessionevents.T_SessionEventsLearningCenterID = parent.T_SessionLearningCenterAssociationID;
            }
			if (t_sessionevents.Id == 0)
            {
                var scheduleid = t_sessionevents.ScheduleID;
                var parent = db.T_Sessions.FirstOrDefault(p => p.ScheduleSessionID == scheduleid);
                t_sessionevents.T_SessionEventsTimeSlotsID = parent.T_SessionTimeSlotAssociatonID;
            }
            // Write your logic here
 
        }
		public void AfterSave(T_SessionEvents t_sessionevents)
        {
			
			

            // Write your logic here
 
		}
		//code for verb action security
        public string[] getVerbsName()
        {
            string[] Verbsarr = new string[] { "BulkUpdate","BulkDelete"   };
            return Verbsarr;
        }
        //
		//code for list of groups
        public string[][] getGroupsName()
        {
            string[][] groupsarr = new string[][] {  };
            return groupsarr;
        }
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCalculationValues(T_SessionEvents t_sessionevents)
        {
            t_sessionevents.setCalculation();
            Dictionary<string, string> Calculations = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_sessionevents.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		//get Templates 
		// select templates 
        public void GetTemplatesForList(string defaultview)
        {
            string pageNameList = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.ListEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageNameList = defaultEntityPage.ToString();
            }
             
            if (!String.IsNullOrEmpty(pageNameList) && IsInRoles)
                ViewBag.TemplatesName = pageNameList;
            else
                ViewBag.TemplatesName = defaultview;

        }
        public void GetTemplatesForDetails(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.ViewEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;

        }
        public void GetTemplatesForEdit(string defaultview)
        {
            string pageEdit = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.EditEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageEdit = defaultEntityPage.ToString();
            }
           
            if (!String.IsNullOrEmpty(pageEdit) && IsInRoles)
                ViewBag.TemplatesName = pageEdit;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForCreateQuick(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.CreateEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
            
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForCreate(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.CreateEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForEditQuick(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.EditEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
          
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForCreateWizard(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.CreateWizardEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
            
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForEditWizard(string defaultview)
        {
            string pageDetails = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.EditWizardEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageDetails = defaultEntityPage.ToString();
            }
            
            if (!String.IsNullOrEmpty(pageDetails) && IsInRoles)
                ViewBag.TemplatesName = pageDetails;
            else
                ViewBag.TemplatesName = defaultview;
        }
        public void GetTemplatesForSearch()
        {
            string pageSearch = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEvents"
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
                var defaultEntityPage = lstDefaultEntityPage.Select(p => p.SearchEntityPage).FirstOrDefault();
                if (defaultEntityPage != null)
                    pageSearch = defaultEntityPage.ToString();
            }

            if (!String.IsNullOrEmpty(pageSearch) && IsInRoles)
                ViewBag.TemplatesName = pageSearch;
            else
                ViewBag.TemplatesName = "SetFSearch";
        }
				//
	[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDerivedDetails(T_SessionEvents t_sessionevents, string IgnoreEditable)
        {
            Dictionary<string, string> derivedlist = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_sessionevents.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDerivedDetailsInline(string host, string value, T_SessionEvents t_sessionevents, string IgnoreEditable)
        {
            Dictionary<string, string> derivedlist = new Dictionary<string, string>();
            return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

		[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getInlineAssociationsOfEntity()
        {
            List<string> list = new List<string> {  };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
	public ActionResult Download(string FileName)
        {
            string filename = FileName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };
            return File(filedata, "application/force-download", Path.GetFileName(FileName));
        }
    }
}


