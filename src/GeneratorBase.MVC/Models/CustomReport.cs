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
    [Table("tbl_CustomReport"), CustomDisplayName("CustomReport", "CustomReport.resx", "Custom Report")]
    public class CustomReport : Entity
    {
        [CustomDisplayName("ReportName", "CustomReport.resx", "Report Name"), Column("ReportName")]
        [Required]
        public string ReportName { get; set; }

        [CustomDisplayName("EntityName", "CustomReport.resx", "Entity Name"), Column("EntityName")]
        [Required]
        public string EntityName { get; set; }
        [CustomDisplayName("PeportType", "CustomReport.resx", "Peport Type"), Column("PeportType")]
        [Required]
        public string PeportType { get; set; }
        [CustomDisplayName("Description", "CustomReport.resx", "Description"), Column("Description")]
        public string Description { get; set; }
        [CustomDisplayName("OtherProperty", "CustomReport.resx", "Other Property"), Column("OtherProperty")]
        public string OtherProperty { get; set; }
        [CustomDisplayName("ViewReport", "CustomReport.resx", "View Report"), Column("ViewReport")]
        [Required]
        public string ViewReport { get; set; }
        public string getDisplayValue()
        {
            try
            {
                var dispValue = (this.ReportName != null ? Convert.ToString(this.ReportName) + "-" : Convert.ToString(this.ReportName)) + (this.PeportType != null ? Convert.ToString(this.PeportType) + "-" : Convert.ToString(this.PeportType));
                dispValue = dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
                this.m_DisplayValue = dispValue;
                return dispValue;
            }
            catch { return ""; }
        }
        public override string getDisplayValueModel()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_DisplayValue))
                    return m_DisplayValue;
                var dispValue = (this.ReportName != null ? Convert.ToString(this.ReportName) + "-" : Convert.ToString(this.ReportName)) + (this.PeportType != null ? Convert.ToString(this.PeportType) + "-" : Convert.ToString(this.PeportType));
                return dispValue != null ? dispValue.TrimEnd("-".ToCharArray()) : "";
            }
            catch { return ""; }
        }
        public void setCalculation() { try { } catch { } }
    }
}

