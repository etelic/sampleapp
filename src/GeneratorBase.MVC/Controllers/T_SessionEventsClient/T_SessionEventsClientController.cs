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
    public partial class T_SessionEventsClientController : BaseController
    {
		private void LoadViewDataForCount(T_SessionEventsClient t_sessioneventsclient, string AssocType)
        {
        }
		private void LoadViewDataAfterOnEdit(T_SessionEventsClient t_sessioneventsclient)
        {
					 ViewBag.T_ClientID = new SelectList(db.T_Clients, "ID", "DisplayValue", t_sessioneventsclient.T_ClientID);
			 ViewBag.T_SessionEventsID = new SelectList(db.T_SessionEventss, "ID", "DisplayValue", t_sessioneventsclient.T_SessionEventsID);
        }
        private void LoadViewDataBeforeOnEdit(T_SessionEventsClient t_sessioneventsclient)
        {
		
               var _objT_Client = new List<T_Client>();
               _objT_Client.Add(t_sessioneventsclient.t_client);
			   			   ViewBag.T_ClientID = new SelectList(_objT_Client, "ID", "DisplayValue", t_sessioneventsclient.T_ClientID);
			               var _objT_SessionEvents = new List<T_SessionEvents>();
               _objT_SessionEvents.Add(t_sessioneventsclient.t_sessionevents);
			   			   ViewBag.T_SessionEventsID = new SelectList(_objT_SessionEvents, "ID", "DisplayValue", t_sessioneventsclient.T_SessionEventsID);
			        }
        private void LoadViewDataAfterOnCreate(T_SessionEventsClient t_sessioneventsclient)
        {
			 ViewBag.T_ClientID = new SelectList(db.T_Clients, "ID", "DisplayValue", t_sessioneventsclient.T_ClientID);
			 ViewBag.T_SessionEventsID = new SelectList(db.T_SessionEventss, "ID", "DisplayValue", t_sessioneventsclient.T_SessionEventsID);
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
	
			if(HostingEntityName == "T_Client" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_SessionEventsClient_T_Client")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.T_ClientID = new SelectList(db.T_Clients.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_Clients.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
								//Manytomany associations
								//var T_SessionClientAssociationlist = db.T_Clients.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
								//ViewBag.t_sessionclientassociation = new SelectList(T_SessionClientAssociationlist, "ID", "DisplayValue");
								//Manytomany associations
								//var T_SessionEventsClientlist = db.T_Clients.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
								//ViewBag.t_sessioneventsclient = new SelectList(T_SessionEventsClientlist, "ID", "DisplayValue");

				long hostid = Convert.ToInt64(HostingEntityID);
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_SessionEventsID == hostid).Select(q => q.T_ClientID);
                //var AvailableList = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue);
		var AvailableList = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue);
                ViewBag.IsTop50 = "none";
				if (AvailableList.Count() > 50)
                    ViewBag.IsTop50 = "block";
				ViewBag.T_ClientIDAvailable  = new SelectList(AvailableList.Take(50).ToList(), "ID", "DisplayValue");
                ViewBag.T_ClientIDSelected = new MultiSelectList(db.T_Clients.Where(p => Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue).ToList(), "ID", "DisplayValue", Selectedlist);
		    }
			if(HostingEntityName == "T_SessionEvents" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_SessionEventsClient_T_SessionEvents")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.T_SessionEventsID = new SelectList(db.T_SessionEventss.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_SessionEventss.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
								//Manytomany associations
								//var T_SessionEventsClientlist = db.T_SessionEventss.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
								//ViewBag.t_sessioneventsclient = new SelectList(T_SessionEventsClientlist, "ID", "DisplayValue");
						//ManytoOne association
						var T_SessionEventsLearningCenterlist = db.T_LearningCenters.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
						ViewBag.t_sessioneventslearningcenter = new SelectList(T_SessionEventsLearningCenterlist, "ID", "DisplayValue");
						//ManytoOne association
						var Schedulelist = db.T_Schedules.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
						ViewBag.schedule = new SelectList(Schedulelist, "ID", "DisplayValue");
						//ManytoOne association
						var T_SessionEventsTimeSlotslist = db.T_TimeSlotss.OrderBy(p => p.DisplayValue).Take(10).Distinct().ToList();
						ViewBag.t_sessioneventstimeslots = new SelectList(T_SessionEventsTimeSlotslist, "ID", "DisplayValue");

				long hostid = Convert.ToInt64(HostingEntityID);
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_ClientID == hostid).Select(q => q.T_SessionEventsID);
                //var AvailableList = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue);
		var AvailableList = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue);
                ViewBag.IsTop50 = "none";
				if (AvailableList.Count() > 50)
                    ViewBag.IsTop50 = "block";
				ViewBag.T_SessionEventsIDAvailable  = new SelectList(AvailableList.Take(50).ToList(), "ID", "DisplayValue");
                ViewBag.T_SessionEventsIDSelected = new MultiSelectList(db.T_SessionEventss.Where(p => Selectedlist.Contains(p.Id)).OrderBy(p => p.DisplayValue).ToList(), "ID", "DisplayValue", Selectedlist);
		    }
        }
		private IQueryable<T_SessionEventsClient> searchRecords(IQueryable<T_SessionEventsClient> lstT_SessionEventsClient, string searchString, bool? IsDeepSearch)
        {
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_SessionEventsClient = lstT_SessionEventsClient.Where(s => (s.t_client!= null && (s.t_client.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_sessionevents!= null && (s.t_sessionevents.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_SessionEventsClient = lstT_SessionEventsClient.Where(s => (s.t_client!= null && (s.t_client.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_sessionevents!= null && (s.t_sessionevents.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_SessionEventsClient;
        }
		private IQueryable<T_SessionEventsClient> sortRecords(IQueryable<T_SessionEventsClient> lstT_SessionEventsClient, string sortBy, string isAsc)
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

	 if(sortBy == "T_ClientID")
                return isAsc.ToLower() == "asc" ? lstT_SessionEventsClient.OrderBy(p => p.t_client.DisplayValue) : lstT_SessionEventsClient.OrderByDescending(p => p.t_client.DisplayValue);
	 if(sortBy == "T_SessionEventsID")
                return isAsc.ToLower() == "asc" ? lstT_SessionEventsClient.OrderBy(p => p.t_sessionevents.DisplayValue) : lstT_SessionEventsClient.OrderByDescending(p => p.t_sessionevents.DisplayValue);
            ParameterExpression paramExpression = Expression.Parameter(typeof(T_SessionEventsClient), "t_sessioneventsclient");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<T_SessionEventsClient>)lstT_SessionEventsClient.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstT_SessionEventsClient.ElementType, lambda.Body.Type },
                    lstT_SessionEventsClient.Expression,
                    lambda));
        }
		public ActionResult FindFSearch(string id, string sourceEntity)
        {
            return null;
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity ,string t_client,string t_sessionevents, bool? RenderPartial)
        {
			int Qcount = Request.UrlReferrer.Query.Count();
			//For Reports
			 if ((RenderPartial == null ? false : RenderPartial.Value))
                Qcount = Request.QueryString.AllKeys.Count();

			ViewBag.CurrentFilter = searchString;
			if(Qcount > 0)
			{
			var T_Clientlist = db.T_Clients.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_client != null)
            {
                var ids = t_client.Split(",".ToCharArray());
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
					var list = T_Clientlist.Union(db.T_Clients.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_client = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_client = new SelectList(T_Clientlist, "ID", "DisplayValue");
			}
			var T_SessionEventslist = db.T_SessionEventss.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessionevents != null)
            {
                var ids = t_sessionevents.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Session Events= ";

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
							   ViewBag.SearchResult += db.T_SessionEventss.Find(Convert.ToInt64(str)).DisplayValue+", ";
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = T_SessionEventslist.Union(db.T_SessionEventss.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessionevents = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_sessionevents = new SelectList(T_SessionEventslist, "ID", "DisplayValue");
			}
			}
			else
			{
			var objT_Client = new List<T_Client>();
		    ViewBag.t_client = new SelectList(objT_Client, "ID", "DisplayValue");
			var objT_SessionEvents = new List<T_SessionEvents>();
		    ViewBag.t_sessionevents = new SelectList(objT_SessionEvents, "ID", "DisplayValue");
			}
				var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
				if (lstFavoriteItem.Count() > 0)
                    ViewBag.FavoriteItem = lstFavoriteItem;

				ViewBag.EntityName = "T_SessionEventsClient";
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
			columns.Add("2", "Client");
			columns.Add("3", "Session Events");
				ViewBag.HideColumns = new MultiSelectList(columns, "Key", "Value");
				if ((RenderPartial == null ? false : RenderPartial.Value))
					return PartialView("~/Views/T_SessionEventsClient/CustomReportSearch.cshtml", new T_SessionEventsClient());
				return View(new T_SessionEventsClient());
            }

		public string conditionFSearch(string property, string operators, string value)
        {
            string expression = string.Empty;
            var PrpertyType = GetDataTypeOfProperty("T_SessionEventsClient", property);
            var dataType = PrpertyType[0];
            property = PrpertyType[1];
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var ValueType = GetDataTypeOfProperty("T_SessionEventsClient", value, true);
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





		// GET: /T_SessionEventsClient/FSearch/
		public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport ,string t_client,string t_sessionevents , string FilterCondition, string HostingEntity, string AssociatedType,string HostingEntityID, string viewtype, string SortOrder, string HideColumns, string GroupByColumn,bool? IsReports)
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
				 var lstT_SessionEventsClient  = from s in db.T_SessionEventsClients
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_SessionEventsClient  = searchRecords(lstT_SessionEventsClient, searchString.ToUpper(),true);
            }
			  if(!string.IsNullOrEmpty(search))
                	search=search.Replace("?IsAddPop=true", "");
			if(!string.IsNullOrEmpty(search)){
				ViewBag.SearchResult += "\r\n General Criterial= "+search;
				lstT_SessionEventsClient = searchRecords(lstT_SessionEventsClient, search,true);
			}
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_SessionEventsClient  = sortRecords(lstT_SessionEventsClient, sortBy, isAsc);
            }
            else   lstT_SessionEventsClient  = lstT_SessionEventsClient.OrderByDescending(c => c.Id);
			lstT_SessionEventsClient = lstT_SessionEventsClient.Include(t=>t.t_client).Include(t=>t.t_sessionevents);
			lstT_SessionEventsClient = Sorting.Sort<T_SessionEventsClient>(lstT_SessionEventsClient, SortOrder);

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
                    ViewBag.SearchResult += " " + GetPropertyDP("T_SessionEventsClient", PropertyName) + " " + Operator + " " + Value + " " + LogicalConnector;
                    iCnt++;
                }
                if (!string.IsNullOrEmpty(whereCondition.ToString()))
                    lstT_SessionEventsClient = lstT_SessionEventsClient.Where(whereCondition.ToString());

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
                    var asso = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient").Associations.FirstOrDefault(p => p.AssociationProperty == prop);
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
                
                var grp_T_SessionEventsClient = lstT_SessionEventsClient;
                var grp_T_SessionEventsClientList = grp_T_SessionEventsClient.GroupByMany(groupcolumn);
                var objT_SessionEventsClientList = new List<T_SessionEventsClient>();

                foreach (var grp in grp_T_SessionEventsClientList)
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
                                var objT_SessionEventsClient = new T_SessionEventsClient();
                                objT_SessionEventsClient = (T_SessionEventsClient)item1;
                                objT_SessionEventsClient.m_DisplayValue = SecondKey;
                                objT_SessionEventsClientList.Add(objT_SessionEventsClient);
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemgrp in grp.Items)
                        {
                            var objT_SessionEventsClient = new T_SessionEventsClient();
                            objT_SessionEventsClient = (T_SessionEventsClient)itemgrp;
                            objT_SessionEventsClient.m_DisplayValue = firstKey.ToString();
                            objT_SessionEventsClientList.Add(objT_SessionEventsClient);
                        }
                    }
                }
				if (objT_SessionEventsClientList.Count() > 0)
                lstT_SessionEventsClient = objT_SessionEventsClientList.AsQueryable();
                ViewBag.IsGroupBy = true;
            }





            var _T_SessionEventsClient = lstT_SessionEventsClient;
			//if (lstT_SessionEventsClient.Where(p => p.t_client != null).Count() <= 50)
		    //ViewBag.t_client = new SelectList(lstT_SessionEventsClient.Where(p => p.t_client != null).Select(P => P.t_client).Distinct(), "ID", "DisplayValue");
            if (t_client != null)
            {
                var ids = t_client.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Client= ";

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
                            var obj = db.T_Clients.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEventsClient = _T_SessionEventsClient.Where(p => ids1.Contains(p.T_ClientID));
            }
				//if (lstT_SessionEventsClient.Where(p => p.t_sessionevents != null).Count() <= 50)
		    //ViewBag.t_sessionevents = new SelectList(lstT_SessionEventsClient.Where(p => p.t_sessionevents != null).Select(P => P.t_sessionevents).Distinct(), "ID", "DisplayValue");
            if (t_sessionevents != null)
            {
                var ids = t_sessionevents.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Session Events= ";

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
                            var obj = db.T_SessionEventss.Find(Convert.ToInt64(str));
                            ViewBag.SearchResult += obj != null ? obj.DisplayValue + ", " : "";
                        }

                    }
                    //
                }
                ids1 = ids1.ToList();
                _T_SessionEventsClient = _T_SessionEventsClient.Where(p => ids1.Contains(p.T_SessionEventsID));
            }
				if (!string.IsNullOrEmpty(SortOrder))
            {
                ViewBag.SearchResult += " \r\n Sort Order = ";
                var sortString = "";
                var sortProps = SortOrder.Split(",".ToCharArray());
                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient");
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
                var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient");
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
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_SessionEventsClient"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_SessionEventsClient"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
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
           // return View("Index", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
		    if (IsReports != null)
            {
			  var lstCustReportItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
                if (lstCustReportItem.Count() > 0)
                    ViewBag.CustReportItem= lstCustReportItem;
                return PartialView("~/Views/T_SessionEventsClient/CustomReportResult.cshtml", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
            }
			if (!Request.IsAjaxRequest())
			{
				if (string.IsNullOrEmpty(viewtype))
                    viewtype = "IndexPartial";
				GetTemplatesForList(viewtype); 
                if ((ViewBag.TemplatesName != null && viewtype != null) && ViewBag.TemplatesName != viewtype)
                    ViewBag.TemplatesName = viewtype;
                return View("Index",_T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
			}
            else
			{
				if (ViewBag.TemplatesName == null)
                    return PartialView("IndexPartial", _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
                else
                 return PartialView(ViewBag.TemplatesName, _T_SessionEventsClient.ToPagedList(pageNumber, pageSize));
			}
        }
		public string ShowGraph(string type, int? inlarge)
        {
		string result = "";
		var entity = "T_SessionEventsClient";
           var chartlist = db.Charts.Where(chrt => chrt.EntityName == entity && chrt.ShowInDashBoard).ToList();
           if (type != "all")
               chartlist = chartlist.Where(chrt => chrt.Id == Convert.ToInt64(type)).ToList();

           var entitylist = db.T_SessionEventsClients;

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
                var gd = db.T_SessionEventsClients.GroupBy(p => p.t_client.DisplayValue).ToList();
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
                chart.Titles.Add(CreateTitle("Count by Client (Top 10)"));
				else
				 chart.Titles.Add(CreateTitle("Count by Client"));
                chart.ChartAreas.Add(CreateChartArea(SeriesChartType.Column, "", "Client", "Session EventsClient"));
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
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", "T_SessionEventsClient", new { type = "1", inlarge = 1 }) + "')\"" : "") + "/></div>";
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
                var gd = db.T_SessionEventsClients.GroupBy(p => p.t_sessionevents.DisplayValue).ToList();
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
                chart.Titles.Add(CreateTitle("Count by Session Events (Top 10)"));
				else
				 chart.Titles.Add(CreateTitle("Count by Session Events"));
                chart.ChartAreas.Add(CreateChartArea(SeriesChartType.Pie, "", "Session Events", "Session EventsClient"));
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
                        string img = "<div class='col-sm-4' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; cursor: pointer;' " + (inlarge == null ? "data-toggle='modal' data-target='#dvPopupBulkOperation' onclick=\"OpenPopUpGraph('PopupBulkOperation','dvPopupBulkOperation','" + Url.Action("ShowGraph", "T_SessionEventsClient", new { type = "2", inlarge = 1 }) + "')\"" : "") + "/></div>";
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
            var _Obj = db1.T_SessionEventsClients.FirstOrDefault(p => p.Id == idvalue);
            return  _Obj==null?"":_Obj.DisplayValue;
        }
		 public object GetRecordById(string id)
        {
		            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            if (string.IsNullOrEmpty(id)) return "";
            var _Obj = db1.T_SessionEventsClients.Find(Convert.ToInt64(id));
            return _Obj;
        }
		public string GetRecordById_Reflection(string id)
        {
							ApplicationContext db1 = new ApplicationContext(new SystemUser());
			if (string.IsNullOrEmpty(id)) return "";
			var _Obj = db1.T_SessionEventsClients.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : EntityComparer.EnumeratePropertyValues<T_SessionEventsClient>(_Obj, "T_SessionEventsClient", new string[] { ""  }); ;
			        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
		            IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
			        }
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_SessionEventsClients;
					string lambda = "";
                    foreach (var asso in AssoNameWithParent.Split("?".ToCharArray()))
                    {
                        lambda += asso + "=" + AssociationID + " OR ";
                    }
                    lambda = lambda.TrimEnd(" OR ".ToCharArray());
                    query = query.Where(lambda);

                   //Type[] exprArgTypes = { query.ElementType };
                   // string propToWhere = AssoNameWithParent;
                   // ParameterExpression p = Expression.Parameter(typeof(T_SessionEventsClient), "p");
                   // MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                   // LambdaExpression lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                   // MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                   // IQueryable q = query.Provider.CreateQuery(methodCall);
                   //list = ((IQueryable<T_SessionEventsClient>)q);
				   list = ((IQueryable<T_SessionEventsClient>)query);
                }
				else if (AssoID == 0)
                {
                   IQueryable query = db.T_SessionEventsClients;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_SessionEventsClient), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_SessionEventsClient>)q);

                }
            }
			FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_SessionEventsClient>(User,list, "T_SessionEventsClient",caller);
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
        public JsonResult GetAllValues(string SearchString, string HostingEntityName, string HostingEntityID, string AssociatedType, string size)
        {
			long hostid = Convert.ToInt64(HostingEntityID);
			int pagesize = 50;
			if (HostingEntityName == "T_Client")
            {
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_ClientID == hostid).Select(q => q.T_SessionEventsID);
				//var Finallist = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id) && (p.DisplayValue.Contains(SearchString))).OrderBy(p => p.DisplayValue);
				 
				
		var Finallist = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id)).ToList();
				
                if(!string.IsNullOrEmpty(SearchString))
                    Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
                else
                    Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();

				var ListCount = Finallist.Count();
				var Results= false;
				 if (!string.IsNullOrEmpty(size))
                    if (size == "All")
                        pagesize = ListCount;
                    else
                        pagesize = Convert.ToInt32(size);
				if (ListCount > pagesize)
					Results = true;
                var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
				var genericResult = new { data = data, Results = Results };
                return Json(genericResult, JsonRequestBehavior.AllowGet);
            }
            if (HostingEntityName == "T_SessionEvents")
            {
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_SessionEventsID == hostid).Select(q => q.T_ClientID);
				//var Finallist = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id) && (p.DisplayValue.Contains(SearchString))).OrderBy(p => p.DisplayValue);
		var Finallist = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id)).ToList();

                if(!string.IsNullOrEmpty(SearchString))
                    Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
                else
                    Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
				var ListCount = Finallist.Count();
				var Results= false;
				 if (!string.IsNullOrEmpty(size))
                    if (size == "All")
                        pagesize = ListCount;
                    else
                        pagesize = Convert.ToInt32(size);
				if (ListCount > pagesize)
					Results = true;
                var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
				var genericResult = new { data = data, Results = Results };
                return Json(genericResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValues1(string SearchString, string HostingEntityName, string HostingEntityID, string AssociatedType, string size , string t_sessioneventslearningcenter,string schedule,string t_sessioneventstimeslots,string T_IsCancelled  )
        {
			long hostid = Convert.ToInt64(HostingEntityID);
			int pagesize = 50;
			if (HostingEntityName == "T_Client")
            {
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_ClientID == hostid).Select(q => q.T_SessionEventsID);
				//var Finallist = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id) && (p.DisplayValue.Contains(SearchString))).OrderBy(p => p.DisplayValue);

		var Finallist = db.T_SessionEventss.Where(p => !Selectedlist.Contains(p.Id)).ToList();
			if(T_IsCancelled!=null)
            {
                try
                {
                    bool boolvalue = Convert.ToBoolean(T_IsCancelled);
                    Finallist =  Finallist.Where(o => o.T_IsCancelled == boolvalue).ToList();
                }
                catch { }
            }
				if(!string.IsNullOrEmpty(SearchString))
                    Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
                else
                    Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
				if (t_sessioneventslearningcenter != null)
                {
                    var ids = t_sessioneventslearningcenter.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    foreach (var str in ids)
                    {
                        //Null Search
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                                ids1.Add(null);
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                var obj = db.T_SessionEventss.Find(Convert.ToInt64(str));
                            }
                        }
                        //
                    }
                    ids1 = ids1.ToList();
                    Finallist = Finallist.Where(p => ids1.Contains(p.T_SessionEventsLearningCenterID)).ToList();
                }
				if (schedule != null)
                {
                    var ids = schedule.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    foreach (var str in ids)
                    {
                        //Null Search
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                                ids1.Add(null);
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                var obj = db.T_SessionEventss.Find(Convert.ToInt64(str));
                            }
                        }
                        //
                    }
                    ids1 = ids1.ToList();
                    Finallist = Finallist.Where(p => ids1.Contains(p.ScheduleID)).ToList();
                }
				if (t_sessioneventstimeslots != null)
                {
                    var ids = t_sessioneventstimeslots.Split(",".ToCharArray());
                    List<long?> ids1 = new List<long?>();
                    foreach (var str in ids)
                    {
                        //Null Search
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                                ids1.Add(null);
                            else
                            {
                                ids1.Add(Convert.ToInt64(str));
                                var obj = db.T_SessionEventss.Find(Convert.ToInt64(str));
                            }
                        }
                        //
                    }
                    ids1 = ids1.ToList();
                    Finallist = Finallist.Where(p => ids1.Contains(p.T_SessionEventsTimeSlotsID)).ToList();
                }
				var ListCount = Finallist.Count();
				var Results= false;
				 if (!string.IsNullOrEmpty(size))
                    if (size == "All")
                        pagesize = ListCount;
                    else
                        pagesize = Convert.ToInt32(size);
				if (ListCount > pagesize)
					Results = true;
                var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
				var genericResult = new { data = data, Results = Results };
                return Json(genericResult, JsonRequestBehavior.AllowGet);
            }
            if (HostingEntityName == "T_SessionEvents")
            {
                var Selectedlist = db.T_SessionEventsClients.Where(p => p.T_SessionEventsID == hostid).Select(q => q.T_ClientID);
				//var Finallist = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id) && (p.DisplayValue.Contains(SearchString))).OrderBy(p => p.DisplayValue);
		var Finallist = db.T_Clients.Where(p => !Selectedlist.Contains(p.Id)).ToList();
                if(!string.IsNullOrEmpty(SearchString))
                    Finallist = Finallist.Where(p=>p.DisplayValue.ToUpper().Contains(SearchString.ToUpper())).OrderBy(p => p.DisplayValue).ToList();
                else
                    Finallist = Finallist.OrderBy(p => p.DisplayValue).ToList();
        
				var ListCount = Finallist.Count();
				var Results= false;
				 if (!string.IsNullOrEmpty(size))
                    if (size == "All")
                        pagesize = ListCount;
                    else
                        pagesize = Convert.ToInt32(size);
				if (ListCount > pagesize)
					Results = true;
                var data = new SelectList(Finallist.Take(pagesize), "ID", "DisplayValue");
				var genericResult = new { data = data, Results = Results };
                return Json(genericResult, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_SessionEventsClients;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_SessionEventsClient), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_SessionEventsClient>)q);
                }
            }
			var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
        }
[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
				try
                {
					Nullable<long> AssoID = Convert.ToInt64(AssociationID);
					if (AssoID != null && AssoID > 0)
					{
						IQueryable query = db.T_SessionEventsClients;
						Type[] exprArgTypes = { query.ElementType };
						string propToWhere = AssoNameWithParent;
						ParameterExpression p = Expression.Parameter(typeof(T_SessionEventsClient), "p");
						MemberExpression member = Expression.PropertyOrField(p, propToWhere);
						LambdaExpression lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
						MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
						IQueryable q = query.Provider.CreateQuery(methodCall);
						list = ((IQueryable<T_SessionEventsClient>)q).Take(20);
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
           list = _fad.FilterDropdown<T_SessionEventsClient>(User,list, "T_SessionEventsClient",caller);
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
            IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            if (!string.IsNullOrEmpty(propNameBR))
            {
				//added new code (Remove old code if everything works)
				var result = list.Select("new(Id," + propNameBR + " as value)");
                return Json(result, JsonRequestBehavior.AllowGet);
				//

                ParameterExpression param = Expression.Parameter(typeof(T_SessionEventsClient), "d");
                //bulid expression tree:data.Field1
                var Property = typeof(T_SessionEventsClient).GetProperty(propNameBR);
                Expression selector = Expression.Property(param, Property);
                Expression pred = Expression.Lambda(selector, param);
                //bulid expression tree:Select(d=>d.Field1)

                var targetType = Property.PropertyType;
                if (targetType.GetGenericArguments().Count() > 0)
                {
                    if (targetType.GetGenericArguments()[0].Name == "DateTime")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select", 
                        new Type[] { typeof(T_SessionEventsClient), typeof(DateTime?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<DateTime?> query = list.Provider.CreateQuery<DateTime?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                }
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (targetType.GetGenericArguments()[0].Name == "Decimal")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEventsClient), typeof(decimal?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<decimal?> query = list.Provider.CreateQuery<decimal?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Boolean")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEventsClient), typeof(bool?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<bool?> query = list.Provider.CreateQuery<bool?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Int32")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEventsClient), typeof(Int32?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<Int32?> query = list.Provider.CreateQuery<Int32?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Int64")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEventsClient), typeof(Int64?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<Int64?> query = list.Provider.CreateQuery<Int64?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                    if (targetType.GetGenericArguments()[0].Name == "Double")
                    {
                        Expression expr = Expression.Call(typeof(Queryable), "Select",
                        new Type[] { typeof(T_SessionEventsClient), typeof(double?) }, Expression.Constant(list.AsQueryable()), pred);
                        IQueryable<double?> query = list.Provider.CreateQuery<double?>(expr).Distinct();
                        return Json(query.AsEnumerable(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Expression expr = Expression.Call(typeof(Queryable), "Select",
                            new Type[] { typeof(T_SessionEventsClient), typeof(string) }, Expression.Constant(list.AsQueryable()), pred);
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
			IQueryable<T_SessionEventsClient> list = db.T_SessionEventsClients;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.T_SessionEventsClients;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(T_SessionEventsClient), "p"));
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
                list = ((IQueryable<T_SessionEventsClient>)q);
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
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_SessionEventsClient")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_SessionEventsClient").ToList();
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
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_SessionEventsClient");
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
                        typeName = "T_SessionEventsClient";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        //long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
						string idMapping = collection["ListOfMappings"];
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = idMapping; //db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_SessionEventsClient" && p.IsDefaultMapping);
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
                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Session EventsClients";
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
                                										 case "T_ClientID":
										 var t_clientId = db.T_Clients.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (t_clientId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "T_SessionEventsID":
										 var t_sessioneventsId = db.T_SessionEventss.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (t_sessioneventsId == null)
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
			string typename = "T_SessionEventsClient";
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
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Session EventsClients";
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_SessionEventsClient");
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
                                										 case "T_ClientID":
										 var strPropertyT_Client = lstEntityProp.FirstOrDefault(p => p.Key == "T_ClientID").Value;
										 ModelReflector.Property propT_Client = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Client").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_Client);
										 //var elementTypeT_Client = db.T_Clients.ElementType;
										 //var propertyInfoT_Client = elementTypeT_Client.GetProperty(propT_Client.Name);
										 // var t_clientId = db.T_Clients.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_Client.GetValue(p, null)) == assovalue);
										 var t_clientId = db.T_Clients.Where(propT_Client.Name+"=\""+assovalue+"\"").FirstOrDefault();
										 if (t_clientId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "T_SessionEventsID":
										 var strPropertyT_SessionEvents = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsID").Value;
										 ModelReflector.Property propT_SessionEvents = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEvents);
										 //var elementTypeT_SessionEvents = db.T_SessionEventss.ElementType;
										 //var propertyInfoT_SessionEvents = elementTypeT_SessionEvents.GetProperty(propT_SessionEvents.Name);
										 // var t_sessioneventsId = db.T_SessionEventss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEvents.GetValue(p, null)) == assovalue);
										 var t_sessioneventsId = db.T_SessionEventss.Where(propT_SessionEvents.Name+"=\""+assovalue+"\"").FirstOrDefault();
										 if (t_sessioneventsId == null)
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
                        T_SessionEventsClient model = new T_SessionEventsClient();
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
					 case "T_ClientID":
		 dynamic t_clientId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertyT_Client = lstEntityProp.FirstOrDefault(p => p.Key == "T_ClientID").Value;
			 ModelReflector.Property propT_Client = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Client").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_Client);
			 //var elementTypeT_Client = db.T_Clients.ElementType;
			 //var propertyInfoT_Client = elementTypeT_Client.GetProperty(propT_Client.Name);			 
			 //t_clientId = db.T_Clients.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_Client.GetValue(p, null)) == columnValue);
			 t_clientId = db.T_Clients.Where(propT_Client.Name + "=\"" + columnValue + "\"").FirstOrDefault();
		 }
		 else
			t_clientId = db.T_Clients.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (t_clientId != null)
			model.T_ClientID = t_clientId.Id;
		  else
			{
				if ((collection["T_ClientID"].ToString() == "true,false"))
				{
					try
					{
						T_Client objT_Client = new T_Client();
						objT_Client.T_Name  = (columnValue);
						db.T_Clients.Add(objT_Client);
						db.SaveChanges();
						model.T_ClientID = objT_Client.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "T_SessionEventsID":
		 dynamic t_sessioneventsId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertyT_SessionEvents = lstEntityProp.FirstOrDefault(p => p.Key == "T_SessionEventsID").Value;
			 ModelReflector.Property propT_SessionEvents = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEvents").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_SessionEvents);
			 //var elementTypeT_SessionEvents = db.T_SessionEventss.ElementType;
			 //var propertyInfoT_SessionEvents = elementTypeT_SessionEvents.GetProperty(propT_SessionEvents.Name);			 
			 //t_sessioneventsId = db.T_SessionEventss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_SessionEvents.GetValue(p, null)) == columnValue);
			 t_sessioneventsId = db.T_SessionEventss.Where(propT_SessionEvents.Name + "=\"" + columnValue + "\"").FirstOrDefault();
		 }
		 else
			t_sessioneventsId = db.T_SessionEventss.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (t_sessioneventsId != null)
			model.T_SessionEventsID = t_sessioneventsId.Id;
		  else
			{
				if ((collection["T_SessionEventsID"].ToString() == "true,false"))
				{
					try
					{
						T_SessionEvents objT_SessionEvents = new T_SessionEvents();
						objT_SessionEvents.T_Title  = (columnValue);
						db.T_SessionEventss.Add(objT_SessionEvents);
						db.SaveChanges();
						model.T_SessionEventsID = objT_SessionEvents.Id;
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
					    if (ValidateModel(model) && string.IsNullOrEmpty(CheckBeforeSave(model)))
                        {
							var result = CheckMandatoryProperties(model);
                            if (result == null || result.Count == 0)
                            {
								db.T_SessionEventsClients.Add(model);
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
		public bool ValidateModel(T_SessionEventsClient validate)
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
                var obj1 = db1.T_SessionEventsClients.Find(Id);
                System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                return PropValue;
				            }
            catch { return null; }
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReadOnlyProperties(T_SessionEventsClient OModel)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEventsClient").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_SessionEventsClient");
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
                            var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient");
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
        public JsonResult GetMandatoryProperties(T_SessionEventsClient OModel, string ruleType)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEventsClient").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    if (ruleType == "OnCreate")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 2).ToList();
                    else if (ruleType == "OnEdit")
                        BR = BR.Where(p => p.AssociatedBusinessRuleTypeID != 1).ToList();
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_SessionEventsClient");
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
                            var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient");
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
        public JsonResult GetValidateBeforeSaveProperties(T_SessionEventsClient OModel, string ruleType)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            var conditions = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEventsClient").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ValidateBeforeSavePropertiesRule(OModel, BR, "T_SessionEventsClient");
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
                                    var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient");
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
        public JsonResult GetLockBusinessRules(T_SessionEventsClient OModel)
        {
            Dictionary<string, string> RulesApplied = new Dictionary<string, string>();
            //string RulesApplied = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEventsClient").ToList();
                var BRAll = BR;
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "T_SessionEventsClient");
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
		private List<string> CheckMandatoryProperties(T_SessionEventsClient OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_SessionEventsClient").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_SessionEventsClient");
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
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_SessionEventsClient").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
            var _Obj = db1.T_SessionEventsClients.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if (_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
				public long? GetIdFromPropertyValue(string PropName, string PropValue)
        {
            ApplicationContext db1 = new ApplicationContext(new SystemUser());
            IQueryable query = db1.T_SessionEventsClients;
            Type[] exprArgTypes = { query.ElementType };
            string propToWhere = PropName;
            ParameterExpression p = Expression.Parameter(typeof(T_SessionEventsClient), "p");
            MemberExpression member = Expression.PropertyOrField(p, propToWhere);
            LambdaExpression lambda = null;
            if (PropValue.ToLower().Trim() != "null")
            lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropValue), member.Type)), p);
            else
                lambda = Expression.Lambda<Func<T_SessionEventsClient, bool>>(Expression.Equal(member, Expression.Constant(null, member.Type)), p);
            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
            IQueryable q = query.Provider.CreateQuery(methodCall);
            long outValue;
            var list1 = ((IQueryable<T_SessionEventsClient>)q);
            if (list1 != null && list1.Count() > 0)
                return Int64.TryParse(list1.FirstOrDefault().Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
				[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
        {
								ApplicationContext db1 = new ApplicationContext(new SystemUser());
            var obj1 = db1.T_SessionEventsClients.Find(Id);
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
        public JsonResult Check1MThresholdValue(T_SessionEventsClient t_sessioneventsclient)
        {
            Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
            return Json(msgThreshold, JsonRequestBehavior.AllowGet);
        }
		public bool CheckBeforeDelete(T_SessionEventsClient t_sessioneventsclient)
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
		public string CheckBeforeSave(T_SessionEventsClient t_sessioneventsclient)
        {
			var AlertMsg = "";
            // Write your logic here
 
			   //Make sure to assign AlertMsg with proper message
			   	
			   //AlertMsg = "Validation Alert - Before Save !! Information not saved.";
               //ModelState.AddModelError("CustomError", AlertMsg);
			   //ViewBag.ApplicationError = AlertMsg;
			
            return AlertMsg;
        }
		public void OnDeleting(T_SessionEventsClient t_sessioneventsclient)
        {
            // Write your logic here
 
		}
        public void OnSaving(T_SessionEventsClient t_sessioneventsclient, ApplicationContext db)
        {
            // Write your logic here
 
        }
		public void AfterSave(T_SessionEventsClient t_sessioneventsclient)
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
        public ActionResult GetCalculationValues(T_SessionEventsClient t_sessioneventsclient)
        {
            t_sessioneventsclient.setCalculation();
            Dictionary<string, string> Calculations = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_sessioneventsclient.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(Calculations, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		//get Templates 
		// select templates 
        public void GetTemplatesForList(string defaultview)
        {
            string pageNameList = "";
            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
                                       where s.EntityName == "T_SessionEventsClient"
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
        public ActionResult GetDerivedDetails(T_SessionEventsClient t_sessioneventsclient, string IgnoreEditable)
        {
            Dictionary<string, string> derivedlist = new Dictionary<string, string>();
            System.Reflection.PropertyInfo[] properties = (t_sessioneventsclient.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            return Json(derivedlist, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDerivedDetailsInline(string host, string value, T_SessionEventsClient t_sessioneventsclient, string IgnoreEditable)
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


