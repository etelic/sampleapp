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
    [Table("tbl_ApiToken"), CustomDisplayName("T_ApiToken", "T_ApiToken.resx", "ApiToken")]
    public class ApiToken : Entity
    {



        [CustomDisplayName("T_AuthToken", "ApiToken.resx", "AuthToken"), Column("AuthToken")]
        [Required] 
		
        public string T_AuthToken { get; set; }

		[DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]

        [CustomDisplayName("T_IssuedOn", "ApiToken.resx", "IssuedOn"), Column("IssuedOn")]
        [Required] 
		
        public DateTime T_IssuedOn { get; set; }

		[DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]

        [CustomDisplayName("T_ExpiresOn", "ApiToken.resx", "ExpiresOn"), Column("ExpiresOn")]
        [Required] 
		
        public DateTime T_ExpiresOn { get; set; }

        [CustomDisplayName("T_UsersID", "ApiToken.resx", "Users"), Column("UsersID")]
        public string T_UsersID { get; set; }
        public virtual IdentityUser t_users { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = ""; 
			dispValue = dispValue!=null?dispValue.TrimEnd("".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = ""; 
			return dispValue!=null?dispValue.TrimEnd("".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

