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
    [Table("tbl_Employeestatus"),CustomDisplayName("T_Employeestatus", "T_Employeestatus.resx", "Employee Status")]
	public class T_Employeestatus : Entity
    {	
		public T_Employeestatus()
        {
            this.t_associatedemployeestatus = new HashSet<T_Employee>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_Employeestatus.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_Employeestatus.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }
		
        public virtual ICollection<T_Employee> t_associatedemployeestatus { get; set; }
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

