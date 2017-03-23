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
    [Table("tbl_DefaultEntityPage")]
	public class DefaultEntityPage : Entity
    {	
		[DisplayName("Entity Name")] [Required] 
        public string EntityName { get; set; }
		[DisplayName("Roles")]  
        public string Roles { get; set; }
		[DisplayName("Page Type")]  
        public string PageType { get; set; }
		[DisplayName("Page Url")]  
        public string PageUrl { get; set; }
		[DisplayName("Other")]  
        public string Other { get; set; }
		[DisplayName("Flag")]  
        public Boolean? Flag { get; set; }
		 public  string getDisplayValue() {
             var dispValue = Convert.ToString(this.EntityName);
             dispValue = dispValue.TrimEnd(" - ".ToCharArray());
             this.m_DisplayValue = dispValue;
             return dispValue;
         }
         public override string getDisplayValueModel() {
             if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
             return Convert.ToString(this.EntityName); 
         }
		 public void setCalculation(){  }
    }
}

