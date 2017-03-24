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
    [Table("tbl_Organization"),CustomDisplayName("T_Organization", "T_Organization.resx", "Organization")]
	public class T_Organization : Entity
    {	
		public T_Organization()
        {
            this.T_EmployeeOrganizationAssociation_t_organization = new HashSet<T_EmployeeOrganizationAssociation>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_Organization.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_Organization.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }
		
        public virtual ICollection<T_EmployeeOrganizationAssociation> T_EmployeeOrganizationAssociation_t_organization { get; set; }
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

