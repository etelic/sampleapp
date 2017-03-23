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
    public class RuleActionContext : DbContext
    {
        public RuleActionContext()
            : base("DefaultConnection")
        {
		 this.ApplyFilters(new List<IFilter<RuleActionContext>>()
                     {
                         new RuleActionsFilter()                                         
                     });
        }
        public IDbSet<RuleAction> RuleActions { get; set; }
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
                objectStateEntry = objectStateEntryList.Where(p => p.Entity is RuleAction).ToList()[0];
                state = objectStateEntry.State;
            }
			SetDisplayValue(objectStateEntryList,state);
            var result = base.SaveChanges();
			return result;
        }
		private void DoBusinessRules()
		{
			//Business Rules goes here.....
		}
		private void SetDisplayValue(List<ObjectStateEntry> objectStateEntryList,EntityState state)
        {   if (state == EntityState.Deleted) return;
			foreach (var objectStateEntry in objectStateEntryList)
            {
                if (objectStateEntry.Entity is RuleAction)
                {
                    var entity = ((GeneratorBase.MVC.Models.RuleAction)(objectStateEntry.Entity));
                    ((GeneratorBase.MVC.Models.RuleAction)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
                }
				            }
        }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
   modelBuilder.Entity<RuleAction>().HasOptional<BusinessRule>(p => p.ruleaction).WithMany(s => s.ruleaction).HasForeignKey(f => f.RuleActionID).WillCascadeOnDelete(false); 
   modelBuilder.Entity<BusinessRule>().HasOptional<BusinessRuleType>(p => p.associatedbusinessruletype).WithMany(s => s.associatedbusinessruletype).HasForeignKey(f => f.AssociatedBusinessRuleTypeID).WillCascadeOnDelete(false); 
   modelBuilder.Entity<RuleAction>().HasOptional<ActionType>(p => p.associatedactiontype).WithMany(s => s.associatedactiontype).HasForeignKey(f => f.AssociatedActionTypeID).WillCascadeOnDelete(false); 
            base.OnModelCreating(modelBuilder);
        }
        public void ApplyFilters(IList<IFilter<RuleActionContext>> filters)
        {
        }
        public class RuleActionsFilter : IFilter<RuleActionContext>
        {
            public RuleActionContext DbContext { get; set; }
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


