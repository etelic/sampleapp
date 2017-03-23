using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_ActionArgs")]
    public class ActionArgs
    {
		[Key]
        public long Id { get; set; }
		[DisplayName("Parameter Name")] [Required] 
        public string ParameterName { get; set; }
		[DisplayName("Parameter Value")] [Required] 
        public string ParameterValue { get; set; }
		[DisplayName("Action")]
        public Nullable<long> ActionArgumentsID { get; set; }
      //  [ForeignKey("ActionArgumentsID")]
        public virtual RuleAction actionarguments { get; set; }
		[DisplayName("DisplayValue")]
        public string DisplayValue{ get; set; }
        public string getDisplayValue() { return Convert.ToString(this.ParameterName); }
    }
}

