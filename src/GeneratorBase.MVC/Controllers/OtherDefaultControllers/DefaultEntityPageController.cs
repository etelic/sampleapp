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
    public partial class DefaultEntityPageController : BaseController
    {
        private void LoadViewDataAfterOnEdit(DefaultEntityPage defaultentitypage)
        {

        }
        private void LoadViewDataBeforeOnEdit(DefaultEntityPage defaultentitypage)
        {
            ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName", defaultentitypage.EntityName);
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles(); 
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "", defaultentitypage.Roles);
            ViewBag.Roles = defaultentitypage.Roles;

            var favorites = db.FavoriteItems;
            ViewBag.Favorites = new SelectList(favorites, "LinkAddress", "Name", defaultentitypage.PageUrl);
            ViewBag.SelectedOtherVal = defaultentitypage.PageUrl;

            ViewBag.PageType = defaultentitypage.PageType;
            ViewBag.ViewEntityPage = defaultentitypage.ViewEntityPage;
            ViewBag.ListEntityPage = defaultentitypage.ListEntityPage;
            ViewBag.EditEntityPage = defaultentitypage.EditEntityPage;
            ViewBag.SearchEntityPage = defaultentitypage.SearchEntityPage;
            ViewBag.DefaultUrl = defaultentitypage.PageUrl;

        }
        private void LoadViewDataAfterOnCreate(DefaultEntityPage defaultentitypage)
        {
            ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles(); 
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            var favorites = db.FavoriteItems;
            ViewBag.Favorites = new SelectList(favorites, "LinkAddress", "Name");
        }
        private void LoadViewDataBeforeOnCreate(string HostingEntityName, string HostingEntityID, string AssociatedType)
        {
            ViewBag.EntityName = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles(); 
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            var favorites = db.FavoriteItems;
            ViewBag.Favorites = new SelectList(favorites, "LinkAddress", "Name");
        }




        private IQueryable<DefaultEntityPage> searchRecords(IQueryable<DefaultEntityPage> lstDefaultEntityPage, string searchString, bool? IsDeepSearch)
        {
            searchString = searchString.Trim();
            if (Convert.ToBoolean(IsDeepSearch))
                lstDefaultEntityPage = lstDefaultEntityPage.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Roles) && s.Roles.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PageType) && s.PageType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PageUrl) && s.PageUrl.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Other) && s.Other.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            else
                lstDefaultEntityPage = lstDefaultEntityPage.Where(s => (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Roles) && s.Roles.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PageType) && s.PageType.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.PageUrl) && s.PageUrl.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Other) && s.Other.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            try
            {
                var boolvalue = Convert.ToBoolean(searchString);
                lstDefaultEntityPage = lstDefaultEntityPage.Union(db.DefaultEntityPages.Where(s => (s.Flag == boolvalue)));
            }
            catch { }
            return lstDefaultEntityPage;
        }


        private IQueryable<DefaultEntityPage> sortRecords(IQueryable<DefaultEntityPage> lstDefaultEntityPage, string sortBy, string isAsc)
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
            ParameterExpression paramExpression = Expression.Parameter(typeof(DefaultEntityPage), "defaultentitypage");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<DefaultEntityPage>)lstDefaultEntityPage.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstDefaultEntityPage.ElementType, lambda.Body.Type },
                    lstDefaultEntityPage.Expression,
                    lambda));
        }

        public ActionResult SetFSearch(string searchString, string HostingEntity)
        {
            int Qcount = Request.UrlReferrer.Query.Count();
            ViewBag.CurrentFilter = searchString;

            if (Qcount > 0)
            {
            }
            else
            {
            }
            return View();
        }

        // GET: /DefaultEntityPage/FSearch/
        public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string Flag)//, string HostingEntity
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


            var lstDefaultEntityPage = from s in db.DefaultEntityPages
                                       select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstDefaultEntityPage = searchRecords(lstDefaultEntityPage, searchString.ToUpper(), true);
            }
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.SearchResult += "\r\n General Criterial= " + search;
                lstDefaultEntityPage = searchRecords(lstDefaultEntityPage, search, true);
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstDefaultEntityPage = sortRecords(lstDefaultEntityPage, sortBy, isAsc);
            }
            else lstDefaultEntityPage = lstDefaultEntityPage.OrderByDescending(c => c.Id);
            lstDefaultEntityPage = lstDefaultEntityPage;
            var _DefaultEntityPage = lstDefaultEntityPage;

            if (Flag != null)
            {
                try
                {
                    bool boolvalue = Convert.ToBoolean(Flag);
                    _DefaultEntityPage = _DefaultEntityPage.Where(o => o.Flag == boolvalue);
                    ViewBag.SearchResult += "\r\n Flag= " + Flag;
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
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "DefaultEntityPage"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_DefaultEntityPage.Count() > 0)
                    pageSize = _DefaultEntityPage.Count();
                return View("ExcelExport", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            }

            // return View("Index", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            if (!Request.IsAjaxRequest())
                return View("Index", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _DefaultEntityPage.ToPagedList(pageNumber, pageSize));


        }


        public string GetDisplayValue(string id)
        {
            ApplicationContext db1 = new ApplicationContext(User);
            if (string.IsNullOrEmpty(id)) return "";
            var _Obj = db1.DefaultEntityPages.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<DefaultEntityPage> list = db.DefaultEntityPages;
            var data = from x in list.OrderBy(q => q.DisplayValue).ToList()
                       select new { Id = x.Id, Name = x.DisplayValue };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {

            IQueryable<DefaultEntityPage> list = db.DefaultEntityPages;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.DefaultEntityPages;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(DefaultEntityPage), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<DefaultEntityPage, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);

                    IQueryable q = query.Provider.CreateQuery(methodCall);

                    list = ((IQueryable<DefaultEntityPage>)q);
                }
            }
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
        public JsonResult GetAllValueMobile(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {

            IQueryable<DefaultEntityPage> list = db.DefaultEntityPages;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                try
                {
                    Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                    if (AssoID != null && AssoID > 0)
                    {
                        IQueryable query = db.DefaultEntityPages;
                        Type[] exprArgTypes = { query.ElementType };
                        string propToWhere = AssoNameWithParent;
                        ParameterExpression p = Expression.Parameter(typeof(DefaultEntityPage), "p");
                        MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                        LambdaExpression lambda = Expression.Lambda<Func<DefaultEntityPage, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);

                        IQueryable q = query.Provider.CreateQuery(methodCall);

                        list = ((IQueryable<DefaultEntityPage>)q).Take(20);
                    }
                }
                catch
                {
                    var data = from x in list.Take(20).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
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
            IQueryable<DefaultEntityPage> list = db.DefaultEntityPages;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                IQueryable query = db.DefaultEntityPages;
                Type[] exprArgTypes = { query.ElementType };
                string propToWhere = AssoNameWithParent;
                var AssoIDs = AssociationID.Split(',').ToList();
                List<ParameterExpression> paramList = new List<ParameterExpression>();
                paramList.Add(Expression.Parameter(typeof(DefaultEntityPage), "p"));
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
                list = ((IQueryable<DefaultEntityPage>)q);
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
            //ViewBag.IsMapping = (db.ImportConfigurations.Where(p => p.Name == "DefaultEntityPage")).Count() > 0 ? true : false;
            var lstMappings = db.ImportConfigurations.Where(p => p.Name == "DefaultEntityPage").ToList();
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
                    Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, SelectList>();
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "DefaultEntityPage");
                    if (entList != null)
                    {

                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                        {
                            long selectedVal = 0;
                            var colSelected = col.FirstOrDefault(p => p.Text.Trim() == prop.DisplayName.Trim());
                            if (colSelected != null)
                                selectedVal = long.Parse(colSelected.Value);

                            objColMap.Add(prop, new SelectList(col, "Value", "Text", selectedVal));
                        }
                    }

                    ViewBag.ColumnMapping = objColMap;
                    ViewBag.FilePath = fileLocation;

                    if (!string.IsNullOrEmpty(collection["ListOfMappings"]))
                    {
                        string typeName = "";
                        string colKey = "";
                        string colDisKey = "";
                        string colListInx = "";
                        typeName = "DefaultEntityPage";
                        //var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();
                        long idMapping = Convert.ToInt64(collection["ListOfMappings"]);
                        string ExistsColumnMappingName = string.Empty;
                        string mappingName = db.ImportConfigurations.Where(p => p.Name == typeName && p.Id == idMapping && !(string.IsNullOrEmpty(p.SheetColumn))).FirstOrDefault().MappingName;
                        var DefaultMapping = db.ImportConfigurations.Where(p => p.Name == typeName && p.MappingName == mappingName && !(string.IsNullOrEmpty(p.SheetColumn))).ToList();

                        if (collection["DefaultMapping"] == "on")
                        {
                            var lstMapping = db.ImportConfigurations.Where(p => p.Name == "DefaultEntityPage" && p.IsDefaultMapping);
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

                        DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Default Entity Pages";
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
            bool SaveMapping = collection["SaveMapping"] == "on" ? true : false;
            string mappingName = collection["MappingName"];
            string DetailMessage = "";
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string typename = "DefaultEntityPage";
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

                DetailMessage = "We have received " + objDataSet.Tables[0].Rows.Count + " new Default Entity Pages";

                Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>> objAssoUnique = new Dictionary<GeneratorBase.MVC.ModelReflector.Association, List<String>>();
                var entList = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(e => e.Name == "DefaultEntityPage");
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

                        DefaultEntityPage model = new DefaultEntityPage();
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
                                case "Roles":
                                    model.Roles = columnValue;
                                    break;
                                case "PageType":
                                    model.PageType = columnValue;
                                    break;
                                case "PageUrl":
                                    model.PageUrl = columnValue;
                                    break;
                                case "Other":
                                    model.Other = columnValue;
                                    break;
                                case "Flag":
                                    model.Flag = Boolean.Parse(columnValue);
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
                                db.DefaultEntityPages.Add(model);
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
        public bool ValidateModel(DefaultEntityPage validate)
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
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetReadOnlyProperties(DefaultEntityPage OModel)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "DefaultEntityPage").ToList();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.ReadOnlyPropertiesRule(OModel, BR, "DefaultEntityPage");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "DefaultEntityPage").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            RequiredProperties.Add(paramName, dispName);
                        }
                    }
                }
            }
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetMandatoryProperties(DefaultEntityPage OModel)
        {
            Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "DefaultEntityPage").ToList();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "DefaultEntityPage");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(2))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 2).Select(v => v.Value.ActionID).ToList());
                        foreach (string paramName in Args.Select(p => p.ParameterName))
                        {
                            var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "DefaultEntityPage").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                            RequiredProperties.Add(paramName, dispName);
                        }
                    }
                }
            }
            return Json(RequiredProperties, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetLockBusinessRules(DefaultEntityPage OModel)
        {
            string RulesApplied = "";
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "DefaultEntityPage").ToList();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.LockEntityRule(OModel, BR, "DefaultEntityPage");
                    BR = BR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
                    {
                        RulesApplied = string.Join(",", BR.Select(p => p.RuleName).ToArray());
                    }
                }
            }
            return Json(RulesApplied, JsonRequestBehavior.AllowGet);
        }
        private List<string> CheckMandatoryProperties(DefaultEntityPage OModel)
        {
            List<string> result = new List<string>();
            if (User.businessrules != null)
            {
                var BR = User.businessrules.Where(p => p.EntityName == "DefaultEntityPage").ToList();
                Dictionary<string, string> RequiredProperties = new Dictionary<string, string>();
                if (BR != null && BR.Count > 0)
                {
                    var ResultOfBusinessRules = db.MandatoryPropertiesRule(OModel, BR, "DefaultEntityPage");
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
                                var dispName = ModelReflector.Entities.FirstOrDefault(p => p.Name == "DefaultEntityPage").Properties.FirstOrDefault(p => p.Name == paramName).DisplayName;
                                result.Add(dispName);
                            }
                        }

                    }
                }
            }
            return result;
        }
    }
}

