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
    [Table("tbl_BusinessRuleType")]
    public class BusinessRuleType
    {
		public BusinessRuleType()
        {
            this.associatedbusinessruletype = new HashSet<BusinessRule>();
        }
		[Key]
        public long Id { get; set; }
		[DisplayName("TypeNo")]  
        public Nullable<Int32> TypeNo { get; set; }
		[DisplayName("Type Name")] [Required] 
        public string TypeName { get; set; }
		[DisplayName("Description")]  
        public string Description { get; set; }
        public virtual ICollection<BusinessRule> associatedbusinessruletype { get; set; }
		[DisplayName("DisplayValue")]
        public string DisplayValue{ get; set; }
				 public  string getDisplayValue() { return Convert.ToString(this.TypeName); }
    }
}

