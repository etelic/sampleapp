using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_Permission")]
	public class Permission
    {
        [Key]
        public long Id { get; set; }
        [DisplayName("EntityName")]
        public string EntityName { get; set; }
        [DisplayName("RoleName")]
        public string RoleName { get; set; }
        [DisplayName("CanEdit")]
        public bool CanEdit { get; set; }
        [DisplayName("CanDelete")]
        public bool CanDelete { get; set; }
        [DisplayName("CanView")]
        public bool CanView { get; set; }
        [DisplayName("CanAdd")]
        public bool CanAdd { get; set; }
        [DisplayName("NoEdit")]
        public string NoEdit { get; set; }
        [DisplayName("NoView")]
        public string NoView { get; set; }
        [DisplayName("Owner")]
        public Nullable<bool> IsOwner { get; set; }
        [DisplayName("UserAssociation")]
        public string UserAssociation { get; set; }
		//code for verb action security
        [DisplayName("Verbs")]
        public string Verbs { get; set; }
        //
		[DisplayName("Self Registration")]
        public Nullable<bool> SelfRegistration { get; set; }
        [NotMapped]
        public string DisplayValue
        {
            get
            {
                return EntityName + "-" + RoleName;
            }
        }
    }
    public class USB
    {
        public USB()
        {
            this.security = new List<UserBasedSecurity>();
        }
        public USB(List<UserBasedSecurity> list)
            : this()
        {
            foreach (var lst in list)
                this.security.Add(lst);
        }
        public List<UserBasedSecurity> security { get; set; }
    }
    public class USBSecurity
    {
        public USBSecurity()
        {
            this.roles = new List<string>();
        }
        public USBSecurity(UserBasedSecurity ub):this()
        {
            this.UBS = ub;
        }
        public List<string> roles { get; set; }
        public UserBasedSecurity UBS { get; set; }
    }
    public class SelectEntityRolesViewModel
    {
        public SelectEntityRolesViewModel()
        {
            this.Entities = new List<SelectPermissionEditorViewModel>();
			this.privileges = new List<PermissionAdminPrivilege>();
        }
        // Enable initialization with an instance of ApplicationRole:
        public SelectEntityRolesViewModel(string RoleName)
            : this()
        {
            this.RoleName = RoleName;
            var Db = new PermissionContext();
            var permissions = Db.Permissions.ToList().Where(p => p.RoleName == RoleName);
			var listprivileges = Db.AdminPrivileges.ToList().Where(p => p.RoleName == RoleName);
			var IsAppHeader = false;
            var IsDefaultHeader = false;
            var rowcnt = 0;
            foreach (var ent in GeneratorBase.MVC.ModelReflector.Entities.Where(p=>!p.IsAdminEntity))
            {
                if (ent.Name.ToUpper() == "PERMISSION") continue;
                
				if (!IsAppHeader && !ent.IsDefault && rowcnt == 0)
                {
                    IsAppHeader = true;
                    rowcnt++;
                }
                else
                    IsAppHeader = false;
                if (!IsDefaultHeader && ent.IsDefault && rowcnt == 1)
                {
                    IsDefaultHeader = true;
                    rowcnt++;
                }
                else
                    IsDefaultHeader = false;

                var rvm = new SelectPermissionEditorViewModel(ent.Name, IsAppHeader, IsDefaultHeader);

                this.Entities.Add(rvm);
            }
            foreach (var perm in permissions)
            {
                var checkUserRole =
                    this.Entities.ToList().Find(r => r.EntityName == perm.EntityName);
                if (checkUserRole == null) continue;
                if (perm.CanEdit)
                    checkUserRole.CanEdit = true;
                if (perm.CanDelete)
                    checkUserRole.CanDelete = true;
                if (perm.CanView)
                    checkUserRole.CanView = true;
                if (perm.CanAdd)
                    checkUserRole.CanAdd = true;
				checkUserRole.IsOwner = perm.IsOwner !=null ?perm.IsOwner.Value:false;
				 checkUserRole.SelfRegistration = perm.SelfRegistration != null ? perm.SelfRegistration.Value : false;
                checkUserRole.UserAssociation = perm.UserAssociation;
				//code for verb action security
                checkUserRole.Verbs = perm.Verbs;
                //
            }
			//foreach (var item in Enum.GetValues(typeof(AdminFeatures)))
			foreach (var item in (new AdminFeaturesDictionary()).getDictionary())
            {
                var privilege = listprivileges.FirstOrDefault(p => p.AdminFeature == item.Key);
                if (privilege != null)
                    this.privileges.Add(privilege);
                else
                {
                    var obj = new PermissionAdminPrivilege();
                    obj.RoleName = this.RoleName;
                    obj.AdminFeature = item.Key;
                    obj.IsAllow = false;
                    obj.IsAdd = false;
                    obj.IsEdit = false;
                    obj.IsDelete = false;
                    this.privileges.Add(obj);
                }
            }
        }
        public string RoleName { get; set; }
        public List<SelectPermissionEditorViewModel> Entities { get; set; }
		public List<PermissionAdminPrivilege> privileges { get; set; }
    }
    public class SelectPermissionEditorViewModel
    {
        public SelectPermissionEditorViewModel() { }
        public SelectPermissionEditorViewModel(string EntityName, bool IsAppHeader, bool IsDefaultHeader)
        {
            this.EntityName = EntityName;
			this.IsAssociatedWithUser = false;
			  //code for verb action security
            this.IsHaveVerbs = false;
    	 this.IsSelfRegistrartion = false;

		 this.IsAppHeader = IsAppHeader;
            this.IsDefaultHeader = IsDefaultHeader;
            //
            var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).Select(p=>p.Name);
            var association = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Associations.Where(p => p.Target == "IdentityUser" || EntList.Contains(p.Target)).ToList();
            if(association != null && association.Count()>0)
            {
                this.IsAssociatedWithUser = true;
                UserAssociationList = association;
            }
			//code for verb action security
            var EntityVerbs = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Verbs.ToList();
            if (EntityVerbs != null && EntityVerbs.Count() > 0)
            {
                this.IsHaveVerbs = true;
                EntityVerbsList = EntityVerbs;
            }
 var userassociation = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName).Associations.Where(p => p.Target == "IdentityUser").ToList();
            if(userassociation != null && userassociation.Count()>0)
            {
                this.IsSelfRegistrartion = true;
            }
            //
        }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
		public bool IsOwner { get; set; }
		public bool SelfRegistration { get; set; }
        public bool IsAssociatedWithUser { get; set; }

		public bool IsAppHeader { get; set; }
        public bool IsDefaultHeader { get; set; }
		//code for verb action security
        public bool IsHaveVerbs { get; set; }
		public bool IsSelfRegistrartion { get; set; }
        //
        public string UserAssociation { get; set; }

		public string Verbs { get; set; }

        public List<GeneratorBase.MVC.ModelReflector.Association> UserAssociationList { get; set; }
		//code for verb action security
        public List<GeneratorBase.MVC.ModelReflector.Verb> EntityVerbsList { get; set; }
        //
        [Required]
        public string EntityName { get; set; }
    }
    public class SelectFlsViewModel
    {
        public SelectFlsViewModel()
        {
            this.Properties = new List<SelectFlsEditorViewModel>();
        }
        // Enable initialization with an instance of ApplicationRole:
        public SelectFlsViewModel(string RoleName, string EntityName)
            : this()
        {
            this.RoleName = RoleName;
            this.RoleName = RoleName;
            var Db = new PermissionContext();
            var permissions = Db.Permissions.ToList().Where(p => p.RoleName == RoleName && p.EntityName == EntityName);
            var entList = GeneratorBase.MVC.ModelReflector.Entities.Where(p=>!p.IsAdminEntity);
            if (EntityName != null)
                entList = entList.Where(p => p.Name == EntityName).ToList();
            foreach (var ent in entList)
            {
                if (ent.Name.ToUpper() == "PERMISSION") continue;
                foreach (var prop in ent.Properties.Where(p => !(p.DisplayName.Contains("WorkFlowInstanceId")))) 
                {
                    var rvm = new SelectFlsEditorViewModel(prop.Name, ent.Name);
                    this.Properties.Add(rvm);
                }
            }
            foreach (var perm in permissions)
            {
                var checkUserRoleEdit =
                    this.Properties.ToList().Where(r => perm.NoEdit != null && perm.NoEdit.Contains(r.PropertyName+","));
                var checkUserRoleView =
                    this.Properties.ToList().Where(r => perm.NoView != null && perm.NoView.Contains(r.PropertyName + ","));
                foreach (var t in checkUserRoleEdit)
                    t.NoEdit = true;
                foreach (var t in checkUserRoleView)
                    t.NoView = true;
                this.CanAdd = perm.CanAdd;
                this.CanDelete = perm.CanDelete;
                this.CanView = perm.CanView;
                this.CanEdit = perm.CanEdit;
            }
        }
        public string RoleName { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        // public string EntityName { get; set; }
        public List<SelectFlsEditorViewModel> Properties { get; set; }
    }
    public class SelectFlsEditorViewModel
    {
        public SelectFlsEditorViewModel() { }
        public SelectFlsEditorViewModel(string PropertyName, string EntityName)
        {
            this.PropertyName = PropertyName;
            this.EntityName = EntityName;
        }
        public bool NoEdit { get; set; }
        public bool NoView { get; set; }
        [Required]
        public string PropertyName { get; set; }
        public string EntityName { get; set; }
    }
}
