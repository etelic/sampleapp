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
    [Table("tbl_BusinessRule")]
    public class BusinessRule
    {
        public BusinessRule()
        {
            this.ruleconditions = new HashSet<Condition>();
            this.ruleaction = new HashSet<RuleAction>();
        }
        [Key]
        public long Id { get; set; }
        [DisplayName("Rule Name")]
        [Required]
        public string RuleName { get; set; }
        [DisplayName("Entity Name")]
        public string EntityName { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Roles")]
        public string Roles { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Created")]
        public Nullable<DateTime> DateCreated1 { get; set; }
        [DisplayName("Notify me")]
        public Boolean EntitySubscribe { get; set; }
        [DisplayName("Is Disabled?")]
        public Boolean Disable { get; set; }

        [DisplayName("Is Freezed?")]
        public Boolean Freeze { get; set; }

        [DisplayName("Information Message")]
        public string InformationMessage { get; set; }

        [DisplayName("Failure Message")]
        public string FailureMessage { get; set; }

        [DisplayName("BusinessRule Type")]
        public Nullable<long> AssociatedBusinessRuleTypeID { get; set; }
        public virtual BusinessRuleType associatedbusinessruletype { get; set; }
         

        public virtual ICollection<Condition> ruleconditions { get; set; }
        public virtual ICollection<RuleAction> ruleaction { get; set; }
        [DisplayName("DisplayValue")]
        public string DisplayValue { get; set; }
        public string getDisplayValue() { return this.EntityName + "-" + Convert.ToString(this.RuleName); }

        [DisplayName("Scheduler Task"), Column("SchedulerTaskID")]
        public Nullable<long> T_SchedulerTaskID { get; set; }
        public virtual T_Schedule t_schedulertask { get; set; }
    }
}

