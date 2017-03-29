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
    [Table("tbl_SessionEventsClient"),CustomDisplayName("T_SessionEventsClient", "T_SessionEventsClient.resx", "Session EventsClient")]
	public class T_SessionEventsClient : Entity
    {	

		[CustomDisplayName("T_ClientID","T_SessionEventsClient.resx","Client"), Column("ClientID")]
        public Nullable<long> T_ClientID { get; set; }
		
        public virtual T_Client t_client { get; set;}
		
		[CustomDisplayName("T_SessionEventsID","T_SessionEventsClient.resx","Session Events"), Column("SessionEventsID")]
        public Nullable<long> T_SessionEventsID { get; set; }
		
        public virtual T_SessionEvents t_sessionevents { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_ClientID != null ? (new ApplicationContext(new SystemUser())).T_Clients.FirstOrDefault(p=>p.Id == this.T_ClientID).DisplayValue + "-" : "")+(this.T_SessionEventsID != null ? (new ApplicationContext(new SystemUser())).T_SessionEventss.FirstOrDefault(p=>p.Id == this.T_SessionEventsID).DisplayValue + "-" : ""); 
			dispValue = dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.t_client != null ?t_client.DisplayValue + "-" : "")+(this.t_sessionevents != null ?t_sessionevents.DisplayValue + "-" : ""); 
			return dispValue!=null?dispValue.TrimEnd("-".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

