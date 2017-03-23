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
    public class ConditionContext : DbContext
    {
        public ConditionContext()
            : base("DefaultConnection")
        {
		 this.ApplyFilters(new List<IFilter<ConditionContext>>()
                     {
                         new ConditionsFilter()                                         
                     });
        }
        public IDbSet<Condition> Conditions { get; set; }
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
                objectStateEntry = objectStateEntryList.Where(p => p.Entity is Condition).ToList()[0];
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
                if (objectStateEntry.Entity is Condition)
                {
                    var entity = ((GeneratorBase.MVC.Models.Condition)(objectStateEntry.Entity));
                    ((GeneratorBase.MVC.Models.Condition)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
                }
				            }
        }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
   modelBuilder.Entity<Condition>().HasOptional<BusinessRule>(p => p.ruleconditions).WithMany(s => s.ruleconditions).HasForeignKey(f => f.RuleConditionsID).WillCascadeOnDelete(false); 
   modelBuilder.Entity<BusinessRule>().HasOptional<BusinessRuleType>(p => p.associatedbusinessruletype).WithMany(s => s.associatedbusinessruletype).HasForeignKey(f => f.AssociatedBusinessRuleTypeID).WillCascadeOnDelete(false); 
            base.OnModelCreating(modelBuilder);
        }
        public void ApplyFilters(IList<IFilter<ConditionContext>> filters)
        {
        }
        public class ConditionsFilter : IFilter<ConditionContext>
        {
            public ConditionContext DbContext { get; set; }
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


