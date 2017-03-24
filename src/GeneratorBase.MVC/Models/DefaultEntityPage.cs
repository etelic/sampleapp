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
        [DisplayName("Entity Name")]
        [Required]
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

        [DisplayName("View Entity  Page")]
        public string ViewEntityPage { get; set; }

        [DisplayName("List Entity Page")]
        public string ListEntityPage { get; set; }

        [DisplayName("Edit Entity Page")]
        public string EditEntityPage { get; set; }

        [DisplayName("Search Entity Page")]
        public string SearchEntityPage { get; set; }
        //
        [DisplayName("Create Quick Entity Page")]
        public string CreateQuickEntityPage { get; set; }

        [DisplayName("Create Entity Page")]
        public string CreateEntityPage { get; set; }

        [DisplayName("Edit Quick Entity Page")]
        public string EditQuickEntityPage { get; set; }

        [DisplayName("Create Wizard Entity Page")]
        public string CreateWizardEntityPage { get; set; }

        [DisplayName("Edit Wizard Entity Page")]
        public string EditWizardEntityPage { get; set; }
        //
        public string getDisplayValue()
        {
            try
            {
                var dispValue = (this.EntityName != null ? Convert.ToString(this.EntityName) + "" : Convert.ToString(this.EntityName));
                dispValue = dispValue != null ? dispValue.TrimEnd("".ToCharArray()) : "";
                this.m_DisplayValue = dispValue;
                return dispValue;
            }
            catch { return ""; }
        }
        public override string getDisplayValueModel()
        {
            if (!string.IsNullOrEmpty(m_DisplayValue))
                return m_DisplayValue;
            return Convert.ToString(this.EntityName);
        }
        public void setCalculation() { }
    }
}

