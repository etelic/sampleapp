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
    public class BusinessRuleController : BaseController
    {
        private BusinessRuleContext db = new BusinessRuleContext();
        private BusinessRuleTypeContext dbAssociatedBusinessRuleType = new BusinessRuleTypeContext();

        // GET: /BusinessRule/
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, string HostingEntity, int? HostingEntityID, string AssociatedType, bool? IsExport, bool? IsFilter, bool? RenderPartial, string HostingEntityName, string IsDisable)
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
            ViewData["HostingEntityName"] = HostingEntityName;
            ViewData["IsDisable"] = IsDisable;

            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            var lstBusinessRule = from s in db.BusinessRules
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstBusinessRule = searchRecords(lstBusinessRule, searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstBusinessRule = sortRecords(lstBusinessRule, sortBy, isAsc);
            }
            else
                lstBusinessRule = lstBusinessRule.OrderByDescending(s => s.Id);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "BusinessRule"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(User.Name) + "BusinessRule"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "BusinessRule"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(User.Name) + "BusinessRule"].Value);
                ViewBag.Pages = pageNumber;
            }
            var _BusinessRule = lstBusinessRule.Include(t => t.associatedbusinessruletype);


            if (!string.IsNullOrEmpty(HostingEntityName))
            {
                if (!string.IsNullOrEmpty(HostingEntityName))
                {
                    string hostName = Convert.ToString(HostingEntityName);
                    _BusinessRule = _BusinessRule.Where(p => p.EntityName == hostName);
                }
                else
                    _BusinessRule = _BusinessRule.Where(p => p.EntityName == "");

            }
            if (!string.IsNullOrEmpty(IsDisable))
            {
                if (!string.IsNullOrEmpty(IsDisable))
                {
                    bool IsDisableVal = Convert.ToBoolean(IsDisable);
                    _BusinessRule = _BusinessRule.Where(p => p.Disable == IsDisableVal);
                }
                else
                    _BusinessRule = _BusinessRule.Where(p => p.EntityName == "");

            }

            if (HostingEntity == "BusinessRuleType" && HostingEntityID != null && AssociatedType == "AssociatedBusinessRuleType")
            {
                if (HostingEntityID != null)
                {
                    long hostid = Convert.ToInt64(HostingEntityID);
                    _BusinessRule = _BusinessRule.Where(p => p.AssociatedBusinessRuleTypeID == hostid);
                }
                else
                    _BusinessRule = _BusinessRule.Where(p => p.AssociatedBusinessRuleTypeID == null);

            }

            if (Convert.ToBoolean(IsExport))
            {
                pageNumber = 1;
                if (_BusinessRule.Count() > 0)
                    pageSize = _BusinessRule.Count();
                return View("ExcelExport", _BusinessRule.ToPagedList(pageNumber, pageSize));
            }

            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_BusinessRule.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _BusinessRule.ToPagedList(pageNumber, pageSize));
        }

        private Object getBusinessRuleList(IQueryable<BusinessRule> businessRule)
        {
            var _businessRule = from obj in businessRule
                                select new
                                {
                                    RuleName = obj.RuleName,
                                    EntityName = obj.EntityName,
                                    Roles = obj.Roles,
                                    DateCreated1 = obj.DateCreated1,
                                    ID = obj.Id
                                };
            return businessRule;
        }
        private IQueryable<BusinessRule> searchRecords(IQueryable<BusinessRule> lstBusinessRule, string searchString)
        {
            searchString = searchString.Trim();
            lstBusinessRule = lstBusinessRule.Where(s => (s.Id != null && SqlFunctions.StringConvert((double)s.Id).Contains(searchString)) || (!String.IsNullOrEmpty(s.RuleName) && s.RuleName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.EntityName) && s.EntityName.ToUpper().Contains(searchString)) || (s.Roles != null && (s.Roles.ToUpper().Contains(searchString))) || (s.associatedbusinessruletype != null && (s.associatedbusinessruletype.DisplayValue.ToUpper().Contains(searchString))) || (!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            try
            {
                var datevalue = Convert.ToDateTime(searchString);
                lstBusinessRule = lstBusinessRule.Union(db.BusinessRules.Where(s => (s.DateCreated1 == datevalue)));
            }
            catch { }
            return lstBusinessRule;
        }

        private IQueryable<BusinessRule> sortRecords(IQueryable<BusinessRule> lstBusinessRule, string sortBy, string isAsc)
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
            ParameterExpression paramExpression = Expression.Parameter(typeof(BusinessRule), "BusinessRule");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<BusinessRule>)lstBusinessRule.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstBusinessRule.ElementType, lambda.Body.Type },
                    lstBusinessRule.Expression,
                    lambda));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValueForFilter(string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            var entitylist = db.BusinessRules.Select(p => p.EntityName).Distinct();
            IDictionary<string, string> finallist = new Dictionary<string, string>();
            foreach (var ent in entitylist)
            {
                var displayname = GeneratorBase.MVC.ModelReflector.Entities.FirstOrDefault(p => p.Name == ent).DisplayName;
                finallist.Add(new KeyValuePair<string, string>(ent, displayname));
            }
            return Json(finallist, JsonRequestBehavior.AllowGet);
        }
        // GET: /BusinessRule/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);
            ViewBag.EntityNameRuleAction = businessrule.EntityName;
            if (businessrule == null)
            {
                return HttpNotFound();
            }

            JournalEntryContext jedb = new JournalEntryContext();
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == id).ToList();


            return View(businessrule);
        }

        // GET: /BusinessRule/Create
        public ActionResult Create(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {

            if (!User.CanAdd("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (HostingEntityName == "BusinessRuleType" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            }
            else
            {
                var objAssociatedBusinessRuleType = new List<BusinessRuleType>();
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(objAssociatedBusinessRuleType, "ID", "DisplayValue");
            }

            ViewData["BusinessRuleParentUrl"] = UrlReferrer;

            //
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            //

            return View();
        }

        // GET: /BusinessRule/CreateWizard
        public ActionResult CreateWizard(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {

            if (!User.CanAdd("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (HostingEntityName == "BusinessRuleType" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            }
            else
            {
                var objAssociatedBusinessRuleType = new List<BusinessRuleType>();
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(objAssociatedBusinessRuleType, "ID", "DisplayValue");
            }


            ViewData["BusinessRuleParentUrl"] = UrlReferrer;
            return View();
        }

        // POST: /BusinessRule/CreateWizard
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult CreateWizard([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID,EntitySubscribe,Disable")] BusinessRule businessrule, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {

                db.BusinessRules.Add(businessrule);
                db.SaveChanges();





                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else return RedirectToAction("Index");
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);




            return View(businessrule);
        }

        // GET: /BusinessRule/CreateQuick
        public ActionResult CreateQuick(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (HostingEntityName == "BusinessRuleType" && Convert.ToInt64(HostingEntityID) > 0)
            {
                long hostid = Convert.ToInt64(HostingEntityID);
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes.Where(p => p.Id == hostid).ToList(), "ID", "DisplayValue", HostingEntityID);
            }
            else
            {
                var objAssociatedBusinessRuleType = new List<BusinessRuleType>();
                ViewBag.AssociatedBusinessRuleTypeID = new SelectList(objAssociatedBusinessRuleType, "ID", "DisplayValue");
            }

            ViewData["BusinessRuleParentUrl"] = UrlReferrer;

            //
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            //

            return View();
        }

        // POST: /BusinessRule/CreateQuick
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuick([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {

                db.BusinessRules.Add(businessrule);
                db.SaveChanges();
                return RedirectToAction("Edit", new { Id = businessrule.Id });
                //return Redirect(Request.UrlReferrer.ToString());
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
            return View(businessrule);
        }

        public ActionResult EditBusinessRule(int? id)
        {
            if (!User.CanEdit("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);
            if (businessrule == null)
            {
                return HttpNotFound();
            }

            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName", businessrule.EntityName);

            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");

            Dictionary<string, string> PropertyList = new Dictionary<string, string>();
            PropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(PropertyList, "key", "value");

            ViewBag.IsContainsCondition = businessrule.ruleconditions.Count() > 0 ? true : false;

            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");

            Dictionary<string, string> TimeRuleApplyOn = new Dictionary<string, string>();
            TimeRuleApplyOn.Add("Add", "Add");
            TimeRuleApplyOn.Add("Update", "Update");
            TimeRuleApplyOn.Add("Delete", "Delete");
            ViewBag.TimeRuleApplyOn = new SelectList(TimeRuleApplyOn, "key", "value");

            ViewBag.IsTimeBasedRule = businessrule.EntitySubscribe;
            var timevalue = businessrule.ruleaction.FirstOrDefault().actionarguments.FirstOrDefault(p => p.ParameterName == "TimeValue");
            ViewBag.TimeValue = timevalue != null ? timevalue.ParameterValue : null;
            var NotifyToExtra = businessrule.ruleaction.FirstOrDefault().actionarguments.FirstOrDefault(p => p.ParameterName == "NotifyToExtra");
            ViewBag.NotifyToExtra = NotifyToExtra != null ? NotifyToExtra.ParameterValue : null;
            var AlertMessage = businessrule.ruleaction.FirstOrDefault();
            ViewBag.AlertMessage = AlertMessage != null ? AlertMessage.ErrorMessage : null;



            return View(businessrule);
        }

        public ActionResult CreateBusinessRule(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewData["BusinessRuleParentUrl"] = UrlReferrer;
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue");
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");

            Dictionary<string, string> list = new Dictionary<string, string>();
            ViewBag.EntityVerb = new SelectList(list, "key", "value");
            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");

            Dictionary<string, string> PropertyList = new Dictionary<string, string>();
            PropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(PropertyList, "key", "value");




            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            //NotifyRoleList
            var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();

            ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");

            Dictionary<string, string> TimeRuleApplyOn = new Dictionary<string, string>();
            TimeRuleApplyOn.Add("Add", "OnAdd");
            TimeRuleApplyOn.Add("Update", "OnUpdate");
            TimeRuleApplyOn.Add("Add,Update", "OnAdd&Update");
            // TimeRuleApplyOn.Add("Update", "Update");
            TimeRuleApplyOn.Add("OnPropertyChange", "OnPropertyChange");
            ViewBag.TimeRuleApplyOn = new SelectList(TimeRuleApplyOn, "key", "value");
            ViewBag.Dropdown = new SelectList(list, "key", "value");
            ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7 = ViewBag.SuggestedPropertyValue = new SelectList(list, "key", "value");

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateBusinessRuleNew([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage, string PropertyName, string ConditionOperator, string ConditionValue, string PropertyRuleValue, string PropertyList, string RoleListValue, string PropertyList1Value, string PropertyList7Value, string Emailids, string TimeRuleApplyOnValue, string lblruletype2, string lblruletype3, string lblruletype4, string lblruletype1, string lblruletype5, string lblruletype6, string lblruletype7, string lblruletype8, string Dropdown, string lblrulecondition, string NotifyToRole)
        {
            if (ModelState.IsValid)
            {
                if (businessrule.Id > 0)
                {
                    BusinessRule businessrule1 = db.BusinessRules.Find(businessrule.Id);
                    db.BusinessRules.Remove(businessrule1);
                    db.SaveChanges();
                }

                if (RoleListValue == "")
                    RoleListValue = "All";
                else
                {
                    string[] RolesIdstr = RoleListValue.Split(",".ToCharArray());
                    var target = "All";
                    var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                    if (results.FirstOrDefault() != null && results[0].ToString() == "All")
                        RoleListValue = results[0].ToString();
                }

                businessrule.Roles = RoleListValue;
                businessrule.DateCreated1 = DateTime.Now;
                if (!string.IsNullOrEmpty(lblruletype1))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "LockEntity";
                    newrule.AssociatedActionTypeID = 1;
                    businessrule.ruleaction.Add(newrule);
                }

                if (!string.IsNullOrEmpty(lblruletype7))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "SetValue";
                    newrule.AssociatedActionTypeID = 7;

                    var setvalues = lblruletype7.Split("?".ToCharArray());
                    foreach (var cond in setvalues)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(",".ToCharArray());
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = param[0];
                        newactionargs.ParameterValue = param[2];
                        //todo dyanmic
                        newrule.actionarguments.Add(newactionargs);
                    }


                    businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype8))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "InvokeAction";
                    newrule.AssociatedActionTypeID = 8;

                    var setvalues = lblruletype8.Split(",".ToCharArray());
                    foreach (var cond in setvalues)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(".".ToCharArray(), 2);
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = param[0].Trim();
                        newactionargs.ParameterValue = param[1].Trim();
                        newrule.actionarguments.Add(newactionargs);
                    }


                    businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype6))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 6;
                    foreach (string str in lblruletype6.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Hidden";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    businessrule.ruleaction.Add(newrule);
                }
                // 
                if (!string.IsNullOrEmpty(lblruletype3))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 2;
                    foreach (string str in lblruletype3.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Mandatory";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype2))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 4;
                    foreach (string str in lblruletype2.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Readonly";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype5))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "FilterDropdown";
                    newrule.AssociatedActionTypeID = 5;
                    foreach (string str in lblruletype5.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "FilterDropdown";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype4))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "TimeBasedAlert";
                    newrule.ErrorMessage = AlertMessage;
                    newrule.AssociatedActionTypeID = 3;
                    ActionArgs newActionArgs = new ActionArgs();
                    newActionArgs.ParameterName = "TimeValue";
                    newActionArgs.ParameterValue = TimeValue;
                    newrule.actionarguments.Add(newActionArgs);
                    ActionArgs newActionArgs1 = new ActionArgs();
                    newActionArgs1.ParameterName = "NotifyTo";
                    newActionArgs1.ParameterValue = lblruletype4;
                    newrule.actionarguments.Add(newActionArgs1);

                    if (!string.IsNullOrEmpty(NotifyToExtra))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyToExtra";
                        newActionArgs2.ParameterValue = NotifyToExtra;
                        newrule.actionarguments.Add(newActionArgs2);
                    }
                    if (!string.IsNullOrEmpty(NotifyToRole))
                    {


                        string[] RolesIdstr = NotifyToRole.Split(",".ToCharArray());
                        var target = "0";
                        var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                        if (results.FirstOrDefault() != null && results[0].Equals("0"))
                            NotifyToRole = "0";

                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyToRole";
                        newActionArgs2.ParameterValue = NotifyToRole;
                        newrule.actionarguments.Add(newActionArgs2);
                    }

                    if (!string.IsNullOrEmpty(TimeRuleApplyOnValue))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyOn";
                        if (!string.IsNullOrEmpty(lblrulecondition))
                            newActionArgs2.ParameterValue = "Add,Update";
                        else
                            newActionArgs2.ParameterValue = TimeRuleApplyOnValue;
                        newrule.actionarguments.Add(newActionArgs2);
                    }
                    businessrule.ruleaction.Add(newrule);

                }

                if (!string.IsNullOrEmpty(lblrulecondition))
                {
                    var conditions = lblrulecondition.Split("?".ToCharArray());
                    foreach (var cond in conditions)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(",".ToCharArray());
                        Condition newcondition = new Condition();
                        newcondition.PropertyName = param[0];
                        newcondition.Operator = param[1];
                        newcondition.Value = param[2];
                        businessrule.ruleconditions.Add(newcondition);
                    }
                }
                db.BusinessRules.Add(businessrule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue");
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");

            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");

            Dictionary<string, string> objPropertyList = new Dictionary<string, string>();
            objPropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(objPropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(objPropertyList, "key", "value");

            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            //NotifyRoleList
            var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
            ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");


            return View(businessrule);
        }


        [HttpPost]
        public ActionResult CreateBusinessRule([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage, string PropertyName, string ConditionOperator, string ConditionValue, string PropertyRuleValue, string PropertyList, string RoleListValue, string PropertyList1Value, string PropertyList7Value, string Emailids, string TimeRuleApplyOnValue)
        {
            if (ModelState.IsValid)
            {
                if (businessrule.Id > 0)
                {
                    BusinessRule businessrule1 = db.BusinessRules.Find(businessrule.Id);
                    db.BusinessRules.Remove(businessrule1);
                    db.SaveChanges();
                }



                businessrule.Roles = RoleListValue;
                businessrule.DateCreated1 = DateTime.Now;
                if (businessrule.AssociatedBusinessRuleTypeID == 1)
                {
                    Condition newcondition = new Condition();
                    newcondition.PropertyName = PropertyList;
                    newcondition.Operator = ConditionOperator;
                    newcondition.Value = ConditionValue;

                    businessrule.ruleconditions.Add(newcondition);

                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "LockEntity";
                    newrule.AssociatedActionTypeID = 1;
                    businessrule.ruleaction.Add(newrule);

                    db.BusinessRules.Add(businessrule);
                    db.SaveChanges();
                }
                if (businessrule.AssociatedBusinessRuleTypeID == 2)
                {
                    Condition newcondition = new Condition();
                    newcondition.PropertyName = PropertyList;
                    newcondition.Operator = ConditionOperator;
                    newcondition.Value = ConditionValue;
                    businessrule.ruleconditions.Add(newcondition);

                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = Convert.ToInt64(PropertyRuleValue);

                    foreach (string str in PropertyList1Value.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        if (newrule.AssociatedActionTypeID == 4)
                            newactionargs.ParameterValue = "Mandatory";
                        else
                            newactionargs.ParameterValue = "Readonly";
                        newrule.actionarguments.Add(newactionargs);
                    }

                    businessrule.ruleaction.Add(newrule);


                    db.BusinessRules.Add(businessrule);
                    db.SaveChanges();
                }
                if (businessrule.AssociatedBusinessRuleTypeID == 4 && !string.IsNullOrEmpty(Emailids))
                {
                    Condition newcondition = new Condition();
                    newcondition.PropertyName = PropertyList;
                    newcondition.Operator = ConditionOperator;
                    newcondition.Value = ConditionValue;

                    businessrule.ruleconditions.Add(newcondition);

                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "SendEmail";
                    newrule.AssociatedActionTypeID = 3;
                    newrule.ErrorMessage = Emailids;
                    businessrule.ruleaction.Add(newrule);

                    db.BusinessRules.Add(businessrule);
                    db.SaveChanges();
                }
                if (businessrule.AssociatedBusinessRuleTypeID == 3)
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "TimeBasedAlert";
                    newrule.ErrorMessage = AlertMessage;

                    if (!string.IsNullOrEmpty(ConditionValue) && !string.IsNullOrEmpty(PropertyList) && !string.IsNullOrEmpty(ConditionOperator))
                    {
                        Condition newcondition = new Condition();
                        newcondition.PropertyName = PropertyList;
                        newcondition.Operator = ConditionOperator;
                        newcondition.Value = ConditionValue;
                        businessrule.ruleconditions.Add(newcondition);
                    }

                    ActionArgs newActionArgs = new ActionArgs();
                    newActionArgs.ParameterName = "TimeValue";
                    newActionArgs.ParameterValue = TimeValue;
                    newrule.actionarguments.Add(newActionArgs);
                    ActionArgs newActionArgs1 = new ActionArgs();
                    newActionArgs1.ParameterName = "NotifyTo";
                    newActionArgs1.ParameterValue = NotifyTo;
                    newrule.actionarguments.Add(newActionArgs1);

                    if (!string.IsNullOrEmpty(NotifyToExtra))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyToExtra";
                        newActionArgs2.ParameterValue = NotifyToExtra;
                        newrule.actionarguments.Add(newActionArgs2);
                    }
                    if (!string.IsNullOrEmpty(TimeRuleApplyOnValue))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyOn";
                        newActionArgs2.ParameterValue = TimeRuleApplyOnValue;
                        newrule.actionarguments.Add(newActionArgs2);
                    }


                    businessrule.ruleaction.Add(newrule);
                    businessrule.Roles = RoleListValue;
                    db.BusinessRules.Add(businessrule);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
                //return Redirect(Request.UrlReferrer.ToString());
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue");
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");

            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");

            Dictionary<string, string> objPropertyList = new Dictionary<string, string>();
            objPropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(objPropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(objPropertyList, "key", "value");

            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            return View(businessrule);
        }
        public JsonResult GetUserAssociation(string Entity)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "LoggedInUser");

            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);

            foreach (var asso in Ent.Associations)
            {
                if (asso.Target == "IdentityUser")
                    list.Add(asso.AssociationProperty, asso.DisplayName);
                else
                {
                    var assoEntity = ModelReflector.Entities.First(p => p.Name == asso.Target);
                    foreach (var asso1 in assoEntity.Associations)
                    {
                        if (asso1.Target == "IdentityUser")
                            list.Add(asso.AssociationProperty, asso.DisplayName);
                    }

                }
            }

            var directUserAsso = Ent.Associations.Where(p => p.Target == "IdentityUser");

            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAssociationsofEntity(string Entity)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
            list.Add("Owner", "LoggedInUser");
            var lstemailProp = Ent.Properties.Where(p => p.IsEmailValidation);
            if (lstemailProp.Count() > 0)
            {
                foreach (var emailprop in lstemailProp)
                {
                    list.Add(Ent.Name + "." + emailprop.Name, Ent.DisplayName + "." + emailprop.DisplayName);
                }
            }
            foreach (var asso in Ent.Associations)
            {
                if (asso.Target == "IdentityUser")
                    list.Add(Ent.Name + "." + asso.AssociationProperty, Ent.DisplayName + "." + asso.DisplayName);
                else
                {
                    var assoEntity = ModelReflector.Entities.First(p => p.Name == asso.Target);
                    foreach (var asso1 in assoEntity.Associations)
                    {
                        if (asso1.Target == "IdentityUser")
                        {
                            if (!list.ContainsKey(asso1.Name + "." + asso1.AssociationProperty))
                                list.Add(asso.Name + "." + asso1.AssociationProperty, asso.Name + "." + asso1.DisplayName);
                        }
                        else
                        {
                            var lstassoemailProperties = assoEntity.Properties.Where(p => p.IsEmailValidation);
                            foreach (var emailprop in lstassoemailProperties)
                            {
                                if (!list.ContainsKey(asso.Name + "." + emailprop.Name))
                                    list.Add(asso.Name + "." + emailprop.Name, asso.Name + "." + emailprop.DisplayName);
                            }
                        }
                    }

                }
            }
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertiesofEntity(string Entity)
        {
            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
            var list = Ent.Properties;
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVerbsofEntity(string Entity)
        {
            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
            Dictionary<string, string> verblist = new Dictionary<string, string>();
            foreach (var item in Ent.Verbs.Where(p => p.Name != "BulkUpdate" && p.Name != "BulkDelete"))
            {
                verblist.Add(Ent.Name + "." + item.Name, Ent.DisplayName + "." + item.DisplayName);
            }
            foreach (var item in Ent.Associations)
            {
                if (item.Target == "IdentityUser") continue;
                var TargetEnt = ModelReflector.Entities.First(p => p.Name == item.Target);
                foreach (var itemTarget in TargetEnt.Verbs.Where(p => p.Name != "BulkUpdate" && p.Name != "BulkDelete"))
                {
                    verblist.Add(item.AssociationProperty + "." + TargetEnt.Name + "." + itemTarget.Name, item.DisplayName + "." + TargetEnt.DisplayName + "." + itemTarget.DisplayName);
                }
            }
            return Json(verblist.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAttributesofTargetEntity(string Entity, string AttributeName)
        {
            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
            var asso = Ent.Associations.FirstOrDefault(p => p.AssociationProperty == AttributeName);
            if (asso != null)
            {
                var EntTarget = ModelReflector.Entities.First(p => p.Name == asso.Target);
                var listTarget = EntTarget.Properties;
                return Json(listTarget.ToList(), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTabsofEntity(string Entity)
        {
            var Ent = ModelReflector.Entities.First(p => p.Name == Entity);
            var AssoList = new List<ModelReflector.Association>();

            foreach (var ent in ModelReflector.Entities.Where(p => p != Ent))
            {
                if (ent.Associations.Any(p => p.Target == Ent.Name))
                {
                    AssoList.AddRange(ent.Associations.Where(p => p.Target == Ent.Name));
                }
            }
            return Json(AssoList.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDropdown(string Entity, string Property)
        {
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == Entity);
            var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == Property);
            var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == Property);
            if (AssociationInfo != null)
            {
                return Json(AssociationInfo.Target, JsonRequestBehavior.AllowGet);
            }
            return Json("Failure", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBRDetailsById(string Id)
        {
            long LongId = Convert.ToInt64(Id);
            var br = db.BusinessRules.FirstOrDefault(p => p.Id == LongId);
            return View("ShortDetails", br);
        }

        public ActionResult CreateQuickTimeRule(string UrlReferrer, string HostingEntityName, string HostingEntityID)
        {
            if (!User.CanAdd("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewData["BusinessRuleParentUrl"] = UrlReferrer;
            //
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName");
            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");
            //
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuickTimeRule([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage)
        {
            if (ModelState.IsValid)
            {
                businessrule.EntitySubscribe = true;
                businessrule.DateCreated1 = DateTime.Now;
                RuleAction newrule = new RuleAction();
                newrule.ActionName = "TimeBasedAlert";
                newrule.ErrorMessage = AlertMessage;
                //NotifyTo 
                //NotifyToExtra

                ActionArgs newActionArgs = new ActionArgs();
                newActionArgs.ParameterName = "TimeValue";
                newActionArgs.ParameterValue = TimeValue;
                newrule.actionarguments.Add(newActionArgs);
                ActionArgs newActionArgs1 = new ActionArgs();
                newActionArgs1.ParameterName = "NotifyTo";
                newActionArgs1.ParameterValue = NotifyTo;
                newrule.actionarguments.Add(newActionArgs1);

                if (!string.IsNullOrEmpty(NotifyToExtra))
                {
                    ActionArgs newActionArgs2 = new ActionArgs();
                    newActionArgs2.ParameterName = "NotifyToExtra";
                    newActionArgs2.ParameterValue = NotifyToExtra;
                    newrule.actionarguments.Add(newActionArgs2);
                }
                businessrule.ruleaction.Add(newrule);

                businessrule.Roles = "All";
                db.BusinessRules.Add(businessrule);
                db.SaveChanges();
                return RedirectToAction("Index");
                //return Redirect(Request.UrlReferrer.ToString());
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
            return View(businessrule);
        }

        public ActionResult Cancel(string UrlReferrer)
        {
            return RedirectToAction("Index");
        }


        // POST: /BusinessRule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID")] BusinessRule businessrule, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {

                db.BusinessRules.Add(businessrule);
                db.SaveChanges();
                return RedirectToAction("Edit", new { Id = businessrule.Id });
                //return Redirect(Request.UrlReferrer.ToString());
            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);
            return View(businessrule);
        }
        public ActionResult EnableDisableRule(int? id)
        {
            if (!User.CanEdit("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);
            businessrule.Disable = !businessrule.Disable;
            db.Entry(businessrule).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: /BusinessRule/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!User.CanEdit("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);

            ViewBag.EntityNameRuleAction = businessrule.EntityName;
            if (businessrule == null)
            {
                return HttpNotFound();
            }


            var _objAssociatedBusinessRuleType = new List<BusinessRuleType>();
            _objAssociatedBusinessRuleType.Add(businessrule.associatedbusinessruletype);
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(_objAssociatedBusinessRuleType, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);


            JournalEntryContext jedb = new JournalEntryContext();
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == id).ToList();
            if (ViewData["BusinessRuleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/BusinessRule"))
                ViewData["BusinessRuleParentUrl"] = Request.UrlReferrer;

            //
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName", businessrule.EntityName);
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
            RoleList.Remove(adminString);
            ViewBag.RoleList = new SelectList(RoleList, "", "");
            //NotifyRoleList
            var RoleListNotify = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRolesNotifyRole(adminString).ToList();
            ViewBag.NotifyRoleList = new SelectList(RoleListNotify, "Key", "Value");

            Dictionary<string, string> list = new Dictionary<string, string>();
            ViewBag.EntityVerb = new SelectList(list, "key", "value");

            list.Add("Owner", "LoggedInUser");
            ViewBag.cmbNotifyTo = new SelectList(list, "key", "value");

            Dictionary<string, string> PropertyList = new Dictionary<string, string>();
            PropertyList.Add("0", "--Select Property--");
            ViewBag.PropertyList = new SelectList(PropertyList, "key", "value");
            ViewBag.PropertyList7 = ViewBag.PropertyList1 = new SelectList(PropertyList, "key", "value");



            Dictionary<string, string> TimeRuleApplyOn = new Dictionary<string, string>();
            TimeRuleApplyOn.Add("Add", "OnAdd");
            TimeRuleApplyOn.Add("Update", "OnUpdate");
            TimeRuleApplyOn.Add("Add,Update", "OnAdd&Update");
            TimeRuleApplyOn.Add("OnPropertyChange", "OnPropertyChange");
            ViewBag.TimeRuleApplyOn = new SelectList(TimeRuleApplyOn, "key", "value");
            ViewBag.Dropdown = new SelectList(list, "key", "value");
            ViewBag.SuggestedDynamicValue71 = ViewBag.SuggestedDynamicValue7 = ViewBag.SuggestedPropertyValue7 = ViewBag.SuggestedPropertyValue = new SelectList(list, "key", "value");
            return View(businessrule);
        }

        // POST: /BusinessRule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID,EntitySubscribe,Disable")] BusinessRule businessrule, string UrlReferrer, string TimeValue, string NotifyTo, string NotifyToExtra, string AlertMessage, string PropertyName, string ConditionOperator, string ConditionValue, string PropertyRuleValue, string PropertyList, string RoleListValue, string PropertyList1Value, string PropertyList7Value, string Emailids, string TimeRuleApplyOnValue, string lblruletype2, string lblruletype3, string lblruletype4, string lblruletype1, string lblruletype5, string lblruletype6, string lblruletype7, string lblruletype8, string Dropdown, string lblrulecondition, string NotifyToRole)
        {

            RuleActionContext dbRuleAction = new RuleActionContext();
            ConditionContext dbCondition = new ConditionContext();
            //ActionArgsContext dbActionArgs = new ActionArgsContext();
            if (ModelState.IsValid)
            {

                // businessrule.Roles = RoleListValue;
                if (businessrule.Id > 0)
                {
                    BusinessRule businessrule1 = db.BusinessRules.Find(businessrule.Id);
                    businessrule1.RuleName = businessrule.RuleName;

                    if (RoleListValue == "")
                        RoleListValue = "All";
                    else
                    {
                        string[] RolesIdstr = RoleListValue.Split(",".ToCharArray());
                        var target = "All";
                        var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                        if (results.FirstOrDefault() != null && results[0].ToString() == "All")
                            RoleListValue = results[0].ToString();
                    }
                    businessrule1.Roles = RoleListValue;
                    businessrule1.EntityName = businessrule.EntityName;
                    businessrule1.EntitySubscribe = businessrule.EntitySubscribe;
                    businessrule1.AssociatedBusinessRuleTypeID = businessrule.AssociatedBusinessRuleTypeID;
                    businessrule1.DisplayValue = businessrule.DisplayValue;
                    businessrule1.DateCreated1 = DateTime.Now;
                    businessrule1.Disable = businessrule.Disable;
                    db.Entry(businessrule1).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(lblruletype1))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "LockEntity";
                    newrule.AssociatedActionTypeID = 1;
                    newrule.RuleActionID = businessrule.Id;
                    //businessrule.ruleaction.Add(newrule);
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                }
                if (!string.IsNullOrEmpty(lblruletype7))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "SetValue";
                    newrule.AssociatedActionTypeID = 7;
                    newrule.RuleActionID = businessrule.Id;
                    var setvalues = lblruletype7.Split("?".ToCharArray());
                    foreach (var cond in setvalues)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(",".ToCharArray());
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = param[0];
                        newactionargs.ParameterValue = param[2];
                        //todo dyanmic
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                }
                if (!string.IsNullOrEmpty(lblruletype8))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "InvokeAction";
                    newrule.AssociatedActionTypeID = 8;
                    var setvalues = lblruletype8.Split(",".ToCharArray());
                    foreach (var cond in setvalues)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(".".ToCharArray(), 2);
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = param[0].Trim();
                        newactionargs.ParameterValue = param[1].Trim();
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges(); ;
                }
                if (!string.IsNullOrEmpty(lblruletype6))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 6;
                    newrule.RuleActionID = businessrule.Id;

                    foreach (string str in lblruletype6.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Hidden";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                    // businessrule.ruleaction.Add(newrule);
                }
                // 
                if (!string.IsNullOrEmpty(lblruletype3))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 2;
                    newrule.RuleActionID = businessrule.Id;
                    foreach (string str in lblruletype3.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Mandatory";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                    //businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype2))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "PropertiesRule";
                    newrule.AssociatedActionTypeID = 4;
                    newrule.RuleActionID = businessrule.Id;
                    foreach (string str in lblruletype2.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "Readonly";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                    //businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype5))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "FilterDropdown";
                    newrule.AssociatedActionTypeID = 5;
                    newrule.RuleActionID = businessrule.Id;
                    foreach (string str in lblruletype5.Split(",".ToCharArray()))
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        ActionArgs newactionargs = new ActionArgs();
                        newactionargs.ParameterName = str;
                        newactionargs.ParameterValue = "FilterDropdown";
                        newrule.actionarguments.Add(newactionargs);
                    }
                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                    //businessrule.ruleaction.Add(newrule);
                }
                if (!string.IsNullOrEmpty(lblruletype4))
                {
                    RuleAction newrule = new RuleAction();
                    newrule.ActionName = "TimeBasedAlert";
                    newrule.ErrorMessage = AlertMessage;
                    newrule.AssociatedActionTypeID = 3;
                    newrule.RuleActionID = businessrule.Id;

                    ActionArgs newActionArgs = new ActionArgs();
                    newActionArgs.ParameterName = "TimeValue";
                    newActionArgs.ParameterValue = TimeValue;
                    newrule.actionarguments.Add(newActionArgs);

                    ActionArgs newActionArgs1 = new ActionArgs();
                    newActionArgs1.ParameterName = "NotifyTo";
                    newActionArgs1.ParameterValue = lblruletype4;
                    newrule.actionarguments.Add(newActionArgs1);

                    if (!string.IsNullOrEmpty(NotifyToExtra))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyToExtra";
                        newActionArgs2.ParameterValue = NotifyToExtra;
                        newrule.actionarguments.Add(newActionArgs2);
                    }

                    if (!string.IsNullOrEmpty(NotifyToRole))
                    {
                        string[] RolesIdstr = NotifyToRole.Split(",".ToCharArray());
                        var target = "0";
                        var results = Array.FindAll(RolesIdstr, s => s.Equals(target));
                        if (results.FirstOrDefault() != null && results[0].Equals("0"))
                            NotifyToRole = "0";

                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyToRole";
                        newActionArgs2.ParameterValue = NotifyToRole;
                        newrule.actionarguments.Add(newActionArgs2);
                    }
                    if (!string.IsNullOrEmpty(TimeRuleApplyOnValue))
                    {
                        ActionArgs newActionArgs2 = new ActionArgs();
                        newActionArgs2.ParameterName = "NotifyOn";
                        if (!string.IsNullOrEmpty(lblrulecondition))
                            newActionArgs2.ParameterValue = "Add,Update";
                        else
                            newActionArgs2.ParameterValue = TimeRuleApplyOnValue;
                        newrule.actionarguments.Add(newActionArgs2);
                    }

                    dbRuleAction.RuleActions.Add(newrule);
                    dbRuleAction.SaveChanges();
                    //businessrule.ruleaction.Add(newrule);

                }

                if (!string.IsNullOrEmpty(lblrulecondition))
                {
                    var conditions = lblrulecondition.Split("?".ToCharArray());
                    foreach (var cond in conditions)
                    {
                        if (string.IsNullOrEmpty(cond)) continue;
                        var param = cond.Split(",".ToCharArray());
                        Condition newcondition = new Condition();
                        newcondition.RuleConditionsID = businessrule.Id;
                        newcondition.PropertyName = param[0];
                        newcondition.Operator = param[1];
                        newcondition.Value = param[2];

                        dbCondition.Conditions.Add(newcondition);
                        dbCondition.SaveChanges();
                        //businessrule.ruleconditions.Add(newcondition);
                    }
                }

                return RedirectToAction("Edit", "BusinessRule", new { id = businessrule.Id });

            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);

            JournalEntryContext jedb = new JournalEntryContext();
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == businessrule.Id).ToList();
            return View(businessrule);
        }

        // GET: /BusinessRule/EditWizard/5
        public ActionResult EditWizard(int? id)
        {
            if (!User.CanEdit("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);
            if (businessrule == null)
            {
                return HttpNotFound();
            }


            var _objAssociatedBusinessRuleType = new List<BusinessRuleType>();
            _objAssociatedBusinessRuleType.Add(businessrule.associatedbusinessruletype);
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(_objAssociatedBusinessRuleType, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);


            JournalEntryContext jedb = new JournalEntryContext();
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == id).ToList();
            if (ViewData["BusinessRuleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/BusinessRule"))
                ViewData["BusinessRuleParentUrl"] = Request.UrlReferrer;
            return View(businessrule);
        }

        // POST: /BusinessRule/EditWizard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWizard([Bind(Include = "Id,RuleName,EntityName,Roles,DateCreated1,AssociatedBusinessRuleTypeID,EntitySubscribe,Disable")] BusinessRule businessrule, string UrlReferrer)
        {
            if (ModelState.IsValid)
            {

                db.Entry(businessrule).State = EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else
                    return RedirectToAction("Index");

            }
            ViewBag.AssociatedBusinessRuleTypeID = new SelectList(dbAssociatedBusinessRuleType.BusinessRuleTypes, "ID", "DisplayValue", businessrule.AssociatedBusinessRuleTypeID);


            JournalEntryContext jedb = new JournalEntryContext();
            ViewBag.JournalEntry = jedb.JournalEntries.Where(p => p.EntityName == "BusinessRule" && p.RecordId == businessrule.Id).ToList();
            return View(businessrule);
        }

        // GET: /BusinessRule/Delete/5
        public ActionResult Delete(int? id)
        {

            if (!User.CanDelete("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessRule businessrule = db.BusinessRules.Find(id);
            if (businessrule == null)
            {
                return HttpNotFound();
            }
            if (ViewData["BusinessRuleParentUrl"] == null && Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("/BusinessRule"))
                ViewData["BusinessRuleParentUrl"] = Request.UrlReferrer;
            return View(businessrule);
        }

        // POST: /BusinessRule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!User.CanDelete("BusinessRule"))
            {
                return RedirectToAction("Index", "Error");
            }

            BusinessRule businessrule = db.BusinessRules.Find(id);
            db.BusinessRules.Remove(businessrule);
            db.SaveChanges();
            if (ViewData["BusinessRuleParentUrl"] != null)
            {
                string parentUrl = ViewData["BusinessRuleParentUrl"].ToString();
                ViewData["BusinessRuleParentUrl"] = null;
                return Redirect(parentUrl);
            }
            else return RedirectToAction("Index");
        }

        public ActionResult SetFSearch(string searchString, string HostingEntity)
        {
            ViewBag.CurrentFilter = searchString;

            var lstBusinessRule = from s in db.BusinessRules
                                  select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstBusinessRule = searchRecords(lstBusinessRule, searchString.ToUpper());
            }
            else
                lstBusinessRule = lstBusinessRule.OrderByDescending(s => s.Id);


            if (lstBusinessRule.Where(p => p.AssociatedBusinessRuleTypeID != null).Select(p => p.AssociatedBusinessRuleTypeID).Distinct().Count() <= 50)
                ViewBag.associatedbusinessruletype = new SelectList(lstBusinessRule.Where(p => p.associatedbusinessruletype != null).Select(P => P.associatedbusinessruletype).Distinct(), "ID", "DisplayValue");

            return View();
        }

        // GET: /BusinessRule/FSearch/
        public ActionResult FSearch(string currentFilter, string searchString, string FSFilter, string sortBy, string isAsc, int? page, int? itemsPerPage, string search, bool? IsExport, string associatedbusinessruletype, string DateCreated1From, string DateCreated1To)//, string HostingEntity
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


            var lstBusinessRule = from s in db.BusinessRules
                                  select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                lstBusinessRule = searchRecords(lstBusinessRule, searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstBusinessRule = sortRecords(lstBusinessRule, sortBy, isAsc);
            }
            else
                lstBusinessRule = lstBusinessRule.OrderByDescending(s => s.Id);


            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.SearchResult += "\r\n General Criterial= " + search;
                lstBusinessRule = searchRecords(lstBusinessRule, search);
            }

            lstBusinessRule = lstBusinessRule.Include(t => t.associatedbusinessruletype);
            var _BusinessRule = lstBusinessRule;

            if (DateCreated1From != null && DateCreated1To != null)
            {
                try
                {
                    ViewBag.SearchResult += "\r\n Date Created= " + DateCreated1From + "-" + DateCreated1To;
                    DateTime from = Convert.ToDateTime(DateCreated1From);
                    DateTime to = Convert.ToDateTime(DateCreated1To);
                    _BusinessRule = _BusinessRule.Where(o => o.DateCreated1 >= from && o.DateCreated1 <= to);
                }
                catch { }
            }
            if (lstBusinessRule.Where(p => p.associatedbusinessruletype != null).Count() <= 50)
                ViewBag.associatedbusinessruletype = new SelectList(lstBusinessRule.Where(p => p.associatedbusinessruletype != null).Select(P => P.associatedbusinessruletype).Distinct(), "ID", "DisplayValue");
            if (associatedbusinessruletype != null)
            {
                var ids = associatedbusinessruletype.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
                ViewBag.SearchResult += "\r\n BusinessRule Type= ";
                foreach (var str in ids)
                    if (!string.IsNullOrEmpty(str))
                    {
                        ids1.Add(Convert.ToInt64(str));
                        ViewBag.SearchResult += dbAssociatedBusinessRuleType.BusinessRuleTypes.Find(Convert.ToInt64(str)).DisplayValue + ", ";
                    }
                _BusinessRule = _BusinessRule.Where(p => ids1.ToList().Contains(p.AssociatedBusinessRuleTypeID));
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
                if (_BusinessRule.Count() > 0)
                    pageSize = _BusinessRule.Count();
                return View("ExcelExport", _BusinessRule.ToPagedList(pageNumber, pageSize));
            }

            return View("Index", _BusinessRule.ToPagedList(pageNumber, pageSize));


        }

        public string GetDisplayValue(string id)
        {
            var _Obj = db.BusinessRules.Find(Convert.ToInt64(id));
            return _Obj == null ? "" : _Obj.DisplayValue;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string key, string AssoNameWithParent, string AssociationID)
        {


            IQueryable<BusinessRule> list = db.BusinessRules.Unfiltered();
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.BusinessRules;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(BusinessRule), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<BusinessRule, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);

                    IQueryable q = query.Provider.CreateQuery(methodCall);

                    list = ((IQueryable<BusinessRule>)q);
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
                        BusinessRule model = new BusinessRule();
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
                                case "RuleName":
                                    model.RuleName = columnValue;
                                    break;
                                case "EntityName":
                                    model.EntityName = columnValue;
                                    break;
                                case "Roles":
                                    model.Roles = columnValue;
                                    break;
                                case "DateCreated1":
                                    model.DateCreated1 = DateTime.Parse(columnValue);
                                    break;
                                case "EntitySubscribe":
                                    model.EntitySubscribe = Boolean.Parse(columnValue);
                                    break;

                                case "AssociatedBusinessRuleTypeID":
                                    long associatedbusinessruletypeId = dbAssociatedBusinessRuleType.BusinessRuleTypes.Where(p => p.DisplayValue == columnValue).ToList()[0].Id;
                                    model.AssociatedBusinessRuleTypeID = associatedbusinessruletypeId;
                                    break;



                                default:
                                    break;
                            }
                        }
                        db.BusinessRules.Add(model);
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

