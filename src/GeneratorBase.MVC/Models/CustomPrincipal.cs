using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace GeneratorBase.MVC.Models
{
    public class CustomPrincipal : IPrincipal, IUser
    {
        public List<Permission> permissions { get; set; }
        public List<BusinessRule> businessrules { get; set; }
        public List<AppSetting> appsettings { get; set; }
        public List<MultiTenantLoginSelected> MultiTenantLoginSelected { get; set; }
        public IPrincipal Original { get; private set; }
        public string Name { get; private set; }
        public string JavaScriptEncodedName { get; private set; }
        public bool IsAdmin { get; set; }
        public List<PermissionAdminPrivilege> adminprivileges { get; set; }
        //public WindowsIdentity Identity { get; private set; } //FOR WINDOWS AUTHENTICATION....
        public dynamic Identity { get; private set; }
        IIdentity IPrincipal.Identity
        {
            get
            {
                var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
                if (Convert.ToBoolean(UseActiveDirectory))
                    return (WindowsIdentity)this.Identity;
                else
                    return (IIdentity)this.Identity;
            }
        }

        public CustomPrincipal(IPrincipal original)
        {
            //FOR WINDOWS AUTHENTICATION....
            var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
            if (Convert.ToBoolean(UseActiveDirectory))
            {
                if (!(original.Identity is WindowsIdentity)) throw new NotImplementedException();
                this.Identity = (WindowsIdentity)original.Identity;
            }
            else
            {
                this.Identity = (IIdentity)original.Identity;
            }
            this.Original = original;
            
            //this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name);
            if (Convert.ToBoolean(UseActiveDirectory))
            {
                //this.Name = original.Identity.Name; 
                //changed for active directory
                 if (original.Identity != null && original.Identity.Name.Split('\\').Length > 1)
                {
                    this.Name = original.Identity.Name.Split('\\')[1];
                    this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name.Split('\\')[1]);
                }
            }
            else
            {
                this.Name = original.Identity.Name;
                this.JavaScriptEncodedName = HttpUtility.JavaScriptStringEncode(original.Identity.Name);
            }
        }

        public IEnumerable<string> GetRoles()
        {
            //FOR WINDOWS AUTHENTICATION....
            var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
            if (Convert.ToBoolean(UseActiveDirectory))
            {
                //var adl = new ActiveDirectoryLookup(this.Identity);
                //return adl.GetRoles();
                var UseActiveDirectoryRole = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
                if (Convert.ToBoolean(UseActiveDirectoryRole))
                {
                    var adl = new ActiveDirectoryLookup(this.Identity);
                    return adl.GetRoles();
                }
                else
                {
                    var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                    CustomRoleProvider RoleProvider = new CustomRoleProvider();
                    string usrname = this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim();
                    return RoleProvider.GetRolesForUser(usrname);
                }
            }
            else
            {
                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.GetRolesForUser(((IIdentity)this.Identity).Name);
            }
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
	public bool IsInRole(string[] roles,IEnumerable<string> AllRoles)
        {
            var rolelist = roles.ToList().ConvertAll(d => d.ToUpper().Trim());

            if (rolelist.Contains("ALL")) return true;
            foreach (var str in AllRoles)
            {
                if (rolelist.Contains(str.Trim().ToUpper())) return true;
            }
            return false;
        }
        public bool IsAdminUser()
        {
            //FOR WINDOWS AUTHENTICATION....
            var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
            if (Convert.ToBoolean(UseActiveDirectory))
            {
                //var adl = new ActiveDirectoryLookup(this.Identity);
                //var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
                //var admins = adminString.Split(',', ';');
                //return adl.GetRoles().Any(r => admins.Contains(r));
                var UseActiveDirectoryRole = ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
                if (Convert.ToBoolean(UseActiveDirectoryRole))
                {
                    var adl = new ActiveDirectoryLookup(this.Identity);
                    var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                    var admins = adminString.Split(',', ';');
                    return adl.GetRoles().Any(r => admins.Contains(r));
                }
                else
                {
                    var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                    var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                    CustomRoleProvider RoleProvider = new CustomRoleProvider();
                    return RoleProvider.IsUserInRole(this.Name.Replace(DomainName.ToUpper() + "\\", "").Trim(), adminString);
                }
            }
            else
            {
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();

                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.IsUserInRole(((IIdentity)this.Identity).Name, adminString);
            }
        }

        public string GetAdminRoles()
        {
            return System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
        }
        public bool IsAdminUser(string username)
        {
            //FOR WINDOWS AUTHENTICATION....
            var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
            if (Convert.ToBoolean(UseActiveDirectory))
            {
                //var adl = new ActiveDirectoryLookup(this.Identity);
                //var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();
                //var admins = adminString.Split(',', ';');
                //return adl.GetRoles().Any(r => admins.Contains(r));
                var UseActiveDirectoryRole = ConfigurationManager.AppSettings["UseActiveDirectoryRole"];
                if (Convert.ToBoolean(UseActiveDirectoryRole))
                {
                    var adl = new ActiveDirectoryLookup(this.Identity);
                    var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                    var admins = adminString.Split(',', ';');
                    return adl.GetRoles().Any(r => admins.Contains(r));
                }
                else
                {
                    var DomainName = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                    var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"];
                    CustomRoleProvider RoleProvider = new CustomRoleProvider();
                    return RoleProvider.IsUserInRole(username, adminString);
                }
            }
            else
            {
                var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles();

                CustomRoleProvider RoleProvider = new CustomRoleProvider();
                return RoleProvider.IsUserInRole(username, adminString);
            }
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
            if (User.IsAdmin || Enum.IsDefined(typeof(PermissionFreeContext), resource))
                return true;
            return this.permissions.Any(p => p.Verbs != null && p.Verbs.Contains(resource) && p.EntityName == entityname);
        }
        //
        public bool CanAddAdminFeature(string resource)
        {
            if (this.IsAdmin) return true;
            return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsAdd);
        }
        public bool CanViewAdminFeature(string resource)
        {
            if (this.IsAdmin) return true;
            return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsAllow);
        }
        public bool CanEditAdminFeature(string resource)
        {
            if (this.IsAdmin) return true;
            return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsEdit);
        }
        public bool CanDeleteAdminFeature(string resource)
        {
            if (this.IsAdmin) return true;
            return this.adminprivileges.Any(p => p.AdminFeature.Equals(resource) && p.IsDelete);
        }
        public bool HasAdminPrivileges()
        {
            if (this.IsAdmin) return true;
            return this.adminprivileges.Any(p => p.IsAllow);
        }
    }
}

