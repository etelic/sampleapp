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
    public partial class T_StudentPerformanceController : BaseController
    {
		private void LoadViewDataForCount(T_StudentPerformance t_studentperformance, string AssocType)
        {
        }
		private void LoadViewDataAfterOnEdit(T_StudentPerformance t_studentperformance)
        {
			 ViewBag.T_StudentCodeID = new SelectList(db.T_Students, "ID", "DisplayValue", t_studentperformance.T_StudentCodeID);
        }
        private void LoadViewDataBeforeOnEdit(T_StudentPerformance t_studentperformance)
        {
               var _objT_StudentCode = new List<T_Student>();
               _objT_StudentCode.Add(t_studentperformance.t_studentcode);
			   			   ViewBag.T_StudentCodeID = new SelectList(_objT_StudentCode, "ID", "DisplayValue", t_studentperformance.T_StudentCodeID);
			        }
        private void LoadViewDataAfterOnCreate(T_StudentPerformance t_studentperformance)
        {
           			 ViewBag.T_StudentCodeID = new SelectList(db.T_Students, "ID", "DisplayValue", t_studentperformance.T_StudentCodeID);
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
			if(HostingEntityName == "T_Student" && Convert.ToInt64(HostingEntityID) > 0 && AssociatedType=="T_StudentCode")
			{
			    long hostid = Convert.ToInt64(HostingEntityID);
				ViewBag.T_StudentCodeID = new SelectList(db.T_Students.Where(p=>p.Id==hostid).ToList(), "ID", "DisplayValue",HostingEntityID);
				ViewBag.DisplayVal = db.T_Students.Where(p => p.Id == hostid).ToList().FirstOrDefault().DisplayValue;
		    }
            else
			{
			var objT_StudentCode = new List<T_Student>();
			 ViewBag.T_StudentCodeID = new SelectList(objT_StudentCode , "ID", "DisplayValue");
		    }
        }
		private IQueryable<T_StudentPerformance> searchRecords(IQueryable<T_StudentPerformance> lstT_StudentPerformance, string searchString, bool? IsDeepSearch)
        {
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_StudentPerformance = lstT_StudentPerformance.Where(s => (s.t_studentcode!= null && (s.t_studentcode.DisplayValue.ToUpper().Contains(searchString) )) ||(s.T_Mark1 != null && SqlFunctions.StringConvert((double)s.T_Mark1).Contains(searchString)) ||(s.T_Mark2 != null && SqlFunctions.StringConvert((double)s.T_Mark2).Contains(searchString)) ||(s.T_TotalMarks != null && SqlFunctions.StringConvert((double)s.T_TotalMarks).Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Grade) && s.T_Grade.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Remarks) && s.T_Remarks.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_StudentPerformance = lstT_StudentPerformance.Where(s => (s.t_studentcode!= null && (s.t_studentcode.DisplayValue.ToUpper().Contains(searchString) )) ||(s.T_Mark1 != null && SqlFunctions.StringConvert((double)s.T_Mark1).Contains(searchString)) ||(s.T_Mark2 != null && SqlFunctions.StringConvert((double)s.T_Mark2).Contains(searchString)) ||(s.T_TotalMarks != null && SqlFunctions.StringConvert((double)s.T_TotalMarks).Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Grade) && s.T_Grade.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Remarks) && s.T_Remarks.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_StudentPerformance;
        }
		private IQueryable<T_StudentPerformance> sortRecords(IQueryable<T_StudentPerformance> lstT_StudentPerformance, string sortBy, string isAsc)
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
	 if(sortBy == "T_StudentCodeID")
                return isAsc.ToLower() == "asc" ? lstT_StudentPerformance.OrderBy(p => p.t_studentcode.DisplayValue) : lstT_StudentPerformance.OrderByDescending(p => p.t_studentcode.DisplayValue);
            ParameterExpression paramExpression = Expression.Parameter(typeof(T_StudentPerformance), "t_studentperformance");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<T_StudentPerformance>)lstT_StudentPerformance.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstT_StudentPerformance.ElementType, lambda.Body.Type },
                    lstT_StudentPerformance.Expression,
                    lambda));
        }
		public ActionResult FindFSearch(string id, string sourceEntity)
        {
            return null;
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity ,string t_studentcode)
        {
			int Qcount = Request.UrlReferrer.Query.Count();
			ViewBag.CurrentFilter = searchString;
			if(Qcount > 0)
			{
			var T_StudentCodelist = db.T_Students.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_studentcode != null)
            {
                var ids = t_studentcode.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Student= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.T_Students.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
					var list = T_StudentCodelist.Union(db.T_Students.Where(p=>ids1.ToList().Contains(p.Id))).Distinct();
					ViewBag.t_studentcode = new SelectList(list, "ID", "DisplayValue");
			}
			else
			{
				ViewBag.t_studentcode = new SelectList(T_StudentCodelist, "ID", "DisplayValue");
			}
			}
			else
			{
			var objT_StudentCode = new List<T_Student>();
		    ViewBag.t_studentcode = new SelectList(objT_StudentCode, "ID", "DisplayValue");
			}
				var lstFavoriteItem = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name);
				if (lstFavoriteItem.Count() > 0)
                    ViewBag.FavoriteItem = lstFavoriteItem;
				return View();
            }
		 // GET: /T_StudentPerformance/FSearch/
		 public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport ,string t_studentcode ,string T_Mark1From,string T_Mark1To,string T_Mark2From,string T_Mark2To,string T_TotalMarksFrom,string T_TotalMarksTo)//, string HostingEntity
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
				 var lstT_StudentPerformance  = from s in db.T_StudentPerformances
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstT_StudentPerformance  = searchRecords(lstT_StudentPerformance, searchString.ToUpper(),true);
            }
			if(!string.IsNullOrEmpty(search)){
				ViewBag.SearchResult += "\r\n General Criterial= "+search;
				lstT_StudentPerformance = searchRecords(lstT_StudentPerformance, search,true);
			}
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstT_StudentPerformance  = sortRecords(lstT_StudentPerformance, sortBy, isAsc);
            }
            else   lstT_StudentPerformance  = lstT_StudentPerformance.OrderByDescending(c => c.Id);
			lstT_StudentPerformance = lstT_StudentPerformance.Include(t=>t.t_studentcode);
            var _T_StudentPerformance = lstT_StudentPerformance;
		 if(T_Mark1From!=null || T_Mark1To !=null)
         {
                try
                {
                    int from = T_Mark1From == null ? 0 : Convert.ToInt32(T_Mark1From);
                    int to =  T_Mark1To == null ? int.MaxValue : Convert.ToInt32(T_Mark1To);
                    _T_StudentPerformance =  _T_StudentPerformance.Where(o => o.T_Mark1 >= from && o.T_Mark1 <= to);
					ViewBag.SearchResult += "\r\n Mark1= "+from+"-"+to;
                }
                catch { }
          }
		 if(T_Mark2From!=null || T_Mark2To !=null)
         {
                try
                {
                    int from = T_Mark2From == null ? 0 : Convert.ToInt32(T_Mark2From);
                    int to =  T_Mark2To == null ? int.MaxValue : Convert.ToInt32(T_Mark2To);
                    _T_StudentPerformance =  _T_StudentPerformance.Where(o => o.T_Mark2 >= from && o.T_Mark2 <= to);
					ViewBag.SearchResult += "\r\n Mark2= "+from+"-"+to;
                }
                catch { }
          }
		 if(T_TotalMarksFrom!=null || T_TotalMarksTo !=null)
         {
                try
                {
                    int from = T_TotalMarksFrom == null ? 0 : Convert.ToInt32(T_TotalMarksFrom);
                    int to =  T_TotalMarksTo == null ? int.MaxValue : Convert.ToInt32(T_TotalMarksTo);
                    _T_StudentPerformance =  _T_StudentPerformance.Where(o => o.T_TotalMarks >= from && o.T_TotalMarks <= to);
					ViewBag.SearchResult += "\r\n Total Marks= "+from+"-"+to;
                }
                catch { }
          }
			//if (lstT_StudentPerformance.Where(p => p.t_studentcode != null).Count() <= 50)
		    //ViewBag.t_studentcode = new SelectList(lstT_StudentPerformance.Where(p => p.t_studentcode != null).Select(P => P.t_studentcode).Distinct(), "ID", "DisplayValue");
            if (t_studentcode != null)
            {
                var ids = t_studentcode.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				 ViewBag.SearchResult += "\r\n Student= ";
                foreach (var str in ids) 
                    if (!string.IsNullOrEmpty(str))
					{
                        ids1.Add(Convert.ToInt64(str));
						 ViewBag.SearchResult += db.T_Students.Find(Convert.ToInt64(str)).DisplayValue+", ";
				    }
                _T_StudentPerformance = _T_StudentPerformance.Where(p => ids1.ToList().Contains(p.T_StudentCodeID));
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
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_StudentPerformance"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "T_StudentPerformance"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_T_StudentPerformance.Count() > 0)
                    pageSize = _T_StudentPerformance.Count();
                return View("ExcelExport", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
            }
           // return View("Index", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
			if (!Request.IsAjaxRequest())
                return View("Index",_T_StudentPerformance.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _T_StudentPerformance.ToPagedList(pageNumber, pageSize));
        }
				public string ShowGraph()
        {
            string result = "";
           {
                var gd = db.T_StudentPerformances.GroupBy(p => p.t_studentcode.DisplayValue).ToList();
                string[] _xval = gd.Select(k => Convert.ToString(k.Key)).ToArray();
                string[] _yval = gd.Select(k => k.Count().ToString()).ToArray();
                var bytes = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column",
                 xValue: _xval,
                 yValues: _yval).AddTitle("Count by Student")
                .GetBytes("png");
                string img = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encoded = Convert.ToBase64String(bytes.ToArray());
                result += String.Format(img, encoded);
			var _yvalT_Mark1 = gd.Select(k => k.Sum(p=>p.T_Mark1)).ToArray();
			var bytesT_Mark1 = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column", 
                 xValue: _xval,
                 yValues: _yvalT_Mark1).AddTitle("Total of Mark1 by Student")
                .GetBytes("png");
                string imgT_Mark1 = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_Mark1 = Convert.ToBase64String(bytesT_Mark1.ToArray());
                result += String.Format(imgT_Mark1, encodedT_Mark1);
			var _yvalT_Mark2 = gd.Select(k => k.Sum(p=>p.T_Mark2)).ToArray();
			var bytesT_Mark2 = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column", 
                 xValue: _xval,
                 yValues: _yvalT_Mark2).AddTitle("Total of Mark2 by Student")
                .GetBytes("png");
                string imgT_Mark2 = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_Mark2 = Convert.ToBase64String(bytesT_Mark2.ToArray());
                result += String.Format(imgT_Mark2, encodedT_Mark2);
			var _yvalT_TotalMarks = gd.Select(k => k.Sum(p=>p.T_TotalMarks)).ToArray();
			var bytesT_TotalMarks = new System.Web.Helpers.Chart(width: 360, height: 230, theme: ChartTheme1.ColumnBar)
                .AddSeries(
                chartType: "Column", 
                 xValue: _xval,
                 yValues: _yvalT_TotalMarks).AddTitle("Total of Total Marks by Student")
                .GetBytes("png");
                string imgT_TotalMarks = "<div class='col-sm-6' style='padding:0px; margin:0px 0px 0px 0px;'><img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap' style='width:100%; border: 1px solid #e3e3e3'></div>";
                string encodedT_TotalMarks = Convert.ToBase64String(bytesT_TotalMarks.ToArray());
                result += String.Format(imgT_TotalMarks, encodedT_TotalMarks);
            }
			 return result;
	    }
        public string GetDisplayValue(string id)
        {
			ApplicationContext db1 = new ApplicationContext(User);
			if (string.IsNullOrEmpty(id)) return "";
			var _Obj = db1.T_StudentPerformances.Find(Convert.ToInt64(id));
            return  _Obj==null?"":_Obj.DisplayValue;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<T_StudentPerformance> list = db.T_StudentPerformances;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
				[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_StudentPerformance> list = db.T_StudentPerformances;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_StudentPerformances;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_StudentPerformance), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_StudentPerformance, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_StudentPerformance>)q);
                }
            }
			FilterApplicationDropdowns _fad = new FilterApplicationDropdowns();
            list = _fad.FilterDropdown<T_StudentPerformance>(User,list, "T_StudentPerformance",caller);
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
		public JsonResult GetAllValueForRB(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_StudentPerformance> list = db.T_StudentPerformances;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.T_StudentPerformances;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(T_StudentPerformance), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<T_StudentPerformance, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<T_StudentPerformance>)q);
                }
            }
			var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
        }
[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValueMobile(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<T_StudentPerformance> list = db.T_StudentPerformances;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
				try
                {
					Nullable<long> AssoID = Convert.ToInt64(AssociationID);
					if (AssoID != null && AssoID > 0)
					{
						IQueryable query = db.T_StudentPerformances;
						Type[] exprArgTypes = { query.ElementType };
						string propToWhere = AssoNameWithParent;
						ParameterExpression p = Expression.Parameter(typeof(T_StudentPerformance), "p");
						MemberExpression member = Expression.PropertyOrField(p, propToWhere);
						LambdaExpression lambda = Expression.Lambda<Func<T_StudentPerformance, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
						MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
						IQueryable q = query.Provider.CreateQuery(methodCall);
						list = ((IQueryable<T_StudentPerformance>)q).Take(20);
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
           list = _fad.FilterDropdown<T_StudentPerformance>(User,list, "T_StudentPerformance",caller);
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
			IQueryable<T_StudentPerformance> list = db.T_StudentPerformances;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.T_StudentPerformances;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(T_StudentPerformance), "p"));
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
                list = ((IQueryable<T_StudentPerformance>)q);
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
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "T_StudentPerformance")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "T_StudentPerformance").ToList();
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
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_StudentPerformance");
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
                        typeName = "T_StudentPerformance";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "T_StudentPerformance" && p.IsDefaultMapping);
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
                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new StudentPerformances";
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
                                										 case "T_StudentCodeID":
										 var t_studentcodeId = db.T_Students.FirstOrDefault(p => p.DisplayValue == assovalue);
										 if (t_studentcodeId == null)
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
			string typename = "T_StudentPerformance";
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
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new StudentPerformances";
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
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "T_StudentPerformance");
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
                                										 case "T_StudentCodeID":
										 var strPropertyT_StudentCode = lstEntityProp.FirstOrDefault(p => p.Key == "T_StudentCodeID").Value;
										 ModelReflector.Property propT_StudentCode = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Student").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_StudentCode);
										 var elementTypeT_StudentCode = db.T_Students.ElementType;
										 var propertyInfoT_StudentCode = elementTypeT_StudentCode.GetProperty(propT_StudentCode.Name);
										 var t_studentcodeId = db.T_Students.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_StudentCode.GetValue(p, null)) == assovalue);
										 if (t_studentcodeId == null)
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
                        T_StudentPerformance model = new T_StudentPerformance();
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
    case "T_Mark1":
    model.T_Mark1 = Int32.Parse(columnValue);
	break;
    case "T_Mark2":
    model.T_Mark2 = Int32.Parse(columnValue);
	break;
    case "T_TotalMarks":
    model.T_TotalMarks = Int32.Parse(columnValue);
	break;
    case "T_Grade":
    model.T_Grade = columnValue;
	break;
    case "T_Remarks":
    model.T_Remarks = columnValue;
	break;
					 case "T_StudentCodeID":
		 dynamic t_studentcodeId = null;
		 if(lstEntityProp.Count > 0)
		 {
			 var strPropertyT_StudentCode = lstEntityProp.FirstOrDefault(p => p.Key == "T_StudentCodeID").Value;
			 ModelReflector.Property propT_StudentCode = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_Student").Properties.FirstOrDefault(p => p.DisplayName == strPropertyT_StudentCode);
			 var elementTypeT_StudentCode = db.T_Students.ElementType;
			 var propertyInfoT_StudentCode = elementTypeT_StudentCode.GetProperty(propT_StudentCode.Name);
			 t_studentcodeId = db.T_Students.AsEnumerable().FirstOrDefault(p => Convert.ToString(propertyInfoT_StudentCode.GetValue(p, null)) == columnValue);
		 }
		 else
			t_studentcodeId = db.T_Students.FirstOrDefault(p=>p.DisplayValue == columnValue);
		 if (t_studentcodeId != null)
			model.T_StudentCodeID = t_studentcodeId.Id;
		  else
			{
				if ((collection["T_StudentCodeID"].ToString() == "true,false"))
				{
					try
					{
						T_Student objT_Student = new T_Student();
						objT_Student.T_Name  = (columnValue);
						db.T_Students.Add(objT_Student);
						db.SaveChanges();
						model.T_StudentCodeID = objT_Student.Id;
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
					    if (ValidateModel(model) && CheckBeforeSave(model))
                        {
							var result = CheckMandatoryProperties(model);
                            if (result == null || result.Count == 0)
                            {
								db.T_StudentPerformances.Add(model);
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
		public bool ValidateModel(T_StudentPerformance validate)
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
        public JsonResult GetReadOnlyProperties(T_StudentPerformance OModel)
        {
			Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "T_StudentPerformance").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "T_StudentPerformance");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
					{
						var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
						foreach (string paramName in Args.Select(p => p.ParameterName))
						{
							var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_StudentPerformance").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
							if(!RequiredProperties.ContainsKey(paramName))
								RequiredProperties.Add(paramName, dispName);
						}
					}
				}
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(T_StudentPerformance OModel)
        {
		Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
		if(User.businessrules != null)
		{
          var BR = User.businessrules.Where(p => p.EntityName == "T_StudentPerformance").ToList();
            if (BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR,"T_StudentPerformance");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach (string paramName in Args.Select(p => p.ParameterName))
                    {
                        var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_StudentPerformance").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
						if(!RequiredProperties.ContainsKey(paramName))
							RequiredProperties.Add(paramName, dispName);
                    }
                }
            }
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(T_StudentPerformance OModel)
        {
            string RulesApplied = "";
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "T_StudentPerformance").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.LockEntityRule(OModel, BR,"T_StudentPerformance");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
					{
						RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
					}
				}
			}
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
		private List<string> CheckMandatoryProperties(T_StudentPerformance OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "T_StudentPerformance").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "T_StudentPerformance");
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
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "T_StudentPerformance").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
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
            var _Obj = db1.T_StudentPerformances.FirstOrDefault(p => p.DisplayValue == displayvalue);
            long outValue;
            if (_Obj != null)
                return Int64.TryParse(_Obj.Id.ToString(), out outValue) ? (long?)outValue : null;
            else return 0;
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPropertyValueByEntityId(long Id, string PropName)
        {
            var obj1 = db.T_StudentPerformances.Find(Id);
            System.Reflection.PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var Property = properties.FirstOrDefault(p => p.Name == PropName);
            object PropValue = Property.GetValue(obj1, null);
            return Json(Convert.ToString(PropValue), JsonRequestBehavior.AllowGet);
        }
		public string checkHidden(string entityName)
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
                    var objRuleActionList = objRuleAction.RuleActions.Where(ra => ra.AssociatedActionTypeID.Value == ActionTypeId && ra.RuleActionID.Value == objBR.Id);
                    if (objRuleActionList.Count() > 0)
                    {
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
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + objCon.Value + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase().indexOf('" + objCon.Value + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = (rbcheck ? "($('input:radio[name= " + objCon.PropertyName + "]:checked').next('span:first')" : "($('option:selected', '#" + objCon.PropertyName + "') ") + ".text().toLowerCase() " + operand + " '" + objCon.Value + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += (rbcheck ? "&& ($('input:radio[name= "+ objCon.PropertyName +"]:checked').next('span:first')" : " && ($('option:selected', '#" + objCon.PropertyName + "')") + ".text().toLowerCase() " + operand + " '" + objCon.Value + "'.toLowerCase())";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (operand.Length > 2)
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + objCon.Value + "'.toLowerCase()) > -1)";
                                            }
                                            else
                                            {
                                                fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase().indexOf('" + objCon.Value + "'.toLowerCase()) > -1)";
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(fnConditionValue))
                                            {
                                                fnConditionValue = "($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + objCon.Value + "'.toLowerCase())";
                                            }
                                            else
                                            {
                                                fnConditionValue += " && ($('#" + objCon.PropertyName + "').val().toLowerCase() " + operand + " '" + objCon.Value + "'.toLowerCase())";
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
        public JsonResult Check1MThresholdValue(T_StudentPerformance t_studentperformance)
        {
            Dictionary<string, string> msgThreshold = new Dictionary<string, string>();
            return Json(msgThreshold, JsonRequestBehavior.AllowGet);
        }
		public bool CheckBeforeDelete(T_StudentPerformance t_studentperformance)
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
		public bool CheckBeforeSave(T_StudentPerformance t_studentperformance)
        {
            var result = true;
            // Write your logic here
 
			if(!result)
			{
			   var AlertMsg = "Validation Alert - Before Save !! Information not saved.";
               ModelState.AddModelError("CustomError", AlertMsg);
			   ViewBag.ApplicationError = AlertMsg;
			}
            return result;
        }
		public void OnDeleting(T_StudentPerformance t_studentperformance)
        {
            // Write your logic here
 
		}
        public void OnSaving(T_StudentPerformance t_studentperformance, ApplicationContext db)
        {
            // Write your logic here
 
        }
		public void AfterSave(T_StudentPerformance t_studentperformance)
        {
            // Write your logic here
 

		}
    }
}


