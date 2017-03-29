using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_SessionEvents"),CustomDisplayName("T_SessionEvents", "T_SessionEvents.resx", "Session Events")]
	public class T_SessionEvents : Entity
    {	
		public T_SessionEvents()
        {
            this.T_SessionEventsClient_t_sessionevents = new HashSet<T_SessionEventsClient>();
        }

		 
		 
		[CustomDisplayName("T_Title","T_SessionEvents.resx","Title"), Column("Title")] [Required] 
		
        public string T_Title { get; set; }

		[DataType(DataType.Date)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)] 
		 
		[CustomDisplayName("T_EventDate","T_SessionEvents.resx","Event Date"), Column("EventDate")]  
		
        public Nullable<DateTime> T_EventDate { get; set; }

		[DataType(DataType.Time)][DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[CustomDisplayName("T_StartTime","T_SessionEvents.resx","Start Time"), Column("StartTime")]  
		
        public Nullable<DateTime> T_StartTime { get; set; }

		[DataType(DataType.Time)][DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[CustomDisplayName("T_EndTime","T_SessionEvents.resx","End Time"), Column("EndTime")]  
		
        public Nullable<DateTime> T_EndTime { get; set; }

		 
		 
		[CustomDisplayName("T_IsCancelled","T_SessionEvents.resx","Cancel this Event"), Column("IsCancelled")]  
		
        public Boolean? T_IsCancelled { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_SessionEvents.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }

		[CustomDisplayName("T_SessionEventsLearningCenterID","T_SessionEvents.resx","Learning Center"), Column("SessionEventsLearningCenterID")]
        public Nullable<long> T_SessionEventsLearningCenterID { get; set; }
		
        public virtual T_LearningCenter t_sessioneventslearningcenter { get; set;}
		
		[CustomDisplayName("ScheduleID","T_SessionEvents.resx","Schedule"), Column("ScheduleID")]
        public Nullable<long> ScheduleID { get; set; }
		
        public virtual T_Schedule schedule { get; set;}
		
		[CustomDisplayName("T_SessionEventsTimeSlotsID","T_SessionEvents.resx","Time Slot"), Column("SessionEventsTimeSlotsID")]
        public Nullable<long> T_SessionEventsTimeSlotsID { get; set; }
		
        public virtual T_TimeSlots t_sessioneventstimeslots { get; set;}
				
        public virtual ICollection<T_SessionEventsClient> T_SessionEventsClient_t_sessionevents { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_Title != null ?Convert.ToString(this.T_Title)+" " : Convert.ToString(this.T_Title))+(this.T_EventDate.HasValue ? this.T_EventDate.Value.ToShortDateString()+" " : "")+(this.T_StartTime != null ?Convert.ToString(this.T_StartTime)+" " : Convert.ToString(this.T_StartTime))+(this.T_EndTime != null ?Convert.ToString(this.T_EndTime)+" " : Convert.ToString(this.T_EndTime))+(this.T_IsCancelled != null ?Convert.ToString(this.T_IsCancelled)+" " : Convert.ToString(this.T_IsCancelled))+(this.T_Description != null ?Convert.ToString(this.T_Description)+" " : Convert.ToString(this.T_Description)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_Title != null ?Convert.ToString(this.T_Title)+" " : Convert.ToString(this.T_Title))+(this.T_EventDate.HasValue ? this.T_EventDate.Value.ToShortDateString()+" " : "")+(this.T_StartTime.HasValue ?Convert.ToString(this.T_StartTime.Value.Add(m_OffSet).ToShortTimeString())+" " : "")+(this.T_EndTime.HasValue ?Convert.ToString(this.T_EndTime.Value.Add(m_OffSet).ToShortTimeString())+" " : "")+(this.T_IsCancelled != null ?Convert.ToString(this.T_IsCancelled)+" " : Convert.ToString(this.T_IsCancelled))+(this.T_Description != null ?Convert.ToString(this.T_Description)+" " : Convert.ToString(this.T_Description)); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

