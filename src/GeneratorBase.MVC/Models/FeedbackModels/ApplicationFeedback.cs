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
    [Table("tbl_ApplicationFeedback")]
    public class ApplicationFeedback : Entity
    {
        [DisplayName("Entity Name")]
        public string EntityName { get; set; }
        [DisplayName("Property Name")]
        public string PropertyName { get; set; }
        [DisplayName("Page Name")]
        public string PageName { get; set; }
        [DisplayName("Page Url Title")]
        public string PageUrlTitle { get; set; }
        [DisplayName("UI Control Name")]
        public string UIControlName { get; set; }
        [DataType(DataType.Url)]
        [Url(ErrorMessage = "Invalid Page Url Format")]
        [DisplayName("Page Url")]
        public string PageUrl { get; set; }
        [DisplayName("Comment Id")]
        public Nullable<long> CommentId { get; set; }
        DateTime? m_ReportedBy = DateTime.Now;
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ReportedBy
        {
            get
            {
                return m_ReportedBy;
            }
            set
            {
                m_ReportedBy = value;
            }
        }
        string m_ReportedByUser = HttpContext.Current.User.Identity.Name;
        [DisplayName("ReportedByUser")]
        public string ReportedByUser
        {
            get
            {
                return m_ReportedByUser;
            }
            set
            {
                m_ReportedByUser = value;
            }
        }
        [DisplayName("Summary")]
        public string Summary { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Attach Image")]
        public string AttachImage { get; set; }
        [DisplayName("Attach Document")]
        public string AttachDocument { get; set; }
        [DisplayName("Type")]
        public Nullable<long> AssociatedApplicationFeedbackTypeID { get; set; }
        public virtual ApplicationFeedbackType associatedapplicationfeedbacktype { get; set; }
        [DisplayName("Status")]
        public Nullable<long> AssociatedApplicationFeedbackStatusID { get; set; }
        public virtual ApplicationFeedbackStatus associatedapplicationfeedbackstatus { get; set; }
        [DisplayName("Priority")]
        public Nullable<long> ApplicationFeedbackPriorityID { get; set; }
        public virtual FeedbackPriority applicationfeedbackpriority { get; set; }
        [DisplayName("Severity")]
        public Nullable<long> ApplicationFeedbackSeverityID { get; set; }
        public virtual FeedbackSeverity applicationfeedbackseverity { get; set; }
        [DisplayName("Assigned To")]
        public Nullable<long> ApplicationFeedbackResourceID { get; set; }
        public virtual FeedbackResource applicationfeedbackresource { get; set; }
        public string getDisplayValue()
        {
            var dispValue = Convert.ToString(this.CommentId) + " - " + Convert.ToString(this.EntityName) + " - " + Convert.ToString(this.PropertyName) + " - " + Convert.ToString(this.Summary);
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
        }
        public override string getDisplayValueModel()
        {
            if (!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            var dispValue = Convert.ToString(this.CommentId) + " - " + Convert.ToString(this.EntityName) + " - " + Convert.ToString(this.PropertyName) + " - " + Convert.ToString(this.Summary);
            return dispValue.TrimEnd(" - ".ToCharArray());
        }
        public void setCalculation() { try { } catch { } }
    }
}

