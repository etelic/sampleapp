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
    [Table("tbl_EmployeeOrganizationAssociation"),CustomDisplayName("T_EmployeeOrganizationAssociation", "T_EmployeeOrganizationAssociation.resx", "EmployeeOrganizationAssociation")]
	public class T_EmployeeOrganizationAssociation : Entity
    {	

		[CustomDisplayName("T_EmployeeID","T_EmployeeOrganizationAssociation.resx","Employee"), Column("EmployeeID")]
        public Nullable<long> T_EmployeeID { get; set; }
		
        public virtual T_Employee t_employee { get; set;}
		
		[CustomDisplayName("T_OrganizationID","T_EmployeeOrganizationAssociation.resx","Organization"), Column("OrganizationID")]
        public Nullable<long> T_OrganizationID { get; set; }
		
        public virtual T_Organization t_organization { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_EmployeeID != null ? (new ApplicationContext(new SystemUser())).T_Employees.FirstOrDefault(p=>p.Id == this.T_EmployeeID).DisplayValue + "-" : "")+(this.T_OrganizationID != null ? (new ApplicationContext(new SystemUser())).T_Organizations.FirstOrDefault(p=>p.Id == this.T_OrganizationID).DisplayValue + "-" : ""); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.t_employee != null ?t_employee.DisplayValue + "-" : "")+(this.t_organization != null ?t_organization.DisplayValue + "-" : ""); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

