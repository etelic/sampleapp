using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Novacode;
using System.IO;
using GeneratorBase.MVC.Models;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeneratorBase.MVC.Controllers
{
    public class TemplateController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var fileList = Directory.GetFiles(Server.MapPath(@"~/Templates/"), "*.docx").Where(file => Regex.IsMatch(Path.GetFileName(file), "^[0-9]+"))
            .Select(f =>
            {
                int id = Int32.Parse(Path.GetFileNameWithoutExtension(f.ToLower()).Replace(".docx", ""));
                string Name = documentName(id).ToLower().Replace(".docx", "");
                return new { id, Name };
            }).ToList();
            if (fileList.Count > 0)
                ViewBag.FileList = fileList;
            return View();
        }
        public ActionResult GenerateDocument(int id)
        {
            string tempfilePath = Server.MapPath(@"~/Templates/" + id + ".docx");
            string uniqeId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string filePath = Server.MapPath(@"~/Templates/" + uniqeId + documentName(id));
            try
            {
                System.IO.File.Copy(tempfilePath, filePath, true);
                var generateDocument = DocX.Load(filePath);
                string appName = CommonFunction.Instance.AppName();
                generateDocument.ReplaceText("App_Name", appName);
                generateDocument.ReplaceText("App_Name".ToUpper(), appName);
                string dateCreate = DateTime.Today.Date.ToString("MMM dd, yyyy");
                generateDocument.ReplaceText("Date_Create", dateCreate);
                string timeCreate = DateTime.Now.ToString("HH:mm:ss");
                generateDocument.ReplaceText("Time_Create", timeCreate);
                CompanyProfile comp = new CompanyProfile();
                generateDocument.ReplaceText("Company_Name", comp.Name);
                generateDocument.ReplaceText("Company_Email", comp.Email);
                generateDocument.ReplaceText("App_URL", CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL());
                generateDocument = ImagePlace(generateDocument, comp.Icon, "#Company_Logo#");
                generateDocument = HeaderImagePlace(generateDocument, comp.Icon, "#Header_Logo#");
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_TOC#")))
                {
                    generateDocument.InsertTableOfContents(paragraph, "Table of Contents", TableOfContentsSwitches.O | TableOfContentsSwitches.H | TableOfContentsSwitches.Z | TableOfContentsSwitches.U);
                }
                generateDocument.ReplaceText("#Table_TOC#", "");

                int tblCnt = 0;
                generateDocument = RoleAndPermissionNew(generateDocument, tblCnt);
                generateDocument = BusinessRules(generateDocument, appName, dateCreate, timeCreate);
                generateDocument.Save();
                MemoryStream memStream = new MemoryStream();
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = appName + " - " + documentName(id).Replace(".docx", "") + " - v1.docx",
                        Inline = false,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                }


                FileInfo objFile = new FileInfo(filePath);
                if (objFile.Exists)
                    objFile.Delete();
                return new FileStreamResult(memStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            catch (Exception ex)
            {
                FileInfo objFile = new FileInfo(filePath);
                if (objFile.Exists)
                    objFile.Delete();
            }
            return RedirectToAction("Index", "Admin");
        }
        private string documentName(int id)
        {
            string docName = string.Empty;
            switch (id)
            {
                case 1:
                    docName = "Common User's Guide.docx";
                    break;
                case 2:
                    docName = "General Deployment Guide.docx";
                    break;
                case 3:
                    docName = "Security Guide.docx";
                    break;
                case 4:
                    docName = "Configuring Business Rules.docx";
                    break;
                case 5:
                    docName = "Database Design Document.docx";
                    break;
                case 9:
                    docName = "Troubleshooting Guide.docx";
                    break;
                case 10:
                    docName = "Turanto User's Guide.docx";
                    break;
                case 7:
                    docName = "Current Security Settings.docx";
                    break;
                case 8:
                    docName = "Current Business Rules.docx";
                    break;
                default:
                    break;
            }
            return docName;
        }
        private DocX BusinessRules(DocX generateDocument, string appName, string dateCreate, string timeCreate)
        {
            BusinessRuleContext db = new BusinessRuleContext();
            RuleActionContext objRuleAction = new RuleActionContext();
            ConditionContext objCondition = new ConditionContext();
            ActionArgsContext objActionArgs = new ActionArgsContext();
            var businessRules = from s in db.BusinessRules
                                select s;

            string prHead = "There are currently " + businessRules.Count() + " no. of business rules in " + appName + " as on " + dateCreate + " at " + timeCreate + ".";
            if (businessRules != null && businessRules.Count() > 0)
            {
                prHead = prHead + "The rules below are arranged according to entity on which they are applied:";
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Business_Rules#")))
                {
                    paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", prHead, generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }

                string OldName = string.Empty;
                int rowCnt = 1;
                foreach (BusinessRule objBR in businessRules.ToList().OrderBy(br => br.EntityName))
                {
                    var Conditions = objCondition.Conditions.ToList().Where(c => c.RuleConditionsID == objBR.Id);
                    var RuleActions = objRuleAction.RuleActions.Where(r => r.RuleActionID == objBR.Id).OrderBy(ra => ra.AssociatedActionTypeID).ToList();
                    if (ModelReflector.Entities.Where(e => e.Name == objBR.EntityName).Count() == 0)
                        continue;

                    foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Business_Rules#")))
                    {
                        if (rowCnt != 1)
                            paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertPageBreakBeforeSelf());
                        if (OldName != objBR.EntityName)
                        {
                            paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading(entityDP(objBR.EntityName), 2, generateDocument)));
                        }

                        string ruleStatus = "Enabled";
                        if (objBR.Disable)
                            ruleStatus = "Disabled";
                        if (objBR.Freeze)
                            ruleStatus += " and Freezed";

                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading(entityDP(objBR.EntityName) + " - " + objBR.RuleName, 3, generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "RULE STATUS: " + ruleStatus, generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "TODO: Rule Description (for example: This rule make sure that new employees get a welcome mail.)", generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Application Roles", 4, generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "This rule is applicable for the following roles: [" + objBR.Roles + "].", generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "For all other roles, the rule is not invoked and the condition is not evaluated.", generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("When to evaluate rule", 4, generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "The rule will be evaluated when:", generateDocument)));
                        paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", RuleEvaluate(objBR), generateDocument)));
                        if (Conditions.Count() > 0)
                        {
                            paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Rule Conditions", 4, generateDocument)));
                            int conditionCnt = 1;
                            foreach (var condition in Conditions)
                            {
                                string strCondtion = conditionCnt.ToString() + ".   If " + entityDP(objBR.EntityName) + "'s '" + propDP(objBR.EntityName, condition.PropertyName) + "' " + operatorDp(condition.Operator) + " '" + condition.Value + "' " + ((Conditions.Count() > 1 && conditionCnt < Conditions.Count()) ? condition.LogicalConnector : " ");
                                paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", strCondtion, generateDocument)));
                                conditionCnt++;
                            }
                        }

                        if (RuleActions.Count() > 0)
                        {
                            paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Rule Actions", 4, generateDocument)));
                            int ruleactionCnt = 1;
                            foreach (var ruleaction in RuleActions.Where(ra => ra.AssociatedActionTypeID != 9))
                            {
                                //string ruleacnDP = ;
                                //if (string.IsNullOrEmpty(ruleacnDP))
                                //{
                                string actionarg = ruleactionCnt.ToString() + ".   " + RuleActionDP(ruleaction, objBR.EntityName);
                                paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", actionarg, generateDocument)));
                                ruleactionCnt++;
                                //}
                            }

                            generateDocument.Paragraphs.Last().Remove(false);
                        }

                        if (OldName != objBR.EntityName)
                        {
                            OldName = objBR.EntityName;
                            generateDocument.Paragraphs.Last().Remove(false);
                        }
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);

                        if (Conditions.Count() > 0)
                        {
                            generateDocument.Paragraphs.Last().Remove(false);
                            foreach (var condition in Conditions)
                            {
                                generateDocument.Paragraphs.Last().Remove(false);
                            }

                        }
                        if (RuleActions.Count() > 0)
                        {
                            foreach (var ruleaction in RuleActions.Where(ra => ra.AssociatedActionTypeID != 9))
                            {
                                generateDocument.Paragraphs.Last().Remove(false);
                            }
                        }
                        break;


                    }
                    rowCnt++;
                }
            }
            else
            {
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Business_Rules#")))
                {
                    paragraph.FindAll("#Table_Business_Rules#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", prHead, generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
            }
            generateDocument.ReplaceText("#Table_Business_Rules#", "");
            return generateDocument;
        }
        private string RuleEvaluate(BusinessRule objBR)
        {
            string ruleevaluate = string.Empty;
            switch (objBR.AssociatedBusinessRuleTypeID)
            {
                case 1:
                    ruleevaluate = "-   Event is added";
                    break;
                case 2:
                    ruleevaluate = "-   Event is updated";
                    break;
                case 3:
                    ruleevaluate = "-   Event is added or updated";
                    break;
                case 4:
                    ruleevaluate = "-   Event is at on property change";
                    break;
                case 5:
                    var ruleaction = objBR.ruleaction.ToList().FirstOrDefault(p => p.AssociatedActionTypeID == 9);
                    if (ruleaction != null)
                    {
                        string startOn = string.Empty;
                        string endOn = string.Empty;
                        string timeOn = string.Empty;
                        foreach (var arg in ruleaction.actionarguments.ToList())
                        {
                            if (arg.ParameterName == "StartDate")
                                startOn = arg.ParameterValue;
                            if (arg.ParameterName == "EndDate")
                                endOn = arg.ParameterValue;
                            if (arg.ParameterName == "DailyTime")
                                timeOn = arg.ParameterValue;
                        }
                        ruleevaluate = "-   Starting on " + startOn + " daily at " + timeOn + " till " + endOn + ".";
                    }
                    break;
                default:
                    break;
            }

            return ruleevaluate;
        }
        private string RuleActionDP(RuleAction objRA, string entityName)
        {
            string ruleaction = string.Empty;
            var actionlst = new List<string>();
            if (objRA.AssociatedActionTypeID != 3 && objRA.AssociatedActionTypeID != 5 && objRA.AssociatedActionTypeID != 7 && objRA.AssociatedActionTypeID != 8)
                actionlst = ArgActionList(objRA.actionarguments.ToList(), entityName);
            switch (objRA.AssociatedActionTypeID)
            {
                case 1:
                    ruleaction = "Lock editing permissions of record.";
                    break;
                case 2:
                    ruleaction = "Make " + string.Join(", ", actionlst) + " property(s) mandatory.";
                    break;
                case 3:
                    {
                        var actionSendEmail = objRA.actionarguments.ToList();
                        var timevalue = actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "TimeValue").ParameterValue;
                        var notifyto = actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "NotifyTo").ParameterValue;
                        var notifytoextra = actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "NotifyToExtra") != null ? actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "NotifyToExtra").ParameterValue : "";
                        var notifytorole = actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "NotifyToRole") != null ? actionSendEmail.FirstOrDefault(ar => ar.ParameterName == "NotifyToRole").ParameterValue : "";
                        if (!string.IsNullOrEmpty(notifytorole))
                        {
                            var dbrole = (new ApplicationDbContext()).Roles;
                            foreach (var role in notifytorole.Split(','))
                            {
                                if (role.ToLower() == "all")
                                    actionlst.Add("All");
                                else
                                    actionlst.Add(dbrole.FirstOrDefault(r => r.Id == role).Name);
                            }

                            notifytorole = string.Join(", ", actionlst);
                        }

                        var template = objRA.ErrorMessage;
                        ruleaction = "Send Email within " + timevalue + " day(s) to " + notifyto + ". " + ((!string.IsNullOrEmpty(notifytoextra) || !string.IsNullOrEmpty(notifytorole)) ? ("This email will be sent to " + (!string.IsNullOrEmpty(notifytorole) ? "users of " + notifytorole + " role" : "") + (!string.IsNullOrEmpty(notifytoextra) ? (!string.IsNullOrEmpty(notifytorole) ? " including " : "") + notifytoextra + " email id(s)." : "")) : "") + ((!string.IsNullOrEmpty(template)) ? "" : "");//" Email template will be :\n" + MvcHtmlString.Create(template) + "" : "");
                        break;
                    }
                case 4:
                    ruleaction = "Make " + string.Join(", ", actionlst) + " property(s) read only.";
                    break;
                case 5:
                    {
                        var argsaction = objRA.actionarguments.ToList();
                        foreach (var argaction in argsaction)
                        {
                            foreach (var entity in ModelReflector.Entities)
                            {
                                var propinfo = entity.Properties.FirstOrDefault(asso => asso.Name == argaction.ParameterName);
                                if (propinfo != null)
                                {
                                    actionlst.Add("'" + entity.DisplayName + "'");
                                    break;
                                }
                            }
                        }
                        ruleaction = "Filter  " + string.Join(", ", actionlst) + " dropdown(s).";
                        break;
                    }
                case 6:
                    ruleaction = "Hide " + string.Join(", ", actionlst) + " property(s).";
                    break;
                case 7:
                    {
                        var argsaction = objRA.actionarguments.ToList();
                        foreach (var argaction in argsaction)
                        {
                            var actparam = propDP(entityName, argaction.ParameterName);
                            var actvalue = argaction.ParameterValue;
                            if (argaction.ParameterValue.StartsWith("[") && argaction.ParameterValue.EndsWith("]"))
                            {
                                var targetProperty = argaction.ParameterValue;
                                targetProperty = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                                if (targetProperty.Contains("."))
                                {
                                    actionlst.Add("'" + actparam + "' (picked from list) value = " + targetProperty);
                                }
                                else
                                    actionlst.Add("'" + actparam + "' (dynamic) value = " + targetProperty);
                            }
                            else
                            {
                                if (argaction.ParameterValue.StartsWith("["))
                                {
                                    var targetProperty = argaction.ParameterValue;
                                    var actionProperty = GetSubstringByString("[", "]", argaction.ParameterValue);
                                    var actualName = propDP(entityName, actionProperty);
                                    actionlst.Add("'" + actparam + "' (dynamic) value = " + targetProperty.Replace("[" + actionProperty + "]", actualName));
                                }
                                else
                                    actionlst.Add("'" + actparam + "' (constant) value = " + actvalue);
                            }
                        }
                        ruleaction = "Set " + string.Join(", ", actionlst);
                        break;
                    }
                case 8:
                    {
                        var argsaction = objRA.actionarguments.ToList();
                        foreach (var argaction in argsaction)
                        {
                            actionlst.Add(argaction.ParameterValue + " button in " + argaction.ParameterName);
                        }
                        ruleaction = "Invoke action of " + string.Join(", ", actionlst);
                        break;
                    }
            }
            return ruleaction;
        }
        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }
        private List<string> ArgActionList(List<ActionArgs> argacn, string entityName)
        {
            var actionlist = new List<string>();
            foreach (var argaction in argacn)
            {

                actionlist.Add("'" + propDP(entityName, argaction.ParameterName) + "'");

            }
            return actionlist;
        }
        public static string operatorDp(string operand)
        {
            string operandDP = string.Empty;
            switch (operand)
            {
                case "=":
                    operandDP = "Equals to";
                    break;
                case ">":
                    operandDP = "Greater than";
                    break;
                case "<":
                    operandDP = "Less than";
                    break;
                case "<=":
                    operandDP = "Less than Or Equals to";
                    break;
                case ">=":
                    operandDP = "Greater than Or Equals to";
                    break;
                case "Contains":
                    operandDP = "Contains";
                    break;
                case "!=":
                    operandDP = "Not Equals to";
                    break;
                default:
                    break;
            }
            return operandDP;
        }
        private string entityDP(string entityName)
        {
            return ModelReflector.Entities.Where(e => e.Name == entityName).Count() > 0 ? ModelReflector.Entities.FirstOrDefault(e => e.Name == entityName).DisplayName : "";
        }
        public static string propDP(string entityName, string propName)
        {
            string propDP = string.Empty;
            try
            {
                var entityInfo = ModelReflector.Entities.FirstOrDefault(e => e.Name == entityName).Properties.Where(p => p.Name == propName);
                if (entityInfo == null)
                    propDP = ModelReflector.Entities.FirstOrDefault(e => e.Name == entityName).Associations.FirstOrDefault(p => p.AssociationProperty == propName).DisplayName;
                else
                    propDP = ModelReflector.Entities.FirstOrDefault(e => e.Name == entityName).Properties.FirstOrDefault(p => p.Name == propName).DisplayName;
            }
            catch
            {
                propDP = propName;
            }
            return propDP;
        }
        private DocX RoleAndPermission(DocX generateDocument, int tblCnt)
        {
            var Db = new ApplicationDbContext();
            var Roles = Db.Roles.Select(p => new
            {
                Role_Name = p.Name,
                Description = "To Be Updated"
            }).OrderBy(p => p.Role_Name);

            if (Roles.Count() > 0)
            {
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Roles#")))
                {
                    paragraph.FindAll("#Table_Roles#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption("Table 1: Roles", string.Empty, generateDocument)));
                    paragraph.FindAll("#Table_Roles#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(Roles, generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
                var isPermission = true;
                foreach (var role in Roles)
                {
                    string strCaption = "Table " + tblCnt.ToString() + ": Permission List of Role: ";
                    var rolePermission = permissionByRole(role.Role_Name).Where(p => p.CanAdd || p.CanEdit || p.CanDelete || p.CanView).ToList().Select(e => new
                    {
                        Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).DisplayName,
                        Add = e.CanAdd ? '\u2713'.ToString() : "",
                        Edit = e.CanEdit ? '\u2713'.ToString() : "",
                        Delete = e.CanDelete ? '\u2713'.ToString() : "",
                        View = e.CanView ? '\u2713'.ToString() : "",
                        Disable_Verbs = e.Verbs == null ? "" : e.Verbs.Replace(',', '\n'),
                        Self_Service = e.UserAssociation
                    });
                    if (rolePermission.Count() > 0)
                    {
                        isPermission = false;
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Permissions#")))
                        {
                            paragraph.FindAll("#Table_Permissions#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, role.Role_Name, generateDocument)));
                            paragraph.FindAll("#Table_Permissions#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(rolePermission, generateDocument)));
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }
                    if (rolePermission.Count() > 0)
                        tblCnt++;
                }
                if (isPermission)
                {
                    foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Permissions#")))
                    {
                        paragraph.FindAll("#Table_Permissions#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Error("No role permissions is defined in your application.", generateDocument)));
                        generateDocument.Paragraphs.Last().Remove(false);
                        break;
                    }
                }
                var isFLS = true;
                foreach (var role in Roles)
                {
                    string strHead = "FLS for role ‘" + role.Role_Name + "’ is as follows:";
                    string strCaption = "Table " + tblCnt.ToString() + ": Field Level Security for ";
                    var roleFLS = permissionByRole(role.Role_Name).Where(p => !string.IsNullOrEmpty(p.NoEdit) || !string.IsNullOrEmpty(p.NoView)).ToList();
                    var roleFLSList = new List<FLS>();
                    foreach (var roleFLSItem in roleFLS)
                    {
                        foreach (var propItem in entityProperty(roleFLSItem.NoEdit.Split(',').ToList(), roleFLSItem.NoView.Split(',').ToList()))
                        {
                            var objFls = new FLS();
                            objFls.Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == roleFLSItem.EntityName).DisplayName;
                            objFls.Property_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == roleFLSItem.EntityName).Properties.FirstOrDefault(p => p.Name == propItem.Trim()).DisplayName;
                            objFls.Cannot_Edit = roleFLSItem.NoEdit.Split(',').ToList().Contains(propItem) ? '\u2713'.ToString() : "";
                            objFls.Cannot_View = roleFLSItem.NoView.Split(',').ToList().Contains(propItem) ? '\u2713'.ToString() : "";
                            roleFLSList.Add(objFls);
                        }
                    }
                    if (roleFLSList.Count() > 0)
                    {
                        isFLS = false;
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_FLS#")))
                        {
                            paragraph.FindAll("#Table_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Heading(strHead, 0, generateDocument))));
                            paragraph.FindAll("#Table_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Caption(strCaption, role.Role_Name, generateDocument))));
                            paragraph.FindAll("#Table_FLS#").ForEach(index => paragraph.InsertTableBeforeSelf((tableWithData(roleFLSList, generateDocument))));
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }
                    if (roleFLSList.Count() > 0)
                        tblCnt++;
                }
                if (isFLS)
                {
                    foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_FLS#")))
                    {
                        paragraph.FindAll("#Table_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Error("No field level security is defined in your application.", generateDocument))));
                        generateDocument.Paragraphs.Last().Remove(false);
                        break;
                    }
                }
            }
            else
            {
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Roles#")))
                {
                    paragraph.FindAll("#Table_Roles#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Error("Please define some roles in your application.", generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Permissions#")))
                {
                    paragraph.FindAll("#Table_Permissions#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Error("No role permissions is defined in your application.", generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_FLS#")))
                {
                    paragraph.FindAll("#Table_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Error("No field level security is defined in your application.", generateDocument))));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
            }
            generateDocument.ReplaceText("#Table_Roles#", "");
            generateDocument.ReplaceText("#Table_Permissions#", "");
            generateDocument.ReplaceText("#Table_FLS#", "");

            generateDocument = UserBasedSecurity(generateDocument, tblCnt);
            return generateDocument;
        }
        private DocX RoleAndPermissionNew(DocX generateDocument, int tblCnt)
        {
            var Db = new ApplicationDbContext();
            var Roles = Db.Roles.Select(p => new
            {
                Role_Name = p.Name,
                UserCnt = p.Users.Count(),
                Description = "To Be Updated"
            }).ToList().OrderBy(p => p.Role_Name);

            int rowCnt = 1;
            if (Roles.Count() > 0)
            {
                var isPermission = true;
                foreach (var role in Roles)
                {
                    if (role.Role_Name == CommonFunction.Instance.AdministratorRoles())
                    {
                        generateDocument.ReplaceText("#User_In_Admin#", Convert.ToString(role.UserCnt));
                        var userMultiple = Db.Users.Select(p => new
                        {
                            RoleCnt = p.Roles.Count()
                        }).ToList().Where(e => e.RoleCnt > 1);
                        generateDocument.ReplaceText("#User_In_Multiple#", Convert.ToString(userMultiple.Count() > 0 ? userMultiple.Count() : 0));
                        continue;
                    }
                    foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                    {
                        if (rowCnt != 1)
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertPageBreakBeforeSelf());
                        paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Role: " + role.Role_Name, 2, generateDocument)));
                        paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Description", 3, generateDocument)));
                        paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "TODO: This is test description for the role. The section may include the details about the role and permissions given to users. This section can also be used to provide instructions for users who would be assigned with this role.", generateDocument)));
                        paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(roleInformation(role.Role_Name, role.UserCnt), generateDocument, false)));

                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        break;
                    }

                    string strCaption = "";
                    var rolePermission = permissionByRole(role.Role_Name).Where(p => p.CanAdd || p.CanEdit || p.CanDelete || p.CanView).ToList().Select(e => new
                    {
                        Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).DisplayName,
                        IsDefault = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).IsDefault,
                        Add = e.CanAdd ? '\u2713'.ToString() : "",
                        Edit = e.CanEdit ? '\u2713'.ToString() : "",
                        Delete = e.CanDelete ? '\u2713'.ToString() : "",
                        View = e.CanView ? '\u2713'.ToString() : "",
                        Disabled_Verbs = e.Verbs == null ? "" : e.Verbs.Replace(',', '\n'),
                        Self_Service = e.UserAssociation,
                        Auto_Registration = e.SelfRegistration
                    });

                    if (rolePermission.Count() > 0)
                    {
                        var Permdefault = rolePermission.Where(e => e.IsDefault == true).Select(e => new
                        {
                            Entity_Name_1Built_in_Turanto2 = e.Entity_Name,
                            Add = e.Add,
                            Edit = e.Edit,
                            Delete = e.Delete,
                            View = e.View,
                            Disabled_Verbs = e.Disabled_Verbs,
                            Self_Service = e.Self_Service,
                            Auto_Registration = e.Auto_Registration
                        });

                        var Permgenerated = rolePermission.Where(e => e.IsDefault == false).Select(e => new
                        {
                            Entity_Name_1Functional_Entity2 = e.Entity_Name,
                            Add = e.Add,
                            Edit = e.Edit,
                            Delete = e.Delete,
                            View = e.View,
                            Disabled_Verbs = e.Disabled_Verbs,
                            Self_Service = e.Self_Service,
                            Auto_Registration = e.Auto_Registration
                        });

                        isPermission = false;
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                        {
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Permission Settings For Role: " + role.Role_Name, 3, generateDocument)));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "The table below lists the permissions allowed for this role.", generateDocument)));
                            if (Permgenerated.Count() > 0)
                            {
                                tblCnt++;
                                strCaption = "Table " + tblCnt.ToString() + ": Permission List of Role: ";
                                paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, role.Role_Name, generateDocument)));
                                paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(Permgenerated, generateDocument)));
                            }
                            if (Permdefault.Count() > 0)
                            {
                                tblCnt++;
                                strCaption = "Table " + tblCnt.ToString() + ": Permission List of Role: ";
                                paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, role.Role_Name, generateDocument)));
                                paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(Permdefault, generateDocument)));
                            }
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            if (Permgenerated.Count() > 0)
                            {
                                generateDocument.Paragraphs.Last().Remove(false);
                            }
                            if (Permdefault.Count() > 0)
                            {
                                generateDocument.Paragraphs.Last().Remove(false);
                            }
                            break;
                        }
                    }

                    if (isPermission)
                    {
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                        {
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Permission Settings For Role: " + role.Role_Name, 3, generateDocument)));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Error("No permissions are defined for this role.", generateDocument)));
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }

                    var isFLS = true;
                    string strHead = "FLS for role ‘" + role.Role_Name + "’ is as follows:";
                    strCaption = "Table " + tblCnt.ToString() + ": Field Level Security for ";
                    var roleFLS = permissionByRole(role.Role_Name).Where(p => !string.IsNullOrEmpty(p.NoEdit) || !string.IsNullOrEmpty(p.NoView)).ToList();
                    var roleFLSList = new List<FLS>();
                    foreach (var roleFLSItem in roleFLS)
                    {
                        foreach (var propItem in entityProperty(roleFLSItem.NoEdit.Split(',').ToList(), roleFLSItem.NoView.Split(',').ToList()))
                        {
                            var objFls = new FLS();
                            objFls.Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == roleFLSItem.EntityName).DisplayName;
                            objFls.Property_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == roleFLSItem.EntityName).Properties.FirstOrDefault(p => p.Name == propItem.Trim()).DisplayName;
                            objFls.Cannot_Edit = roleFLSItem.NoEdit.Split(',').ToList().Contains(propItem) ? '\u2713'.ToString() : "";
                            objFls.Cannot_View = roleFLSItem.NoView.Split(',').ToList().Contains(propItem) ? '\u2713'.ToString() : "";
                            roleFLSList.Add(objFls);
                        }
                    }
                    if (roleFLSList.Count() > 0)
                    {
                        tblCnt++;
                        isFLS = false;
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                        {
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Field Level Security (FLS) For Role: " + role.Role_Name, 3, generateDocument)));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Heading(strHead, 0, generateDocument))));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Caption(strCaption, role.Role_Name, generateDocument))));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertTableBeforeSelf((tableWithData(roleFLSList, generateDocument))));
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }

                    if (isFLS)
                    {
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                        {
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Field Level Security (FLS) For Role: " + role.Role_Name, 3, generateDocument)));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((Error("No field level security is defined for this role.", generateDocument))));
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }

                    var user = new UserDefinePagesRoleContext();
                    var userpagelist = user.UserDefinePagesRoles;
                    var userpage = userpagelist.FirstOrDefault(u => u.RoleName == role.Role_Name);
                    if (userpage != null)
                    {
                        foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                        {
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Custom Page For Role: " + role.Role_Name, 3, generateDocument)));
                            paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf((tblCaption("", "This role has a specific custom page designed. This enables users who login into the application with this role to see a page customized for the task this role can perform.", generateDocument))));
                            generateDocument.Paragraphs.Last().Remove(false);
                            generateDocument.Paragraphs.Last().Remove(false);
                            break;
                        }
                    }
                }
                rowCnt++;
            }
            else
            {
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_Role_Permission_FLS#")))
                {
                    paragraph.FindAll("#Table_Role_Permission_FLS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Error("Please define some roles in your application.", generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    break;
                }
            }
            generateDocument.ReplaceText("#Table_Role_Permission_FLS#", "");
            //generateDocument = EntityBasedSecurity(generateDocument, tblCnt);
            generateDocument = UserBasedSecurity(generateDocument, tblCnt);
            return generateDocument;
        }
        private DocX EntityBasedSecurity(DocX generateDocument, int tblCnt)
        {
            var Db = new ApplicationDbContext();
            var Roles = Db.Roles.Where(p => p.Name.ToLower().Trim() != "admin").Select(p => new
            {
                Role_Name = p.Name
            }).OrderBy(p => p.Role_Name);
            var rolePermission = new List<Permission>();
            if (Roles.Count() > 0)
            {
                foreach (var role in Roles)
                {
                    rolePermission.AddRange(permissionByRole(role.Role_Name).ToList());
                }
            }

            if (rolePermission.Count() > 0)
            {
                var roleNewPermission = rolePermission.Select(e => new
                {
                    Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).DisplayName,
                    IsDefault = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).IsDefault,
                    Role = e.RoleName,
                    Add = e.CanAdd ? '\u2713'.ToString() : "",
                    Edit = e.CanEdit ? '\u2713'.ToString() : "",
                    Delete = e.CanDelete ? '\u2713'.ToString() : "",
                    View = e.CanView ? '\u2713'.ToString() : "",
                    Disabled_Verbs = e.Verbs == null ? "" : e.Verbs.Replace(',', '\n'),
                    Self_Service = e.UserAssociation
                    //,Auto_Registration = e.SelfRegistration
                });

                string strCaption = "";
                var Permdefault = roleNewPermission.Where(e => e.IsDefault == true).Select(e => new
                {
                    Entity_1Built_in_Turanto2 = e.Entity_Name,
                    Role = e.Role,
                    Add = e.Add,
                    Edit = e.Edit,
                    Delete = e.Delete,
                    View = e.View,
                    Disabled_Verbs = e.Disabled_Verbs,
                    Self_Service = e.Self_Service
                    //,Auto_Registration = e.Auto_Registration
                }).OrderBy(e => e.Entity_1Built_in_Turanto2);

                var Permgenerated = roleNewPermission.Where(e => e.IsDefault == false).Select(e => new
                {
                    Entity_Name_1Functional2 = e.Entity_Name,
                    Role = e.Role,
                    Add = e.Add,
                    Edit = e.Edit,
                    Delete = e.Delete,
                    View = e.View,
                    Disabled_Verbs = e.Disabled_Verbs,
                    Self_Service = e.Self_Service
                    //,Auto_Registration = e.Auto_Registration
                }).OrderBy(e => e.Entity_Name_1Functional2);

                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_EntityBasedSecurity#")))
                {
                    if (Permgenerated.Count() > 0)
                    {
                        tblCnt++;
                        strCaption = "Table " + tblCnt.ToString() + ": Entity Based Security View for Functional Entities ";
                        paragraph.FindAll("#Table_EntityBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, "", generateDocument)));
                        paragraph.FindAll("#Table_EntityBasedSecurity#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(Permgenerated, generateDocument)));
                    }
                    if (Permdefault.Count() > 0)
                    {
                        tblCnt++;
                        strCaption = "Table " + tblCnt.ToString() + ": Entity Based Security View for Entities Built in Turanto ";
                        paragraph.FindAll("#Table_EntityBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, "", generateDocument)));
                        paragraph.FindAll("#Table_EntityBasedSecurity#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(Permdefault, generateDocument)));
                    }
                    if (Permgenerated.Count() > 0)
                    {
                        generateDocument.Paragraphs.Last().Remove(false);
                        //generateDocument.Paragraphs.Last().Remove(false);
                    }
                    if (Permdefault.Count() > 0)
                    {
                        generateDocument.Paragraphs.Last().Remove(false);
                        //generateDocument.Paragraphs.Last().Remove(false);
                    }
                    break;
                }
            }
            generateDocument.ReplaceText("#Table_EntityBasedSecurity#", "");
            generateDocument = UserBasedSecurity(generateDocument, tblCnt);
            return generateDocument;
        }
        private DocX UserBasedSecurity(DocX generateDocument, int tblCnt)
        {
            if (ModelReflector.Entities.Where(p => p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).Count() > 0)
            {
                var Db = new UserBasedSecurityContext();
                var ubsFList = Db.UserBasedSecurities.ToList();
                var ubsList = ubsFList.Select(e => new
                {
                    Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).DisplayName,
                    Target_Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.TargetEntityName) != null ? ModelReflector.Entities.FirstOrDefault(p => p.Name == e.TargetEntityName).DisplayName : e.TargetEntityName,
                    Ignore_For_Roles = e.RolesToIgnore,
                    Enable_Hierarchy = Convert.ToString(e.Other1) == "true" ? '\u2713'.ToString() : ""
                });
                if (ubsList.Count() > 0)
                {
                    tblCnt++;
                    string UbsEntity = ((ubsFList.Where(p => p.IsMainEntity == true).Count() > 0) ? ModelReflector.Entities.FirstOrDefault(e => e.Name == (ubsFList.FirstOrDefault(p => p.IsMainEntity == true).EntityName)).DisplayName : "_____");
                    string strCaption = "Table " + tblCnt.ToString() + ": User Based Security";
                    foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_UserBasedSecurity#")))
                    {

                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("User Based Security", 1, generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "The table displays the entities that falls under user based security and the roles for which the security is ignored.", generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, "", generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(ubsList, generateDocument)));

                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("ENTITY: ", "The entities in the first column are filtered under user based security. The entities other than this will have be acted as per CRUD permissions for a role.", generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("TARGET ENTITY: ", "The entities in second column are the source entities of filtration. Here the primary entity associated directly with user is " + UbsEntity + " and all entities in first column are directly or indirectly associated with it.", generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("IGNORE FOR ROLES: ", "The user based security on an entity will not be applicable on role(s) defined against it. The users of these role will have privilege to see records of all users of given entity.", generateDocument)));
                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("ENABLE HIERARCHY: ", "If true, the hierarchical supervisor can be assigned to act upon the records of fellow user.", generateDocument)));

                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);

                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);
                        generateDocument.Paragraphs.Last().Remove(false);

                        paragraph.FindAll("#Table_UserBasedSecurity#").ForEach(index => paragraph.InsertPageBreakBeforeSelf());
                        break;
                    }

                }
            }
            generateDocument.ReplaceText("#Table_UserBasedSecurity#", "");
            generateDocument = DynamicMapping(generateDocument, tblCnt);
            return generateDocument;
        }
        private DocX DynamicMapping(DocX generateDocument, int tblCnt)
        {
            var Db = new ApplicationContext(new SystemUser());
            var drmFList = Db.DynamicRoleMappings.ToList();
            var drmList = drmFList.Select(e => new
            {
                Entity_Name = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).DisplayName,
                Role = e.RoleId,
                Entity_Column = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).Properties.Where(pre => pre.Name == e.Condition).Count() > 0 ? ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).Properties.FirstOrDefault(pre => pre.Name == e.Condition).DisplayName : ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).Associations.FirstOrDefault(a => a.AssociationProperty == e.Condition).DisplayName,
                Value = e.Value,
                User_Relation = ModelReflector.Entities.FirstOrDefault(p => p.Name == e.EntityName).Properties.FirstOrDefault(p => p.Name == e.UserRelation).DisplayName
            }).ToList();
            if (drmList.Count() > 0)
            {
                tblCnt++;
                string strCaption = "Table " + tblCnt.ToString() + ": Dynamic Role Mapping";
                foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_DynamicMapping#")))
                {
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Heading("Dynamic Role Mapping", 1, generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strCaption, "", generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertTableBeforeSelf(tableWithData(drmList, generateDocument)));

                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("ENTITY NAME: ", "The entity(s) displayed here is/are directly associated with user and on this/these entity Dynamic Role Mapping is/are applied.", generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("ROLE: ", "If the value of entity column matches with value defined here, the user will be assigned with this role.", generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("ENTITY COLUMN: ", "The application will check the value in the entity column defined here.", generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("VALUE: ", "If the value of record matches with this value, the user will be assigned with this role.", generateDocument)));
                    paragraph.FindAll("#Table_DynamicMapping#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("USER RELATION: ", "This shows how the user is associated with entity defined here.", generateDocument)));

                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);

                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);

                    break;
                }

            }
            generateDocument.ReplaceText("#Table_DynamicMapping#", "");

            tblCnt++;
            string strSecurityCaption = "Table " + tblCnt.ToString() + ": Login Account Settings";
            foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Table_SCS#")))
            {
                paragraph.FindAll("#Table_SCS#").ForEach(index => paragraph.InsertParagraphBeforeSelf(Caption(strSecurityCaption, "", generateDocument)));
                generateDocument.Paragraphs.Last().Remove(false);
            }
            generateDocument.ReplaceText("#Table_SCS#", "");

            var S1 = Db.AppSettings.FirstOrDefault(p => p.DisplayValue == "ApplySecurityPolicy").Value;
            var S2 = Db.AppSettings.FirstOrDefault(p => p.DisplayValue == "MaxFailedAccessAttemptsBeforeLockout").Value;
            var S3 = Db.AppSettings.FirstOrDefault(p => p.DisplayValue == "DefaultAccountLockoutTimeSpan").Value;
            var S4 = Db.AppSettings.FirstOrDefault(p => p.DisplayValue == "PasswordExpirationInDays").Value;
            generateDocument.ReplaceText("#S1#", S1);
            generateDocument.ReplaceText("#S2#", S2);
            generateDocument.ReplaceText("#S3#", S3);
            generateDocument.ReplaceText("#S4#", S4);

            foreach (var paragraph in generateDocument.Paragraphs.Where(p => p.Text.Contains("#Login_Setting#")))
            {
                if (CommonFunction.Instance.UseActiveDirectory().ToLower() == "true")
                {
                    paragraph.FindAll("#Login_Setting#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "This application is configured for Windows Domain Login Integration. Thus, all Active Directory settings for login are applicable. The domain is: " + CommonFunction.Instance.DomainName(), generateDocument)));
                    paragraph.FindAll("#Login_Setting#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "The application uses built in user login mechanism and it is not enabled for the following settings:", generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                    generateDocument.Paragraphs.Last().Remove(false);
                }
                else
                {
                    paragraph.FindAll("#Login_Setting#").ForEach(index => paragraph.InsertParagraphBeforeSelf(tblCaption("", "The application uses built in user login mechanism and it is enabled for the following settings:", generateDocument)));
                    generateDocument.Paragraphs.Last().Remove(false);
                }
            }
            generateDocument.ReplaceText("#Login_Setting#", "");


            return generateDocument;
        }
        private List<string> entityProperty(List<string> propEditList, List<string> propViewList)
        {
            List<string> properties = new List<string>();
            properties = propEditList.Union(propViewList).ToList();
            return properties.Where(p => !string.IsNullOrEmpty(p.Trim())).ToList();
        }
        private List<RoleInfo> roleInformation(string RoleName, int UserCnt)
        {
            var roleInfolst = new List<RoleInfo>();

            RoleInfo obj1 = new RoleInfo();
            obj1.CellDesc = "No. of Users:";
            obj1.CellCnt = UserCnt.ToString();
            roleInfolst.Add(obj1);

            return roleInfolst;
        }
        private Table tableWithData(IEnumerable<dynamic> listData, DocX document, bool IsHeader = true)
        {
            var resultType = listData.GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Single().GetGenericArguments().Single();
            var columns = resultType.GetProperties();
            var columnNames = columns.Select(column => column.Name).ToList();
            var table = document.AddTable(listData.Count() + 1, columns.Length);
            table.Alignment = Alignment.center;

            table.Design = TableDesign.GridTable2Accent3;
            if (IsHeader)
                table = tableHeader(table, columnNames, document);
            table = tableRow(table, columnNames, listData, document);
            if (!IsHeader)
            {
                table.AutoFit = AutoFit.Window;
                table.RemoveRow(0);
            }
            return table;
        }
        private Table tableWithData(IEnumerable<dynamic> listData, List<string> colString, DocX document, bool IsHeader = true)
        {
            var table = document.AddTable(listData.Count() + 1, colString.Count);
            table.Alignment = Alignment.center;

            table.Design = TableDesign.GridTable2Accent3;
            if (IsHeader)
                table = tableHeader(table, colString);
            table = tableRow(table, colString, listData, document);
            return table;
        }
        private Table tableHeader(Table table, List<string> colString)
        {
            for (int i = 0; i < colString.Count; i++)
            {
                table.Rows[0].Cells[i].Paragraphs[0].Append(colString[i].Replace("_", " ").Replace("1", "(").Replace("2", ")").Replace("#3", "").Replace("#4", ""));
                table.Rows[0].Cells[i].Paragraphs[0].Alignment = Alignment.left;
                table.Rows[0].Cells[i].Paragraphs[0].SetLineSpacing(LineSpacingTypeAuto.Auto);
            }
            return table;
        }
        private Table tableHeader(Table table, List<string> colString, DocX document)
        {
            float availableSpace = (document.PageWidth - document.MarginLeft - document.MarginRight) / colString.Count;
            //table.AutoFit = AutoFit.Window;
            for (int i = 0; i < colString.Count; i++)
            {
                table.Rows[0].Cells[i].VerticalAlignment = VerticalAlignment.Center;
                table.Rows[0].Cells[i].Paragraphs[0].Append(colString[i].Replace("_", " ").Replace("1", "(").Replace("2", ")").Replace("#3", "").Replace("#4", ""));
                table.Rows[0].Cells[i].Paragraphs[0].Alignment = Alignment.left;
                table.Rows[0].Cells[i].Paragraphs[0].SetLineSpacing(LineSpacingTypeAuto.Auto);
                table.Rows[0].Cells[i].Width = availableSpace;
            }
            return table;
        }
        private Table tableRow(Table table, List<string> colString, IEnumerable<dynamic> listData, DocX document)
        {
            float availableSpace = (document.PageWidth - document.MarginLeft - document.MarginRight) / colString.Count;

            var rowCnt = 1;
            foreach (dynamic rowData in listData)
            {
                for (int i = 0; i < colString.Count; i++)
                {
                    var value = ((rowData.GetType()).GetProperty(colString[i])).GetValue(rowData, null);
                    table.Rows[rowCnt].Cells[i].VerticalAlignment = VerticalAlignment.Center;
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].Append(value);
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].Alignment = Alignment.left;
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].SetLineSpacing(LineSpacingTypeAuto.Auto);
                    table.Rows[0].Cells[i].Width = availableSpace;
                }
                rowCnt++;
            }
            return table;
        }
        private Table tableRow(Table table, List<string> colString, IEnumerable<dynamic> listData)
        {
            var rowCnt = 1;
            foreach (dynamic rowData in listData)
            {
                for (int i = 0; i < colString.Count; i++)
                {
                    var value = ((rowData.GetType()).GetProperty(colString[i])).GetValue(rowData, null);
                    table.Rows[rowCnt].Cells[i].VerticalAlignment = VerticalAlignment.Center;
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].Append(value);
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].Alignment = Alignment.left;
                    table.Rows[rowCnt].Cells[i].Paragraphs[0].SetLineSpacing(LineSpacingTypeAuto.Auto);
                }
                rowCnt++;
            }
            return table;
        }
        private Paragraph Heading(string strText, int heading, DocX document)
        {
            var prgHead = document.InsertParagraph();
            prgHead.Append(strText);
            if (heading > 0)
            {
                if (heading == 1)
                    prgHead.Heading(HeadingType.Heading1);
                else if (heading == 2)
                    prgHead.Heading(HeadingType.Heading2);
                else if (heading == 3)
                    prgHead.Heading(HeadingType.Heading3);
                else if (heading == 4)
                    prgHead.Heading(HeadingType.Heading4);
            }
            if (heading != 3 && heading != 4)
                prgHead.SetLineSpacing(LineSpacingTypeAuto.AutoAfter);
            return prgHead;
        }
        //private Novacode tblAddList(List<string> list, DocX document)
        //{
        //    var prgCaption = document.InsertParagraph();
        //    var doclist = document.AddList(listType: ListItemType.Numbered, startNumber: 1);
        //    document.InsertList(doclist);
        //    return prgCaption;
        //}
        private Paragraph Caption(string strText, string strGray, DocX document)
        {
            var prgCaption = document.InsertParagraph();
            prgCaption.Bold().Font(new System.Drawing.FontFamily("Calibri"));
            prgCaption.Append(strText).Color(System.Drawing.Color.FromArgb(54, 95, 145));
            if (!string.IsNullOrEmpty(strGray))
                prgCaption.Append(strGray).Color(System.Drawing.Color.Gray);
            prgCaption.Heading(HeadingType.Caption);
            prgCaption.SetLineSpacing(LineSpacingTypeAuto.AutoAfter);
            prgCaption.Alignment = Alignment.center;
            return prgCaption;
        }
        private Paragraph tblCaption(string strGray, string strText, DocX document)
        {
            var prgCaption = document.InsertParagraph();

            if (!string.IsNullOrEmpty(strGray))
            {
                prgCaption.Append(strGray.ToUpper()).Font(new System.Drawing.FontFamily("Calibri")).Color(System.Drawing.Color.FromArgb(36, 63, 96));
            }
            prgCaption.Append(strText).Color(System.Drawing.Color.Black);
            prgCaption.SetLineSpacing(LineSpacingTypeAuto.AutoAfter);
            prgCaption.Alignment = Alignment.both;
            return prgCaption;
        }
        private Paragraph Error(string strText, DocX document)
        {
            var prgHead = document.InsertParagraph();
            prgHead.Append(strText);
            prgHead.SetLineSpacing(LineSpacingTypeAuto.AutoAfter);
            return prgHead;
        }
        private DocX ImagePlace(DocX document, string companyLogo, string value)
        {
            if (!string.IsNullOrEmpty(companyLogo))
            {
                string logoPath = Server.MapPath("~/logo/") + companyLogo;
                var original = System.Drawing.Image.FromFile(logoPath);
                using (var image = new MemoryStream(ResizeImage(original, new Size(200, 200))))
                {
                    var img = document.AddImage(image);
                    var picture = img.CreatePicture();
                    int countReplace = 0;
                    int count = value.Length;
                    foreach (Paragraph paragraph in document.Paragraphs)
                    {
                        List<int> valuesIndex = paragraph.FindAll(value);
                        if (valuesIndex.Count > 0)
                        {
                            if (count > 0)
                            {
                                if (valuesIndex.Count > count)
                                    valuesIndex.RemoveRange(count, valuesIndex.Count - count);
                            }
                            valuesIndex.Reverse();
                            foreach (int valueIndex in valuesIndex)
                            {
                                countReplace += 1;
                                paragraph.InsertPicture(picture, valueIndex + value.Length);
                                paragraph.RemoveText(valueIndex, value.Length);
                                if (countReplace == count)
                                {
                                    document.Save();
                                    return document;
                                }
                            }
                        }
                    }
                }
            }
            return document;
        }
        private DocX HeaderImagePlace(DocX document, string companyLogo, string value)
        {
            if (!string.IsNullOrEmpty(companyLogo))
            {
                string logoPath = Server.MapPath("~/logo/") + companyLogo;
                var original = System.Drawing.Image.FromFile(logoPath);
                using (var image = new MemoryStream(ResizeImage(original, new Size(42, 40))))
                {
                    var img = document.AddImage(image);
                    var picture = img.CreatePicture();
                    int countReplace = 0;
                    int count = value.Length;
                    foreach (Paragraph paragraph in document.Headers.odd.Paragraphs)
                    {
                        List<int> valuesIndex = paragraph.FindAll(value);
                        if (valuesIndex.Count > 0)
                        {
                            if (count > 0)
                            {
                                if (valuesIndex.Count > count)
                                    valuesIndex.RemoveRange(count, valuesIndex.Count - count);
                            }
                            valuesIndex.Reverse();
                            foreach (int valueIndex in valuesIndex)
                            {
                                countReplace += 1;
                                paragraph.InsertPicture(picture, valueIndex + value.Length);
                                paragraph.RemoveText(valueIndex, value.Length);
                                if (countReplace == count)
                                {
                                    document.Save();
                                    return document;
                                }
                            }
                        }
                    }
                }
            }
            return document;
        }
        private byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }
        public static byte[] ResizeImage(System.Drawing.Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                if (originalWidth > size.Width && originalHeight > size.Height)
                {
                    float percentWidth = (float)size.Width / (float)originalWidth;
                    float percentHeight = (float)size.Height / (float)originalHeight;
                    float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                    newWidth = (int)(originalWidth * percent);
                    newHeight = (int)(originalHeight * percent);
                }
                else
                {
                    newWidth = originalWidth;
                    newHeight = originalHeight;
                }
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            System.Drawing.Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            //return newImage;
            MemoryStream ms = new MemoryStream();
            newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        private List<Permission> permissionByRole(string rolename)
        {
            List<Permission> permissions = new List<Permission>();
            using (var pc = new PermissionContext())
            {
                var rolePermissions = pc.Permissions.Where(p => p.RoleName.Trim() == rolename.Trim()).ToList();
                foreach (var entity in GeneratorBase.MVC.ModelReflector.Entities)
                {
                    var calculated = new Permission();
                    var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                    calculated.EntityName = entity.Name;
                    calculated.RoleName = rolename;
                    calculated.CanEdit = raw.Any(p => p.CanEdit);
                    calculated.CanDelete = raw.Any(p => p.CanDelete);
                    calculated.CanAdd = raw.Any(p => p.CanAdd);
                    calculated.CanView = raw.Any(p => p.CanView);
                    calculated.IsOwner = raw.Any(p => p.IsOwner != null && p.IsOwner.Value);
                    if (calculated.IsOwner != null && calculated.IsOwner.Value)
                        calculated.UserAssociation = raw.FirstOrDefault().UserAssociation;
                    else
                        calculated.UserAssociation = string.Empty;

                    //code for verb action security
                    var verblist = raw.Select(x => x.Verbs).ToList();
                    var verbrolecount = verblist.Count();
                    List<string> allverbs = new List<string>();
                    foreach (var verb in verblist)
                    {
                        if (verb != null)
                            allverbs.AddRange(verb.Split(",".ToCharArray()).ToList());
                    }
                    var blockedverbs = allverbs.GroupBy(p => p).Where(p => p.Count() == verbrolecount);
                    if (blockedverbs.Count() > 0)
                        calculated.Verbs = string.Join(",", blockedverbs.Select(b => b.Key).ToList());
                    else
                        calculated.Verbs = string.Empty;


                    //FLS
                    var listEdit = raw.Select(p => p.NoEdit).ToList();
                    var listView = raw.Select(p => p.NoView).ToList();
                    var resultEdit = "";
                    var resultView = "";
                    foreach (var str in listEdit)
                    {
                        if (str != null)
                            resultEdit += str;
                    }
                    foreach (var str in listView)
                    {
                        if (str != null)
                            resultView += str;
                    }
                    calculated.NoEdit = resultEdit;
                    calculated.NoView = resultView;
                    //
                    permissions.Add(calculated);
                }
            }
            return permissions;
        }
    }
    class FLS
    {
        public string Entity_Name { get; set; }
        public string Property_Name { get; set; }
        public string Cannot_Edit { get; set; }
        public string Cannot_View { get; set; }
    }
    class RoleInfo
    {
        public string CellDesc { get; set; }
        public string CellCnt { get; set; }
    }
}
