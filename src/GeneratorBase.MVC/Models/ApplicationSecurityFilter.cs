using GeneratorBase.MVC.Models;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;
using System;
namespace GeneratorBase.MVC
{
	public class ParameterRebinder : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
	public class ApplicationSecurityFilter : IFilter<ApplicationContext>
    {
		IUser user;
        public ApplicationContext DbContext { get; set; }
        public ApplicationSecurityFilter(IUser user)
        {
            this.user = user;
        }
		public void ApplyBasicSecurity()
		{
			if (string.IsNullOrEmpty(user.Name))
                return;
            List<long?> doclist = new List<long?>();
						
            if (!user.CanView("FileDocument"))
            {
                DbContext.FileDocuments = new FilteredDbSet<FileDocument>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Client"))
            {
                DbContext.T_Clients = new FilteredDbSet<T_Client>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_LearningCenter"))
            {
                DbContext.T_LearningCenters = new FilteredDbSet<T_LearningCenter>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Session"))
            {
                DbContext.T_Sessions = new FilteredDbSet<T_Session>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_TimeSlots"))
            {
                DbContext.T_TimeSlotss = new FilteredDbSet<T_TimeSlots>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_SessionEvents"))
            {
                DbContext.T_SessionEventss = new FilteredDbSet<T_SessionEvents>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_SessionClientAssociation"))
            {
                DbContext.T_SessionClientAssociations = new FilteredDbSet<T_SessionClientAssociation>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_SessionEventsClient"))
            {
                DbContext.T_SessionEventsClients = new FilteredDbSet<T_SessionEventsClient>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			if (user.CanView("Document"))
                DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => doclist.Contains(d.Id));
            else
                DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => d.Id == 0);
		}
        public void ApplyMainSecurity()
        {
        }
        public void ApplyUserBasedSecurity()
        {
            if (string.IsNullOrEmpty(user.Name))
                return;
            UserBasedSecurityContext UBS = new UserBasedSecurityContext();
			if (UBS.UserBasedSecurities.Count() == 0) return;
			string mainEntity = UBS.UserBasedSecurities.FirstOrDefault(p => p.IsMainEntity).EntityName;
			List<long?> doclist = new List<long?>();
		Expression<Func<FileDocument, bool>> predicateFileDocument = n => false;
		bool flagFileDocument = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "FileDocument").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("FileDocument"))
					break;
	}
	if(flagFileDocument)
		DbContext.FileDocuments = new FilteredDbSet<FileDocument>(DbContext, predicateFileDocument);
		Expression<Func<T_Client, bool>> predicateT_Client = n => false;
		bool flagT_Client = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Client").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Client"))
					break;
	}
	if(flagT_Client)
		DbContext.T_Clients = new FilteredDbSet<T_Client>(DbContext, predicateT_Client);
		Expression<Func<T_LearningCenter, bool>> predicateT_LearningCenter = n => false;
		bool flagT_LearningCenter = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_LearningCenter").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_LearningCenter"))
					break;
	}
	if(flagT_LearningCenter)
		DbContext.T_LearningCenters = new FilteredDbSet<T_LearningCenter>(DbContext, predicateT_LearningCenter);
		Expression<Func<T_Session, bool>> predicateT_Session = n => false;
		bool flagT_Session = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Session").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Session"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_LearningCenter" && mainEntity != "T_Session")
        {
				var list = DbContext.T_LearningCenters.Select(p => p.Id).ToList();
				//DbContext.T_Sessions = new FilteredDbSet<T_Session>(DbContext, d => (d.t_sessionlearningcenterassociation!=null && list.Contains(d.T_SessionLearningCenterAssociationID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Session = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Session = predicateT_Session.Or(d =>  (d.t_sessionlearningcenterassociation!=null && list.Contains(d.T_SessionLearningCenterAssociationID.Value)) );
					 else
					 {
						predicateT_Session = (d =>  (d.t_sessionlearningcenterassociation!=null && list.Contains(d.T_SessionLearningCenterAssociationID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_TimeSlots" && mainEntity != "T_Session")
        {
				var list = DbContext.T_TimeSlotss.Select(p => p.Id).ToList();
				//DbContext.T_Sessions = new FilteredDbSet<T_Session>(DbContext, d => (d.t_sessiontimeslotassociaton!=null && list.Contains(d.T_SessionTimeSlotAssociatonID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Session = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Session = predicateT_Session.Or(d =>  (d.t_sessiontimeslotassociaton!=null && list.Contains(d.T_SessionTimeSlotAssociatonID.Value)) );
					 else
					 {
						predicateT_Session = (d =>  (d.t_sessiontimeslotassociaton!=null && list.Contains(d.T_SessionTimeSlotAssociatonID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Schedule" && mainEntity != "T_Session")
        {
				var list = DbContext.T_Schedules.Select(p => p.Id).ToList();
				//DbContext.T_Sessions = new FilteredDbSet<T_Session>(DbContext, d => (d.schedulesession!=null && list.Contains(d.ScheduleSessionID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Session = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Session = predicateT_Session.Or(d =>  (d.schedulesession!=null && list.Contains(d.ScheduleSessionID.Value)) );
					 else
					 {
						predicateT_Session = (d =>  (d.schedulesession!=null && list.Contains(d.ScheduleSessionID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_Session)
		DbContext.T_Sessions = new FilteredDbSet<T_Session>(DbContext, predicateT_Session);
		Expression<Func<T_TimeSlots, bool>> predicateT_TimeSlots = n => false;
		bool flagT_TimeSlots = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_TimeSlots").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_TimeSlots"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_LearningCenter" && mainEntity != "T_TimeSlots")
        {
				var list = DbContext.T_LearningCenters.Select(p => p.Id).ToList();
				//DbContext.T_TimeSlotss = new FilteredDbSet<T_TimeSlots>(DbContext, d => (d.t_learningcentertimeslotsassociation!=null && list.Contains(d.T_LearningCenterTimeSlotsAssociationID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_TimeSlots = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_TimeSlots = predicateT_TimeSlots.Or(d =>  (d.t_learningcentertimeslotsassociation!=null && list.Contains(d.T_LearningCenterTimeSlotsAssociationID.Value)) );
					 else
					 {
						predicateT_TimeSlots = (d =>  (d.t_learningcentertimeslotsassociation!=null && list.Contains(d.T_LearningCenterTimeSlotsAssociationID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_TimeSlots)
		DbContext.T_TimeSlotss = new FilteredDbSet<T_TimeSlots>(DbContext, predicateT_TimeSlots);
		Expression<Func<T_SessionEvents, bool>> predicateT_SessionEvents = n => false;
		bool flagT_SessionEvents = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_SessionEvents").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_SessionEvents"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_LearningCenter" && mainEntity != "T_SessionEvents")
        {
				var list = DbContext.T_LearningCenters.Select(p => p.Id).ToList();
				//DbContext.T_SessionEventss = new FilteredDbSet<T_SessionEvents>(DbContext, d => (d.t_sessioneventslearningcenter!=null && list.Contains(d.T_SessionEventsLearningCenterID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionEvents = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionEvents = predicateT_SessionEvents.Or(d =>  (d.t_sessioneventslearningcenter!=null && list.Contains(d.T_SessionEventsLearningCenterID.Value)) );
					 else
					 {
						predicateT_SessionEvents = (d =>  (d.t_sessioneventslearningcenter!=null && list.Contains(d.T_SessionEventsLearningCenterID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Schedule" && mainEntity != "T_SessionEvents")
        {
				var list = DbContext.T_Schedules.Select(p => p.Id).ToList();
				//DbContext.T_SessionEventss = new FilteredDbSet<T_SessionEvents>(DbContext, d => (d.schedule!=null && list.Contains(d.ScheduleID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionEvents = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionEvents = predicateT_SessionEvents.Or(d =>  (d.schedule!=null && list.Contains(d.ScheduleID.Value)) );
					 else
					 {
						predicateT_SessionEvents = (d =>  (d.schedule!=null && list.Contains(d.ScheduleID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_TimeSlots" && mainEntity != "T_SessionEvents")
        {
				var list = DbContext.T_TimeSlotss.Select(p => p.Id).ToList();
				//DbContext.T_SessionEventss = new FilteredDbSet<T_SessionEvents>(DbContext, d => (d.t_sessioneventstimeslots!=null && list.Contains(d.T_SessionEventsTimeSlotsID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionEvents = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionEvents = predicateT_SessionEvents.Or(d =>  (d.t_sessioneventstimeslots!=null && list.Contains(d.T_SessionEventsTimeSlotsID.Value)) );
					 else
					 {
						predicateT_SessionEvents = (d =>  (d.t_sessioneventstimeslots!=null && list.Contains(d.T_SessionEventsTimeSlotsID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_SessionEvents)
		DbContext.T_SessionEventss = new FilteredDbSet<T_SessionEvents>(DbContext, predicateT_SessionEvents);
		Expression<Func<T_SessionClientAssociation, bool>> predicateT_SessionClientAssociation = n => false;
		bool flagT_SessionClientAssociation = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_SessionClientAssociation").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_SessionClientAssociation"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Client" && mainEntity != "T_SessionClientAssociation")
        {
				var list = DbContext.T_Clients.Select(p => p.Id).ToList();
				//DbContext.T_SessionClientAssociations = new FilteredDbSet<T_SessionClientAssociation>(DbContext, d => (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionClientAssociation = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionClientAssociation = predicateT_SessionClientAssociation.Or(d =>  (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
					 else
					 {
						predicateT_SessionClientAssociation = (d =>  (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Session" && mainEntity != "T_SessionClientAssociation")
        {
				var list = DbContext.T_Sessions.Select(p => p.Id).ToList();
				//DbContext.T_SessionClientAssociations = new FilteredDbSet<T_SessionClientAssociation>(DbContext, d => (d.t_session!=null && list.Contains(d.T_SessionID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionClientAssociation = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionClientAssociation = predicateT_SessionClientAssociation.Or(d =>  (d.t_session!=null && list.Contains(d.T_SessionID.Value)) );
					 else
					 {
						predicateT_SessionClientAssociation = (d =>  (d.t_session!=null && list.Contains(d.T_SessionID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_SessionClientAssociation)
		DbContext.T_SessionClientAssociations = new FilteredDbSet<T_SessionClientAssociation>(DbContext, predicateT_SessionClientAssociation);
		Expression<Func<T_SessionEventsClient, bool>> predicateT_SessionEventsClient = n => false;
		bool flagT_SessionEventsClient = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_SessionEventsClient").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_SessionEventsClient"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Client" && mainEntity != "T_SessionEventsClient")
        {
				var list = DbContext.T_Clients.Select(p => p.Id).ToList();
				//DbContext.T_SessionEventsClients = new FilteredDbSet<T_SessionEventsClient>(DbContext, d => (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionEventsClient = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionEventsClient = predicateT_SessionEventsClient.Or(d =>  (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
					 else
					 {
						predicateT_SessionEventsClient = (d =>  (d.t_client!=null && list.Contains(d.T_ClientID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_SessionEvents" && mainEntity != "T_SessionEventsClient")
        {
				var list = DbContext.T_SessionEventss.Select(p => p.Id).ToList();
				//DbContext.T_SessionEventsClients = new FilteredDbSet<T_SessionEventsClient>(DbContext, d => (d.t_sessionevents!=null && list.Contains(d.T_SessionEventsID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_SessionEventsClient = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_SessionEventsClient = predicateT_SessionEventsClient.Or(d =>  (d.t_sessionevents!=null && list.Contains(d.T_SessionEventsID.Value)) );
					 else
					 {
						predicateT_SessionEventsClient = (d =>  (d.t_sessionevents!=null && list.Contains(d.T_SessionEventsID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_SessionEventsClient)
		DbContext.T_SessionEventsClients = new FilteredDbSet<T_SessionEventsClient>(DbContext, predicateT_SessionEventsClient);
			//document security
			if(UBS.UserBasedSecurities.Count()>0)
			{
				
				DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => doclist.Contains(d.Id));
			}
        }
    }
	//Add Dynamic Role
	public class AddDynamicRoles
    {
        private static string GetDisplayValueForAssociation(string EntityName, string id)
        {
            try
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                MethodInfo mc = controller.GetMethod("GetDisplayValue");
                object[] MethodParams = new object[] { id };
                var obj = mc.Invoke(objController, MethodParams);
                return Convert.ToString(obj == null ? "" : obj);
            }
            catch { return id; }
        }
        private bool CheckCondition(object obj1, string EntityName, string PropName, string value)
        {
			if (ModelReflector.Entities == null) return false;
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            if (EntityInfo != null)
            {
                var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
                if (PropInfo != null)
                {
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                    string DataType = PropInfo.DataType;
                    PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == PropName);
                    object PropValue = Property.GetValue(obj1, null);
                    if (AssociationInfo != null)
                    {
                        if (PropValue != null)
                            PropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(PropValue));
                        DataType = "Association";
                    }
                    return ValidateValueAgainstRule(PropValue, DataType, "=", value, Property);
                }
            }
            return false;
        }
        private static bool ValidateValueAgainstRule(object PropValue, string DataType, string condition, string value, PropertyInfo property)
        {
            if (PropValue == null) return false;
            bool result = false;
            Type targetType = property.PropertyType;
            if (property.PropertyType.IsGenericType)
                targetType = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
            if (DataType == "Association")
            {
                targetType = typeof(System.String);
                PropValue = Convert.ToString(PropValue).Trim();
            }
            dynamic value1 = Convert.ChangeType(PropValue, targetType);
            dynamic value2 = Convert.ChangeType(value.Trim(), targetType);
            switch (condition)
            {
                case "=": if (value1 == value2) return true; break;
                case ">": if (value1 > value2) return true; break;
                case "<": if (value1 < value2) return true; break;
                case "<=": if (value1 <= value2) return true; break;
                case ">=": if (value1 >= value2) return true; break;
                case "!=": if (value1 != value2) return true; break;
                case "Contains": if ((Convert.ToString(value1)).Contains(value2.ToString())) return true; break;
            }
            return result;
        }
        public string[] AddRolesDynamic(string[] roles, string userid)
        {
            return roles;
        }
    }
	//End Dynamic Role
	 //Check Owner level permission
    public class CheckPermissionForOwner
    {
        private bool CheckUserCondition(IUser user, Object original)
        {
            ApplicationDbContext userdb = new ApplicationDbContext();
            var userObj = userdb.Users.FirstOrDefault(p => p.UserName == user.Name);
            if (userObj != null && original.ToString() == userObj.Id)
                return false;
            return true;
        }
        private object getObject(IUser user, string EntityName, Object obj, bool FromContext, string propName)
        {
            Object original;
            propName = propName.Trim();
			try{
				if (FromContext)
				{
					obj = (System.Data.Entity.Infrastructure.DbPropertyValues)obj;
					original = ((System.Data.Entity.Infrastructure.DbPropertyValues)obj)[propName + "ID"];
				}
				else
				{
					PropertyInfo[] properties = (obj.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
					var Property = properties.FirstOrDefault(p => p.Name == propName + "ID");
					original = Property.GetValue(obj, null);
				}
				return original;
			}
			catch{return null;}
        }
		public bool CheckLockCondition(string EntityName, Object obj, IUser user, bool FromContext)
        {	
	    try{	
            ApplicationContext db = new ApplicationContext(user);
            bool result = false;
            var type = obj.GetType();
            var assolist = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Associations.ToList();
            PropertyInfo[] properties = type.GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            foreach(var asso in assolist)
            {
		if (asso.Target == null || asso.Target == "IdentityUser") continue;
                var BR = user.businessrules.Where(p => p.EntityName == asso.Target).ToList();
                if (BR.Count() == 0) continue;
				object value = null;
				if (!FromContext)
                    value = type.GetProperty(asso.Name.ToLower()).GetValue(obj, null);
                else
                {
                    obj = (System.Data.Entity.Infrastructure.DbPropertyValues)obj;
                    var original = ((System.Data.Entity.Infrastructure.DbPropertyValues)obj)[asso.Name + "ID"];
                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + asso.Target + "Controller");
                    object objController = Activator.CreateInstance(controller, null);
                    MethodInfo mc = controller.GetMethod("GetRecordById");
                    object[] MethodParams = new object[] { Convert.ToString(original) };
                    value = mc.Invoke(objController, MethodParams);
                }
 				if (value != null)
                {
					var ResultOfBusinessRules = db.LockEntityRule(value, BR, asso.Target);
					if (ResultOfBusinessRules != null && ResultOfBusinessRules.Keys.Select(p => p.TypeNo).Contains(11))
					{
					    return true;
					}
				}
            }
            return result;
}catch{return false;}
        }
        public bool CheckOwnerPermission(string EntityName, Object obj, IUser user, bool FromContext)
        {
            var result = true;
            ApplicationContext db = new ApplicationContext(user);
            var OwnerAssociationName = user.OwnerAssociation(EntityName);
            return result;
        }
    }
    //End Owner level permission
}
