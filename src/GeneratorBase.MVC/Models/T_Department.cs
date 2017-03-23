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
    [Table("tbl_Department"),DisplayName("Department")]
	public class T_Department : Entity
    {	
		public T_Department()
        {
            this.t_departmentcode = new HashSet<T_Student>();
        }

		 
		 
		[DisplayName("Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[DisplayName("Description"), Column("Description")]  
		
        public string T_Description { get; set; }

		 
		 
		[DisplayName("Code"), Column("Code")]  
		
        public string T_Code { get; set; }
		
        public virtual ICollection<T_Student> t_departmentcode { get; set; }
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

