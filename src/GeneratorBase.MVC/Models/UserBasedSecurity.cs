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
    [Table("tbl_UserBasedSecurity")]
    public class UserBasedSecurity
    {
		[Key]
        public long Id { get; set; }
		[DisplayName("EntityName")] [Required]
        public string EntityName { get; set; }
        [DisplayName("TargetEntityName")]
        [Required]
        public string TargetEntityName { get; set; }
        [DisplayName("AssociationName")]
        [Required]
        public string AssociationName { get; set; }
        [DisplayName("IsMainEntity")]
        public bool IsMainEntity { get; set; }
		[DisplayName("RolesToIgnore")]
        public string RolesToIgnore { get; set; }
        [DisplayName("Other1")]
        public string Other1 { get; set; }
        [DisplayName("Other2")]
        public string Other2 { get; set; }
    }
}

