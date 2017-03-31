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
    [Table("tbl_Employeetype"),CustomDisplayName("T_Employeetype", "T_Employeetype.resx", "Employee Type")]
	public class T_Employeetype : Entity
    {	
		public T_Employeetype()
        {
            this.t_associatedemployeetype = new HashSet<T_Employee>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_Employeetype.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_Employeetype.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }
		
        public virtual ICollection<T_Employee> t_associatedemployeetype { get; set; }
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

