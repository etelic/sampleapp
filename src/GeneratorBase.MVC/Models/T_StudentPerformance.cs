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
    [Table("tbl_StudentPerformance"),DisplayName("StudentPerformance")]
	public class T_StudentPerformance : Entity
    {	
            
           [Range(0,100)]

		 
		 
		[DisplayName("Mark1"), Column("Mark1")]  
		
        public Nullable<Int32> T_Mark1 { get; set; }
            
           [Range(0,100)]

		 
		 
		[DisplayName("Mark2"), Column("Mark2")]  
		
        public Nullable<Int32> T_Mark2 { get; set; }

		 
		 
		[DisplayName("Total Marks"), Column("TotalMarks")]  
		
        public Nullable<Int32> T_TotalMarks { get { var T_TotalMarks_Value1 = this.T_Mark1;
 var T_TotalMarks_Value2 = this.T_Mark2;
 return T_TotalMarks_Value1+T_TotalMarks_Value2 ;
} set { } }

		 
		 
		[DisplayName("Grade"), Column("Grade")]  
		
        public string T_Grade { get; set; }

		 
		 
		[DisplayName("Remarks"), Column("Remarks")]  
		
        public string T_Remarks { get; set; }

		[DisplayName("Student"), Column("StudentCodeID")]
        public Nullable<long> T_StudentCodeID { get; set; }
		
        public virtual T_Student t_studentcode { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_Grade != null ?Convert.ToString(this.T_Grade)+"-" : Convert.ToString(this.T_Grade)); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_Grade != null ?Convert.ToString(this.T_Grade)+"-" : Convert.ToString(this.T_Grade)); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  var T_TotalMarks_Value1 = this.T_Mark1;
 var T_TotalMarks_Value2 = this.T_Mark2;
 this.T_TotalMarks = T_TotalMarks_Value1+T_TotalMarks_Value2 ;
 }catch{}}
    }
}

