using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_PermissionAdminPrivilege")]
	public class PermissionAdminPrivilege
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("AdminFeature")]
        public string AdminFeature { get; set; }
        [DisplayName("RoleName")]
        public string RoleName { get; set; }
        [DisplayName("View")]
        public bool IsAllow { get; set; }
        [DisplayName("Add")]
        public bool IsAdd { get; set; }
        [DisplayName("Edit")]
        public bool IsEdit { get; set; }
        [DisplayName("Delete")]
        public bool IsDelete { get; set; }
        [NotMapped]
        public string DisplayValue
        {
            get
            {
                return AdminFeature + "-" + RoleName;
            }
        }
    }

}
