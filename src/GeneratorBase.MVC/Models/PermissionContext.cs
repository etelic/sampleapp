using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class PermissionContext : DbContext
    {
        IUser user;
        public PermissionContext(IUser user)
            : base("DefaultConnection")
        {
            this.user = user;
        }
        public PermissionContext()
            : base("DefaultConnection")
        {

        }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionAdminPrivilege> AdminPrivileges { get; set; }
        public override int SaveChanges()
        {
            var result = 0;
            var entries = this.ChangeTracker.Entries().Where(e => e.State.HasFlag(EntityState.Added) ||
                                                                   e.State.HasFlag(EntityState.Modified) ||
                                                                   e.State.HasFlag(EntityState.Deleted));
            var originalStates = new Dictionary<DbEntityEntry, EntityState>();
            //var entry = entries.FirstOrDefault();
            //if (entry != null)
            //{
            //    if (entry.State == EntityState.Modified)
            //        DoAuditEntry.MakeUpdateJournalEntry(user, entry);
            //    if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
            //        DoAuditEntry.MakeAddJournalEntry(user, null, entry);
            //}
            foreach (var entry in entries)
            {
                try
                {
                    if (entry.State == EntityState.Modified)
                        DoAuditEntry.MakeUpdateJournalEntry(user, entry);
                    if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                        DoAuditEntry.MakeAddJournalEntry(user, null, entry);
                }
                catch { continue; }
            }
            result = base.SaveChanges();
            return result;

        }
        private void DoBusinessRules()
        {
            //Business Rules goes here.....
        }
    }
}
