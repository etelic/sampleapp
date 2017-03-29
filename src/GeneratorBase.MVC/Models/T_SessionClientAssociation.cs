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
    [Table("tbl_SessionClientAssociation"),CustomDisplayName("T_SessionClientAssociation", "T_SessionClientAssociation.resx", "SessionClientAssociation")]
	public class T_SessionClientAssociation : Entity
    {	

		[CustomDisplayName("T_ClientID","T_SessionClientAssociation.resx","Client"), Column("ClientID")]
        public Nullable<long> T_ClientID { get; set; }
		
        public virtual T_Client t_client { get; set;}
		
		[CustomDisplayName("T_SessionID","T_SessionClientAssociation.resx","Session"), Column("SessionID")]
        public Nullable<long> T_SessionID { get; set; }
		
        public virtual T_Session t_session { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_ClientID != null ? (new ApplicationContext(new SystemUser())).T_Clients.FirstOrDefault(p=>p.Id == this.T_ClientID).DisplayValue + "-" : "")+(this.T_SessionID != null ? (new ApplicationContext(new SystemUser())).T_Sessions.FirstOrDefault(p=>p.Id == this.T_SessionID).DisplayValue + "-" : ""); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.t_client != null ?t_client.DisplayValue + "-" : "")+(this.t_session != null ?t_session.DisplayValue + "-" : ""); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

