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
    [Table("tbl_FavoriteItem")]
    public class FavoriteItem : Entity
    {
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Link Address")]
        public string LinkAddress { get; set; }
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
        string m_LastUpdatedByUser = HttpContext.Current.User.Identity.Name;
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
        [DisplayName("EntityName")]
        public string EntityName { get; set; }
        public  string getDisplayValue() {
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