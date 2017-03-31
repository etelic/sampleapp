using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using RecurrenceGenerator;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_Schedule"), DisplayName("Schedule")]
    public class T_Schedule : Entity
    {
        public T_Schedule()
        {
            this.T_RepeatOn_t_schedule = new HashSet<T_RepeatOn>();
        }



        [DisplayName("Name"), Column("Name")]
        [Required]
                public string T_Name { get; set; }



        [DisplayName("Description"), Column("Description")]
        public string T_Description { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date Time"), Column("StartDateTime")]
        [Required]
        public DateTime T_StartDateTime { get; set; }


        //
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Time"), Column("StartTime")]  
        public Nullable<DateTime> T_StartTime { get; set; }
        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DisplayName("End Time"), Column("EndTime")]  
        public Nullable<DateTime> T_EndTime { get; set; }
        //


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date"), Column("EndDate")]
        public Nullable<DateTime> T_EndDate { get; set; }

        [DisplayName("Occurrence Limit Count"), Column("OccurrenceLimitCount")]
        public Nullable<Int32> T_OccurrenceLimitCount { get; set; }

        [DisplayName("Summary"), Column("Summary")]
        public string T_Summary { get; set; }

        [DisplayName("Schedule Type"), Column("AssociatedScheduleTypeID")]
        public Nullable<long> T_AssociatedScheduleTypeID { get; set; }

        public virtual T_Scheduletype t_associatedscheduletype { get; set; }

        [DisplayName("Repeat Type"), Column("AssociatedRecurringScheduleDetailsTypeID")]
        public Nullable<long> T_AssociatedRecurringScheduleDetailsTypeID { get; set; }

        public virtual T_RecurringScheduleDetailstype t_associatedrecurringscheduledetailstype { get; set; }

        [DisplayName("Repeat Every"), Column("RecurringRepeatFrequencyID")]
        public Nullable<long> T_RecurringRepeatFrequencyID { get; set; }

        public virtual T_RecurringFrequency t_recurringrepeatfrequency { get; set; }

        [DisplayName("Repeat By"), Column("RepeatByID")]
        public Nullable<long> T_RepeatByID { get; set; }

        public virtual T_MonthlyRepeatType t_repeatby { get; set; }

        [DisplayName("Ends"), Column("RecurringTaskEndTypeID")]
        public Nullable<long> T_RecurringTaskEndTypeID { get; set; }

        public virtual T_RecurringEndType t_recurringtaskendtype { get; set; }

        public virtual ICollection<T_RepeatOn> T_RepeatOn_t_schedule { get; set; }
        [NotMapped]
        public virtual ICollection<T_RecurrenceDays> T_RecurrenceDays_T_RepeatOn { get; set; }
        [NotMapped]
        public virtual ICollection<long?> SelectedT_RecurrenceDays_T_RepeatOn { get; set; }
        [DisplayName("EntityName"), Column("EntityName")]
        public string T_EntityName { get; set; }
        public string getDisplayValue()
        {
            try
            {
                var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + "  " : Convert.ToString(this.T_Name)) + (this.T_StartDateTime != null ? Convert.ToString(this.T_StartDateTime) + " - " : Convert.ToString(this.T_StartDateTime)) + (this.T_AssociatedRecurringScheduleDetailsTypeID != null ? (new ApplicationContext(new SystemUser())).T_RecurringScheduleDetailstypes.FirstOrDefault(p => p.Id == this.T_AssociatedRecurringScheduleDetailsTypeID).DisplayValue + " - " : "") + (this.T_AssociatedScheduleTypeID != null ? (new ApplicationContext(new SystemUser())).T_Scheduletypes.FirstOrDefault(p => p.Id == this.T_AssociatedScheduleTypeID).DisplayValue + " - " : "") + (this.T_RecurringRepeatFrequencyID != null ? (new ApplicationContext(new SystemUser())).T_RecurringFrequencys.FirstOrDefault(p => p.Id == this.T_RecurringRepeatFrequencyID).DisplayValue + " - " : "");
                dispValue = dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
                this.m_DisplayValue = dispValue;
                return dispValue;
            }
            catch { return ""; }
        }
        public override string getDisplayValueModel()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_DisplayValue))
                    return m_DisplayValue;
                var dispValue = (this.T_Name != null ? Convert.ToString(this.T_Name) + "  " : Convert.ToString(this.T_Name)) + (this.T_StartDateTime != null ? Convert.ToString(this.T_StartDateTime.Add(m_OffSet)) + " - " : Convert.ToString(this.T_StartDateTime)) + (this.T_EndDate != null ? Convert.ToString(this.T_EndDate.Value.ToShortDateString()) + " - " : Convert.ToString(this.T_EndDate)) + (this.t_associatedrecurringscheduledetailstype != null ? t_associatedrecurringscheduledetailstype.DisplayValue + " - " : "") + (this.t_associatedscheduletype != null ? t_associatedscheduletype.DisplayValue + " - " : "") + (this.t_recurringrepeatfrequency != null ? t_recurringrepeatfrequency.DisplayValue + " - " : "");
                return dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
            }
            catch { return ""; }
        }
        public void setCalculation() { try { } catch { } }
        public List<TemplateEvents> getEvents(T_Schedule task)//mahesh
        {
            ApplicationContext dbschedule = new ApplicationContext(new SystemUser());

            List<TemplateEvents> result = new List<TemplateEvents>();

            var ScheduledStartTime = task.T_StartTime.Value.ToLongTimeString();
            var ScheduledEndTime = task.T_EndTime.Value.ToLongTimeString();

            task.T_StartDateTime = Convert.ToDateTime(task.T_StartDateTime.ToLongDateString() + " " + task.T_StartTime.Value.ToLongTimeString());

            var ScheduledDateTimeEnd = DateTime.Now.AddYears(1);
            var occurrences = task.T_OccurrenceLimitCount != null ? task.T_OccurrenceLimitCount : 0;
            var skip = task.T_RecurringRepeatFrequencyID != null ? Convert.ToInt32(task.T_RecurringRepeatFrequencyID) : 0;

            if(task.T_AssociatedScheduleTypeID == 1)
            {
                TemplateEvents obj = new TemplateEvents();
                obj.EventDate = task.T_StartDateTime;
                obj.StartTime = task.T_StartDateTime;
                obj.Title = task.T_Name;
                obj.EndTime = Convert.ToDateTime(task.T_StartDateTime.ToShortDateString() + " " + ScheduledEndTime);
                result.Add(obj);
                return result;
            }

            if (task.T_RecurringTaskEndTypeID == 3)
                ScheduledDateTimeEnd = task.T_EndDate.Value;

            if (task.T_AssociatedRecurringScheduleDetailsTypeID == 1)
            {
                DailyRecurrenceSettings we;
                if (task.T_RecurringTaskEndTypeID == 2)
                    we = new DailyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
                else
                    we = new DailyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
                var values = we.GetValues(skip);
                foreach (var evt in values.Values)
                {
                    TemplateEvents obj = new TemplateEvents();
                    obj.EventDate = evt;
                    obj.StartTime = evt;
                    obj.Title = task.T_Name;
                    obj.EndTime = Convert.ToDateTime(evt.ToShortDateString() + " " + ScheduledEndTime);
                    result.Add(obj);
                }
            }
            if (task.T_AssociatedRecurringScheduleDetailsTypeID == 2)
            {
                WeeklyRecurrenceSettings we;
                SelectedDayOfWeekValues selectedValues = new SelectedDayOfWeekValues();
                if (task.T_RecurringTaskEndTypeID == 2)
                    we = new WeeklyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
                else
                    we = new WeeklyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
                selectedValues.Sunday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(1);
                selectedValues.Monday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(2);
                selectedValues.Tuesday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(3);
                selectedValues.Wednesday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(4);
                selectedValues.Thursday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(5);
                selectedValues.Friday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(6);
                selectedValues.Saturday = task.SelectedT_RecurrenceDays_T_RepeatOn.ToList().Contains(7);
                var values = we.GetValues(skip, selectedValues);
                foreach (var evt in values.Values)
                {
                    TemplateEvents obj = new TemplateEvents();
                    obj.EventDate = evt;
                    obj.StartTime = evt;
                    obj.Title = task.T_Name;
                    obj.EndTime = Convert.ToDateTime(evt.ToShortDateString() + " " + ScheduledEndTime);
                    result.Add(obj);
                }
            }
            if (task.T_AssociatedRecurringScheduleDetailsTypeID == 3)
            {
                MonthlyRecurrenceSettings we;


                if (task.T_RecurringTaskEndTypeID == 2)
                    we = new MonthlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
                else
                    we = new MonthlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
                we.AdjustmentValue = 0;
                skip = skip++;
                RecurrenceValues value = new RecurrenceValues();
                if (task.T_RepeatByID == 3)
                    value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.Day, skip);
                if (task.T_RepeatByID == 4)
                    value = we.GetValues(MonthlySpecificDatePartOne.First, MonthlySpecificDatePartTwo.Day, skip);
                if (task.T_RepeatByID == 1)
                    value = we.GetValues(task.T_StartDateTime.Day, skip);
                if (task.T_RepeatByID == 2)
                    value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.WeekendDay, skip);
                foreach (var evt in value.Values)
                {
                    TemplateEvents obj = new TemplateEvents();
                    obj.EventDate = evt;
                    obj.StartTime = evt;
                    obj.Title = task.T_Name;
                    obj.EndTime = Convert.ToDateTime(evt.ToShortDateString() + " " + ScheduledEndTime);
                    result.Add(obj);
                }
            }
            if (task.T_AssociatedRecurringScheduleDetailsTypeID == 4)
            {
                YearlyRecurrenceSettings we;
                if (task.T_RecurringTaskEndTypeID == 2)
                    we = new YearlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
                else
                    we = new YearlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
                var values = we.GetValues(task.T_StartDateTime.Day, task.T_StartDateTime.Month);
                foreach (var evt in values.Values)
                {
                    TemplateEvents obj = new TemplateEvents();
                    obj.EventDate = evt;
                    obj.StartTime = evt;
                    obj.Title = task.T_Name;
                    obj.EndTime = Convert.ToDateTime(evt.ToShortDateString() + " " + ScheduledEndTime);
                    result.Add(obj);
                }
            }
            //return Json(result, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
   

}

