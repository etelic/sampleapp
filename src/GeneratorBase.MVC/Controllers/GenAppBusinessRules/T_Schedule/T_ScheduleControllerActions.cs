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
		public ActionResult CreateEvent(string startDate, string slotID)
        {
            Dictionary<string,string> list = new Dictionary<string,string>();
			ViewData["startDate"] = startDate;
            ViewData["slotID"] = slotID;
            list.Add("T_Session" , "Session");
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
			            if (t_schedule.T_EntityName == "T_Session")
            {
                flagT_RepeatOn = false;
                foreach (var evt in tempdb.T_SessionEventss.Where(p => p.ScheduleID == t_schedule.Id))
                {
                    tempdb.T_SessionEventss.Remove(evt);
                    flagT_RepeatOn = true;
                }
                if (flagT_RepeatOn)
                    tempdb.SaveChanges();
                T_Schedule scheduleevents = new T_Schedule();
                var result = scheduleevents.getEvents(t_schedule);
                foreach (var item in result)
                {
                    T_SessionEvents obj = new T_SessionEvents();

                    obj.ScheduleID = t_schedule.Id;
                    obj.T_EventDate = item.EventDate;
                    obj.T_StartTime = item.StartTime;
                    obj.T_Title = item.Title;
                    obj.T_EndTime = item.EndTime;

                    tempdb.T_SessionEventss.Add(obj);
                }
                tempdb.SaveChanges();
            }
			        }
		public ActionResult getAllMeetingEvents()
        {
            List<TemplateEvents> result = new List<TemplateEvents>();
            foreach (var item in db.T_Schedules.ToList())
            {
                if (item != null && item.T_EntityName == "T_Session")
                    foreach (var obj in db.T_SessionEventss.Where(p => p.ScheduleID == item.Id).ToList())
                    {
                        TemplateEvents _temp = new TemplateEvents();
                        _temp.Id = obj.Id;
                        _temp.Title = obj.T_Title;
                        _temp.StartTime = Convert.ToDateTime(obj.T_EventDate.Value.ToShortDateString() + " " + obj.T_StartTime.Value.ToLongTimeString()).ToLocalTime();
                        _temp.EndTime = Convert.ToDateTime(obj.T_EventDate.Value.ToShortDateString() + " " + obj.T_EndTime.Value.ToLongTimeString()).ToLocalTime();
                        _temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                        _temp.ScheduleID = obj.ScheduleID;
                        _temp.EntityName = item.T_EntityName;
                        result.Add(_temp);
                    }

            }
            //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
					if (scheduleobj != null && scheduleobj.T_EntityName == "T_Session")
                    foreach (var obj in db.T_SessionEventss.Where(p => p.ScheduleID == id).ToList())
                    {
                        TemplateEvents _temp = new TemplateEvents();
                        _temp.Id = obj.Id;
                        _temp.Title = obj.T_Title;
                        _temp.StartTime = Convert.ToDateTime(obj.T_EventDate.Value.ToShortDateString() + " " + obj.T_StartTime.Value.ToLongTimeString()).ToLocalTime();
                        _temp.EndTime = Convert.ToDateTime(obj.T_EventDate.Value.ToShortDateString() + " " + obj.T_EndTime.Value.ToLongTimeString()).ToLocalTime();
                        _temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                        _temp.ScheduleID = obj.ScheduleID;
                        _temp.EntityName = scheduleobj.T_EntityName;
                        result.Add(_temp);
                    }
					
                }
            // result.ForEach(p => p.schedule = null);
            //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult EventsUpdate(string id,string EntityName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            long ID = Convert.ToInt64(id);
            if (EntityName == "T_Session")
            {
                var obj = db.T_SessionEventss.Find(ID);
                if (obj == null)
                {
                    return HttpNotFound();
                }
                TemplateEvents template = new TemplateEvents();
                template.Id = obj.Id;
                template.Title = obj.T_Title;
                template.StartTime = obj.T_StartTime;
                template.EndTime = obj.T_EndTime;
                template.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                template.ScheduleID = obj.ScheduleID;
                template.EntityName = obj.schedule.T_EntityName;
				var objMeeting = db.T_Sessions.FirstOrDefault(p => p.ScheduleSessionID == obj.ScheduleID);
                template.MainMeetingID = objMeeting !=null ? objMeeting.Id : 0;
				
				List<UpdateButton> buttons = new List<UpdateButton>();
						
						buttons.Add(new UpdateButton {Type="Meeting", ButtonText = "Associate Session with Client", Target = "T_SessionClientAssociation", AssociatedType = "T_SessionClientAssociation_T_Session", HostingEntityName = "T_Session", PopupTitle = "Session" });
						
						buttons.Add(new UpdateButton {Type="Event", ButtonText = "Associate Session Event with Client", Target = "T_SessionEventsClient", AssociatedType = "T_SessionEventsClient_T_SessionEvents", HostingEntityName = "T_SessionEvents", PopupTitle = "T_SessionEventsClient" });
				if(buttons.Count > 0)
					ViewBag.buttons = buttons;

                return View(template);
            }
            return View();
        }

		public ActionResult getTimeSlotsByIds(string ids)
        {
            List<TemplateEvents> result = new List<TemplateEvents>();
            foreach(var item in ids.Split(",".ToCharArray()))
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                   var obj = db.T_TimeSlotss.Find(Convert.ToInt64(item));
                    TemplateEvents _temp = new TemplateEvents();
                    _temp.Id = 0;
                    _temp.Title = obj.DisplayValue;
                    _temp.StartTime = Convert.ToDateTime(obj.T_StartTime.ToShortDateString() + " " + obj.T_StartTime.ToLongTimeString()).ToLocalTime();
                    _temp.EndTime = Convert.ToDateTime(obj.T_EndTime.ToShortDateString() + " " + obj.T_EndTime.ToLongTimeString()).ToLocalTime();
                    //_temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                    _temp.ScheduleID = 0;
                    _temp.EntityName = Convert.ToString(obj.Id);
                    result.Add(_temp);
                }
		    }
           
            //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

		public ActionResult getTimeSlots(string EntityName, long id)
        {
            List<TemplateEvents> result = new List<TemplateEvents>();
 if (EntityName == "T_LearningCenter")
                foreach (var obj in db.T_TimeSlotss.Where(p => p.T_LearningCenterTimeSlotsAssociationID == id))
                {
                    TemplateEvents _temp = new TemplateEvents();
                    _temp.Id = 0;
                    _temp.Title = obj.DisplayValue;
                    _temp.StartTime = Convert.ToDateTime(obj.T_StartTime.ToShortDateString() + " " + obj.T_StartTime.ToLongTimeString()).ToLocalTime();
                    _temp.EndTime = Convert.ToDateTime(obj.T_EndTime.ToShortDateString() + " " + obj.T_EndTime.ToLongTimeString()).ToLocalTime();
                    //_temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                    _temp.ScheduleID = 0;
                    _temp.EntityName = Convert.ToString(obj.Id);
                    result.Add(_temp);
                }
            if (EntityName == "T_Session")
            foreach (var obj in db.T_Sessions.Where(p => p.Id == id).Select(p => p.t_sessiontimeslotassociaton))
            {
                TemplateEvents _temp = new TemplateEvents();
                _temp.Id = 0;
                _temp.Title = obj.DisplayValue;
                _temp.StartTime = Convert.ToDateTime(obj.T_StartTime.ToShortDateString() + " " + obj.T_StartTime.ToLongTimeString()).ToLocalTime();
                _temp.EndTime = Convert.ToDateTime(obj.T_EndTime.ToShortDateString() + " " + obj.T_EndTime.ToLongTimeString()).ToLocalTime();
                //_temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                _temp.ScheduleID = 0;
               _temp.EntityName = Convert.ToString(obj.Id);
                result.Add(_temp);
            }
            if (EntityName == "T_SessionEvents")
            foreach (var obj in db.T_SessionEventss.Where(p => p.Id == id).Select(p => p.t_sessioneventstimeslots))
            {
                TemplateEvents _temp = new TemplateEvents();
                _temp.Id = 0;
                _temp.Title = obj.DisplayValue;
                _temp.StartTime = Convert.ToDateTime(obj.T_StartTime.ToShortDateString() + " " + obj.T_StartTime.ToLongTimeString()).ToLocalTime();
                _temp.EndTime = Convert.ToDateTime(obj.T_EndTime.ToShortDateString() + " " + obj.T_EndTime.ToLongTimeString()).ToLocalTime();
                //_temp.IsCancelled = obj.T_IsCancelled == null ? false : obj.T_IsCancelled.Value;
                _temp.ScheduleID = 0;
               _temp.EntityName = Convert.ToString(obj.Id);
                result.Add(_temp);
            }
            //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
			var jsonResult = Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
		public ActionResult ShowFullCalendar(string t_sessionlearningcenterassociation,string t_sessiontimeslotassociaton,string schedulesession,string t_sessionclientassociation)
        {
		  var objlist = db.T_Schedules.AsQueryable();
		 
			var T_SessionLearningCenterAssociationlist = db.T_LearningCenters.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessionlearningcenterassociation != null && t_sessionlearningcenterassociation != "null")
            {
                var ids = t_sessionlearningcenterassociation.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				   foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                            }
                            else
                            {
                               //ids1.Add(Convert.ToInt64(str));
							   var idvalue = Convert.ToInt64(str);
                              ids1.AddRange(db.T_LearningCenters.Find(Convert.ToInt64(str)).t_sessionlearningcenterassociation.Select(p =>(long?)p.Id));
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = T_SessionLearningCenterAssociationlist.Union(db.T_LearningCenters.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessionlearningcenterassociation = new SelectList(list.ToList(), "ID", "DisplayValue");
					objlist = objlist.Where(p => ids1.Contains(p.Id));
			}
			else
			{
				ViewBag.t_sessionlearningcenterassociation = new SelectList(T_SessionLearningCenterAssociationlist.ToList(), "ID", "DisplayValue");
			}
			var T_SessionTimeSlotAssociatonlist = db.T_TimeSlotss.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessiontimeslotassociaton != null && t_sessiontimeslotassociaton != "null")
            {
                var ids = t_sessiontimeslotassociaton.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				   foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                            }
                            else
                            {
                               //ids1.Add(Convert.ToInt64(str));
							   var idvalue = Convert.ToInt64(str);
                              ids1.AddRange(db.T_TimeSlotss.Find(Convert.ToInt64(str)).t_sessiontimeslotassociaton.Select(p =>(long?)p.Id));
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = T_SessionTimeSlotAssociatonlist.Union(db.T_TimeSlotss.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessiontimeslotassociaton = new SelectList(list.ToList(), "ID", "DisplayValue");
					objlist = objlist.Where(p => ids1.Contains(p.Id));
					ViewData["TimeSlots"] = t_sessiontimeslotassociaton;
			}
			else
			{
				ViewBag.t_sessiontimeslotassociaton = new SelectList(T_SessionTimeSlotAssociatonlist.ToList(), "ID", "DisplayValue");
			}
			var ScheduleSessionlist = db.T_Schedules.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (schedulesession != null && schedulesession != "null")
            {
                var ids = schedulesession.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				   foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                            }
                            else
                            {
                               ids1.Add(Convert.ToInt64(str));
                            }
                        }
                    }

					ids1 = ids1.ToList();
					var list = ScheduleSessionlist.Union(db.T_Schedules.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.schedulesession = new SelectList(list.ToList(), "ID", "DisplayValue");
					objlist = objlist.Where(p => ids1.Contains(p.Id));
			}
			else
			{
				ViewBag.schedulesession = new SelectList(ScheduleSessionlist.ToList(), "ID", "DisplayValue");
			}
			var T_SessionClientAssociationlist = db.T_Clients.OrderBy(p=>p.DisplayValue).Take(10).Distinct();
            if (t_sessionclientassociation != null && t_sessionclientassociation != "null")
            {
                var ids = t_sessionclientassociation.Split(",".ToCharArray());
                List<long?> ids1 = new List<long?>();
				  foreach (var str in ids)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str == "NULL")
                            {
                                ids1.Add(null);
                            }
                            else
                            {
                               //ids1.Add(Convert.ToInt64(str));
							   var idvalue= Convert.ToInt64(str);
							   ids1.AddRange(db.T_SessionClientAssociations.Where(p=>p.T_ClientID ==idvalue).Select(p=>p.t_session.ScheduleSessionID));
                            }
                        }
                    }
					ids1 = ids1.ToList();
					var list = T_SessionClientAssociationlist.Union(db.T_Clients.Where(p=>ids1.Contains(p.Id))).Distinct();
					ViewBag.t_sessionclientassociation = new SelectList(list.ToList(), "ID", "DisplayValue");
					objlist = objlist.Where(p => ids1.Contains(p.Id));
			}
			else
			{
				ViewBag.t_sessionclientassociation = new SelectList(T_SessionClientAssociationlist.ToList(), "ID", "DisplayValue");
			}
			 return View(objlist.ToList());
		}
	}
	public class UpdateButton
    {
        public string ButtonText { get; set; }
        public string Target { get; set; }
        public string AssociatedType { get; set; }
        public string HostingEntityName { get; set; }
        public string PopupTitle { get; set; }
        public string Type { get; set; }
    }
}

