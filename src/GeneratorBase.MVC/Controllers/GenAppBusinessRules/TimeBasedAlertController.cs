using GeneratorBase.MVC.Models;
using RecurrenceGenerator;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{

    public class TimeBasedAlertController : Controller
    {
        public DbSet GetGenericData(ApplicationContext context, Type _type)
        {
            return context.Set(_type);
        }
        public ActionResult ScheduledTask(string EntityName, long BizId)
        {
            var user = new InternalUser();
            var MainBiz = new BusinessRule();
            List<BusinessRule> businessrules = new List<BusinessRule>();
            using (var br = new BusinessRuleContext())
            {
                var rolebr = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable).ToList();
                MainBiz = rolebr.FirstOrDefault(p => p.Id == BizId);
                foreach (var rules in rolebr)
                {
                    if (rules.Roles.Split(",".ToCharArray()).Contains("All"))
                    {
                        businessrules.Add(rules);
                    }
                }
            }
            if (MainBiz != null)
            {
                (user).businessrules = businessrules.ToList();
                var database = new ApplicationContext(user);
                var myType = Type.GetType("GeneratorBase.MVC.Models." + EntityName + "");
                var data = GetGenericData(database, myType).ToListAsync();
                foreach (var item in data.Result)
                {
                    if (ApplyRule.CheckRule<object>(item, MainBiz, MainBiz.EntityName))
                    {
                        database.Entry(item).State = EntityState.Modified;
                        database.SaveChanges();
                    }
                }
                ScheduledTaskHistoryContext sthcontext = new ScheduledTaskHistoryContext();
                var itemhistory = sthcontext.ScheduledTaskHistorys.FirstOrDefault(p => p.BusinessRuleId == MainBiz.Id);
                itemhistory.Status = "Processed";
                sthcontext.Entry(itemhistory).State = EntityState.Modified;
                sthcontext.SaveChanges();

                RegisterScheduledTask nexttask = new RegisterScheduledTask();
                nexttask.RegisterTask(MainBiz.EntityName, MainBiz.Id);
            }
            return Json("Success", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotifyOneTime(string EntityName, long Id, long actionid, string userName)
        {
            var AppName = CommonFunction.Instance.AppName();
            var server = CommonFunction.Instance.Server();
            string NotifyTo = "";
            string NotifyToExtra = "";
            string NotifyToRole = "";
            string emailTo = "";
            var alertMessage = "";
            var ruleactiondb = new RuleActionContext();
            var act = ruleactiondb.RuleActions.First(p => p.Id == actionid);
            var ruledb = new BusinessRuleContext();

            var br = ruledb.BusinessRules.Find(act.RuleActionID);
            var subject = br.RuleName;
            alertMessage += act.ErrorMessage;
            var argslist = act.actionarguments.ToList();
            foreach (var args in argslist)
            {
                if (args.ParameterName == "NotifyTo")
                    NotifyTo = args.ParameterValue;
                if (args.ParameterName == "NotifyToExtra")
                    NotifyToExtra = args.ParameterValue;
                if (args.ParameterName == "NotifyToRole")
                    NotifyToRole = args.ParameterValue;
            }
            if (!string.IsNullOrEmpty(userName))
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                MethodInfo mc = controller.GetMethod("GetRecordById");
                object[] MethodParams = new object[] { Convert.ToString(Id) };
                var entry = mc.Invoke(objController, MethodParams);

                getEmail objgetEmail = new getEmail();
                var emails1 = objgetEmail.getEmailids(new SystemUser(), NotifyTo.Split(",".ToCharArray()), entry, NotifyToRole.Split(",".ToCharArray()), userName);
                emailTo = emails1;
                if (!string.IsNullOrEmpty(NotifyToRole))
                {
                    emailTo += "," + objgetEmail.getUserEmailidsFromRoles(NotifyToRole.Split(",".ToCharArray()));
                }
            }
            if (!string.IsNullOrEmpty(NotifyToExtra))
                emailTo += "," + NotifyToExtra;
            emailTo = emailTo.Trim(',');
            emailTo = emailTo.Trim().TrimEnd(",".ToCharArray());
            emailTo = emailTo.Replace(",,", ",");
            //
            if (alertMessage.ToUpper().Contains("###RECORD###"))
            {
                try
                {
                    Type controller1 = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                    object objController1 = Activator.CreateInstance(controller1, null);
                    MethodInfo mc1 = controller1.GetMethod("GetRecordById_Reflection");
                    object[] MethodParams1 = new object[] { Convert.ToString(Id) };
                    var msgDetails1 = Convert.ToString(mc1.Invoke(objController1, MethodParams1));
                    alertMessage = alertMessage.Replace("###Record###", msgDetails1);
                }
                catch { }
            }
            if (alertMessage.ToUpper().Contains("###RECORDLINK###"))
            {
                alertMessage = alertMessage.Replace("###RecordLink###", "<a href=\"" + "http://" + server + Url.Action("Edit", EntityName, new { Id = Id }) + "\">Link</a>");

                //alertMessage += "<br/><a href=\"" + "http://" + server + Url.Action("Edit", EntityName, new { Id = Id }) + "\">Click to review</a>";
            }
            if (!string.IsNullOrEmpty(emailTo))
            {
                SendEmail mail = new SendEmail();
                var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "Business Rule");
                if (EmailTemplate != null)
                {
                    string mailbody = string.Empty;
                    if (!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                    {
                        mailbody = EmailTemplate.EmailContent;
                        mailbody = mailbody.Replace("###Message###", alertMessage);
                    }
                    if (!string.IsNullOrEmpty(EmailTemplate.EmailSubject))
                        subject = EmailTemplate.EmailSubject;
                    emailTo = string.Join(",", emailTo.Split(',').Distinct().ToArray());
                    mail.Notify(emailTo, mailbody, subject);
                }
            }
            return null;
        }
    }
}