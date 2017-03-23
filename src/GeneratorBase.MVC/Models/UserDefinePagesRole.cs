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
    [Table("tbl_UserDefinePagesRole")]
    public class UserDefinePagesRole
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("Page Id")]
        public long PageId { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
    }
}