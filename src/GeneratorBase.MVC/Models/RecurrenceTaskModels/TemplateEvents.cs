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
    //[Table("tbl_ScheduleEvents"), DisplayName("Schedule Events")]
    public class TemplateEvents : Entity//mahesh
    {
        public string Title { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string Description { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsNotify { get; set; }
        public string EmailTo { get; set; }
        public string Notes { get; set; }
        public string EntityName { get; set; }
        public Nullable<long> MainMeetingID { get; set; }
        public Nullable<DateTime> EventDate { get; set; }

        [DisplayName("Schedule"), Column("ScheduleID")]
        public Nullable<long> ScheduleID { get; set; }
        public virtual T_Schedule schedule { get; set; }
        public string getDisplayValue()
        {
            try
            {
                var dispValue = this.Title + " : " + this.StartTime + "-" + this.EndTime;
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
                var dispValue = this.Title + " : " + this.StartTime + "-" + this.EndTime;
                this.m_DisplayValue = dispValue;
                return dispValue != null ? dispValue : "";
            }
            catch { return ""; }
        }

    }

}

