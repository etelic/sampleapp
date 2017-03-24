//@using Microsoft.AspNet.Identity
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
using RecurrenceGenerator;
namespace GeneratorBase.MVC.Controllers
{
    public partial class T_ScheduleController : BaseController
    {
		public ActionResult CreateEvent()
        {
            Dictionary<string,string> list = new Dictionary<string,string>();
            return View(list);
        }
	    public void AfterSave(T_Schedule t_schedule) //mahesh
        {
            bool flagT_RepeatOn = false;
            ApplicationContext tempdb = new ApplicationContext(new SystemUser());
            foreach (var obj in tempdb.T_RepeatOns.Where(a => a.T_ScheduleID == t_schedule.Id))
            {
                tempdb.T_RepeatOns.Remove(obj);
                flagT_RepeatOn = true;
            }
            if (flagT_RepeatOn)
                tempdb.SaveChanges();
            if (t_schedule.SelectedT_RecurrenceDays_T_RepeatOn != null)
            {
                foreach (var pgs in t_schedule.SelectedT_RecurrenceDays_T_RepeatOn)
                {
                    T_RepeatOn objT_RepeatOn = new T_RepeatOn();
                    objT_RepeatOn.T_ScheduleID = t_schedule.Id;
                    objT_RepeatOn.T_RecurrenceDaysID = pgs;
                    tempdb.T_RepeatOns.Add(objT_RepeatOn);
                }
                tempdb.SaveChanges();
            }
			           
            // Write your logic here

        }
		public ActionResult getEvents(string ids)//mahesh
        {
            List<TemplateEvents> result = new List<TemplateEvents>();
            if (!string.IsNullOrEmpty(ids))
                foreach (var item in ids.Split(",".ToCharArray()))
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var id = Convert.ToInt64(item);
                    var scheduleobj = db.T_Schedules.Find(id);
								
                }
            // result.ForEach(p => p.schedule = null);
            return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EventsUpdate(string id,string EntityName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            long ID = Convert.ToInt64(id);
            return View();
        }
	}
}

