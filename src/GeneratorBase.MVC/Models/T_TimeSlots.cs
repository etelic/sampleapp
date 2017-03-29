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
    [Table("tbl_TimeSlots"),CustomDisplayName("T_TimeSlots", "T_TimeSlots.resx", "Time Slots")]
	public class T_TimeSlots : Entity
    {	
		public T_TimeSlots()
        {
            this.t_sessiontimeslotassociaton = new HashSet<T_Session>();
            this.t_sessioneventstimeslots = new HashSet<T_SessionEvents>();
        }

		 
		 
		[CustomDisplayName("T_SlotNo","T_TimeSlots.resx","SlotNo"), Column("SlotNo")]  
		
        public Nullable<long> T_SlotNo { get; set; }

		[DataType(DataType.Time)][DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[CustomDisplayName("T_StartTime","T_TimeSlots.resx","Start Time"), Column("StartTime")] [Required] 
		
        public DateTime T_StartTime { get; set; }

		[DataType(DataType.Time)][DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[CustomDisplayName("T_EndTime","T_TimeSlots.resx","End Time"), Column("EndTime")] [Required] 
		
        public DateTime T_EndTime { get; set; }

		[CustomDisplayName("T_LearningCenterTimeSlotsAssociationID","T_TimeSlots.resx","Learning Center"), Column("LearningCenterTimeSlotsAssociationID")]
        public Nullable<long> T_LearningCenterTimeSlotsAssociationID { get; set; }
		
        public virtual T_LearningCenter t_learningcentertimeslotsassociation { get; set;}
				
        public virtual ICollection<T_Session> t_sessiontimeslotassociaton { get; set; }
		
        public virtual ICollection<T_SessionEvents> t_sessioneventstimeslots { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_StartTime != null ?Convert.ToString(this.T_StartTime)+"-" : Convert.ToString(this.T_StartTime))+(this.T_EndTime != null ?Convert.ToString(this.T_EndTime)+"-" : Convert.ToString(this.T_EndTime)); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_StartTime != null ?Convert.ToString(this.T_StartTime.Add(m_OffSet).ToShortTimeString())+"-" : "")+(this.T_EndTime != null ?Convert.ToString(this.T_EndTime.Add(m_OffSet).ToShortTimeString())+"-" : ""); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

