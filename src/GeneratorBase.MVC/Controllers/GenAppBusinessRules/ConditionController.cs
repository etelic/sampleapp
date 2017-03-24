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
namespace GeneratorBase.MVC.Controllers
{
    public class ConditionController : BaseController
    {
        private ConditionContext db = new ConditionContext();
        private BusinessRuleContext dbRuleConditions = new BusinessRuleContext();
        // GET: /Condition/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? RenderPartial)
        {
            ViewData["HostingEntity"] = HostingEntity;
            ViewData["HostingEntityID"] = HostingEntityID;
            ViewData["AssociatedType"] = AssociatedType;
            var lstCondition = from s in db.Conditions
                               select s;
            lstCondition = lstCondition.OrderByDescending(s => s.Id);
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            var _Condition = lstCondition.Include(t => t.ruleconditions);
            if (HostingEntity == "BusinessRule" && HostingEntityID != null && AssociatedType == "RuleConditions")
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                var brlist = dbRuleConditions.BusinessRules.Where(p => p.Id == hostid).ToList();
                //customized
                if (brlist.Count > 0)
                {
                    var Entity = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => !p.IsAdminEntity && p.Name == brlist[0].EntityName);
                    if (Entity != null)
                        ViewBag.PropertyList = Entity.Properties;
                    else
                        ViewBag.PropertyList = new List<GeneratorBase.MVC.ModelReflector.Property>();
                }
                _Condition = _Condition.Where(p => p.RuleConditionsID == hostid);
            }
            _Condition = _Condition.OrderBy(p=>p.Id);
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_Condition.ToPagedList(pageNumber, pageSize));
            else
            {
                return PartialView("IndexPartial", _Condition.ToPagedList(pageNumber, pageSize));
            }
            return View(_Condition.ToPagedList(pageNumber, pageSize));
        }
        private IQueryable<Condition> searchRecords(IQueryable<Condition> lstCondition, string searchString)
        {
            lstCondition = lstCondition.Where(s => (!String.IsNullOrEmpty(s.PropertyName) && s.PropertyName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Operator) && s.Operator.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Value) && s.Value.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LogicalConnector) && s.LogicalConnector.ToUpper().Contains(searchString)) || (s.ruleconditions != null && (s.ruleconditions.DisplayValue.ToUpper().Contains(searchString))));
            return lstCondition;
        }
        private IQueryable<Condition> sortRecords(IQueryable<Condition> lstCondition, string sortBy, string isAsc)
        {
            var elementType = typeof(Condition);
            var param = Expression.Parameter(elementType, "condition");
            var prop = elementType.GetProperty(sortBy);
            Type type = Nullable.GetUnderlyingType(prop.PropertyType);
            if (type == null)
                type = prop.PropertyType;
            if (type.Equals(typeof(string)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, string>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(char)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, char?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(int)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, int?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(float)) || type.Equals(typeof(double)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, double?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(decimal)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, decimal?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(long)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, long?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            if (type.Equals(typeof(DateTime)))
            {
                var mySortExpression = Expression.Lambda<Func<Condition, DateTime?>>(Expression.Property(param, sortBy), param);
                if (isAsc == "ASC")
                    lstCondition = lstCondition.OrderBy(mySortExpression);
                else
                    lstCondition = lstCondition.OrderByDescending(mySortExpression);
            }
            // This last part won't work but I left it so that it can compile (all routes need to return value etc.)
            return lstCondition;
        }
        // GET: /Condition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }
        // GET: /Condition/Create
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            }
            else
            {
                var objRuleConditions = new List<BusinessRule>();
                ViewBag.RuleConditionsID = new SelectList(objRuleConditions, "ID", "DisplayValue");
            }
            ViewData["ConditionParentUrl"] = UrlReferrer;
            return View();
        }
        // GET: /Condition/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            }
            else
            {
                var objRuleConditions = new List<BusinessRule>();
                ViewBag.RuleConditionsID = new SelectList(objRuleConditions, "ID", "DisplayValue");
            }
            ViewData["ConditionParentUrl"] = UrlReferrer;
            return View();
        }
        // POST: /Condition/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWizard([Bind(Include = "Id,PropertyName,Operator,Value,LogicalConnector,RuleConditionsID")] Condition condition, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules, "ID", "DisplayValue", condition.RuleConditionsID);
            return View(condition);
        }
        // GET: /Condition/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (HostingEntityName == "BusinessRule" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                var brlist = dbRuleConditions.BusinessRules.Where(p => p.Id == hostid).ToList();
                ViewBag.RuleConditionsID = new SelectList(brlist, "ID", "DisplayValue", HostingEntityID);
                //customized
                if (brlist.Count > 0)
                {
                    ViewBag.PropertyList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => !p.IsAdminEntity && p.Name == brlist[0].EntityName).Properties, "Name", "DisplayName");
                } //customized
            }
            else
            {
                var objRuleConditions = new List<BusinessRule>();
                ViewBag.RuleConditionsID = new SelectList(objRuleConditions, "ID", "DisplayValue");
            }
            ViewData["ConditionParentUrl"] = UrlReferrer;
            return View();
        }
        // POST: /Condition/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include = "Id,PropertyName,Operator,Value,LogicalConnector,RuleConditionsID")] Condition condition, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                //,Hash = "Condition"
                return RedirectToAction("Edit", "BusinessRule", new { id = condition.RuleConditionsID });
            }
            ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules, "ID", "DisplayValue", condition.RuleConditionsID);
            return View(condition);
        }
        public ActionResult Cancel(string UrlReferrer)
        {
            if (!string.IsNullOrEmpty(UrlReferrer))
                return Redirect(UrlReferrer);
            else
                return RedirectToAction("Index");
        }
        // POST: /Condition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PropertyName,Operator,Value,LogicalConnector,RuleConditionsID")] Condition condition, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules, "ID", "DisplayValue", condition.RuleConditionsID);
            return View(condition);
        }
        // GET: /Condition/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!User.CanEdit("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            var _objRuleConditions = new List<BusinessRule>();
            _objRuleConditions.Add(condition.ruleconditions);
            ViewBag.RuleConditionsID = new SelectList(_objRuleConditions, "ID", "DisplayValue", condition.RuleConditionsID);
            //customized
            if (condition.RuleConditionsID != null && condition.RuleConditionsID > 0)
            {
                var br = dbRuleConditions.BusinessRules.Find(condition.RuleConditionsID);
                if (br != null)
                    ViewBag.PropertyList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => !p.IsAdminEntity && p.Name == br.EntityName).Properties, "Name", "DisplayName", condition.PropertyName);
            }
            //customized
            if (ViewData["ConditionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/Condition"))
                ViewData["ConditionParentUrl"] = Request.UrlReferrer;
            return View(condition);
        }
        // POST: /Condition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PropertyName,Operator,Value,LogicalConnector,RuleConditionsID")] Condition condition, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condition).State = EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else
                    return RedirectToAction("Index");
            }
            ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules, "ID", "DisplayValue", condition.RuleConditionsID);
            return View(condition);
        }
        // GET: /Condition/EditWizard/5
        public ActionResult EditWizard(int? id)
        {
            if (!User.CanEdit("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            var _objRuleConditions = new List<BusinessRule>();
            _objRuleConditions.Add(condition.ruleconditions);
            ViewBag.RuleConditionsID = new SelectList(_objRuleConditions, "ID", "DisplayValue", condition.RuleConditionsID);
            if (ViewData["ConditionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/Condition"))
                ViewData["ConditionParentUrl"] = Request.UrlReferrer;
            return View(condition);
        }
        // POST: /Condition/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include = "Id,PropertyName,Operator,Value,LogicalConnector,RuleConditionsID")] Condition condition, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(condition).State = EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else
                    return RedirectToAction("Index");
            }
            ViewBag.RuleConditionsID = new SelectList(dbRuleConditions.BusinessRules, "ID", "DisplayValue", condition.RuleConditionsID);
            return View(condition);
        }
        // GET: /Condition/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!User.CanDelete("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            if (ViewData["ConditionParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/Condition"))
                ViewData["ConditionParentUrl"] = Request.UrlReferrer;
            return View(condition);
        }
        // POST: /Condition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!User.CanDelete("Condition"))
            {
                return RedirectToAction("Index", "Error");
            }
            Condition condition = db.Conditions.Find(id);
            BusinessRule BR = dbRuleConditions.BusinessRules.Where(br => br.Id == condition.RuleConditionsID).FirstOrDefault();
            db.Conditions.Remove(condition);
            db.SaveChanges();
            //,Hash = "Condition"
            return RedirectToAction("Edit", "BusinessRule", new { id = BR.Id });
        }
        public ActionResult SetFSearch(string searchString, string HostingEntity)
        {
            ViewBag.CurrentFilter = searchString;
            var lstCondition = from s in db.Conditions
                               select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstCondition = searchRecords(lstCondition, searchString.ToUpper());
            }
            else
                lstCondition = lstCondition.OrderByDescending(s => s.Id);
            if (lstCondition.Where(p => p.RuleConditionsID != null).Select(p => p.RuleConditionsID).Distinct().Count() <= 50)
                ViewBag.ruleconditions = new SelectList(lstCondition.Where(p => p.ruleconditions != null).Select(P => P.ruleconditions).Distinct(), "ID", "DisplayValue");
            return View();
        }
        // GET: /Condition/FSearch/
        public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string ruleconditions)//, string HostingEntity
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
            var lstCondition = from s in db.Conditions
                               select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstCondition = searchRecords(lstCondition, searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstCondition = sortRecords(lstCondition, sortBy, isAsc);
            }
            else
                lstCondition = lstCondition.OrderByDescending(s => s.Id);
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.SearchResult += "\r\n General Criterial= " + search;
                lstCondition = searchRecords(lstCondition, search);
            }
            lstCondition = lstCondition.Include(t => t.ruleconditions);
            var _Condition = lstCondition;
            if (lstCondition.Where(p => p.ruleconditions != null).Count() <= 50)
                ViewBag.ruleconditions = new SelectList(lstCondition.Where(p => p.ruleconditions != null).Select(P => P.ruleconditions).Distinct(), "ID", "DisplayValue");
            if (ruleconditions != null)
            {
                var ids = ruleconditions.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n Business Rule= ";
                foreach (var str in ids)
                    if (!string.IsNullOrEmpty(str))
                    {
                        ids1.Add(Convert.ToInt64(str));
                        ViewBag.SearchResult += dbRuleConditions.BusinessRules.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                    }
                _Condition = _Condition.Where(p => ids1.ToList().Contains(p.RuleConditionsID));
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
            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_Condition.Count() > 0)
                    pageSize = _Condition.Count();
                return View("ExcelExport", _Condition.ToPagedList(pageNumber, pageSize));
            }
            return View("Index", _Condition.ToPagedList(pageNumber, pageSize));
        }
        public string GetDisplayValue(string id)
        {
            var _Obj = db.Conditions.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : _Obj.DisplayValue;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string key, string AssoNameWithParent, string AssociationID)
        {
            IQueryable<Condition> list = db.Conditions.Unfiltered();
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.Conditions;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(Condition), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<Condition, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<Condition>)q);
                }
            }
            if (key != null && key.Length > 0)
            {
                var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).ToList()
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
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (Request.Files["FileUpload"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelFiles"), Request.Files["FileUpload"].FileName);
                    if (System.IO.File.Exists(fileLocation))
                        System.IO.File.Delete(fileLocation);
                    Request.Files["FileUpload"].SaveAs(fileLocation);
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
                    Dictionary<GeneratorBase.MVC.ModelReflector.Property, List<SelectListItem>> objColMap = new Dictionary<GeneratorBase.MVC.ModelReflector.Property, List<SelectListItem>>();
                    var entList = GeneratorBase.MVC.ModelReflector.Entities.Where(e => e.Name == "Issue").ToList()[0];
                    if (entList != null)
                    {
                        foreach (var prop in entList.Properties.Where(p => p.Name != "DisplayValue"))
                        {
                            objColMap.Add(prop, col);
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
        public ActionResult ImportData(FormCollection collection)
        {
            string FilePath = collection["hdnFilePath"];
            var columnlist = collection["lblColumn"];
            var selectedlist = collection["colList"];
            string fileLocation = FilePath;
            string excelConnectionString = string.Empty;
            string fileExtension = System.IO.Path.GetExtension(fileLocation);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
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
                if (selectedlist != null && columnlist != null)
                {
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        Condition model = new Condition();
                        var tblColumns = columnlist.Split(',').ToList();
                        var sheetColumns = selectedlist.Split(',').ToList();
                        for (int j = 0; j < sheetColumns.Count; j++)
                        {
                            string columntable = tblColumns[j];
                            int columnSheet = 0;
                            if (string.IsNullOrEmpty(sheetColumns[j]))
                                continue;
                            else
                                columnSheet = Convert.ToInt32(sheetColumns[j]) - 1;
                            string columnValue = objDataSet.Tables[0].Rows[i][columnSheet].ToString();
                            if (string.IsNullOrEmpty(columnValue))
                                continue;
                            switch (columntable)
                            {
                                case "PropertyName":
                                    model.PropertyName = columnValue;
                                    break;
                                case "Operator":
                                    model.Operator = columnValue;
                                    break;
                                case "Value":
                                    model.Value = columnValue;
                                    break;
                                case "LogicalConnector":
                                    model.LogicalConnector = columnValue;
                                    break;
                                case "RuleConditionsID":
                                    long ruleconditionsId = dbRuleConditions.BusinessRules.Where(p => p.DisplayValue == columnValue).ToList()[0].Id;
                                    model.RuleConditionsID = ruleconditionsId;
                                    break;
                                default:
                                    break;
                            }
                        }
                        db.Conditions.Add(model);
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch { }
                }
                return RedirectToAction("Index");
            }
            return View();
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

