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
    public class BusinessRuleTypeContext : DbContext
    {
        public BusinessRuleTypeContext()
            : base("DefaultConnection")
        {
		 this.ApplyFilters(new List<IFilter<BusinessRuleTypeContext>>()
                     {
                         new BusinessRuleTypesFilter()                                         
                     });
        }
        public IDbSet<BusinessRuleType> BusinessRuleTypes { get; set; }
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
                objectStateEntry = objectStateEntryList.Where(p => p.Entity is BusinessRuleType).ToList()[0];
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
                if (objectStateEntry.Entity is BusinessRuleType)
                {
                    var entity = ((GeneratorBase.MVC.Models.BusinessRuleType)(objectStateEntry.Entity));
                    ((GeneratorBase.MVC.Models.BusinessRuleType)(objectStateEntry.Entity)).DisplayValue = entity.getDisplayValue();
                }
				            }
        }
        public void ApplyFilters(IList<IFilter<BusinessRuleTypeContext>> filters)
        {
            foreach (var filter in filters)
            {
                filter.DbContext = this;
                filter.ApplyBasicSecurity();
				filter.ApplyMainSecurity();
				filter.ApplyUserBasedSecurity();
            }
        }
        public class BusinessRuleTypesFilter : IFilter<BusinessRuleTypeContext>
        {
            public BusinessRuleTypeContext DbContext { get; set; }
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


