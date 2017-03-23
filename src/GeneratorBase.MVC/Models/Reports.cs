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
    [Table("tbl_Reports")]
    public class Reports
    {
		[Key]
        public long Id { get; set; }
		[DisplayName("Name")] [Required] 
        public string Name { get; set; }
		[DisplayName("Description")]  
        public string Description { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("CreatedBy")]
        public int CreatedById { get; set; }
		[NotMapped]
        public string DisplayValue
        {
			get
            {
               return  Convert.ToString(Name);
            }           
        }
    }
}

