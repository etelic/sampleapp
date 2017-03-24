using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public interface IUser
    {
        string Name { get; }
        string JavaScriptEncodedName { get; }
        bool IsAdmin{ get; }
        bool IsAdminUser();
        bool IsInRole(string role);
        bool IsInRole(string[] roles);
        IEnumerable<string> GetRoles();
        bool CanAdd(string resource);
        bool CanView(string resource);
        bool CanView(string resource, string property);
        bool CanEdit(string resource);
        bool CanEdit(string resource, string property);
        bool CanDelete(string resource);
	bool ImposeOwnerPermission(string resource);
        string OwnerAssociation(string resource);
	bool CanEditItem(string resource,Object obj, IUser user);
        bool CanDeleteItem(string resource, Object obj, IUser user);
        string FLSAppliedOnProperties(string resource);
        List<BusinessRule> businessrules { get;}
        List<Permission> permissions { get;  }
        List<MultiTenantLoginSelected> MultiTenantLoginSelected { get; }
        List<PermissionAdminPrivilege> adminprivileges { get; }
    }
}