using GeneratorBase.MVC.Models;
using System;
using System.Linq;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;
            string entity = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var RecdId = 0;
            if (filterContext.ActionParameters.First().Key.ToUpper() == "ID")
                RecdId = Convert.ToInt32(filterContext.ActionParameters.First().Value);
            //Generate an audit
            JournalEntry audit = new JournalEntry()
            {
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                EntityName = entity,
                RecordInfo = "<a href=\"" + request.RawUrl + "\">" + (RecdId > 0 ? EntityComparer.GetDisplayValueForAssociation(entity, Convert.ToString(RecdId)) : "Click to view") + "</a>",
                DateTimeOfEntry = DateTime.Now,
                RecordId = RecdId,
                Type = filterContext.ActionDescriptor.ActionName
            };
            //Stores the Audit in the Database
            JournalEntryContext context = new JournalEntryContext();
            context.JournalEntries.Add(audit);
            context.SaveChanges();
            //Finishes executing the Action as normal 
            base.OnActionExecuting(filterContext);
        }
    }
}