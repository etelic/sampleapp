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
    [Table("tbl_ActionType")]
    public class ActionType
    {
		public ActionType()
        {
            this.associatedactiontype = new HashSet<RuleAction>();
        }
		[Key]
        public long Id { get; set; }
		[DisplayName("TypeNo")]  
        public Nullable<Int32> TypeNo { get; set; }
		[DisplayName("Action Type Name")] [Required] 
        public string ActionTypeName { get; set; }
		[DisplayName("Description")]  
        public string Description { get; set; }
        public virtual ICollection<RuleAction> associatedactiontype { get; set; }
		[DisplayName("DisplayValue")]
        public string DisplayValue{ get; set; }
				 public  string getDisplayValue() { return Convert.ToString(this.ActionTypeName); }
    }
}

