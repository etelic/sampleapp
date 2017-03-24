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
    [Table("tbl_MultiTenantExtraAccess"), CustomDisplayName("MultiTenantExtraAccess", "MultiTenantExtraAccess.resx", "Application Security Access")]
    public class MultiTenantExtraAccess : Entity
    {
        [CustomDisplayName("T_User", "MultiTenantExtraAccess.resx", "User"), Column("User")]
        
        public string T_User { get; set; }
        [CustomDisplayName("T_MainEntity", "MultiTenantExtraAccess.resx", "MainEntity")]
        [NotMapped]
        public string T_MainEntity { get; set; }
        
        [Column("MainEntityID")]
        public long? T_MainEntityID { get; set; }

        [CustomDisplayName("T_MainEntityValue", "MultiTenantLoginSelected.resx", "MainEntityValue"), Column("MainEntityValue")]
        public string T_MainEntityValue { get; set; }

        public string getDisplayValue()
        {
             return ""; 
        }
        public string DisplayValue
        {
            get;
            set; 
        }
        public override string getDisplayValueModel()
        {
            return "";
        }
        public void setCalculation() { try { } catch { } }
    }
}

