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
    [Table("tbl_EmailTemplate")]
    public class EmailTemplate : Entity
    {
        [DisplayName("Email Template Type")]
        public Nullable<long> AssociatedEmailTemplateTypeID { get; set; }
        public virtual EmailTemplateType associatedemailtemplatetype { get; set; }
        [DisplayName("Email Content")]
        [System.Web.Mvc.AllowHtml]
        public string EmailContent { get; set; }
        [DisplayName("Email Subject")]
        public string EmailSubject { get; set; }
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

            var dispValue = (this.AssociatedEmailTemplateTypeID != null ? (new ApplicationContext(new SystemUser())).EmailTemplateTypes.Find(this.AssociatedEmailTemplateTypeID).DisplayValue + "  " : ""); 
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        public override string getDisplayValueModel() {
            if (!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            return (this.AssociatedEmailTemplateTypeID != null ? (new ApplicationContext(new SystemUser())).EmailTemplateTypes.Find(this.AssociatedEmailTemplateTypeID).DisplayValue + "  " : ""); 
        }
    }
}