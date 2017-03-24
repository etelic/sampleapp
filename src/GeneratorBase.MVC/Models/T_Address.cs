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
    [Table("tbl_Address"),CustomDisplayName("T_Address", "T_Address.resx", "Address")]
	public class T_Address : Entity
    {	

		 
		 
		[CustomDisplayName("T_AddressLine1","T_Address.resx","AddressLine1"), Column("AddressLine1")]  
		
        public string T_AddressLine1 { get; set; }

		 
		 
		[CustomDisplayName("T_AddressLine2","T_Address.resx","AddressLine2"), Column("AddressLine2")]  
		
        public string T_AddressLine2 { get; set; }

		 
		 
		[CustomDisplayName("T_ZipCode","T_Address.resx","ZipCode"), Column("ZipCode")]  
		
        public string T_ZipCode { get; set; }

		[CustomDisplayName("T_AddressCountryID","T_Address.resx","Address Country"), Column("AddressCountryID")]
        public Nullable<long> T_AddressCountryID { get; set; }
		
        public virtual T_Country t_addresscountry { get; set;}
		
		[CustomDisplayName("T_AddressStateID","T_Address.resx","Address State"), Column("AddressStateID")]
        public Nullable<long> T_AddressStateID { get; set; }
		
        public virtual T_State t_addressstate { get; set;}
		
		[CustomDisplayName("T_AddressCityID","T_Address.resx","Address City"), Column("AddressCityID")]
        public Nullable<long> T_AddressCityID { get; set; }
		
        public virtual T_City t_addresscity { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_AddressLine1 != null ?Convert.ToString(this.T_AddressLine1)+"-" : Convert.ToString(this.T_AddressLine1))+(this.T_AddressLine2 != null ?Convert.ToString(this.T_AddressLine2)+"-" : Convert.ToString(this.T_AddressLine2))+(this.T_ZipCode != null ?Convert.ToString(this.T_ZipCode)+"-" : Convert.ToString(this.T_ZipCode))+(this.T_AddressCityID != null ? (new ApplicationContext(new SystemUser())).T_Citys.FirstOrDefault(p=>p.Id == this.T_AddressCityID).DisplayValue + "-" : "")+(this.T_AddressStateID != null ? (new ApplicationContext(new SystemUser())).T_States.FirstOrDefault(p=>p.Id == this.T_AddressStateID).DisplayValue + "-" : "")+(this.T_AddressCountryID != null ? (new ApplicationContext(new SystemUser())).T_Countrys.FirstOrDefault(p=>p.Id == this.T_AddressCountryID).DisplayValue + "-" : ""); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_AddressLine1 != null ?Convert.ToString(this.T_AddressLine1)+"-" : Convert.ToString(this.T_AddressLine1))+(this.T_AddressLine2 != null ?Convert.ToString(this.T_AddressLine2)+"-" : Convert.ToString(this.T_AddressLine2))+(this.T_ZipCode != null ?Convert.ToString(this.T_ZipCode)+"-" : Convert.ToString(this.T_ZipCode))+(this.t_addresscity != null ?t_addresscity.DisplayValue + "-" : "")+(this.t_addressstate != null ?t_addressstate.DisplayValue + "-" : "")+(this.t_addresscountry != null ?t_addresscountry.DisplayValue + "-" : ""); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

