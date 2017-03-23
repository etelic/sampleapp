using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
namespace GeneratorBase.MVC.Models
{
    public class ActionArgsContext : DbContext
    {
        public ActionArgsContext()
            : base("DefaultConnection")
        {
            this.ApplyFilters(new List<IFilter<ActionArgsContext>>()
                     {
                         new ActionArgssFilter()                                         
                     });
        }
        public IDbSet<ActionArgs> ActionArgss { get; set; }
        public override int SaveChanges()
        {
            DoBusinessRules();
            ObjectContext ctx = ((IObjectContextAdapter)this).ObjectContext;
            List<ObjectStateEntry> objectStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added
                                                           | EntityState.Modified
                                                           | EntityState.Deleted)
                .ToList();
            EntityState state = new EntityState();
            ObjectStateEntry objectStateEntry = null;
            if (objectStateEntryList.Count > 0)
            {
                objectStateEntry = objectStateEntryList.Where(p => p.Entity is ActionArgs).ToList()[0];
                state = objectStateEntry.State;
            }
            SetDisplayValue(objectStateEntryList, state);
            var result = base.SaveChanges();
            return result;
        }
        private void DoBusinessRules()
        {
            //Business Rules goes here.....
        }
        private void SetDisplayValue(List<ObjectStateEntry> objectStateEntryList, EntityState state)
        {
            if (state == EntityState.Deleted) return;
            foreach (var objectStateEntry in objectStateEntryList)
            {
                if (objectStateEntry.Entity is ActionArgs)
                {
                    var entity = ((GeneratorBase.MVC.Models.ActionArgs)(objectStateEntry.Entity));
                    ((GeneratorBase.MVC.Models.ActionArgs)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
                }
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionArgs>().HasOptional<RuleAction>(p => p.actionarguments).WithMany(s => s.actionarguments).HasForeignKey(f => f.ActionArgumentsID).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleAction>().HasOptional<BusinessRule>(p => p.ruleaction).WithMany(s => s.ruleaction).HasForeignKey(f => f.RuleActionID).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleAction>().HasOptional<ActionType>(p => p.associatedactiontype).WithMany(s => s.associatedactiontype).HasForeignKey(f => f.AssociatedActionTypeID).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
        public void ApplyFilters(IList<IFilter<ActionArgsContext>> filters)
        {
        }
        public class ActionArgssFilter : IFilter<ActionArgsContext>
        {
            public ActionArgsContext DbContext { get; set; }
            public void ApplyBasicSecurity()
            {
            }
            public void ApplyMainSecurity()
            {
            }
            public void ApplyUserBasedSecurity()
            {
            }
        }
    }
}


