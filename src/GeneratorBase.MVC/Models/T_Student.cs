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
    [Table("tbl_Student"),DisplayName("Student")]
	public class T_Student : Entity
    {	

		 
		 
		[DisplayName("Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[DisplayName("Description"), Column("Description")]  
		
        public string T_Description { get; set; }

	 [Unique(typeof(ApplicationContext),ErrorMessage="Must be unique")]
		 
		 
		[DisplayName("Code"), Column("Code")]  
		
        public string T_Code { get; set; }

		 
		 
		[DisplayName("Fulltime"), Column("Fulltime")]  
		
        public Boolean? T_Fulltime { get; set; }

		 
		 
		[DisplayName("Parttime"), Column("Parttime")]  
		
        public Boolean? T_Parttime { get; set; }

		[DisplayName("School"), Column("SchoolNameID")]
        public Nullable<long> T_SchoolNameID { get; set; }
		
        public virtual T_School t_schoolname { get; set;}
		
		[DisplayName("Department"), Column("DepartmentCodeID")]
        public Nullable<long> T_DepartmentCodeID { get; set; }
		
        public virtual T_Department t_departmentcode { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name))+(this.T_Code != null ?Convert.ToString(this.T_Code)+"-" : Convert.ToString(this.T_Code)); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name))+(this.T_Code != null ?Convert.ToString(this.T_Code)+"-" : Convert.ToString(this.T_Code)); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

