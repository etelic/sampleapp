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
    [Table("tbl_RepeatOn"),DisplayName("Repeat On")]
	public class T_RepeatOn : Entity
    {	

		[DisplayName("Schedule"), Column("ScheduleID")]
        public Nullable<long> T_ScheduleID { get; set; }
		
        public virtual T_Schedule t_schedule { get; set;}
		
		[DisplayName("Recurrence Days"), Column("RecurrenceDaysID")]
        public Nullable<long> T_RecurrenceDaysID { get; set; }
		
        public virtual T_RecurrenceDays t_recurrencedays { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = ""; 
			dispValue = dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = ""; 
			return dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

