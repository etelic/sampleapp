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
    [Table("tbl_LearningCenter"),CustomDisplayName("T_LearningCenter", "T_LearningCenter.resx", "Learning Center")]
	public class T_LearningCenter : Entity
    {	
		public T_LearningCenter()
        {
            this.t_learningcentertimeslotsassociation = new HashSet<T_TimeSlots>();
            this.t_sessionlearningcenterassociation = new HashSet<T_Session>();
            this.t_sessioneventslearningcenter = new HashSet<T_SessionEvents>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_LearningCenter.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_LearningCenter.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }
		
        public virtual ICollection<T_TimeSlots> t_learningcentertimeslotsassociation { get; set; }
		
        public virtual ICollection<T_Session> t_sessionlearningcenterassociation { get; set; }
		
        public virtual ICollection<T_SessionEvents> t_sessioneventslearningcenter { get; set; }
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
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name)); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

