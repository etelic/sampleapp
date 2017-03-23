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
    public partial class ApplicationFeedbackController : BaseController
    {
		private void LoadViewDataAfterOnEdit(ApplicationFeedback applicationfeedback)
        {
			 ViewBag.AssociatedApplicationFeedbackTypeID = new SelectList(db.ApplicationFeedbackTypes, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackTypeID);
			 ViewBag.AssociatedApplicationFeedbackStatusID = new SelectList(db.ApplicationFeedbackStatuss, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackStatusID);
			 ViewBag.ApplicationFeedbackPriorityID = new SelectList(db.FeedbackPrioritys, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackPriorityID);
			 ViewBag.ApplicationFeedbackSeverityID = new SelectList(db.FeedbackSeveritys, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackSeverityID);
			 ViewBag.ApplicationFeedbackResourceID = new SelectList(db.FeedbackResources, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackResourceID);
        }
        private void LoadViewDataBeforeOnEdit(ApplicationFeedback applicationfeedback)
        {
                          var _objAssociatedApplicationFeedbackType = new List<ApplicationFeedbackType>();
               _objAssociatedApplicationFeedbackType.Add(applicationfeedback.associatedapplicationfeedbacktype);
			   ViewBag.AssociatedApplicationFeedbackTypeID = new SelectList(_objAssociatedApplicationFeedbackType, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackTypeID);
               var _objAssociatedApplicationFeedbackStatus = new List<ApplicationFeedbackStatus>();
               _objAssociatedApplicationFeedbackStatus.Add(applicationfeedback.associatedapplicationfeedbackstatus);
			   ViewBag.AssociatedApplicationFeedbackStatusID = new SelectList(_objAssociatedApplicationFeedbackStatus, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackStatusID);
               var _objApplicationFeedbackPriority = new List<FeedbackPriority>();
               _objApplicationFeedbackPriority.Add(applicationfeedback.applicationfeedbackpriority);
			   ViewBag.ApplicationFeedbackPriorityID = new SelectList(_objApplicationFeedbackPriority, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackPriorityID);
               var _objApplicationFeedbackSeverity = new List<FeedbackSeverity>();
               _objApplicationFeedbackSeverity.Add(applicationfeedback.applicationfeedbackseverity);
			   ViewBag.ApplicationFeedbackSeverityID = new SelectList(_objApplicationFeedbackSeverity, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackSeverityID);
               var _objApplicationFeedbackResource = new List<FeedbackResource>();
               _objApplicationFeedbackResource.Add(applicationfeedback.applicationfeedbackresource);
			   ViewBag.ApplicationFeedbackResourceID = new SelectList(_objApplicationFeedbackResource, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackResourceID);
        }
        private void LoadViewDataAfterOnCreate(ApplicationFeedback applicationfeedback)
        {
           			 ViewBag.AssociatedApplicationFeedbackTypeID = new SelectList(db.ApplicationFeedbackTypes, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackTypeID);
			 ViewBag.AssociatedApplicationFeedbackStatusID = new SelectList(db.ApplicationFeedbackStatuss, "ID", "DisplayValue", applicationfeedback.AssociatedApplicationFeedbackStatusID);
			 ViewBag.ApplicationFeedbackPriorityID = new SelectList(db.FeedbackPrioritys, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackPriorityID);
			 ViewBag.ApplicationFeedbackSeverityID = new SelectList(db.FeedbackSeveritys, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackSeverityID);
			 ViewBag.ApplicationFeedbackResourceID = new SelectList(db.FeedbackResources, "ID", "DisplayValue", applicationfeedback.ApplicationFeedbackResourceID);
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
			if(HostingEntityName == "ApplicationFeedbackType" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="AssociatedApplicationFeedbackType")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.AssociatedApplicationFeedbackTypeID = new SelectList(db.ApplicationFeedbackTypes.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.ApplicationFeedbackTypes.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
				var objAssociatedApplicationFeedbackType = new List<ApplicationFeedbackType>();
                ViewBag.AssociatedApplicationFeedbackTypeID = new SelectList(objAssociatedApplicationFeedbackType , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "ApplicationFeedbackStatus" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="AssociatedApplicationFeedbackStatus")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.AssociatedApplicationFeedbackStatusID = new SelectList(db.ApplicationFeedbackStatuss.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.ApplicationFeedbackStatuss.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
				var objAssociatedApplicationFeedbackStatus = new List<ApplicationFeedbackStatus>();
                ViewBag.AssociatedApplicationFeedbackStatusID = new SelectList(objAssociatedApplicationFeedbackStatus , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "FeedbackPriority" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="ApplicationFeedbackPriority")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.ApplicationFeedbackPriorityID = new SelectList(db.FeedbackPrioritys.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.FeedbackPrioritys.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
				var objApplicationFeedbackPriority = new List<FeedbackPriority>();
                ViewBag.ApplicationFeedbackPriorityID = new SelectList(objApplicationFeedbackPriority , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "FeedbackSeverity" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="ApplicationFeedbackSeverity")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.ApplicationFeedbackSeverityID = new SelectList(db.FeedbackSeveritys.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.FeedbackSeveritys.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
				var objApplicationFeedbackSeverity = new List<FeedbackSeverity>();
                ViewBag.ApplicationFeedbackSeverityID = new SelectList(objApplicationFeedbackSeverity , "ID", "DisplayValue");
		    }
			if(HostingEntityName == "FeedbackResource" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="ApplicationFeedbackResource")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.ApplicationFeedbackResourceID = new SelectList(db.FeedbackResources.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.FeedbackResources.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
				var objApplicationFeedbackResource = new List<FeedbackResource>();
                ViewBag.ApplicationFeedbackResourceID = new SelectList(objApplicationFeedbackResource , "ID", "DisplayValue");
		    }
        }
		private IQueryable<ApplicationFeedback> searchRecords(IQueryable<ApplicationFeedback> lstApplicationFeedback, string searchString, bool? IsDeepSearch)
        {
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstApplicationFeedback = lstApplicationFeedback.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageName) && s.PageName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageUrlTitle) && s.PageUrlTitle.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.UIControlName) && s.UIControlName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageUrl) && s.PageUrl.ToUpper().Contains(searchString)) ||(s.CommentId != null && SqlFunctions.StringConvert((double)s.CommentId).Contains(searchString)) ||(s.associatedapplicationfeedbacktype!= null && (s.associatedapplicationfeedbacktype.DisplayValue.ToUpper().Contains(searchString) )) ||(s.associatedapplicationfeedbackstatus!= null && (s.associatedapplicationfeedbackstatus.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackpriority!= null && (s.applicationfeedbackpriority.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackseverity!= null && (s.applicationfeedbackseverity.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackresource!= null && (s.applicationfeedbackresource.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.ReportedByUser) && s.ReportedByUser.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Summary) && s.Summary.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstApplicationFeedback = lstApplicationFeedback.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageName) && s.PageName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageUrlTitle) && s.PageUrlTitle.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.UIControlName) && s.UIControlName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.PageUrl) && s.PageUrl.ToUpper().Contains(searchString)) ||(s.CommentId != null && SqlFunctions.StringConvert((double)s.CommentId).Contains(searchString)) ||(s.associatedapplicationfeedbacktype!= null && (s.associatedapplicationfeedbacktype.DisplayValue.ToUpper().Contains(searchString) )) ||(s.associatedapplicationfeedbackstatus!= null && (s.associatedapplicationfeedbackstatus.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackpriority!= null && (s.applicationfeedbackpriority.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackseverity!= null && (s.applicationfeedbackseverity.DisplayValue.ToUpper().Contains(searchString) )) ||(s.applicationfeedbackresource!= null && (s.applicationfeedbackresource.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.ReportedByUser) && s.ReportedByUser.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Summary) && s.Summary.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Description) && s.Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
			try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstApplicationFeedback = lstApplicationFeedback.Union(db.ApplicationFeedbacks.Where(s => (s.ReportedBy == datevalue) ));
            }
            catch { }
            return lstApplicationFeedback;
        }
		private IQueryable<ApplicationFeedback> sortRecords(IQueryable<ApplicationFeedback> lstApplicationFeedback, string sortBy, string isAsc)
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
	 if(sortBy == "AssociatedApplicationFeedbackTypeID")
                return isAsc.ToLower() == "asc" ? lstApplicationFeedback.OrderBy(p => p.associatedapplicationfeedbacktype.DisplayValue) : lstApplicationFeedback.OrderByDescending(p => p.associatedapplicationfeedbacktype.DisplayValue);
	 if(sortBy == "AssociatedApplicationFeedbackStatusID")
                return isAsc.ToLower() == "asc" ? lstApplicationFeedback.OrderBy(p => p.associatedapplicationfeedbackstatus.DisplayValue) : lstApplicationFeedback.OrderByDescending(p => p.associatedapplicationfeedbackstatus.DisplayValue);
	 if(sortBy == "ApplicationFeedbackPriorityID")
                return isAsc.ToLower() == "asc" ? lstApplicationFeedback.OrderBy(p => p.applicationfeedbackpriority.DisplayValue) : lstApplicationFeedback.OrderByDescending(p => p.applicationfeedbackpriority.DisplayValue);
	 if(sortBy == "ApplicationFeedbackSeverityID")
                return isAsc.ToLower() == "asc" ? lstApplicationFeedback.OrderBy(p => p.applicationfeedbackseverity.DisplayValue) : lstApplicationFeedback.OrderByDescending(p => p.applicationfeedbackseverity.DisplayValue);
	 if(sortBy == "ApplicationFeedbackResourceID")
                return isAsc.ToLower() == "asc" ? lstApplicationFeedback.OrderBy(p => p.applicationfeedbackresource.DisplayValue) : lstApplicationFeedback.OrderByDescending(p => p.applicationfeedbackresource.DisplayValue);
            ParameterExpression paramExpression = Expression.Parameter(typeof(ApplicationFeedback), "applicationfeedback");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<ApplicationFeedback>)lstApplicationFeedback.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstApplicationFeedback.ElementType, lambda.Body.Type },
                    lstApplicationFeedback.Expression,
                    lambda));
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity ,string associatedapplicationfeedbacktype,string associatedapplicationfeedbackstatus,string applicationfeedbackpriority,string applicationfeedbackseverity,string applicationfeedbackresource)
        {
			int Qcount = Request.UrlReferrer.Query.Count();
			ViewBag.CurrentFilter = searchString;
			if(Qcount > 0)
			{
			var AssociatedApplicationFeedbackTypelist = db.ApplicationFeedbackTypes.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (associatedapplicationfeedbacktype != null)
            {
                var ids = associatedapplicationfeedbacktype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Type= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.ApplicationFeedbackTypes.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = AssociatedApplicationFeedbackTypelist.Union(db.ApplicationFeedbackTypes.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.associatedapplicationfeedbacktype = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.associatedapplicationfeedbacktype = new SelectList(AssociatedApplicationFeedbackTypelist, "ID", "DisplayValue");
			}
			var AssociatedApplicationFeedbackStatuslist = db.ApplicationFeedbackStatuss.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (associatedapplicationfeedbackstatus != null)
            {
                var ids = associatedapplicationfeedbackstatus.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Status= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.ApplicationFeedbackStatuss.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = AssociatedApplicationFeedbackStatuslist.Union(db.ApplicationFeedbackStatuss.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.associatedapplicationfeedbackstatus = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.associatedapplicationfeedbackstatus = new SelectList(AssociatedApplicationFeedbackStatuslist, "ID", "DisplayValue");
			}
			var ApplicationFeedbackPrioritylist = db.FeedbackPrioritys.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (applicationfeedbackpriority != null)
            {
                var ids = applicationfeedbackpriority.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Priority= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackPrioritys.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = ApplicationFeedbackPrioritylist.Union(db.FeedbackPrioritys.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.applicationfeedbackpriority = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.applicationfeedbackpriority = new SelectList(ApplicationFeedbackPrioritylist, "ID", "DisplayValue");
			}
			var ApplicationFeedbackSeveritylist = db.FeedbackSeveritys.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (applicationfeedbackseverity != null)
            {
                var ids = applicationfeedbackseverity.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Severity= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackSeveritys.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = ApplicationFeedbackSeveritylist.Union(db.FeedbackSeveritys.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.applicationfeedbackseverity = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.applicationfeedbackseverity = new SelectList(ApplicationFeedbackSeveritylist, "ID", "DisplayValue");
			}
			var ApplicationFeedbackResourcelist = db.FeedbackResources.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (applicationfeedbackresource != null)
            {
                var ids = applicationfeedbackresource.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Assigned To= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackResources.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = ApplicationFeedbackResourcelist.Union(db.FeedbackResources.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.applicationfeedbackresource = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.applicationfeedbackresource = new SelectList(ApplicationFeedbackResourcelist, "ID", "DisplayValue");
			}
			}
			else
			{
			var objAssociatedApplicationFeedbackType = new List<ApplicationFeedbackType>();
		    ViewBag.associatedapplicationfeedbacktype = new SelectList(objAssociatedApplicationFeedbackType, "ID", "DisplayValue");
			var objAssociatedApplicationFeedbackStatus = new List<ApplicationFeedbackStatus>();
		    ViewBag.associatedapplicationfeedbackstatus = new SelectList(objAssociatedApplicationFeedbackStatus, "ID", "DisplayValue");
			var objApplicationFeedbackPriority = new List<FeedbackPriority>();
		    ViewBag.applicationfeedbackpriority = new SelectList(objApplicationFeedbackPriority, "ID", "DisplayValue");
			var objApplicationFeedbackSeverity = new List<FeedbackSeverity>();
		    ViewBag.applicationfeedbackseverity = new SelectList(objApplicationFeedbackSeverity, "ID", "DisplayValue");
			var objApplicationFeedbackResource = new List<FeedbackResource>();
		    ViewBag.applicationfeedbackresource = new SelectList(objApplicationFeedbackResource, "ID", "DisplayValue");
			}
				var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
				if (lstFavoriteItem.Count() > 0)
                    ViewBag.FavoriteItem = lstFavoriteItem;
				return View();
            }
		 // GET: /ApplicationFeedback/FSearch/
		 public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport ,string associatedapplicationfeedbacktype,string associatedapplicationfeedbackstatus,string applicationfeedbackpriority,string applicationfeedbackseverity,string applicationfeedbackresource ,string CommentIdFrom,string CommentIdTo,string ReportedByFrom,string ReportedByTo)//, string HostingEntity
        {
			ViewBag.SearchResult = "";
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
				 var lstApplicationFeedback  = from s in db.ApplicationFeedbacks
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstApplicationFeedback  = searchRecords(lstApplicationFeedback, searchString.ToUpper(),true);
            }
			if(!string.IsNullOrEmpty(search)){
				ViewBag.SearchResult += "\r\n General Criterial= "+search;
				lstApplicationFeedback = searchRecords(lstApplicationFeedback, search,true);
			}
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstApplicationFeedback  = sortRecords(lstApplicationFeedback, sortBy, isAsc);
            }
            else   lstApplicationFeedback  = lstApplicationFeedback.OrderByDescending(c => c.Id);
			lstApplicationFeedback = lstApplicationFeedback.Include(t=>t.associatedapplicationfeedbacktype).Include(t=>t.associatedapplicationfeedbackstatus).Include(t=>t.applicationfeedbackpriority).Include(t=>t.applicationfeedbackseverity).Include(t=>t.applicationfeedbackresource);
            var _ApplicationFeedback = lstApplicationFeedback;
		 if(CommentIdFrom!=null || CommentIdTo !=null)
         {
                try
                {
                    long from = CommentIdFrom == null ? 0 : Convert.ToInt64(CommentIdFrom);
                    long to =  CommentIdTo == null ? long.MaxValue : Convert.ToInt64(CommentIdTo);
                    _ApplicationFeedback =  _ApplicationFeedback.Where(o => o.CommentId >= from && o.CommentId <= to);
					ViewBag.SearchResult += "\r\n Comment Id= "+from+"-"+to;
                }
                catch { }
          }
				if(ReportedByFrom!=null || ReportedByTo !=null)
				{
						try
						{
							DateTime from = ReportedByFrom == null ? Convert.ToDateTime("01/01/1900") : Convert.ToDateTime(ReportedByFrom);
							DateTime to = ReportedByTo == null ? DateTime.MaxValue : Convert.ToDateTime(ReportedByTo);
							_ApplicationFeedback =  _ApplicationFeedback.Where(o => o.ReportedBy >= from && o.ReportedBy <= to);
							ViewBag.SearchResult += "\r\n Reported By= "+from.ToShortDateString()+"-"+to.ToShortDateString();
						}
						catch { }
				}
			//if (lstApplicationFeedback.Where(p => p.associatedapplicationfeedbacktype != null).Count() <= 50)
		    //ViewBag.associatedapplicationfeedbacktype = new SelectList(lstApplicationFeedback.Where(p => p.associatedapplicationfeedbacktype != null).Select(P => P.associatedapplicationfeedbacktype).Distinct(), "ID", "DisplayValue");
            if (associatedapplicationfeedbacktype != null)
            {
                var ids = associatedapplicationfeedbacktype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Type= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.ApplicationFeedbackTypes.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _ApplicationFeedback = _ApplicationFeedback.Where(p => ids1.ToList().Contains(p.AssociatedApplicationFeedbackTypeID));
            }
				//if (lstApplicationFeedback.Where(p => p.associatedapplicationfeedbackstatus != null).Count() <= 50)
		    //ViewBag.associatedapplicationfeedbackstatus = new SelectList(lstApplicationFeedback.Where(p => p.associatedapplicationfeedbackstatus != null).Select(P => P.associatedapplicationfeedbackstatus).Distinct(), "ID", "DisplayValue");
            if (associatedapplicationfeedbackstatus != null)
            {
                var ids = associatedapplicationfeedbackstatus.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Status= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.ApplicationFeedbackStatuss.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _ApplicationFeedback = _ApplicationFeedback.Where(p => ids1.ToList().Contains(p.AssociatedApplicationFeedbackStatusID));
            }
				//if (lstApplicationFeedback.Where(p => p.applicationfeedbackpriority != null).Count() <= 50)
		    //ViewBag.applicationfeedbackpriority = new SelectList(lstApplicationFeedback.Where(p => p.applicationfeedbackpriority != null).Select(P => P.applicationfeedbackpriority).Distinct(), "ID", "DisplayValue");
            if (applicationfeedbackpriority != null)
            {
                var ids = applicationfeedbackpriority.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Priority= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackPrioritys.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _ApplicationFeedback = _ApplicationFeedback.Where(p => ids1.ToList().Contains(p.ApplicationFeedbackPriorityID));
            }
				//if (lstApplicationFeedback.Where(p => p.applicationfeedbackseverity != null).Count() <= 50)
		    //ViewBag.applicationfeedbackseverity = new SelectList(lstApplicationFeedback.Where(p => p.applicationfeedbackseverity != null).Select(P => P.applicationfeedbackseverity).Distinct(), "ID", "DisplayValue");
            if (applicationfeedbackseverity != null)
            {
                var ids = applicationfeedbackseverity.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Severity= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackSeveritys.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _ApplicationFeedback = _ApplicationFeedback.Where(p => ids1.ToList().Contains(p.ApplicationFeedbackSeverityID));
            }
				//if (lstApplicationFeedback.Where(p => p.applicationfeedbackresource != null).Count() <= 50)
		    //ViewBag.applicationfeedbackresource = new SelectList(lstApplicationFeedback.Where(p => p.applicationfeedbackresource != null).Select(P => P.applicationfeedbackresource).Distinct(), "ID", "DisplayValue");
            if (applicationfeedbackresource != null)
            {
                var ids = applicationfeedbackresource.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Assigned To= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.FeedbackResources.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _ApplicationFeedback = _ApplicationFeedback.Where(p => ids1.ToList().Contains(p.ApplicationFeedbackResourceID));
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
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "ApplicationFeedback"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_ApplicationFeedback.Count() > 0)
                    pageSize = _ApplicationFeedback.Count();
                return View("ExcelExport", _ApplicationFeedback.ToPagedList(pageNumber, pageSize));
            }
           // return View("Index", _ApplicationFeedback.ToPagedList(pageNumber, pageSize));
			if (!Request.IsAjaxRequest())
                return View("Index",_ApplicationFeedback.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _ApplicationFeedback.ToPagedList(pageNumber, pageSize));
        }
		public string ShowGraph()
        {
            string result = "";
           {
                var gd = db.ApplicationFeedbacks.GroupBy(p => p.associatedapplicationfeedbacktype.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 180, theme: ChartTheme.Yellow)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Type")
                .GetBytes("png");
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
            }
           {
                var gd = db.ApplicationFeedbacks.GroupBy(p => p.associatedapplicationfeedbackstatus.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 180, theme: ChartTheme.Green)
                .AddSeries(
                chartType: "Pie",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Status")
                .GetBytes("png");
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
            }
           {
                var gd = db.ApplicationFeedbacks.GroupBy(p => p.applicationfeedbackpriority.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 180, theme: ChartTheme.Blue)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Priority")
                .GetBytes("png");
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
            }
           {
                var gd = db.ApplicationFeedbacks.GroupBy(p => p.applicationfeedbackseverity.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 180, theme: ChartTheme.Yellow)
                .AddSeries(
                chartType: "Pie",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Severity")
                .GetBytes("png");
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
            }
           {
                var gd = db.ApplicationFeedbacks.GroupBy(p => p.applicationfeedbackresource.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 180, theme: ChartTheme.Green)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Assigned To")
                .GetBytes("png");
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
            }
			 return result;
	    }
        public string GetDisplayValue(string id)
        {
			ApplicationContext db1 = new ApplicationContext(User);
			if (string.IsNullOrEmpty(id)) return "";
			var _Obj = db1.ApplicationFeedbacks.Find(Convert.ToInt64(id));
            return  _Obj==null?"":_Obj.DisplayValue;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<ApplicationFeedback> list = db.ApplicationFeedbacks;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<ApplicationFeedback> list = db.ApplicationFeedbacks;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.ApplicationFeedbacks;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(ApplicationFeedback), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<ApplicationFeedback, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<ApplicationFeedback>)q);
                }
            }
			FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<ApplicationFeedback>(User,list, "ApplicationFeedback",caller);
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
                     var data = from x in list.Where(p=>p.Id != val).Take(9).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
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
		public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<ApplicationFeedback> list = db.ApplicationFeedbacks;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
				try
                {
					Nullable<long> AssoID = Convert.ToInt64(AssociationID);
					if (AssoID != null && AssoID > 0)
					{
						IQueryable query = db.ApplicationFeedbacks;
						Type[] exprArgTypes = { query.ElementType };
						string propToWhere = AssoNameWithParent;
						ParameterExpression p = Expression.Parameter(typeof(ApplicationFeedback), "p");
						MemberExpression member = Expression.PropertyOrField(p, propToWhere);
						LambdaExpression lambda = Expression.Lambda<Func<ApplicationFeedback, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
						MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
						IQueryable q = query.Provider.CreateQuery(methodCall);
						list = ((IQueryable<ApplicationFeedback>)q).Take(20);
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
           list = _fad.FilterDropdown<ApplicationFeedback>(User,list, "ApplicationFeedback",caller);
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
                     var data = from x in list.Where(p=>p.Id != val).Take(20).Union(list.Where(p=>p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
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
			IQueryable<ApplicationFeedback> list = db.ApplicationFeedbacks;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.ApplicationFeedbacks;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(ApplicationFeedback), "p"));
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
                list = ((IQueryable<ApplicationFeedback>)q);
            }
			if (key != null && key.Length > 0)
            {
			   var data = from x in list.Where(p=>p.DisplayValue.Contains(key)).Take(10).OrderBy(q=>q.DisplayValue).ToList()
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
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "ApplicationFeedback")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "ApplicationFeedback").ToList();
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
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "ApplicationFeedback");
                    if (entList != null)
                    {
                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue" && p.Name != "CommentId" && p.Name != "AttachImage" && p.Name != "AttachDocument"))
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
                        typeName = "ApplicationFeedback";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "ApplicationFeedback" && p.IsDefaultMapping);
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
                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Application Feedbacks";
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
                                										 case "AssociatedApplicationFeedbackTypeID":
										 var associatedapplicationfeedbacktypeId = db.ApplicationFeedbackTypes.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (associatedapplicationfeedbacktypeId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "AssociatedApplicationFeedbackStatusID":
										 var associatedapplicationfeedbackstatusId = db.ApplicationFeedbackStatuss.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (associatedapplicationfeedbackstatusId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackPriorityID":
										 var applicationfeedbackpriorityId = db.FeedbackPrioritys.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (applicationfeedbackpriorityId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackSeverityID":
										 var applicationfeedbackseverityId = db.FeedbackSeveritys.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (applicationfeedbackseverityId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackResourceID":
										 var applicationfeedbackresourceId = db.FeedbackResources.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (applicationfeedbackresourceId == null)
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
			string typename = "ApplicationFeedback";
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
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Application Feedbacks";
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "ApplicationFeedback");
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
                                										 case "AssociatedApplicationFeedbackTypeID":
										 var strPropertyAssociatedApplicationFeedbackType = lstEntityProp.FirstOrDefault(p => p.Key == "AssociatedApplicationFeedbackTypeID").Value;
										 ModelReflector.Property propAssociatedApplicationFeedbackType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedbackType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyAssociatedApplicationFeedbackType);
										 var elementTypeAssociatedApplicationFeedbackType = db.ApplicationFeedbackTypes.ElementType;
										 var propertyInfoAssociatedApplicationFeedbackType = elementTypeAssociatedApplicationFeedbackType.GetProperty(propAssociatedApplicationFeedbackType.Name);
										 var associatedapplicationfeedbacktypeId = db.ApplicationFeedbackTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoAssociatedApplicationFeedbackType.GetValue(p, null)) == assovalue);
										 if (associatedapplicationfeedbacktypeId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "AssociatedApplicationFeedbackStatusID":
										 var strPropertyAssociatedApplicationFeedbackStatus = lstEntityProp.FirstOrDefault(p => p.Key == "AssociatedApplicationFeedbackStatusID").Value;
										 ModelReflector.Property propAssociatedApplicationFeedbackStatus = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedbackStatus").Properties.FirstOrDefault(p => p.DisplayName == strPropertyAssociatedApplicationFeedbackStatus);
										 var elementTypeAssociatedApplicationFeedbackStatus = db.ApplicationFeedbackStatuss.ElementType;
										 var propertyInfoAssociatedApplicationFeedbackStatus = elementTypeAssociatedApplicationFeedbackStatus.GetProperty(propAssociatedApplicationFeedbackStatus.Name);
										 var associatedapplicationfeedbackstatusId = db.ApplicationFeedbackStatuss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoAssociatedApplicationFeedbackStatus.GetValue(p, null)) == assovalue);
										 if (associatedapplicationfeedbackstatusId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackPriorityID":
										 var strPropertyApplicationFeedbackPriority = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackPriorityID").Value;
										 ModelReflector.Property propApplicationFeedbackPriority = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackPriority").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackPriority);
										 var elementTypeApplicationFeedbackPriority = db.FeedbackPrioritys.ElementType;
										 var propertyInfoApplicationFeedbackPriority = elementTypeApplicationFeedbackPriority.GetProperty(propApplicationFeedbackPriority.Name);
										 var applicationfeedbackpriorityId = db.FeedbackPrioritys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackPriority.GetValue(p, null)) == assovalue);
										 if (applicationfeedbackpriorityId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackSeverityID":
										 var strPropertyApplicationFeedbackSeverity = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackSeverityID").Value;
										 ModelReflector.Property propApplicationFeedbackSeverity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackSeverity").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackSeverity);
										 var elementTypeApplicationFeedbackSeverity = db.FeedbackSeveritys.ElementType;
										 var propertyInfoApplicationFeedbackSeverity = elementTypeApplicationFeedbackSeverity.GetProperty(propApplicationFeedbackSeverity.Name);
										 var applicationfeedbackseverityId = db.FeedbackSeveritys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackSeverity.GetValue(p, null)) == assovalue);
										 if (applicationfeedbackseverityId == null)
											uniqueassoValues.Add(assovalue);
										 break;
																		 case "ApplicationFeedbackResourceID":
										 var strPropertyApplicationFeedbackResource = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackResourceID").Value;
										 ModelReflector.Property propApplicationFeedbackResource = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackResource").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackResource);
										 var elementTypeApplicationFeedbackResource = db.FeedbackResources.ElementType;
										 var propertyInfoApplicationFeedbackResource = elementTypeApplicationFeedbackResource.GetProperty(propApplicationFeedbackResource.Name);
										 var applicationfeedbackresourceId = db.FeedbackResources.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackResource.GetValue(p, null)) == assovalue);
										 if (applicationfeedbackresourceId == null)
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
                        ApplicationFeedback model = new ApplicationFeedback();
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
    case "EntityName":
    model.EntityName = columnValue;
	break;
    case "PropertyName":
    model.PropertyName = columnValue;
	break;
    case "PageName":
    model.PageName = columnValue;
	break;
    case "PageUrlTitle":
    model.PageUrlTitle = columnValue;
	break;
    case "UIControlName":
    model.UIControlName = columnValue;
	break;
    case "PageUrl":
    model.PageUrl = columnValue;
	break;
    case "ReportedBy":
    model.ReportedBy = DateTime.Parse(columnValue);
	break;
    case "Summary":
    model.Summary = columnValue;
	break;
    case "Description":
    model.Description = columnValue;
	break;
					 case "AssociatedApplicationFeedbackTypeID":
		 var strPropertyAssociatedApplicationFeedbackType = lstEntityProp.FirstOrDefault(p => p.Key == "AssociatedApplicationFeedbackTypeID").Value;
         ModelReflector.Property propAssociatedApplicationFeedbackType = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedbackType").Properties.FirstOrDefault(p => p.DisplayName == strPropertyAssociatedApplicationFeedbackType);
         var elementTypeAssociatedApplicationFeedbackType = db.ApplicationFeedbackTypes.ElementType;
         var propertyInfoAssociatedApplicationFeedbackType = elementTypeAssociatedApplicationFeedbackType.GetProperty(propAssociatedApplicationFeedbackType.Name);
         var associatedapplicationfeedbacktypeId = db.ApplicationFeedbackTypes.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoAssociatedApplicationFeedbackType.GetValue(p, null)) == columnValue);
		 if (associatedapplicationfeedbacktypeId != null)
			model.AssociatedApplicationFeedbackTypeID = associatedapplicationfeedbacktypeId.Id;
		  else
			{
				if ((collection["AssociatedApplicationFeedbackTypeID"].ToString() == "true,false"))
				{
					try
					{
						ApplicationFeedbackType objApplicationFeedbackType = new ApplicationFeedbackType();
						objApplicationFeedbackType.Name  = (columnValue);
						db.ApplicationFeedbackTypes.Add(objApplicationFeedbackType);
						db.SaveChanges();
						model.AssociatedApplicationFeedbackTypeID = objApplicationFeedbackType.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "AssociatedApplicationFeedbackStatusID":
		 var strPropertyAssociatedApplicationFeedbackStatus = lstEntityProp.FirstOrDefault(p => p.Key == "AssociatedApplicationFeedbackStatusID").Value;
         ModelReflector.Property propAssociatedApplicationFeedbackStatus = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedbackStatus").Properties.FirstOrDefault(p => p.DisplayName == strPropertyAssociatedApplicationFeedbackStatus);
         var elementTypeAssociatedApplicationFeedbackStatus = db.ApplicationFeedbackStatuss.ElementType;
         var propertyInfoAssociatedApplicationFeedbackStatus = elementTypeAssociatedApplicationFeedbackStatus.GetProperty(propAssociatedApplicationFeedbackStatus.Name);
         var associatedapplicationfeedbackstatusId = db.ApplicationFeedbackStatuss.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoAssociatedApplicationFeedbackStatus.GetValue(p, null)) == columnValue);
		 if (associatedapplicationfeedbackstatusId != null)
			model.AssociatedApplicationFeedbackStatusID = associatedapplicationfeedbackstatusId.Id;
		  else
			{
				if ((collection["AssociatedApplicationFeedbackStatusID"].ToString() == "true,false"))
				{
					try
					{
						ApplicationFeedbackStatus objApplicationFeedbackStatus = new ApplicationFeedbackStatus();
						objApplicationFeedbackStatus.Name  = (columnValue);
						db.ApplicationFeedbackStatuss.Add(objApplicationFeedbackStatus);
						db.SaveChanges();
						model.AssociatedApplicationFeedbackStatusID = objApplicationFeedbackStatus.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "ApplicationFeedbackPriorityID":
		 var strPropertyApplicationFeedbackPriority = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackPriorityID").Value;
         ModelReflector.Property propApplicationFeedbackPriority = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackPriority").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackPriority);
         var elementTypeApplicationFeedbackPriority = db.FeedbackPrioritys.ElementType;
         var propertyInfoApplicationFeedbackPriority = elementTypeApplicationFeedbackPriority.GetProperty(propApplicationFeedbackPriority.Name);
         var applicationfeedbackpriorityId = db.FeedbackPrioritys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackPriority.GetValue(p, null)) == columnValue);
		 if (applicationfeedbackpriorityId != null)
			model.ApplicationFeedbackPriorityID = applicationfeedbackpriorityId.Id;
		  else
			{
				if ((collection["ApplicationFeedbackPriorityID"].ToString() == "true,false"))
				{
					try
					{
						FeedbackPriority objFeedbackPriority = new FeedbackPriority();
						objFeedbackPriority.Name  = (columnValue);
						db.FeedbackPrioritys.Add(objFeedbackPriority);
						db.SaveChanges();
						model.ApplicationFeedbackPriorityID = objFeedbackPriority.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "ApplicationFeedbackSeverityID":
		 var strPropertyApplicationFeedbackSeverity = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackSeverityID").Value;
         ModelReflector.Property propApplicationFeedbackSeverity = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackSeverity").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackSeverity);
         var elementTypeApplicationFeedbackSeverity = db.FeedbackSeveritys.ElementType;
         var propertyInfoApplicationFeedbackSeverity = elementTypeApplicationFeedbackSeverity.GetProperty(propApplicationFeedbackSeverity.Name);
         var applicationfeedbackseverityId = db.FeedbackSeveritys.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackSeverity.GetValue(p, null)) == columnValue);
		 if (applicationfeedbackseverityId != null)
			model.ApplicationFeedbackSeverityID = applicationfeedbackseverityId.Id;
		  else
			{
				if ((collection["ApplicationFeedbackSeverityID"].ToString() == "true,false"))
				{
					try
					{
						FeedbackSeverity objFeedbackSeverity = new FeedbackSeverity();
						objFeedbackSeverity.Name  = (columnValue);
						db.FeedbackSeveritys.Add(objFeedbackSeverity);
						db.SaveChanges();
						model.ApplicationFeedbackSeverityID = objFeedbackSeverity.Id;
					}
					catch
					{
						continue;
					}
				}
			}
         break;
		 case "ApplicationFeedbackResourceID":
		 var strPropertyApplicationFeedbackResource = lstEntityProp.FirstOrDefault(p => p.Key == "ApplicationFeedbackResourceID").Value;
         ModelReflector.Property propApplicationFeedbackResource = ModelReflector.Entities.FirstOrDefault(p => p.Name == "FeedbackResource").Properties.FirstOrDefault(p => p.DisplayName == strPropertyApplicationFeedbackResource);
         var elementTypeApplicationFeedbackResource = db.FeedbackResources.ElementType;
         var propertyInfoApplicationFeedbackResource = elementTypeApplicationFeedbackResource.GetProperty(propApplicationFeedbackResource.Name);
         var applicationfeedbackresourceId = db.FeedbackResources.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoApplicationFeedbackResource.GetValue(p, null)) == columnValue);
		 if (applicationfeedbackresourceId != null)
			model.ApplicationFeedbackResourceID = applicationfeedbackresourceId.Id;
		  else
			{
				if ((collection["ApplicationFeedbackResourceID"].ToString() == "true,false"))
				{
					try
					{
						FeedbackResource objFeedbackResource = new FeedbackResource();
						objFeedbackResource.ResourceId  = Convert.ToInt64(columnValue);
				 try { objFeedbackResource.FirstName =(columnValue); }
                 catch { objFeedbackResource.FirstName = default(string); }
						db.FeedbackResources.Add(objFeedbackResource);
						db.SaveChanges();
						model.ApplicationFeedbackResourceID = objFeedbackResource.Id;
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
					    if (ValidateModel(model))
                        {
							var result = CheckMandatoryProperties(model);
                            if (result == null || result.Count == 0)
                            {
								db.ApplicationFeedbacks.Add(model);
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
                                    ViewBag.ImportError = "Row No : " + (i + 1) + " Some Required Value Missing";
                                else
                                    ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " Some Required Value Missing";
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
		public bool ValidateModel(ApplicationFeedback validate)
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
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReadOnlyProperties(ApplicationFeedback OModel)
        {
			Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "ApplicationFeedback").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "ApplicationFeedback");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(4))
					{
						var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
						foreach (string paramName in Args.Select(p => p.ParameterName))
						{
							var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedback").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
							RequiredProperties.Add(paramName, dispName);
						}
					}
				}
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(ApplicationFeedback OModel)
        {
		Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
		if(User.businessrules != null)
		{
          var BR = User.businessrules.Where(p => p.EntityName == "ApplicationFeedback").ToList();
            if (BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR,"ApplicationFeedback");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach (string paramName in Args.Select(p => p.ParameterName))
                    {
                        var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedback").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                        RequiredProperties.Add(paramName, dispName);
                    }
                }
            }
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(ApplicationFeedback OModel)
        {
            string RulesApplied = "";
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "ApplicationFeedback").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.LockEntityRule(OModel, BR,"ApplicationFeedback");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(1))
					{
						RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
					}
				}
			}
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
		private List<string> CheckMandatoryProperties(ApplicationFeedback OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "ApplicationFeedback").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "ApplicationFeedback");
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
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ApplicationFeedback").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
            var _Obj = db1.ApplicationFeedbacks.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if (_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
        {
            var obj1 = db.ApplicationFeedbacks.Find(Id);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
        }
    }
}

