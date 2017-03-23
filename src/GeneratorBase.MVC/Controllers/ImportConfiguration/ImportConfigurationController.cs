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
namespace GeneratorBase.MVC.Controllers
{
    public partial class ImportConfigurationController : BaseController
    {
		private Object getImportConfigurationList(IQueryable<ImportConfiguration> importconfiguration)
        {
            var _importconfiguration = from obj in importconfiguration
                            select new
                            {
								TableColumn = obj.TableColumn,SheetColumn = obj.SheetColumn,UniqueColumn = obj.UniqueColumn,LastUpdate = obj.LastUpdate,LastUpdateUser = obj.LastUpdateUser,Name = obj.Name,DisplayValue = obj.DisplayValue, ID = obj.Id
							};
            return _importconfiguration;
        }
		private Object getImportConfigurationItem(ImportConfiguration importconfiguration)
        {
            return new
            {
				TableColumn=importconfiguration.TableColumn,SheetColumn=importconfiguration.SheetColumn,UniqueColumn=importconfiguration.UniqueColumn,LastUpdate=importconfiguration.LastUpdate,LastUpdateUser=importconfiguration.LastUpdateUser,Name=importconfiguration.Name,ConcurrencyKey = importconfiguration.ConcurrencyKey==null?null:System.Convert.ToBase64String(importconfiguration.ConcurrencyKey, 0, importconfiguration.ConcurrencyKey.Length),DisplayValue = importconfiguration.DisplayValue, ID = importconfiguration.Id
			};
        }
		private IQueryable<ImportConfiguration> searchRecords(IQueryable<ImportConfiguration> lstImportConfiguration, string searchString, bool? IsDeepSearch)
        {
		if(Convert.ToBoolean(IsDeepSearch))
            lstImportConfiguration = lstImportConfiguration.Where(s => (!String.IsNullOrEmpty(s.TableColumn) && s.TableColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.SheetColumn) && s.SheetColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.UniqueColumn) && s.UniqueColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.LastUpdateUser) && s.LastUpdateUser.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)) );
		else
			 lstImportConfiguration = lstImportConfiguration.Where(s => (!String.IsNullOrEmpty(s.TableColumn) && s.TableColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.SheetColumn) && s.SheetColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.UniqueColumn) && s.UniqueColumn.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)) );
			try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstImportConfiguration = lstImportConfiguration.Union(db.ImportConfigurations.Where(s => (s.LastUpdate == datevalue) ));
            }
            catch { }
            return lstImportConfiguration;
        }
		private IQueryable<ImportConfiguration> sortRecords(IQueryable<ImportConfiguration> lstImportConfiguration, string sortBy, string isAsc)
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
            ParameterExpression paramExpression = Expression.Parameter(typeof(ImportConfiguration), "importconfiguration");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<ImportConfiguration>)lstImportConfiguration.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstImportConfiguration.ElementType, lambda.Body.Type },
                    lstImportConfiguration.Expression,
                    lambda));
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity )
        {
			int Qcount = Request.UrlReferrer.Query.Count();
			ViewBag.CurrentFilter = searchString;
			if(Qcount > 0)
			{
			}
			else
			{
			}
                return View();
            }
		 // GET: /ImportConfiguration/FSearch/
		 public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport  ,string LastUpdateFrom,string LastUpdateTo)//, string HostingEntity
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
				 var lstImportConfiguration  = from s in db.ImportConfigurations
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstImportConfiguration  = searchRecords(lstImportConfiguration, searchString.ToUpper(),true);
            }
			if(!string.IsNullOrEmpty(search)){
				ViewBag.SearchResult += "\r\n General Criterial= "+search;
				lstImportConfiguration = searchRecords(lstImportConfiguration, search,true);
			}
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstImportConfiguration  = sortRecords(lstImportConfiguration, sortBy, isAsc);
            }
            else   lstImportConfiguration  = lstImportConfiguration.OrderByDescending(c => c.Id);
			lstImportConfiguration = lstImportConfiguration;
            var _ImportConfiguration = lstImportConfiguration;
		 if(LastUpdateFrom!=null && LastUpdateTo !=null)
            {
                try
                {
                    DateTime from = Convert.ToDateTime(LastUpdateFrom);
                    DateTime to = Convert.ToDateTime(LastUpdateTo);
                    _ImportConfiguration =  _ImportConfiguration.Where(o => o.LastUpdate >= from && o.LastUpdate <= to);
					ViewBag.SearchResult += "\r\n Last Update= "+LastUpdateFrom+"-"+LastUpdateTo;
                }
                catch { }
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
            if (Request.Cookies["pageSize" + User.Name + "ImportConfiguration"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + User.Name + "ImportConfiguration"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_ImportConfiguration.Count() > 0)
                    pageSize = _ImportConfiguration.Count();
                return View("ExcelExport", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
            }
           // return View("Index", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
			if (!Request.IsAjaxRequest())
                return View("Index",_ImportConfiguration.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _ImportConfiguration.ToPagedList(pageNumber, pageSize));
        }
        public string GetDisplayValue(string id)
        {
			ApplicationContext db1 = new ApplicationContext(User);
			if (string.IsNullOrEmpty(id)) return "";
			var _Obj = db1.ImportConfigurations.Find(Convert.ToInt64(id));
            return  _Obj==null?"":_Obj.DisplayValue;
        }
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAllValue(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
			IQueryable<ImportConfiguration> list = db.ImportConfigurations;
			            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.ImportConfigurations;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(ImportConfiguration), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<ImportConfiguration, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<ImportConfiguration>)q);
                }
            }
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
        public JsonResult GetAllMultiSelectValue(string key, string AssoNameWithParent, string AssociationID)
        {
			IQueryable<ImportConfiguration> list = db.ImportConfigurations;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.ImportConfigurations;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(ImportConfiguration), "p"));
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
                list = ((IQueryable<ImportConfiguration>)q);
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
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (fileExtension == ".xlsx")
            {
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            excelConnection.Open();
            DataTable dt = new DataTable();
            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null)
            {
                return null;
            }
            String[] excelSheets = new String[dt.Rows.Count];
            int t = 0;
            foreach (DataRow row in dt.Rows)
            {
                excelSheets[t] = row["TABLE_NAME"].ToString();
                t++;
            }
            excelConnection.Close();
            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
            DataSet objDataSet = new DataSet();
            string query = string.Format("Select * from [{0}]", excelSheets[0]);
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
            {
                dataAdapter.Fill(objDataSet);
            }
            excelConnection1.Close();
            return objDataSet;
        }
		[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
         public ActionResult Upload([Bind(Include = "FileUpload")] HttpPostedFileBase FileUpload)
        {
            if (FileUpload != null)
            {
                 string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelFiles"), System.IO.Path.GetFileName(FileUpload.FileName));
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
                    Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>();
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "ImportConfiguration");
                    if (entList != null)
                    {
                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                        {
                            long selectedVal = 0;
                            var colSelected = col.FirstOrDefault(p=> p.Text.Trim() == prop.DisplayName.Trim());
                            if (colSelected != null)
                                selectedVal = long.Parse(colSelected.Value);
                            objColMap.Add(prop, new SelectList(col,"Value", "Text", selectedVal));
                        }
                    }
                    ViewBag.ColumnMapping = objColMap;
                    ViewBag.FilePath = fileLocation;
                }
                else
                {
                    ModelState.AddModelError("", "Plese select Excel File.");
                }
            }
            return View("Index");
        }
		  [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult ConfirmImportData(FormCollection collection)
        {
            string FilePath = collection["hdnFilePath"];
            var columnlist = collection["lblColumn"];
            var columndisplaynamelist = collection["lblColumnDisplayName"];
            var selectedlist = collection["colList"];
            string DetailMessage = "";
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string fileExtension = System.IO.Path.GetExtension(fileLocation);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
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
                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new importconfigurations";
                Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "ImportConfiguration");
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
                                default:
                                    break;
                            }
                            #endregion
                        }
                        if (uniqueassoValues.Count > 0)
                        {
                            DetailMessage += ", " + uniqueassoValues.Count() + " new " + association.DisplayName + "s";
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
                if (ViewBag.ConfirmImportData != null)
                    return View("Index");
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
            string fileExtension = System.IO.Path.GetExtension(fileLocation);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                DataSet objDataSet = DataImport(fileExtension, fileLocation);
                if (System.IO.File.Exists(fileLocation))
                    System.IO.File.Delete(fileLocation);
                if (selectedlist != null && columnlist != null)
                {
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
						  var sheetColumns = selectedlist.Split(',').ToList();
                        if (AreAllColumnsEmpty(objDataSet.Tables[0].Rows[i], sheetColumns))
                            continue;
                        ImportConfiguration model = new ImportConfiguration();
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
    case "TableColumn":
    model.TableColumn = columnValue;
	break;
    case "SheetColumn":
    model.SheetColumn = columnValue;
	break;
    case "UniqueColumn":
    model.UniqueColumn = columnValue;
	break;
    case "LastUpdate":
    model.LastUpdate = DateTime.Parse(columnValue);
	break;
    case "Name":
    model.Name = columnValue;
	break;
                                default:
                                    break;
                            }
                        }
					    if (ValidateModel(model))
                        {
                            db.ImportConfigurations.Add(model);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (ViewBag.ImportError == null)
                                ViewBag.ImportError = "Row No : " + (i + 1) + " Some Required Value Missing";
                            else
                                ViewBag.ImportError += "<br/> Row No : " + (i + 1) + " Some Required Value Missing";
                        }
                    }
                }
                 if (ViewBag.ImportError != null)
                    return View("Index");
                else
                    return RedirectToAction("Index");
            }
            return View();
        }
		public bool ValidateModel(ImportConfiguration validate)
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
        public JsonResult GetReadOnlyProperties(ImportConfiguration OModel)
        {
			Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "ImportConfiguration").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "ImportConfiguration");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(4))
					{
						var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
						foreach (string paramName in Args.Select(p => p.ParameterName))
						{
							var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ImportConfiguration").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
							RequiredProperties.Add(paramName, dispName);
						}
					}
				}
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
		[AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(ImportConfiguration OModel)
        {
		Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
		if(User.businessrules != null)
		{
          var BR = User.businessrules.Where(p => p.EntityName == "ImportConfiguration").ToList();
            if (BR != null && BR.Count > 0)
            {
                var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR,"ImportConfiguration");
                BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(2))
                {
                    var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                    foreach (string paramName in Args.Select(p => p.ParameterName))
                    {
                        var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "ImportConfiguration").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                        RequiredProperties.Add(paramName, dispName);
                    }
                }
            }
			}
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(ImportConfiguration OModel)
        {
            string RulesApplied = "";
			if(User.businessrules != null)
			{
				var BR = User.businessrules.Where(p => p.EntityName == "ImportConfiguration").ToList();
				if (BR != null && BR.Count > 0)
				{
					var ResultOfBusinessRules = db.LockEntityRule(OModel, BR,"ImportConfiguration");
					BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
					if (ResultOfBusinessRules.Keys.Select(p=>p.TypeNo).Contains(1))
					{
						RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
					}
				}
			}
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
    }
}

