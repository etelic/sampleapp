using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
namespace GeneratorBase.MVC.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        { }
        public override void CreateRole(string roleName) { }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) { return true; }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch) { return new string[] { "Admin" }; }
        //public override string[] GetUsersInRole(string roleName)
        //{
        //    return new string[] { "Admin" };
        //}
        public override string[] GetUsersInRole(string roleId)
        {
            if (roleId == "")
                return new string[] { "" };
            using (var usersContext = new ApplicationDbContext(true))
            {
                if (roleId == "0")
                {
                    var allUsers = usersContext.Users.Select(p => p.Id);
                    return allUsers.ToArray();
                }
                var role = usersContext.Roles.FirstOrDefault(r => roleId == r.Id);
                var users = role.Users.Select(p => p.UserId);
                return users.ToArray();
            }
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) { }
        public override bool RoleExists(string roleName) { return true; }
        public override string ApplicationName
        {
            get { return ApplicationName; }
            set { }
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var usersContext = new ApplicationDbContext(true))
            {
                var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                    return false;
                if (user.Roles == null)
                    return false;
                var roleIds = user.Roles.Select(r => r.RoleId);
                var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id));
                string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
                var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"].Value;
                if (!string.IsNullOrEmpty(UserCurrentRole))
                {
                    string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                    roles = roles.Where(u => splittedarray.Contains(u.Name.Trim()));
                }
                AddDynamicRoles adr = new AddDynamicRoles();
                return roles.Any(r => r.Name == roleName) || adr.AddRolesDynamic(new string[] { }, user.Id).Contains(roleName);
            }
        }
        public override string[] GetRolesForUser(string username)
        {
            using (var usersContext = new ApplicationDbContext(true))
            {
                var user = usersContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                    return new string[] { };
                if (user.Roles == null)
                    return new string[] { };
                var roleIds = user.Roles.Select(r => r.RoleId);
                var roles = usersContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToArray();
                AddDynamicRoles adr = new AddDynamicRoles();
                roles = adr.AddRolesDynamic(roles, user.Id);
                string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
                var UserCurrentRole = HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"] == null ? string.Empty : HttpContext.Current.Request.Cookies[AppUrl+"CurrentRole"].Value;
                if (!string.IsNullOrEmpty(UserCurrentRole))
                {
                    string[] splittedarray = UserCurrentRole.Split(",".ToCharArray()).Select(p => p.Trim()).ToArray();
                    roles = roles.Where(u => splittedarray.Contains(u.Trim())).ToArray();
                }
                return roles;
            }
        }
        // -- Snip --
        public override string[] GetAllRoles()
        {
            using (var usersContext = new ApplicationDbContext())
            {
                return usersContext.Roles.Select(r => r.Name).ToArray();
            }
        }
        public Dictionary<string, string> GetAllRolesNotifyRole(string adminString)
        {
            Dictionary<string, string> RolesList = new Dictionary<string, string>();
            RolesList.Add("0", "All");
            using (var usersContext = new ApplicationDbContext())
            {
                var _rolelist = usersContext.Roles.ToList();
                foreach (var item in _rolelist)
                {
                    if (item.Name == adminString)
                        continue;
                    RolesList.Add(item.Id, item.Name);
                }
            }
            return RolesList;
        }
        public Dictionary<string, string> GetAllRolesBR()
        {
            Dictionary<string, string> RolesList = new Dictionary<string, string>();
            RolesList.Add("0", "All");
            using (var usersContext = new ApplicationDbContext())
            {
                var _rolelist = usersContext.Roles.ToList();
                foreach (var item in _rolelist)
                {
                    RolesList.Add(item.Id, item.Name);
                }
            }
            return RolesList;
        }
        // -- Snip --
    }
}