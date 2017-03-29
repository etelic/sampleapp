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
    [Table("tbl_Session"),CustomDisplayName("T_Session", "T_Session.resx", "Session")]
	public class T_Session : Entity
    {	
		public T_Session()
        {
            this.T_SessionClientAssociation_t_session = new HashSet<T_SessionClientAssociation>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_Session.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_Session.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }

		 
		 
		[CustomDisplayName("T_IsOpen","T_Session.resx","Is Open"), Column("IsOpen")]  
		
        public Boolean? T_IsOpen { get; set; }

		[CustomDisplayName("T_SessionLearningCenterAssociationID","T_Session.resx","Learning Center"), Column("SessionLearningCenterAssociationID")]
        public Nullable<long> T_SessionLearningCenterAssociationID { get; set; }
		
        public virtual T_LearningCenter t_sessionlearningcenterassociation { get; set;}
		
		[CustomDisplayName("T_SessionTimeSlotAssociatonID","T_Session.resx","Time Slot"), Column("SessionTimeSlotAssociatonID")]
        public Nullable<long> T_SessionTimeSlotAssociatonID { get; set; }
		
        public virtual T_TimeSlots t_sessiontimeslotassociaton { get; set;}
		
		[CustomDisplayName("ScheduleSessionID","T_Session.resx","Session Schedule"), Column("ScheduleSessionID")]
        public Nullable<long> ScheduleSessionID { get; set; }
		
        public virtual T_Schedule schedulesession { get; set;}
				
        public virtual ICollection<T_SessionClientAssociation> T_SessionClientAssociation_t_session { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name))+ " - "+this.schedulesession.T_StartTime.Value.Add(m_OffSet).ToShortTimeString(); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

