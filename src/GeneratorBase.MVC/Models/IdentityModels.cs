using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Security;
using System.Data.Entity;
using System.Web;
using System.Linq;
namespace GeneratorBase.MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
	public class AuthenticationDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthenticationDbContext()
            : base("AuthenticationConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             var user = modelBuilder.Entity<IdentityUser>()
         .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName).IsRequired();
            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey })
                .ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("AspNetUserClaims");
            var role = modelBuilder.Entity<IdentityRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name).IsRequired();
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
		 this.ApplyFilters(new List<IFilter<ApplicationDbContext>>()
                     {
                         new UserFilter()                                         
                     });
        }
		 public ApplicationDbContext(bool nofilter)
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
		public IDbSet<LoginAttempts> LoginAttempts { get; set; }
		public IDbSet<PasswordHistory> PasswordHistorys { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
	    modelBuilder.Entity<JournalEntry>().HasKey(au => au.Id).ToTable("tbl_JournalEntry");
             var user = modelBuilder.Entity<IdentityUser>()
         .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName).IsRequired();
            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey })
                .ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("AspNetUserClaims");
            var role = modelBuilder.Entity<IdentityRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name).IsRequired();
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
			modelBuilder.Entity<T_Student>().ToTable("tbl_T_Student");
            modelBuilder.Entity<T_Student>().HasOptional<T_School>(p => p.t_schoolname).WithMany().WillCascadeOnDelete(false); ;
			modelBuilder.Entity<T_StudentPerformance>().ToTable("tbl_T_StudentPerformance");
            modelBuilder.Entity<T_StudentPerformance>().HasOptional<T_Student>(p => p.t_studentcode).WithMany().WillCascadeOnDelete(false); ;
        }
		public void ApplyFilters(IList<IFilter<ApplicationDbContext>> filters)
        {
            foreach (var filter in filters)
            {
                filter.DbContext = this;               
                filter.ApplyUserBasedSecurity();
            }
        }
        public class UserFilter : IFilter<ApplicationDbContext>
        {
            public ApplicationDbContext DbContext { get; set; }
			public void ApplyBasicSecurity()
			{
			}
            public void ApplyMainSecurity()
            {
            }
            public void ApplyUserBasedSecurity()
            {
                if (((CustomPrincipal)HttpContext.Current.User).IsAdmin() || string.IsNullOrEmpty(((CustomPrincipal)HttpContext.Current.User).Identity.Name))
                    return;
                UserBasedSecurityContext UBS = new UserBasedSecurityContext();
                if(UBS.UserBasedSecurities.Where(p=>p.IsMainEntity).Count()>0)
                {
                    var _ubs = UBS.UserBasedSecurities.FirstOrDefault(p => p.IsMainEntity);
                    if (_ubs.RolesToIgnore != null && ((CustomPrincipal)HttpContext.Current.User).IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        return;
                    DbContext.Users = new FilteredDbSet<ApplicationUser>(DbContext, d => d.UserName == HttpContext.Current.User.Identity.Name);
                }
            }
        }
    }
 public class IdentityManager
    {
        public bool RoleExists(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.RoleExists(name);
        }
        public bool CreateRole(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }
        public bool CreateUser(ApplicationUser user, string password)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) { AllowOnlyAlphanumericUserNames = false };
            var idResult = um.Create(user, password);
            return idResult.Succeeded;
        }
        public bool AddUserToRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) { AllowOnlyAlphanumericUserNames = false };
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }
        public void ClearUserRoles(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) { AllowOnlyAlphanumericUserNames = false };
            var user = um.FindById(userId);
            using (var ctx = new ApplicationDbContext())
            {
                var roleIds = user.Roles.Select(r => r.RoleId);
                var roles = ctx.Roles.Where(r => roleIds.Contains(r.Id))
                                     .Select(r => r.Name);
                um.RemoveFromRoles(userId, roles.ToArray());
            }
        }
        public void ClearUsersFromRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
               new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) { AllowOnlyAlphanumericUserNames = false };
            um.RemoveFromRole(userId, roleName);
        }
    }
}
