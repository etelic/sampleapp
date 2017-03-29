using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using RecurrenceGenerator;
namespace GeneratorBase.MVC
{
    public partial class ApplicationContext : DbContext
    {
        public static bool CheckIfModified(DbEntityEntry dbEntityEntry)
        {
            bool changed = false;
            if (dbEntityEntry.State == EntityState.Modified)
            {

                var OriginalObj = dbEntityEntry.GetDatabaseValues();
                var CurrentObj = dbEntityEntry.CurrentValues;
                string id = Convert.ToString(CurrentObj.GetValue<object>("Id"));
                string dispValue = Convert.ToString(CurrentObj.GetValue<object>("DisplayValue"));
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    if (property == "DisplayValue" || property == "ConcurrencyKey") continue;
                    var original = OriginalObj.GetValue<object>(property);
                    var current = CurrentObj.GetValue<object>(property);
                    if (original != current && (original == null || !original.Equals(current)))
                    {
                        changed = true;
                        break;
                    }
                }
                if (!changed)
                {
                    dbEntityEntry.State = EntityState.Unchanged;
                }

            }
            else changed= true;
            return changed;
        }
        private static void CancelChanges(DbEntityEntry entry)
        {
            if (entry.State == EntityState.Added)
                entry.State = EntityState.Detached;
            else
                entry.State = EntityState.Unchanged;
        }
		private bool Check1MThresholdCondition(DbEntityEntry entry)
        {
            var entity = entry.Entity.GetType();
            var ResourceId_CurrentObj = entry.CurrentValues;
            return true;
        }
		private void SetAutoProperty(DbEntityEntry dbEntityEntry)
		{
			if (dbEntityEntry.State == EntityState.Modified)
            {
			 //var EntityName = dbEntityEntry.Entity.GetType().Name;
			 var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
             var EntityName = entityType.Name;
		
		if (EntityName == "T_TimeSlots")
		{	
					var T_SlotNo_CurrentObj = dbEntityEntry.CurrentValues;
                    long T_SlotNo_CurrentValue = Convert.ToInt64((T_SlotNo_CurrentObj.GetValue<object>("T_SlotNo")));
                    long T_SlotNo_CorrectValue = Convert.ToInt64((T_SlotNo_CurrentObj.GetValue<object>("Id"))) + 0;
                    if (T_SlotNo_CurrentValue != T_SlotNo_CorrectValue)
                    {
                        var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("T_SlotNo");
                        Type targetType = propertyInfo.PropertyType;
                        if (propertyInfo.PropertyType.IsGenericType)
                            targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                        object safeValue = Convert.ChangeType(T_SlotNo_CorrectValue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
		}
	
				if (EntityName == "ApplicationFeedback")
                {
                    var CommentId_CurrentObj = dbEntityEntry.CurrentValues;
                    long CommentId_CurrentValue = Convert.ToInt64((CommentId_CurrentObj.GetValue<object>("CommentId")));
                    long CommentId_CorrectValue = Convert.ToInt64((CommentId_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                    if (CommentId_CurrentValue != CommentId_CorrectValue)
                    {
                        var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("CommentId");
                        Type targetType = propertyInfo.PropertyType;
                        if (propertyInfo.PropertyType.IsGenericType)
                            targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                        object safeValue = Convert.ChangeType(CommentId_CorrectValue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
                if (EntityName == "FeedbackResource")
                {
                    var ResourceId_CurrentObj = dbEntityEntry.CurrentValues;
                    long ResourceId_CurrentValue = Convert.ToInt64((ResourceId_CurrentObj.GetValue<object>("ResourceId")));
                    long ResourceId_CorrectValue = Convert.ToInt64((ResourceId_CurrentObj.GetValue<object>("Id"))) + (1 - 1);
                    if (ResourceId_CurrentValue != ResourceId_CorrectValue)
                    {
                        var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty("ResourceId");
                        Type targetType = propertyInfo.PropertyType;
                        if (propertyInfo.PropertyType.IsGenericType)
                            targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                        object safeValue = Convert.ChangeType(ResourceId_CorrectValue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
		}
		}
		private void SetAutoProperty(Dictionary<DbEntityEntry, EntityState> originals)
        {
            bool flag = false;
            foreach (var kvp in originals.Where(e => e.Value.HasFlag(EntityState.Added)))
            {
                var entry = kvp.Key;
                if (entry.Entity is T_TimeSlots)
                {
                    var entity = ((GeneratorBase.MVC.Models.T_TimeSlots)(entry.Entity));
                    if (entity.T_SlotNo !=  entity.Id)
                    {
                        entity.T_SlotNo =  entity.Id;
						entity.DisplayValue = entity.getDisplayValue();
						flag= true;
                    }
                }
    
				if (entry.Entity is ApplicationFeedback)
                {
                    var entity = ((GeneratorBase.MVC.Models.ApplicationFeedback)(entry.Entity));
                    if (entity.CommentId != entity.Id + (1 - 1))
                    {
                        entity.CommentId = entity.Id + (1 - 1);
                        entity.DisplayValue = entity.getDisplayValue();
                        flag = true;
                    }
                }
                if (entry.Entity is FeedbackResource)
                {
                    var entity = ((GeneratorBase.MVC.Models.FeedbackResource)(entry.Entity));
                    if (entity.ResourceId != entity.Id + (1 - 1))
                    {
                        entity.ResourceId = entity.Id + (1 - 1);
                        entity.DisplayValue = entity.getDisplayValue();
                        flag = true;
                    }
                }           
            }
			if(flag)
				base.SaveChanges();
        }
		private void EncryptValue(Dictionary<DbEntityEntry, EntityState> originals)
        {	
			bool flag = false;
            foreach (var kvp in originals)
            {
				var entry = kvp.Key;
               
            }
			if(flag)
				base.SaveChanges();
		}
		private bool CheckOwnerPermission(DbEntityEntry dbEntityEntry)
        {
            var result = false;
            if (dbEntityEntry.State == EntityState.Modified || dbEntityEntry.State == EntityState.Deleted)
            {
                var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
                var EntityName = entityType.Name;
                if (user.ImposeOwnerPermission(EntityName))
                {
                    var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                    CheckPermissionForOwner obj = new CheckPermissionForOwner();
                    result = obj.CheckOwnerPermission(EntityName, DataBaseValues, user,true);
                }
            }
            return result;
        }
		private bool CheckLockCondition(DbEntityEntry dbEntityEntry)
        {
            var result = false;
            if (dbEntityEntry.State == EntityState.Modified || dbEntityEntry.State == EntityState.Deleted)
            {
                var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
                var EntityName = entityType.Name;
                var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                CheckPermissionForOwner obj = new CheckPermissionForOwner();
                result = obj.CheckLockCondition(EntityName, DataBaseValues, user, true);
            }
            return result;
        }
		private void CheckFieldLevelSecurity(DbEntityEntry dbEntityEntry)
        {
			var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
            var EntityBR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
            if (dbEntityEntry.State == EntityState.Modified)
            {
                string FLSProperties = user.FLSAppliedOnProperties(EntityName);
                if (!string.IsNullOrEmpty(FLSProperties))
                {
                    var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                    foreach (string propertyName in FLSProperties.Split(",".ToCharArray()))
                    {
                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            object propertyValue = DataBaseValues[propertyName];
                            var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                            Type targetType = propertyInfo.PropertyType;
                            if (propertyInfo.PropertyType.IsGenericType)
                                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                            object safeValue = (propertyValue == null) ? null : Convert.ChangeType(propertyValue, targetType);
                            propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                        }
                    }
                }
                 //Readonly Properties (Business Rule)
                if (EntityBR != null && EntityBR.Count > 0)
                {
					var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                    var ResultOfBusinessRules = ReadOnlyPropertiesRule(DataBaseValues.ToObject(), user.businessrules, EntityName);
                    var BR = EntityBR.Where(p => ResultOfBusinessRules.Values.Select(x => x.BRID).ToList().Contains(p.Id)).ToList();
                    if (ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(4))
                    {
                        var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessRules.Where(p => p.Key.TypeNo == 4).Select(v => v.Value.ActionID).ToList());
                        foreach (string propertyName in Args.Select(p => p.ParameterName))
                        {
                            object propertyValue = DataBaseValues[propertyName];
                            var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(propertyName);
                            Type targetType = propertyInfo.PropertyType;
                            if (propertyInfo.PropertyType.IsGenericType)
                                targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                            object safeValue = (propertyValue == null) ? null : Convert.ChangeType(propertyValue, targetType);
                            propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                        }
                    }

                }
            }
			var ResultOfBusinessSetValueRules = SetValueRule(dbEntityEntry.Entity, user.businessrules, EntityName,dbEntityEntry.State);
            if (ResultOfBusinessSetValueRules.Keys.Select(p => p.TypeNo).Contains(7))
            {
                var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                //var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessSetValueRules.Where(p => p.Key.TypeNo == 7).Select(v => v.Value.ActionID).ToList());
                foreach (var property in Args)
                {
                    dynamic finalvalue=null;
                    bool flagDynamic = false;
					var paramValue = property.ParameterValue;
                    var paramValue2 = "";
                    if (paramValue.Contains("[") && paramValue.Contains("]"))
                    {
                        paramValue = paramValue.Substring(paramValue.IndexOf('['), paramValue.IndexOf(']') + 1);
                        paramValue2 = property.ParameterValue.Substring(paramValue.Length);
                    }
                    if (paramValue.StartsWith("[") && paramValue.EndsWith("]"))
                    {
						var targetProperty = paramValue;
                        targetProperty = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                        if (targetProperty.Contains("."))
                        {
                            var targetProperties = targetProperty.Split(".".ToCharArray());
                            if(targetProperties.Length>1)
                            {
                                var propInfo = dbEntityEntry.Entity.GetType().GetProperty(targetProperties[0]);
                                var firstprop = propInfo.GetValue(dbEntityEntry.Entity, new object[] { });

                                var AssoInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propInfo.Name);
                                if (AssoInfo != null)
                                {
                                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssoInfo.Target + "Controller");
                                    object objController = Activator.CreateInstance(controller, null);
                                    MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                                    object[] MethodParams = new object[] { firstprop, targetProperties[1] };
                                    //var firstvalue =(mc.Invoke(objController, MethodParams));
                                    //finalvalue = firstvalue.GetType().GetProperty(targetProperties[1]).GetValue(firstvalue, new object[] { });
                                    finalvalue = (mc.Invoke(objController, MethodParams));
                                }
                                else
                                {

                                }
                            }
                        }
                        else
                        {
                            var targetpropInfo = dbEntityEntry.Entity.GetType().GetProperty(targetProperty);
							if (targetpropInfo == null)
                                continue;
                            finalvalue = targetpropInfo.GetValue(dbEntityEntry.Entity, new object[] { });
                        }
                        flagDynamic = true;
                    }
                    else finalvalue = property.ParameterValue;
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(property.ParameterName);
					if (propertyInfo == null)
                        continue;
                    Type targetType = propertyInfo.PropertyType;
                    if (propertyInfo.PropertyType.IsGenericType)
                        targetType = propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
                    
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == property.ParameterName);
                    if (AssociationInfo != null && !flagDynamic)
                    {
                        Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller");
                        object objController = Activator.CreateInstance(controller, null);
                        System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromDisplayValue");
                        object[] MethodParams = new object[] { property.ParameterValue };
                        var obj = mc.Invoke(objController, MethodParams);
                        object PropValue = obj;
                        propertyInfo.SetValue(dbEntityEntry.Entity, PropValue, null);
                    }
                    else
                    {
						//changes to set value as today(current date time)
                        if (targetType.Name == "DateTime")
                        {
                            if (flagDynamic)
                            {
                                if (finalvalue != null)
                                    finalvalue = ApplyRule.EvaluateDateTime(Convert.ToString(finalvalue), paramValue2);
                            }
                            else
                                finalvalue = ApplyRule.EvaluateDateForActionInTarget(Convert.ToString(finalvalue));
                        }
                        object safeValue = (finalvalue == null) ? null : Convert.ChangeType(finalvalue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
            }
        }
		private bool IsExternalEntity(object entity, string EntityName, EntityState state)
        {
            return false;
        }
		private void MakeUpdateJournalEntry(DbEntityEntry dbEntityEntry)
        {
			//var EntityName = dbEntityEntry.Entity.GetType().Name;
			var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
			var EntityName = entityType.Name;
            if (EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart")
            {
                var OriginalObj = dbEntityEntry.GetDatabaseValues();
                var CurrentObj =dbEntityEntry.CurrentValues;
                string id = Convert.ToString(CurrentObj.GetValue<object>("Id"));
                string dispValue = Convert.ToString(CurrentObj.GetValue<object>("DisplayValue"));
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    if (property == "DisplayValue" || property == "ConcurrencyKey") continue;
                    var original = OriginalObj.GetValue<object>(property);
                    var current = CurrentObj.GetValue<object>(property);
                    if (original != current && (original == null || !original.Equals(current)))
                        using (var db = new JournalEntryContext())
                        {
                            JournalEntry Je = new JournalEntry();
                            Je.DateTimeOfEntry = DateTime.Now;
                            Je.EntityName = EntityName;
                            Je.UserName = user.Name;
                            Je.Type = dbEntityEntry.State.ToString();
                            var displayValue = dispValue;
                            Je.RecordInfo = displayValue;
                            Je.PropertyName = property;
                            var assolist = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Associations.Where(p => p.AssociationProperty == property).ToList();
                            if (assolist.Count() > 0)
                            {
                                Je.PropertyName = assolist[0].DisplayName;
                                if (original != null)
                                    Je.OldValue = EntityComparer.GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(original));
                                if (current != null)
                                    Je.NewValue = EntityComparer.GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(current));
                            }
                            else
                            {
                                Je.OldValue = Convert.ToString(original);
                                Je.NewValue = Convert.ToString(current);
                            }
                            Je.RecordId = Convert.ToInt64(id);
                            db.JournalEntries.Add(Je);
                            db.SaveChanges();
                        }
                }
            }
			        }
		private bool CheckPermissionOnEntity(string EntityName, EntityState state)
        {
			if (!user.CanAdd(EntityName) && state == EntityState.Added)
               return false;
            if (!user.CanEdit(EntityName) && state == EntityState.Modified)
               return false;
            if (!user.CanDelete(EntityName) && state == EntityState.Deleted)
               return false;
            return true;
        }
        private void SetDisplayValue(DbEntityEntry dbEntityEntry)
        {
            Type EntityType = dbEntityEntry.Entity.GetType();
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
            EntityObj.DisplayValue = EntityObj.getDisplayValue();
        }
		private void SetCalculatedValue(DbEntityEntry dbEntityEntry)
        {
            Type EntityType = dbEntityEntry.Entity.GetType();
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
            try
			{
				EntityObj.setCalculation();
			}
			catch(Exception ex){}
        }
		private void AfterSave(Dictionary<DbEntityEntry, EntityState> originals)
		{
			foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
				//dynamic EntityObj = Convert.ChangeType(entry.Entity, entityType);
				try
				{
					Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
					object objController = Activator.CreateInstance(controller, null);
					System.Reflection.MethodInfo mc = controller.GetMethod("AfterSave");
					object[] MethodParams = new object[] { entry.Entity };
					mc.Invoke(objController, MethodParams);
				}
				catch{}
			}
		}

		private void OrderedListCheck(DbEntityEntry dbEntityEntry)
        {
            if (dbEntityEntry.State == EntityState.Deleted) return;
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
        }

		private void AssignOneToManyCurrentOnUpdate(DbEntityEntry dbEntityEntry)
        {
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
        }
        private void AssignOneToManyCurrentOnAdd(Dictionary<DbEntityEntry, EntityState> originals)
        {
            foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
            }
        }
        private void MakeAddJournalEntry(Dictionary<DbEntityEntry, EntityState> originals)
        {
	
            foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
                if (EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart")
                {
                    if (kvp.Value.HasFlag(EntityState.Added))
                    {
                        var entity = (IEntity)entry.Entity;
                        var id =  entry.OriginalValues["Id"];
                        JournalEntryContext db = new JournalEntryContext();
                        JournalEntry Je = new JournalEntry();
                        Je.DateTimeOfEntry = DateTime.Now;
                        Je.EntityName = EntityName;
                        Je.UserName = user.Name;
                        Je.Type = "Added";
                        Je.RecordId = Convert.ToInt64(id);
                        Type EntityType = entry.Entity.GetType();
                        dynamic EntityObj = Convert.ChangeType(entry.Entity, EntityType);
                        var displayValue = EntityObj.DisplayValue;
                        Je.RecordInfo = displayValue;
                        db.JournalEntries.Add(Je);
                        db.SaveChanges();
                    }
                }
            }	 
	
        }
        private void MakeDeleteJournalEntry(DbEntityEntry dbEntityEntry)
        {
			var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
			var EntityName = entityType.Name;
            if (EntityName == "ImportConfiguration" || EntityName == "DefaultEntityPage" || EntityName == "DynamicRoleMapping" || EntityName == "AppSetting" || EntityName == "EmailTemplate" || EntityName == "EntityDataSource" || EntityName == "DataSourceParameters" || EntityName == "PropertyMapping" || EntityName == "T_Chart")
            {
                var CurrentObj = dbEntityEntry.GetDatabaseValues();
                string id = Convert.ToString(CurrentObj.GetValue<object>("Id"));
                string dispValue = Convert.ToString(CurrentObj.GetValue<object>("DisplayValue"));
                JournalEntryContext db = new JournalEntryContext();
                JournalEntry Je = new JournalEntry();
                Je.DateTimeOfEntry = DateTime.Now;
                Je.EntityName = EntityName;
                Je.UserName = user.Name;
                Je.Type = "Deleted";
                Je.RecordId = Convert.ToInt64(id);
                Je.RecordInfo = dispValue;
                db.JournalEntries.Add(Je);
                db.SaveChanges();
            }
     
        }
		private bool ViolatingBusinessRules(DbEntityEntry dbEntityEntry)
        {
            var entityType = ObjectContext.GetObjectType(dbEntityEntry.Entity.GetType());
            var EntityName = entityType.Name;
            var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
            if (BR.Count > 0 && dbEntityEntry.State == EntityState.Modified)
            {
                var CurrentObj = dbEntityEntry.CurrentValues;
                var id = Convert.ToString((CurrentObj.GetValue<object>("Id")));
                if (id != null && Convert.ToInt64(id) > 0)
                {
                    var ResultOfBusinessRules = LockEntityRule(dbEntityEntry.Entity, BR, dbEntityEntry.Entity.GetType().Name);
                    if (ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(1))
                        return true;
                }
            }
			if (BR.Count > 0)
            {
                var ResultOfBusinessRules = ValidateBeforeSavePropertiesRule(dbEntityEntry.Entity, BR, EntityName);
                if (ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(10))
                    return true;
            }
            return false;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> LockEntityRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                var listruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && (p.associatedactiontype.TypeNo == 1 || p.associatedactiontype.TypeNo == 11));
                if (listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if (!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                foreach (var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    ActionTypeContext atc = new ActionTypeContext();
                    var typeobj = atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if (typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ValidateBeforeSavePropertiesRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
				var listruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 10);
                if (listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if (!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);

                foreach (var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    ActionTypeContext atc = new ActionTypeContext();
                    var typeobj = atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    if (typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> MandatoryPropertiesRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
				var listruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 2);
                if (listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if (!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);

                foreach (var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    ActionTypeContext atc = new ActionTypeContext();
                    var typeobj = atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if (typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ReadOnlyPropertiesRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                //var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                //var ruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4 && !p.IsElseAction);
                //if (!ConditionResult)
                //    ruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4 && p.IsElseAction);

				var listruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4);
                if (listruleactions.Count() == 0) continue;
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = listruleactions.Where(p =>!p.IsElseAction);
                if (!ConditionResult)
                    ruleactions = listruleactions.Where(p => p.IsElseAction);
                foreach (var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    ActionTypeContext atc = new ActionTypeContext();
                    var typeobj = atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    //if (!RuleDictionaryResult.ContainsKey(typeno))
                    if (typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> SetValueRule(object entity, List<BusinessRule> BR, string name, EntityState currentState)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                if (br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(currentState) != "Added")
                    continue;
                else if ((br.AssociatedBusinessRuleTypeID == 5 || br.AssociatedBusinessRuleTypeID == 2) && Convert.ToString(currentState) != "Modified")
                    continue;

                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                var ruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 7 && !p.IsElseAction);
                if (!ConditionResult)
                    ruleactions = ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 7 && p.IsElseAction);
                foreach (var act in ruleactions)
                {
                    GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue obj = new GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue();
                    obj.ActionID = act.Id;
                    obj.BRID = br.Id;
                    ActionTypeContext atc = new ActionTypeContext();
                    var typeobj = atc.ActionTypes.Find(act.AssociatedActionTypeID);
                    var typeno = typeobj != null ? typeobj.TypeNo : 0;
                    // if (!RuleDictionaryResult.ContainsKey(typeno))
                    if (typeobj != null)
                        RuleDictionaryResult.Add(typeobj, obj);
                }
            }
            return RuleDictionaryResult;
        }
        public void InvokeActionRule(Dictionary<DbEntityEntry, EntityState> originals)
        {
            foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
                var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
                if (BR != null && BR.Count() > 0)
                {
                    bool addflag = kvp.Value.HasFlag(EntityState.Added);
                    var ruleactiondb = new RuleActionContext();
                    var ruleconditiondb = new ConditionContext();
                    foreach (var br in BR)
                    {
                        if (br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(kvp.Value) != "Added")
                            continue;
                        else if (br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(kvp.Value) != "Modified")
                            continue;

                        foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 8))//&& p.associatedactiontype.TypeNo == 3))
                        {
                            var condition = ruleconditiondb.Conditions.Where(p => p.RuleConditionsID == br.Id);
                            ActionArgsContext actionargs = new ActionArgsContext();
                            var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                            var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                            if (!ConditionResult)
                                argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                            foreach (var args in argslist)
                            {
                                var arguments = args.ParameterValue.Split(".".ToCharArray());
                                if (arguments.Length == 2)
                                {
                                    var propInfo = entry.Entity.GetType().GetProperty(args.ParameterName);
                                    var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                    InvokeAction(arguments[0], arguments[1], Convert.ToInt32(propvalue), user, this);
                                }
                                else if (arguments[0].Contains("CopyTo"))
                                {
                                    var propInfo = entry.Entity.GetType().GetProperty("Id");
                                    var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                    InvokeCopyAction(EntityName, arguments[0], Convert.ToInt32(propvalue), user, this);
                                }
                                else
                                {
                                    var propInfo = entry.Entity.GetType().GetProperty("Id");
                                    var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                    InvokeAction(EntityName, arguments[0], Convert.ToInt32(propvalue), user, this);
                                }
                            }
                        }
                    }

                }
            }
        }
		public void TimeBasedAlert(Dictionary<DbEntityEntry, EntityState> originals)
        {
            foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
                var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();
                if (BR != null && BR.Count() > 0)
                {
                    bool addflag = kvp.Value.HasFlag(EntityState.Added);
                    var ruleactiondb = new RuleActionContext();
                    var ruleconditiondb = new ConditionContext();
                    var alertMessage = "";
                    string days = "";
                    string NotifyTo = "";
                    string NotifyToRole = "";
                    long actionid = 0;
                    foreach (var br in BR)
                    {
                        if (br.AssociatedBusinessRuleTypeID == 3)
                            continue;
                        if (br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(kvp.Value) != "Added")
                            continue;
                        else if (br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(kvp.Value) != "Modified")
                            continue;
                        foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 3))//&& p.associatedactiontype.TypeNo == 3))
                        {
                            var condition = ruleconditiondb.Conditions.Where(p => p.RuleConditionsID == br.Id);
                            ActionArgsContext actionargs = new ActionArgsContext();
                            var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                            var notifyon = "Add,Update";
                            var notifyonParam = argslist.FirstOrDefault(p => p.ParameterName == "NotifyOn");
                            if (notifyonParam != null)
                                notifyon = notifyonParam.ParameterValue;
                            if ((addflag && notifyon.Contains("Add")) || (!addflag && notifyon.Contains("Update")))
                                if (condition.Count() > 0)
                                {
                                    var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                                    if (!ConditionResult)
                                        argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                                    alertMessage += act.ErrorMessage;
                                    actionid = act.Id;
                                    foreach (var args in argslist)
                                    {
                                        if (args.ParameterName == "TimeValue")
                                            days = args.ParameterValue;
                                        if (args.ParameterName == "NotifyTo")
                                            NotifyTo = args.ParameterValue;
                                        if (args.ParameterName == "NotifyToRole")
                                        {
                                            NotifyToRole = args.ParameterValue;
                                        }
                                    }
                                }
                                else
                                {
                                    alertMessage += act.ErrorMessage;
                                    actionid = act.Id;
                                    foreach (var args in argslist)
                                    {
                                        if (args.ParameterName == "TimeValue")
                                            days = args.ParameterValue;
                                        if (args.ParameterName == "NotifyTo")
                                            NotifyTo = args.ParameterValue;
                                        if (args.ParameterName == "NotifyToRole")
                                        {
                                            NotifyToRole = args.ParameterValue;
                                        }
                                    }
                                }
                            if (!string.IsNullOrEmpty(days))
                            {
                                DateTimeOffset callbackTime;
                                if (Convert.ToInt32(days) == 0)
                                    callbackTime = DateTimeOffset.Now.AddSeconds(10);
                                // callbackTime = DateTimeOffset.Now.AddMinutes(1);
                                else
                                    callbackTime = DateTimeOffset.Now.AddDays(Convert.ToDouble(days));
                                Uri callbackUrl = new Uri(string.Format("http://localhost//AshokCalendarUINew//TimeBasedAlert//NotifyOneTime?EntityName=" + EntityName + "&ID=" + entry.OriginalValues["Id"] + "&actionid=" + actionid + "&userName=" + user.Name));
                                Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);

                            }
                        }//
                    }

                }
            }
        }
        public void TimeBasedAlert(DbEntityEntry originals)
        {
            var entityType = ObjectContext.GetObjectType(originals.Entity.GetType());
            var EntityName = entityType.Name;
            var BR = user.businessrules.Where(p => p.EntityName == EntityName).ToList();

            string id = "";
            if (originals.State != EntityState.Added)
                id = Convert.ToString(originals.OriginalValues["Id"]);
            else
                id = Convert.ToString(originals.CurrentValues["Id"]);

            if (BR != null && BR.Count() > 0)
            {
                bool addflag = originals.State.HasFlag(EntityState.Added);
                var ruleactiondb = new RuleActionContext();
                var ruleconditiondb = new ConditionContext();
                var alertMessage = "";
                string days = "";
                string NotifyTo = "";
                string NotifyToRole = "";
                long actionid = 0;
                foreach (var br in BR)
                {
                    if (br.AssociatedBusinessRuleTypeID != 3)
                        continue;
                    if (br.AssociatedBusinessRuleTypeID == 1 && Convert.ToString(originals.State) != "Added")
                        continue;
                    else if (br.AssociatedBusinessRuleTypeID == 2 && Convert.ToString(originals.State) != "Modified")
                        continue;
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 3))//&& p.associatedactiontype.TypeNo == 3))
                    {
                        var condition = ruleconditiondb.Conditions.Where(p => p.RuleConditionsID == br.Id);
                        ActionArgsContext actionargs = new ActionArgsContext();
                        var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && !act.IsElseAction).ToList();
                        var notifyon = "Add,Update";
                        var notifyonParam = argslist.FirstOrDefault(p => p.ParameterName == "NotifyOn");
                        if (notifyonParam != null)
                            notifyon = notifyonParam.ParameterValue;
                        if ((addflag && notifyon.Contains("Add")) || (!addflag && notifyon.Contains("Update")))
                            if (condition.Count() > 0)
                            {
                                var ConditionResult = ApplyRule.CheckRule<object>(originals.Entity, br, EntityName);
                                if (!ConditionResult)
                                    argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id && act.IsElseAction).ToList();
                                alertMessage += act.ErrorMessage;
                                actionid = act.Id;
                                foreach (var args in argslist)
                                {
                                    if (args.ParameterName == "TimeValue")
                                        days = args.ParameterValue;
                                    if (args.ParameterName == "NotifyTo")
                                        NotifyTo = args.ParameterValue;
                                    if (args.ParameterName == "NotifyToRole")
                                    {
                                        NotifyToRole = args.ParameterValue;
                                    }
                                }

                            }
                            else
                            {
                                alertMessage += act.ErrorMessage;
                                actionid = act.Id;
                                foreach (var args in argslist)
                                {
                                    if (args.ParameterName == "TimeValue")
                                        days = args.ParameterValue;
                                    if (args.ParameterName == "NotifyTo")
                                        NotifyTo = args.ParameterValue;
                                    if (args.ParameterName == "NotifyToRole")
                                    {
                                        NotifyToRole = args.ParameterValue;
                                    }
                                }
                            }
                        if (!string.IsNullOrEmpty(days))
                        {
                            DateTimeOffset callbackTime;
                            if (Convert.ToInt32(days) == 0)
                                callbackTime = DateTimeOffset.Now.AddSeconds(10);
                            else
                                callbackTime = DateTimeOffset.Now.AddDays(Convert.ToDouble(days));

                            Uri callbackUrl = new Uri(string.Format("http://localhost//AshokCalendarUINew//TimeBasedAlert//NotifyOneTime?EntityName=" + EntityName + "&ID=" + id + "&actionid=" + actionid + "&userName=" + user.Name));
                            Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);
                        }
                    }//
                }

            }

        }
		private string CheckBeforeSave(object entity, string EntityName)
        {  try
            {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("CheckBeforeSave");
            object[] MethodParams = new object[] { entity };
            var obj = mc.Invoke(objController, MethodParams);
            return Convert.ToString(obj);
			}
            catch { return ""; }
        }
		private bool InvokeAction(string EntityName, string VerbName, int? Param, IUser user, ApplicationContext db)
        {
            try
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                System.Reflection.MethodInfo mc = controller.GetMethod(VerbName, BindingFlags.Public | BindingFlags.Instance);
                object[] MethodParams = new object[] { Param, user, db, true };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToBoolean(obj);
            }
            catch { return true; }
        }
		private bool InvokeCopyAction(string EntityName, string VerbName, int? Param, IUser user, ApplicationContext db)
        {
            string sourceId = Convert.ToString(Param);
            try
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                System.Reflection.MethodInfo mc = controller.GetMethod(VerbName, BindingFlags.Public | BindingFlags.Instance);
                object[] MethodParams = new object[] { sourceId, "", null, "", "", "",db };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToBoolean(obj);
            }
            catch { return true; }
        }
		private void OnSavingCustom(DbEntityEntry dbEntityEntry, ApplicationContext db)
        {
		 try
            {
            Type EntityType = dbEntityEntry.Entity.GetType();
            var EntityName = EntityType.Name;
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("OnSaving");
            object[] MethodParams = new object[] { EntityObj,db };
            mc.Invoke(objController, MethodParams);
			}
            catch { return; }
        }
		private bool CheckBeforeDelete(object entity, string EntityName)
        {
		 try
            {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("CheckBeforeDelete");
            object[] MethodParams = new object[] { entity };
            var obj = mc.Invoke(objController, MethodParams);
            return Convert.ToBoolean(obj);
			}
            catch { return true; }
        }
		private void OnDeletingCustom(DbEntityEntry dbEntityEntry)
        {
		 try
            {
            Type EntityType = dbEntityEntry.Entity.GetType();
            var EntityName = EntityType.Name;
            dynamic EntityObj = Convert.ChangeType(dbEntityEntry.Entity, EntityType);
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("OnDeleting");
            object[] MethodParams = new object[] { EntityObj };
            mc.Invoke(objController, MethodParams);
			}
            catch { return; }
        }
		public void ApplyFilters(IList<IFilter<ApplicationContext>> filters)
        {
            foreach (var filter in filters)
            {
                filter.DbContext = this;
                if (user != null && (!string.IsNullOrEmpty(user.Name)))
                {
					filter.ApplyBasicSecurity();
                    filter.ApplyMainSecurity();
                    filter.ApplyUserBasedSecurity();
                }
            }
        }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
			   modelBuilder.Entity<T_Session>().HasOptional<T_LearningCenter>(p => p.t_sessionlearningcenterassociation).WithMany(s => s.t_sessionlearningcenterassociation).HasForeignKey(f => f.T_SessionLearningCenterAssociationID).WillCascadeOnDelete(false);
   modelBuilder.Entity<T_Session>().HasOptional<T_TimeSlots>(p => p.t_sessiontimeslotassociaton).WithMany(s => s.t_sessiontimeslotassociaton).HasForeignKey(f => f.T_SessionTimeSlotAssociatonID).WillCascadeOnDelete(false);
   modelBuilder.Entity<T_TimeSlots>().HasOptional<T_LearningCenter>(p => p.t_learningcentertimeslotsassociation).WithMany(s => s.t_learningcentertimeslotsassociation).HasForeignKey(f => f.T_LearningCenterTimeSlotsAssociationID).WillCascadeOnDelete(false); 
   modelBuilder.Entity<T_TimeSlots>().HasOptional<T_LearningCenter>(p => p.t_learningcentertimeslotsassociation).WithMany(s => s.t_learningcentertimeslotsassociation).HasForeignKey(f => f.T_LearningCenterTimeSlotsAssociationID).WillCascadeOnDelete(false);
   modelBuilder.Entity<T_SessionEvents>().HasOptional<T_LearningCenter>(p => p.t_sessioneventslearningcenter).WithMany(s => s.t_sessioneventslearningcenter).HasForeignKey(f => f.T_SessionEventsLearningCenterID).WillCascadeOnDelete(false);
   modelBuilder.Entity<T_SessionEvents>().HasOptional<T_TimeSlots>(p => p.t_sessioneventstimeslots).WithMany(s => s.t_sessioneventstimeslots).HasForeignKey(f => f.T_SessionEventsTimeSlotsID).WillCascadeOnDelete(false);
   modelBuilder.Entity<T_TimeSlots>().HasOptional<T_LearningCenter>(p => p.t_learningcentertimeslotsassociation).WithMany(s => s.t_learningcentertimeslotsassociation).HasForeignKey(f => f.T_LearningCenterTimeSlotsAssociationID).WillCascadeOnDelete(false); 
  base.OnModelCreating(modelBuilder);
        }
    }
	public enum PermissionFreeContext
	{
		Home,
		Admin,
		NearByLocations,
		ResourceLocalization,
		Chart,
		ApiHelp,
		BusinessRule
	}
}
public class RegisterScheduledTask
{
      private DateTime getNextRunTimeOfTask(BusinessRule br)
    {
        GeneratorBase.MVC.ApplicationContext db1 = new GeneratorBase.MVC.ApplicationContext(new InternalUser());
        var result = DateTime.MinValue;
        var task = db1.T_Schedules.Find(br.T_SchedulerTaskID); //br.t_schedulertask;

        if (task.T_AssociatedScheduleTypeID == 1)
            if (task.T_StartDateTime > DateTime.Now)
                return task.T_StartDateTime;
            else return DateTime.MinValue;

        var ScheduledTime = task.T_StartDateTime.ToShortTimeString();
        var ScheduledDateTimeEnd = DateTime.Now.AddYears(10);
        var occurrences = task.T_OccurrenceLimitCount != null ? task.T_OccurrenceLimitCount : 0;
        var skip = task.t_recurringrepeatfrequency != null ? task.t_recurringrepeatfrequency.T_Name : 0;
        if (task.t_recurringtaskendtype.DisplayValue == "On Specified Date")
            ScheduledDateTimeEnd = task.T_EndDate.Value;
        
		if (task.t_associatedrecurringscheduledetailstype.DisplayValue == "Daily")
        {
            DailyRecurrenceSettings we;
            if (task.T_RecurringTaskEndTypeID == 2)
                we = new DailyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new DailyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(skip);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(DateTime.Now);
            if (nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if (task.t_associatedrecurringscheduledetailstype.DisplayValue == "Weekly")
        {
            WeeklyRecurrenceSettings we;
            SelectedDayOfWeekValues selectedValues = new SelectedDayOfWeekValues();
            if (task.T_RecurringTaskEndTypeID == 2)
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new WeeklyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            selectedValues.Sunday = task.T_RepeatOn_t_schedule.Select(p=>p.T_RecurrenceDaysID).ToList().Contains(1);
            selectedValues.Monday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(2);
            selectedValues.Tuesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(3);
            selectedValues.Wednesday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(4);
            selectedValues.Thursday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(5);
            selectedValues.Friday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(6);
            selectedValues.Saturday = task.T_RepeatOn_t_schedule.Select(p => p.T_RecurrenceDaysID).ToList().Contains(7);
            var values = we.GetValues(skip, selectedValues);
            var test = values.Values.First();
            var nextdate = we.GetNextDate(DateTime.Now);
            if (nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if (task.t_associatedrecurringscheduledetailstype.DisplayValue == "Monthly")
        {
            MonthlyRecurrenceSettings we;
            
            
            if (task.T_RecurringTaskEndTypeID == 2)
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new MonthlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            we.AdjustmentValue = 0;
            skip = skip++;
            RecurrenceValues value;
            if(task.T_RepeatByID == 3)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.Day, skip);
            if (task.T_RepeatByID == 4)
                value = we.GetValues(MonthlySpecificDatePartOne.First, MonthlySpecificDatePartTwo.Day, skip);
            if (task.T_RepeatByID == 1)
                value = we.GetValues(task.T_StartDateTime.Day, skip);
            if (task.T_RepeatByID == 2)
                value = we.GetValues(MonthlySpecificDatePartOne.Last, MonthlySpecificDatePartTwo.WeekendDay, skip);
            var nextdate = we.GetNextDate(DateTime.Now);
            if (nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        if (task.t_associatedrecurringscheduledetailstype.DisplayValue == "Yearly")
        {
            YearlyRecurrenceSettings we;
            if (task.T_RecurringTaskEndTypeID == 2)
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, occurrences.Value);
            else
                we = new YearlyRecurrenceSettings(task.T_StartDateTime, ScheduledDateTimeEnd);
            var values = we.GetValues(task.T_StartDateTime.Day, task.T_StartDateTime.Month);
            var nextdate = we.GetNextDate(DateTime.Now);
            if (nextdate <= ScheduledDateTimeEnd)
                result = nextdate;
            else result = DateTime.MinValue;
        }
        return result;

    }
    public void RegisterTask(string EntityName, long BizId)
    {
        BusinessRuleContext brcontext = new BusinessRuleContext();
        var MainBiz = brcontext.BusinessRules.Find(BizId);
        var task = MainBiz.t_schedulertask;

        RegisterScheduledTask RegisterTask = new RegisterScheduledTask();
        var nextDate = getNextRunTimeOfTask(MainBiz);
        if (nextDate > DateTime.MinValue)
        {
            Uri callbackUrl = new Uri(string.Format("http://localhost//AshokCalendarUINew//TimeBasedAlert//ScheduledTask?EntityName=" + EntityName + "&BizId=" + BizId));
            var callbackid = Revalee.Client.RevaleeRegistrar.ScheduleCallback(nextDate, callbackUrl);
            ScheduledTaskHistoryContext historycontext = new ScheduledTaskHistoryContext();
            ScheduledTaskHistory historyItem = new ScheduledTaskHistory();
            historyItem.BusinessRuleId = MainBiz.Id;
            historyItem.CallbackUri = Convert.ToString(callbackUrl);
            historyItem.Status = "Pending";
            historyItem.GUID = callbackid.ToString();
            historyItem.TaskName = MainBiz.DisplayValue;
            historyItem.RunDateTime = Convert.ToString(nextDate);
            historycontext.ScheduledTaskHistorys.Add(historyItem);
            historycontext.SaveChanges();
        }
         
    }
}

