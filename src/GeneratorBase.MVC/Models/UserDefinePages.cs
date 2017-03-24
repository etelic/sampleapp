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
    [Table("tbl_UserDefinePages")]
    public class UserDefinePages
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("Page Name")]
        [Required]
        public string PageName { get; set; }
        [DisplayName("Page Content")]
        [System.Web.Mvc.AllowHtml]
        public string PageContent { get; set; }
    }
    public class CreateUserDefinePageViewModel
    {
        public CreateUserDefinePageViewModel()
        {
            //this.Roles = new List<SelectUserRoleEditorViewModel>();
        }
        public CreateUserDefinePageViewModel(UserDefinePages userpages)
            : this()
        {
            this.PageName = userpages.PageName;
            this.PageContent = userpages.PageContent;
            var Db = new ApplicationDbContext();
            var allRoles = Db.Roles;
            //foreach (var role in allRoles)
            //{
            //    var rvm = new SelectUserRoleEditorViewModel(role);
            //    this.Roles.Add(rvm);
            //}
            var userdefinepagesrole = new UserDefinePagesRoleContext();
            var disableroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId != userpages.Id);
            //foreach (var userRole in disableroles)
            //{
            //    var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
            //    checkUserRole.isdisabled = true;
            //}
            var roleslist = disableroles.Select(a => a.RoleName).ToList();
            this.RoleList = new SelectList(allRoles.Where(r => !roleslist.Any(dr => dr == r.Name)).ToList(), "Name", "Name", null);
        }
        [DisplayName("Page Name")]
        [Required]
        public string PageName { get; set; }
        public string PageContent { get; set; }
        public SelectList RoleList { get; set; }
        //public List<SelectUserRoleEditorViewModel> Roles { get; set; }
    }
    public class EditUserDefinePageViewModel
    {
        public EditUserDefinePageViewModel()
        {
            this.Roles = new List<SelectUserRoleEditorViewModel>();
        }
        public EditUserDefinePageViewModel(Int64 Id)
            : this()
        {
            var Db = new ApplicationDbContext();
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                var rvm = new SelectUserRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }
            if (Id == 0)
                return;
            var db = new UserDefinePagesContext();
            var userpages = db.UserDefinePagess.Where(u => u.Id == Id).ToList()[0];
            this.Id = userpages.Id;
            this.PageName = userpages.PageName;
            this.PageContent = userpages.PageContent;
            var userdefinepagesrole = new UserDefinePagesRoleContext();
            var pagesroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId == userpages.Id);
            var disableroles = userdefinepagesrole.UserDefinePagesRoles.Where(u => u.PageId != userpages.Id);
            foreach (var userRole in pagesroles)
            {
                var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
                checkUserRole.Selected = true;
            }
            foreach (var userRole in disableroles)
            {
                var checkUserRole = this.Roles.Find(r => r.RoleName == userRole.RoleName);
                checkUserRole.isdisabled = true;
            }
        }
        public Int64 Id { get; set; }
        [DisplayName("Page Name")]
        [Required]
        public string PageName { get; set; }
        public string PageContent { get; set; }
        public List<SelectUserRoleEditorViewModel> Roles { get; set; }
    }
    public class SelectUserRoleEditorViewModel
    {
        public SelectUserRoleEditorViewModel() { }
        public SelectUserRoleEditorViewModel(IdentityRole role)
        {
            this.Id = role.Id;
            this.RoleName = role.Name;
        }
        public string Id { get; set; }
        public bool Selected { get; set; }
        public string RoleName { get; set; }
        public bool isdisabled { get; set; }
    }
}

