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
			if (user.IsAdmin() || string.IsNullOrEmpty(user.Name))
                return;
            List<long?> doclist = new List<long?>();
						
            if (!user.CanView("FileDocument"))
            {
                DbContext.FileDocuments = new FilteredDbSet<FileDocument>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_School"))
            {
                DbContext.T_Schools = new FilteredDbSet<T_School>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Student"))
            {
                DbContext.T_Students = new FilteredDbSet<T_Student>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Department"))
            {
                DbContext.T_Departments = new FilteredDbSet<T_Department>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_StudentPerformance"))
            {
                DbContext.T_StudentPerformances = new FilteredDbSet<T_StudentPerformance>(DbContext, d => d.Id == 0);
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
            if (user.IsAdmin() || string.IsNullOrEmpty(user.Name))
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
		Expression<Func<T_School, bool>> predicateT_School = n => false;
		bool flagT_School = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_School").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_School"))
					break;
	}
	if(flagT_School)
		DbContext.T_Schools = new FilteredDbSet<T_School>(DbContext, predicateT_School);
		Expression<Func<T_Student, bool>> predicateT_Student = n => false;
		bool flagT_Student = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Student").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Student"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_School" && mainEntity != "T_Student")
        {
				var list = DbContext.T_Schools.Select(p => p.Id).ToList();
				//DbContext.T_Students = new FilteredDbSet<T_Student>(DbContext, d => (d.t_schoolname!=null && list.Contains(d.T_SchoolNameID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Student = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Student = predicateT_Student.Or(d =>  (d.t_schoolname!=null && list.Contains(d.T_SchoolNameID.Value)) );
					 else
					 {
						predicateT_Student = (d =>  (d.t_schoolname!=null && list.Contains(d.T_SchoolNameID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Department" && mainEntity != "T_Student")
        {
				var list = DbContext.T_Departments.Select(p => p.Id).ToList();
				//DbContext.T_Students = new FilteredDbSet<T_Student>(DbContext, d => (d.t_departmentcode!=null && list.Contains(d.T_DepartmentCodeID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Student = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Student = predicateT_Student.Or(d =>  (d.t_departmentcode!=null && list.Contains(d.T_DepartmentCodeID.Value)) );
					 else
					 {
						predicateT_Student = (d =>  (d.t_departmentcode!=null && list.Contains(d.T_DepartmentCodeID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_Student)
		DbContext.T_Students = new FilteredDbSet<T_Student>(DbContext, predicateT_Student);
		Expression<Func<T_Department, bool>> predicateT_Department = n => false;
		bool flagT_Department = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Department").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Department"))
					break;
	}
	if(flagT_Department)
		DbContext.T_Departments = new FilteredDbSet<T_Department>(DbContext, predicateT_Department);
		Expression<Func<T_StudentPerformance, bool>> predicateT_StudentPerformance = n => false;
		bool flagT_StudentPerformance = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_StudentPerformance").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_StudentPerformance"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Student" && mainEntity != "T_StudentPerformance")
        {
				var list = DbContext.T_Students.Select(p => p.Id).ToList();
				//DbContext.T_StudentPerformances = new FilteredDbSet<T_StudentPerformance>(DbContext, d => (d.t_studentcode!=null && list.Contains(d.T_StudentCodeID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_StudentPerformance = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_StudentPerformance = predicateT_StudentPerformance.Or(d =>  (d.t_studentcode!=null && list.Contains(d.T_StudentCodeID.Value)) );
					 else
					 {
						predicateT_StudentPerformance = (d =>  (d.t_studentcode!=null && list.Contains(d.T_StudentCodeID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_StudentPerformance)
		DbContext.T_StudentPerformances = new FilteredDbSet<T_StudentPerformance>(DbContext, predicateT_StudentPerformance);
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
