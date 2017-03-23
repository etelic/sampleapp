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
    [Table("tbl_Condition")]
    public class Condition
    {
		[Key]
        public long Id { get; set; }
		[DisplayName("Property Name")] [Required] 
        public string PropertyName { get; set; }
		[DisplayName("Operator")]  
        public string Operator { get; set; }
		[DisplayName("Value")]  
        public string Value { get; set; }
		[DisplayName("Logical Connector")]  
        public string LogicalConnector { get; set; }
		[DisplayName("Business Rule")]
        public Nullable<long> RuleConditionsID { get; set; }
      //  [ForeignKey("RuleConditionsID")]
        public virtual BusinessRule ruleconditions { get; set; }
		[DisplayName("DisplayValue")]
        public string DisplayValue{ get; set; }
				 public string getDisplayValue() { return Convert.ToString(this.PropertyName); }
    }
}

