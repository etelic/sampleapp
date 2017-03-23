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
namespace GeneratorBase.MVC
{
    public partial class ApplicationContext : DbContext
    {
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
            var ResultOfBusinessSetValueRules = SetValueRule(dbEntityEntry.Entity, user.businessrules, EntityName);
            if (ResultOfBusinessSetValueRules.Keys.Select(p => p.TypeNo).Contains(7))
            {
                var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                //var DataBaseValues = dbEntityEntry.GetDatabaseValues();
                var Args = GeneratorBase.MVC.Models.BusinessRuleContext.GetActionArguments(ResultOfBusinessSetValueRules.Where(p => p.Key.TypeNo == 7).Select(v => v.Value.ActionID).ToList());
                foreach (var property in Args)
                {
                    dynamic finalvalue=null;
                    bool flagDynamic = false;
                    if (property.ParameterValue.StartsWith("[") && property.ParameterValue.EndsWith("]"))
                    {

                        var targetProperty = property.ParameterValue;
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
                            finalvalue = targetpropInfo.GetValue(dbEntityEntry.Entity, new object[] { });
                        }
                        flagDynamic = true;
                    }
                    else finalvalue = property.ParameterValue;
                    var propertyInfo = dbEntityEntry.Entity.GetType().GetProperty(property.ParameterName);
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
                        object safeValue = (finalvalue == null) ? null : Convert.ChangeType(finalvalue, targetType);
                        propertyInfo.SetValue(dbEntityEntry.Entity, safeValue, null);
                    }
                }
            }
        }
		private void MakeUpdateJournalEntry(DbEntityEntry dbEntityEntry)
        {
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
		private void BroadcastMessageForMonitoring(Dictionary<DbEntityEntry, EntityState> originals)
        {
            foreach (var kvp in originals)
            {
                var entry = kvp.Key;
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                var EntityName = entityType.Name;
                if (EntityName == "T_School")
                {
                    if (kvp.Value.HasFlag(EntityState.Added) || kvp.Value.HasFlag(EntityState.Modified))
                    {
                       var entity = (IEntity)entry.Entity;
                        var id = entry.OriginalValues["Id"];
                        var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GeneratorBase.MVC.Controllers.LiveMonitoring>();
                        GeneratorBase.MVC.Controllers.DataLiveMonitoring dlm = new Controllers.DataLiveMonitoring();
                        dlm.action = kvp.Value.ToString();
                        dlm.username = user.Name;
                        dlm.entitydisplayname = Convert.ToString(entry.OriginalValues["DisplayValue"]);
                        dlm.entityid = Convert.ToString(id);
                        dlm.entityname = EntityName;
                        dlm.datetime = DateTime.Now.ToString();
                        var jsondlm = System.Web.Helpers.Json.Encode(dlm);
                        context.Clients.All.broadcastMessage(jsondlm);
                    }
					else
					{
						var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GeneratorBase.MVC.Controllers.LiveMonitoring>();
						GeneratorBase.MVC.Controllers.DataLiveMonitoring dlm = new Controllers.DataLiveMonitoring();
						dlm.action ="Deleted";
						dlm.username = user.Name;
						dlm.entitydisplayname = "One Record Deleted";
						dlm.entityname = EntityName;
						dlm.datetime = DateTime.Now.ToString();
						var jsondlm = System.Web.Helpers.Json.Encode(dlm);
						context.Clients.All.broadcastMessage(jsondlm);
					}

                }
            }
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
        private void MakeAddJournalEntry(Dictionary<DbEntityEntry, EntityState> originals)
        {
	
        }
        private void MakeDeleteJournalEntry(DbEntityEntry dbEntityEntry)
        {
     
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
            return false;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> LockEntityRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                if (ConditionResult)
                {
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 1))
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
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> MandatoryPropertiesRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                if (ConditionResult)
                {
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 2))
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
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> ReadOnlyPropertiesRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                if (ConditionResult)
                {
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 4))
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
            }
            return RuleDictionaryResult;
        }
        public Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> SetValueRule(object entity, List<BusinessRule> BR, string name)
        {
            Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue> RuleDictionaryResult = new Dictionary<ActionType, GeneratorBase.MVC.Models.BusinessRuleContext.MappedValue>();
            var ruleactiondb = new RuleActionContext();
            foreach (var br in BR)
            {
                var ConditionResult = ApplyRule.CheckRule<object>(entity, br, name);
                if (ConditionResult)
                {
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.associatedactiontype.TypeNo == 7))
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
                        foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 8))//&& p.associatedactiontype.TypeNo == 3))
                        {
                            var condition = ruleconditiondb.Conditions.Where(p => p.RuleConditionsID == br.Id);
                            ActionArgsContext actionargs = new ActionArgsContext();
                            var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id).ToList();
                            var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                            if (ConditionResult)
                            {
                                foreach (var args in argslist)
                                {
                                    var arguments = args.ParameterValue.Split(".".ToCharArray());
                                    if (arguments.Length == 2)
                                    {
                                        var propInfo = entry.Entity.GetType().GetProperty(args.ParameterName);
                                        var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                        InvokeAction(arguments[0], arguments[1], Convert.ToInt32(propvalue));
                                    }
                                    else
                                    {
                                        var propInfo = entry.Entity.GetType().GetProperty("Id");
                                        var propvalue = propInfo.GetValue(entry.Entity, new object[] { });
                                        InvokeAction(EntityName, arguments[0], Convert.ToInt32(propvalue));
                                    }
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
                        foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 3))//&& p.associatedactiontype.TypeNo == 3))
                        {
                            var condition = ruleconditiondb.Conditions.Where(p => p.RuleConditionsID == br.Id);
                            ActionArgsContext actionargs = new ActionArgsContext();
                            var argslist = actionargs.ActionArgss.Where(p => p.ActionArgumentsID == act.Id).ToList();
                            var notifyon = "Add,Update";
                            var notifyonParam = argslist.FirstOrDefault(p => p.ParameterName == "NotifyOn");
                            if (notifyonParam != null)
                                notifyon = notifyonParam.ParameterValue;
                            if ((addflag && notifyon.Contains("Add")) || (!addflag && notifyon.Contains("Update")))
                                if (condition.Count() > 0)
                                {
                                    var ConditionResult = ApplyRule.CheckRule<object>(entry.Entity, br, EntityName);
                                    if (ConditionResult)
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
                                    Uri callbackUrl = new Uri(string.Format("http://localhost//Students//TimeBasedAlert//NotifyOneTime?EntityName=" + EntityName + "&ID=" + entry.OriginalValues["Id"] + "&actionid=" + actionid + "&userName=" + user.Name));
                                    Revalee.Client.RevaleeRegistrar.ScheduleCallback(callbackTime, callbackUrl);
                                
                            }
                        }//
                    }

                }
            }
        }
		private bool CheckBeforeSave(object entity, string EntityName)
        {  try
            {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            System.Reflection.MethodInfo mc = controller.GetMethod("CheckBeforeSave");
            object[] MethodParams = new object[] { entity };
            var obj = mc.Invoke(objController, MethodParams);
            return Convert.ToBoolean(obj);
			}
            catch { return true; }
        }
		private bool InvokeAction(string EntityName, string VerbName, int? Param)
        {
            try
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                System.Reflection.MethodInfo mc = controller.GetMethod(VerbName, BindingFlags.Public | BindingFlags.Instance);
                object[] MethodParams = new object[] { Param };
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
			   modelBuilder.Entity<T_Student>().HasOptional<T_Department>(p => p.t_departmentcode).WithMany(s => s.t_departmentcode).HasForeignKey(f => f.T_DepartmentCodeID).WillCascadeOnDelete(false); 
  base.OnModelCreating(modelBuilder);
        }
    }
	public enum PermissionFreeContext
	{
		Home,
		Admin,
		NearByLocations,
	}
}

