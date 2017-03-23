using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
namespace GeneratorBase.MVC.Models
{
    public class BusinessRuleContext : DbContext
    {
        public BusinessRuleContext()
            : base("DefaultConnection")
        {
		 this.ApplyFilters(new List<IFilter<BusinessRuleContext>>()
                     {
                         new BusinessRulesFilter()                                         
                     });
        }
        public IDbSet<BusinessRule> BusinessRules { get; set; }
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
                objectStateEntry = objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0];
                state = objectStateEntry.State;
            }
			SetDisplayValue(objectStateEntryList,state);
            if (objectStateEntry != null && state == EntityState.Modified)
            {
                var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
                MakeUpdateJournalEntry(id, objectStateEntry.State.ToString(), objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0]);
            }
            if (objectStateEntry != null && state == EntityState.Deleted)
            {
                var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
                ConditionContext brconditions = new ConditionContext();
                long? longid = Convert.ToInt64(id);
                var condlist = brconditions.Conditions.Where(p => p.RuleConditionsID.Value == longid);
                foreach (var cond in condlist)
                    brconditions.Conditions.Remove(cond);
                brconditions.SaveChanges();
                RuleActionContext brActions = new RuleActionContext();
                ActionArgsContext brArgs = new ActionArgsContext();
                var actionlist = brActions.RuleActions.Where(p => p.RuleActionID.Value == longid);
                foreach (var action in actionlist)
                {
                    brActions.RuleActions.Remove(action);
                    var argslist = brArgs.ActionArgss.Where(p=>p.ActionArgumentsID == action.Id);
                    foreach (var args in argslist)
                        brArgs.ActionArgss.Remove(args);
                    brArgs.SaveChanges();
                }
                brActions.SaveChanges();
            }
            var result = base.SaveChanges();
			if (objectStateEntryList.Count > 0)
            {
                if (objectStateEntry != null && state == EntityState.Added)
                {
                    var id = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
                    MakeAddJournalEntry(id, state.ToString(), objectStateEntryList.Where(p => p.Entity is BusinessRule).ToList()[0]);
                }
            }
			return result;
        }
        public static List<ActionArgs> GetActionArguments(List<long> ActionIds)
        {
            ActionArgsContext aac = new ActionArgsContext();
            return aac.ActionArgss.Where(p => ActionIds.Contains(p.ActionArgumentsID.Value)).ToList();
        }
		private void DoBusinessRules()
		{
			//Business Rules goes here.....
		}
		private void SetDisplayValue(List<ObjectStateEntry> objectStateEntryList,EntityState state)
        {   if (state == EntityState.Deleted) return;
			foreach (var objectStateEntry in objectStateEntryList)
            {
                if (objectStateEntry.Entity is BusinessRule)
                {
                    var entity = ((GeneratorBase.MVC.Models.BusinessRule)(objectStateEntry.Entity));
                    ((GeneratorBase.MVC.Models.BusinessRule)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
                }
				            }
        }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
   modelBuilder.Entity<BusinessRule>().HasOptional<BusinessRuleType>(p => p.associatedbusinessruletype).WithMany(s => s.associatedbusinessruletype).HasForeignKey(f => f.AssociatedBusinessRuleTypeID).WillCascadeOnDelete(false); 
            base.OnModelCreating(modelBuilder);
        }
        public void ApplyFilters(IList<IFilter<BusinessRuleContext>> filters)
        {
        }
        public class BusinessRulesFilter : IFilter<BusinessRuleContext>
        {
            public BusinessRuleContext DbContext { get; set; }
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
 private void MakeAddJournalEntry(string id, string state,ObjectStateEntry entry)
 {
            JournalEntryContext db = new JournalEntryContext();
            JournalEntry Je = new JournalEntry();
            Je.DateTimeOfEntry = DateTime.Now;
            Je.EntityName = "BusinessRule";
			Je.UserName = System.Web.HttpContext.Current.User.Identity.Name;
            Je.Type = state;
			if(entry.Entity is BusinessRule)
			{
				var displayValue = ((GeneratorBase.MVC.Models.BusinessRule)(entry.Entity)).DisplayValue;
				Je.RecordInfo = displayValue;
				Je.RecordId = Convert.ToInt64(id);
				db.JournalEntries.Add(Je);
				db.SaveChanges();
			}
}
private void MakeUpdateJournalEntry(string id, string state, ObjectStateEntry entry)
{
    List<string> result = new List<string>();
    using (var context = new BusinessRuleContext())
    {
        var _BusinessRule = context.BusinessRules.Find(Convert.ToInt64(id));
        result = EntityComparer.EnumeratePropertyDifferences<BusinessRule>(_BusinessRule, ((GeneratorBase.MVC.Models.BusinessRule)(entry.Entity)),state,"BusinessRule").ToList();
    }
}
public class MappedValue
{
    public long ActionID { get; set; }
    public long BRID { get; set; }
}
    }
}


