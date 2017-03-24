using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string token { get; set; }
        public string provider { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Display(Name = "User name")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
        public string Email { get; set; }
    }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role name")]
        public string Name { get; set; }
        // Return a pre-poulated instance of AppliationUser:
        public IdentityRole GetRole()
        {
            var role = new IdentityRole()
            {
                Name = this.Name,
            };
            return role;
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9@_.]{1,40}$", ErrorMessage = "No special characters other than period (.), underscore (_) and at sign (@) are allowed. No spaces.")]
        [StringLength(50, ErrorMessage = "Less than 50 characters")]
        public string UserName { get; set; }
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage =
            "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        // New Fields added to extend Application User class:
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
        public string Email { get; set; }
        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
            };
            return user;
        }
    }
    public class EditRoleViewModel
    {
        public EditRoleViewModel() { }
        // Allow Initialization with an instance of ApplicationUser:
        public EditRoleViewModel(IdentityRole Role)
        {
            this.Name = Role.Name;
            this.OriginalName = Role.Name;
            this.id = Role.Id;
        }
        [Required]
        [Display(Name = "Role Name")]
        public string id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
    }
    public class EditUserViewModel
    {
        public EditUserViewModel() { }
        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
            this.Id = user.Id;
            this.LockoutEndDateUtc = user.LockoutEndDateUtc;
        }
        [Required]
        [Display(Name = "User Name")]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9@_.]{1,40}$", ErrorMessage = "No special characters other than period (.), underscore (_) and at sign (@) are allowed. No spaces.")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter valid email-id.")]
        public string Email { get; set; }
        public string Id { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
    }
    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }
        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Id = user.Id;
            var Db = new ApplicationDbContext();
            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }
            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            var roleIds = user.Roles.Select(r => r.RoleId);
            var roles = Db.Roles.Where(r => roleIds.Contains(r.Id))
                                 .Select(r => r.Name);
            foreach (var role in roles)
            {
                var checkUserRole =
                    this.Roles.Find(r => r.RoleName == role);
                checkUserRole.Selected = true;
            }
        }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }
    public class SelectUsersInRoleViewModel
    {
        public SelectUsersInRoleViewModel()
        {
            this.Users = new List<SelectUserEditorViewModel>();
        }
        // Enable initialization with an instance of ApplicationUser:
        public SelectUsersInRoleViewModel(IdentityRole role, string searchkey)
            : this()
        {
            this.RoleName = role.Name;
            var Db = new ApplicationDbContext();
            // Add all available users to the list of EditorViewModels:
            this.UserCount = Db.Users.Count();
            var allUsers = Db.Users.OrderBy(p => p.UserName).Take(50).ToList();//search
            if (!string.IsNullOrEmpty(searchkey))
            {
                this.UserCount = Db.Users.Where(p => p.UserName.ToUpper().Contains(searchkey.ToUpper()) || p.FirstName.ToUpper().Contains(searchkey.ToUpper()) || p.LastName.ToUpper().Contains(searchkey.ToUpper())).Count();
                allUsers = Db.Users.Where(p => p.UserName.ToUpper().Contains(searchkey.ToUpper()) || p.FirstName.ToUpper().Contains(searchkey.ToUpper()) || p.LastName.ToUpper().Contains(searchkey.ToUpper())).OrderBy(p => p.UserName).Take(50).ToList();
            }
            
            foreach (var user in allUsers)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectUserEditorViewModel(user);
                this.Users.Add(rvm);
            }
            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var user in allUsers)
            {
                var checkUsersInRole = this.Users.Find(r => r.UserName == user.UserName);
                var roleIds = user.Roles.Select(r => r.RoleId);
                checkUsersInRole.Selected = Db.Roles.Where(r => roleIds.Contains(r.Id))
                                                    .Any(r => r.Name == RoleName);
            }
        }
        // Enable initialization with an instance of ApplicationUser:
        public SelectUsersInRoleViewModel(IdentityRole role)
            : this()
        {
            this.RoleName = role.Name;
            var Db = new ApplicationDbContext();
            // Add all available users to the list of EditorViewModels:
            var allUsers = Db.Users.ToList();
            foreach (var user in allUsers)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectUserEditorViewModel(user);
                this.Users.Add(rvm);
            }
            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var user in allUsers)
            {
                var checkUsersInRole = this.Users.Find(r => r.UserName == user.UserName);
                var roleIds = user.Roles.Select(r => r.RoleId);
                checkUsersInRole.Selected = Db.Roles.Where(r => roleIds.Contains(r.Id))
                                                    .Any(r => r.Name == RoleName);
            }
        }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public List<SelectUserEditorViewModel> Users { get; set; }
    }
    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
        }
        public bool Selected { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
    // Used to display a single user with a checkbox, within a list structure:
    public class SelectUserEditorViewModel
    {
        public SelectUserEditorViewModel() { }
        public SelectUserEditorViewModel(IdentityUser user)
        {
            this.UserName = user.UserName;
        }
        public bool Selected { get; set; }
        [Required]
        public string UserName { get; set; }
    }
    //third party
    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    //
}
