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
    [Table("tbl_ImportConfiguration")]
    public class ImportConfiguration : Entity
    {
        [DisplayName("Table Column")]
        public string TableColumn { get; set; }
        [DisplayName("Sheet Column")]
        public string SheetColumn { get; set; }
        [DisplayName("Unique Column")]
        public string UniqueColumn { get; set; }
        DateTime? m_LastUpdate = DateTime.Now;
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> LastUpdate
        {
            get
            {
                return m_LastUpdate;
            }
            set
            {
                m_LastUpdate = value;
            }
        }
        string m_LastUpdateUser = HttpContext.Current.User.Identity.Name;
        [DisplayName("LastUpdateUser")]
        public string LastUpdateUser
        {
            get
            {
                return m_LastUpdateUser;
            }
            set
            {
                m_LastUpdateUser = value;
            }
        }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("MappingName")]
        public string MappingName { get; set; }
        [DisplayName("IsDefaultMapping")]
        public bool IsDefaultMapping { get; set; }
        public string getDisplayValue() {
            var dispValue = Convert.ToString(this.Name);
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        public override string getDisplayValueModel() {
            if (!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            return Convert.ToString(this.Name); 
        }
    }
}

