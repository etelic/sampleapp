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
    [Table("tbl_Chart"), CustomDisplayName("T_Chart", "T_Chart.resx", "")]
    public class T_Chart : Entity
    {
        [CustomDisplayName("EntityName", "T_Chart.resx", "Entity Name"), Column("EntityName")]
        public string EntityName { get; set; }

        [CustomDisplayName("ChartTitle", "T_Chart.resx", "Chart Title"), Column("ChartTitle")]
        public string ChartTitle { get; set; }

        [CustomDisplayName("ChartType", "T_Chart.resx", "Chart Type"), Column("ChartType")]
        public string ChartType { get; set; }

        [CustomDisplayName("XAxis", "T_Chart.resx", "X Axis"), Column("XAxis")]
        public string XAxis { get; set; }

        [CustomDisplayName("YAxis", "T_Chart.resx", "Y Axis"), Column("YAxis")]
        public string YAxis { get; set; }

        [CustomDisplayName("ShowInDashBoard", "T_Chart.resx", "Show In DashBoard"), Column("ShowInDashBoard")]
        public bool ShowInDashBoard { get; set; }
        
        public string getDisplayValue()
        {
            try
            {
                var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : "");
                dispValue = dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
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
                var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) : "");
                return dispValue != null ? dispValue.TrimEnd(" - ".ToCharArray()) : "";
            }
            catch { return ""; }
        }
        public void setCalculation() { try { } catch { } }
    }
}

