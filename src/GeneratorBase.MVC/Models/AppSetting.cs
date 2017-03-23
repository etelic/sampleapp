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
    [Table("tbl_AppSetting")]
    public class AppSetting : Entity
    {
        [Unique(typeof(ApplicationContext), ErrorMessage = "Must be unique")]
        [DisplayName("Key")]
        [Required]
        public string Key { get; set; }
        [DisplayName("Value")]
        [Required]
        public string Value { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Group Name")]
        public Nullable<long> AssociatedAppSettingGroupID { get; set; }
        public virtual AppSettingGroup associatedappsettinggroup { get; set; }
        [DisplayName("IsDefault")]
        public bool IsDefault { get; set; }
        DateTime? m_LastUpdatedBy = DateTime.Now;
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LastUpdatedBy
        {
            get
            {
                return m_LastUpdatedBy;
            }
            set
            {
                m_LastUpdatedBy = value;
            }
        }
        string m_LastUpdatedByUser = HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.Name : "";
        [DisplayName("LastUpdatedByUser")]
        public string LastUpdatedByUser
        {
            get
            {
                return m_LastUpdatedByUser;
            }
            set
            {
                m_LastUpdatedByUser = value;
            }
        }
        public  string getDisplayValue() {
           var  dispValue = Convert.ToString(this.Key);
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        public override string getDisplayValueModel() {
            if (!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            return Convert.ToString(this.Key);
        }
    }
}