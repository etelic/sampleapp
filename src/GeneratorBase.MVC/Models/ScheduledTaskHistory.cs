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
    [Table("tbl_ScheduledTaskHistory")]
    public class ScheduledTaskHistory
    {
        public ScheduledTaskHistory()
        {
        }
		[Key]
        public long Id { get; set; }
		[DisplayName("TaskName")]
        [Required] 
        public string TaskName { get; set; }
        
        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("GUID")]
        public string GUID { get; set; }

        [DisplayName("CallbackUri")]
        public string CallbackUri { get; set; }

        [DisplayName("BusinessRuleId")]
        public long? BusinessRuleId { get; set; }

        [DisplayName("Description")]  
        public string Description { get; set; }

        [DisplayName("Other")]
        public string Other { get; set; }

        [DisplayName("RunDateTime")]
        public string RunDateTime { get; set; }

		[DisplayName("DisplayValue")]
        public string DisplayValue{ get; set; }
        public string getDisplayValue() { return Convert.ToString(this.TaskName); }
    }
}

