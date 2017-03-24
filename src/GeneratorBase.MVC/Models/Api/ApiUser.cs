using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class ApiUser : IUser
    {
        public ApiUser(string username)
        {
            this.Name = username;
            this.JavaScriptEncodedName = username;
        }
        public string Name { get; set; }
        public string JavaScriptEncodedName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAdminUser()
        {
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();

            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.IsUserInRole(this.Name, adminString);
        }
        public bool IsInRole(string role)
        {
            /// This is a magic string. We should call IsAdmin instead of IsInRole("Admin")
            if ("Admin".Equals(role, System.StringComparison.Ordinal))
            {
                return IsAdminUser();
            }

            return this.GetRoles().Contains(role);
        }
        public bool IsInRole(string[] roles)
        {
            var rolelist = roles.ToList().ConvertAll(d => d.ToUpper().Trim());

            if (rolelist.Contains("ALL")) return true;
            foreach (var str in this.GetRoles())
            {
                if (rolelist.Contains(str.Trim().ToUpper())) return true;
            }
            return false;
        }
        public IEnumerable<string> GetRoles()
        {
            CustomRoleProvider RoleProvider = new CustomRoleProvider();
            return RoleProvider.GetRolesForUser(this.Name);
        }
        public bool CanAdd(string resource)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanAdd);
        }


        public bool CanView(string resource)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanView);
        }


        public bool CanView(string resource, string property)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoView == null || !p.NoView.Contains(property + ",")));
        }


        public bool CanEdit(string resource)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanEdit);
        }

        public bool CanEdit(string resource, string property)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && (p.NoEdit == null || !p.NoEdit.Contains(property + ",")));
        }


        public bool CanDelete(string resource)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && p.CanDelete);
        }
        public bool ImposeOwnerPermission(string resource)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;

            return this.permissions.Any(p => p.EntityName.Equals(resource) && p.IsOwner.Value);
        }
        public string OwnerAssociation(string resource)
        {
            if (this.permissions.Any(p => p.EntityName.Equals(resource) && p.IsOwner.Value))
            {
                return this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource) && p.IsOwner.Value && !string.IsNullOrEmpty(p.UserAssociation)).UserAssociation;
            }
            return string.Empty;
        }
        public bool CanEditItem(string resource, object obj, IUser User)
        {
            return CanEdit(resource) && (!ImposeOwnerPermission(resource) || (ImposeOwnerPermission(resource) && (!(new CheckPermissionForOwner()).CheckOwnerPermission(resource, obj, User, false)))) && !((new CheckPermissionForOwner()).CheckLockCondition(resource, obj, User, false));
        }
        public bool CanDeleteItem(string resource, object obj, IUser User)
        {
            return CanDelete(resource) && (!ImposeOwnerPermission(resource) || (ImposeOwnerPermission(resource) && (!(new CheckPermissionForOwner()).CheckOwnerPermission(resource, obj, User, false)))) && !((new CheckPermissionForOwner()).CheckLockCondition(resource, obj, User, false));
        }
        public string FLSAppliedOnProperties(string resource)
        {
            var permission = this.permissions.FirstOrDefault(p => p.EntityName.Equals(resource));
            if (permission != null)
                return permission.NoEdit + permission.NoView;
            return string.Empty;
        }
        //code for verb action security
        public bool CanUseVerb(string resource, string entityname, IUser User)
        {
            if (Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;
            if (User.IsAdminUser())
                return true;
            return this.permissions.Any(p => p.Verbs != null && p.Verbs.Contains(resource) && p.EntityName == entityname);
        }
        public List<BusinessRule> businessrules { get; set; }
        public List<Permission> permissions { get; set; }
        public List<MultiTenantLoginSelected> MultiTenantLoginSelected { get; set; }
        public List<PermissionAdminPrivilege> adminprivileges { get; set; }
    }
}