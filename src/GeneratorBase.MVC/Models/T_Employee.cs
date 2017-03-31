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
    [Table("tbl_Employee"),CustomDisplayName("T_Employee", "T_Employee.resx", "Employee")]
	public class T_Employee : Entity
    {	
		public T_Employee()
        {
            this.T_EmployeeOrganizationAssociation_t_employee = new HashSet<T_EmployeeOrganizationAssociation>();
        }

		 
		 
		[CustomDisplayName("T_Title","T_Employee.resx","Title"), Column("Title")]  
		
        public string T_Title { get; set; }

		 
		 
		[CustomDisplayName("T_FirstName","T_Employee.resx","First Name"), Column("FirstName")] [Required] 
		
        public string T_FirstName { get; set; }

		 
		 
		[CustomDisplayName("T_MiddleName","T_Employee.resx","Middle Name"), Column("MiddleName")]  
		
        public string T_MiddleName { get; set; }

		 
		 
		[CustomDisplayName("T_LastName","T_Employee.resx","Last Name"), Column("LastName")]  
		
        public string T_LastName { get; set; }
            
           [EmailAddress]

		 
		 
		[CustomDisplayName("T_Email","T_Employee.resx","Email"), Column("Email")]  
		
        public string T_Email { get; set; }
            
           [RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid Phone No")]

		 
		 
		[CustomDisplayName("T_PhoneNo","T_Employee.resx","Phone No"), Column("PhoneNo")]  
		
        public string T_PhoneNo { get; set; }

		 
		 
		[CustomDisplayName("T_IdentificationNo","T_Employee.resx","Identification No"), Column("IdentificationNo")]  
		
        public string T_IdentificationNo { get; set; }

		 
		 
		[CustomDisplayName("T_Picture","T_Employee.resx","Picture"), Column("Picture")]  
		
        public Nullable<long> T_Picture { get; set; }

		[CustomDisplayName("T_AssociatedEmployeeTypeID","T_Employee.resx","Employee Type"), Column("AssociatedEmployeeTypeID")]
        public Nullable<long> T_AssociatedEmployeeTypeID { get; set; }
		
        public virtual T_Employeetype t_associatedemployeetype { get; set;}
		
		[CustomDisplayName("T_AssociatedEmployeeStatusID","T_Employee.resx","Employee Status"), Column("AssociatedEmployeeStatusID")]
        public Nullable<long> T_AssociatedEmployeeStatusID { get; set; }
		
        public virtual T_Employeestatus t_associatedemployeestatus { get; set;}
		
		[CustomDisplayName("T_EmployeeUserLoginID","T_Employee.resx","Employee User Login"),  Column("EmployeeUserLoginID")]
        public string T_EmployeeUserLoginID { get; set; }
        public virtual IdentityUser t_employeeuserlogin { get; set; }

		[CustomDisplayName("T_EmployeeAddressID","T_Employee.resx","Employee Address"), Column("EmployeeAddressID")]
        public Nullable<long> T_EmployeeAddressID { get; set; }
		
        public virtual T_Address t_employeeaddress { get; set;}
				
        public virtual ICollection<T_EmployeeOrganizationAssociation> T_EmployeeOrganizationAssociation_t_employee { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_FirstName != null ?Convert.ToString(this.T_FirstName)+" " : Convert.ToString(this.T_FirstName))+(this.T_LastName != null ?Convert.ToString(this.T_LastName)+" " : Convert.ToString(this.T_LastName)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_FirstName != null ?Convert.ToString(this.T_FirstName)+" " : Convert.ToString(this.T_FirstName))+(this.T_LastName != null ?Convert.ToString(this.T_LastName)+" " : Convert.ToString(this.T_LastName)); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

