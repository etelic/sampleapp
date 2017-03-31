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
			
            if (!user.CanView("T_Employee"))
            {
                DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => d.Id == 0);
            }
            else
            {
				doclist.AddRange(DbContext.T_Employees.Select(p => p.T_Picture).ToList());
            }
			
            if (!user.CanView("T_Country"))
            {
                DbContext.T_Countrys = new FilteredDbSet<T_Country>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_State"))
            {
                DbContext.T_States = new FilteredDbSet<T_State>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_City"))
            {
                DbContext.T_Citys = new FilteredDbSet<T_City>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Address"))
            {
                DbContext.T_Addresss = new FilteredDbSet<T_Address>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Employeetype"))
            {
                DbContext.T_Employeetypes = new FilteredDbSet<T_Employeetype>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Employeestatus"))
            {
                DbContext.T_Employeestatuss = new FilteredDbSet<T_Employeestatus>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_Organization"))
            {
                DbContext.T_Organizations = new FilteredDbSet<T_Organization>(DbContext, d => d.Id == 0);
            }
            else
            {
            }
			
            if (!user.CanView("T_EmployeeOrganizationAssociation"))
            {
                DbContext.T_EmployeeOrganizationAssociations = new FilteredDbSet<T_EmployeeOrganizationAssociation>(DbContext, d => d.Id == 0);
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
				if (string.IsNullOrEmpty(user.Name))
                    return;	
				List<long?> doclist = new List<long?>();			
            var T_Organization_T_EmployeeOrganizationAssociationlist = DbContext.T_EmployeeOrganizationAssociations.Where(t => t.t_employee.t_employeeuserlogin.UserName == user.Name ).Select(p=>p.T_OrganizationID).ToList();
			if(T_Organization_T_EmployeeOrganizationAssociationlist.Count() > 0)
            {
                ApplicationDbContext adb = new ApplicationDbContext();
                var extra = adb.MultiTenantExtraAccess.Where(p => p.T_User == user.Name).Select(p=>p.T_MainEntityID).ToList();
                T_Organization_T_EmployeeOrganizationAssociationlist = T_Organization_T_EmployeeOrganizationAssociationlist.Union(extra).Distinct().ToList();
                var selectedlist = user.MultiTenantLoginSelected.Where(p=>p.T_MainEntity != -1).Select(p=>p.T_MainEntity).ToList();
                if (selectedlist.Count()>0)
                T_Organization_T_EmployeeOrganizationAssociationlist = T_Organization_T_EmployeeOrganizationAssociationlist.Intersect(selectedlist).ToList();
            }
            DbContext.T_Organizations = new FilteredDbSet<T_Organization>(DbContext, d => T_Organization_T_EmployeeOrganizationAssociationlist.Contains(d.Id));
            var T_Employee_T_EmployeeOrganizationAssociation_T_Organizationlist = DbContext.T_Organizations.Select(p => p.Id).ToList();
            var T_Employee_T_EmployeeOrganizationAssociationlist = DbContext.T_EmployeeOrganizationAssociations.Where(d => T_Employee_T_EmployeeOrganizationAssociation_T_Organizationlist.Contains(d.T_OrganizationID.Value)).Select(p => p.T_EmployeeID).ToList();
			DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => T_Employee_T_EmployeeOrganizationAssociationlist.Contains(d.Id));
		//document security
		doclist.AddRange(DbContext.T_Employees.Select(p => p.T_Picture).ToList());

		DbContext.Documents = new FilteredDbSet<Document>(DbContext, d => doclist.Contains(d.Id));
        }
        public void ApplyUserBasedSecurity()
        {
            if (string.IsNullOrEmpty(user.Name))
                return;
            UserBasedSecurityContext UBS = new UserBasedSecurityContext();
			if (UBS.UserBasedSecurities.Count() == 0) return;
			string mainEntity = UBS.UserBasedSecurities.FirstOrDefault(p => p.IsMainEntity).EntityName;
			List<long?> doclist = new List<long?>();
		Expression<Func<T_Employee, bool>> predicateT_Employee = n => false;
		bool flagT_Employee = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Employee").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Employee"))
					break;
		if (_ubs.IsMainEntity)
        {
			//DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => (d.t_employeeuserlogin!=null && d.t_employeeuserlogin.UserName == user.Name ) );
			predicateT_Employee = predicateT_Employee.Or(d =>  (d.t_employeeuserlogin!=null && d.t_employeeuserlogin.UserName == user.Name ) );
			flagT_Employee = true;
		}
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Employeetype" && mainEntity != "T_Employee")
        {
				var list = DbContext.T_Employeetypes.Select(p => p.Id).ToList();
				//DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => (d.t_associatedemployeetype!=null && list.Contains(d.T_AssociatedEmployeeTypeID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Employee = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Employee = predicateT_Employee.Or(d =>  (d.t_associatedemployeetype!=null && list.Contains(d.T_AssociatedEmployeeTypeID.Value)) );
					 else
					 {
						predicateT_Employee = (d =>  (d.t_associatedemployeetype!=null && list.Contains(d.T_AssociatedEmployeeTypeID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Employeestatus" && mainEntity != "T_Employee")
        {
				var list = DbContext.T_Employeestatuss.Select(p => p.Id).ToList();
				//DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => (d.t_associatedemployeestatus!=null && list.Contains(d.T_AssociatedEmployeeStatusID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Employee = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Employee = predicateT_Employee.Or(d =>  (d.t_associatedemployeestatus!=null && list.Contains(d.T_AssociatedEmployeeStatusID.Value)) );
					 else
					 {
						predicateT_Employee = (d =>  (d.t_associatedemployeestatus!=null && list.Contains(d.T_AssociatedEmployeeStatusID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Address" && mainEntity != "T_Employee")
        {
				var list = DbContext.T_Addresss.Select(p => p.Id).ToList();
				//DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, d => (d.t_employeeaddress!=null && list.Contains(d.T_EmployeeAddressID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Employee = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Employee = predicateT_Employee.Or(d =>  (d.t_employeeaddress!=null && list.Contains(d.T_EmployeeAddressID.Value)) );
					 else
					 {
						predicateT_Employee = (d =>  (d.t_employeeaddress!=null && list.Contains(d.T_EmployeeAddressID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_Employee)
		DbContext.T_Employees = new FilteredDbSet<T_Employee>(DbContext, predicateT_Employee);
		Expression<Func<T_EmployeeOrganizationAssociation, bool>> predicateT_EmployeeOrganizationAssociation = n => false;
		bool flagT_EmployeeOrganizationAssociation = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_EmployeeOrganizationAssociation").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_EmployeeOrganizationAssociation"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Employee" && mainEntity != "T_EmployeeOrganizationAssociation")
        {
				var list = DbContext.T_Employees.Select(p => p.Id).ToList();
				//DbContext.T_EmployeeOrganizationAssociations = new FilteredDbSet<T_EmployeeOrganizationAssociation>(DbContext, d => (d.t_employee!=null && list.Contains(d.T_EmployeeID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_EmployeeOrganizationAssociation = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_EmployeeOrganizationAssociation = predicateT_EmployeeOrganizationAssociation.Or(d =>  (d.t_employee!=null && list.Contains(d.T_EmployeeID.Value)) );
					 else
					 {
						predicateT_EmployeeOrganizationAssociation = (d =>  (d.t_employee!=null && list.Contains(d.T_EmployeeID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Organization" && mainEntity != "T_EmployeeOrganizationAssociation")
        {
				var list = DbContext.T_Organizations.Select(p => p.Id).ToList();
				//DbContext.T_EmployeeOrganizationAssociations = new FilteredDbSet<T_EmployeeOrganizationAssociation>(DbContext, d => (d.t_organization!=null && list.Contains(d.T_OrganizationID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_EmployeeOrganizationAssociation = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_EmployeeOrganizationAssociation = predicateT_EmployeeOrganizationAssociation.Or(d =>  (d.t_organization!=null && list.Contains(d.T_OrganizationID.Value)) );
					 else
					 {
						predicateT_EmployeeOrganizationAssociation = (d =>  (d.t_organization!=null && list.Contains(d.T_OrganizationID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_EmployeeOrganizationAssociation)
		DbContext.T_EmployeeOrganizationAssociations = new FilteredDbSet<T_EmployeeOrganizationAssociation>(DbContext, predicateT_EmployeeOrganizationAssociation);
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
		Expression<Func<T_Country, bool>> predicateT_Country = n => false;
		bool flagT_Country = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Country").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Country"))
					break;
	}
	if(flagT_Country)
		DbContext.T_Countrys = new FilteredDbSet<T_Country>(DbContext, predicateT_Country);
		Expression<Func<T_State, bool>> predicateT_State = n => false;
		bool flagT_State = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_State").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_State"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Country" && mainEntity != "T_State")
        {
				var list = DbContext.T_Countrys.Select(p => p.Id).ToList();
				//DbContext.T_States = new FilteredDbSet<T_State>(DbContext, d => (d.t_statecountry!=null && list.Contains(d.T_StateCountryID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_State = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_State = predicateT_State.Or(d =>  (d.t_statecountry!=null && list.Contains(d.T_StateCountryID.Value)) );
					 else
					 {
						predicateT_State = (d =>  (d.t_statecountry!=null && list.Contains(d.T_StateCountryID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_State)
		DbContext.T_States = new FilteredDbSet<T_State>(DbContext, predicateT_State);
		Expression<Func<T_City, bool>> predicateT_City = n => false;
		bool flagT_City = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_City").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_City"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Country" && mainEntity != "T_City")
        {
				var list = DbContext.T_Countrys.Select(p => p.Id).ToList();
				//DbContext.T_Citys = new FilteredDbSet<T_City>(DbContext, d => (d.t_citycountry!=null && list.Contains(d.T_CityCountryID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_City = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_City = predicateT_City.Or(d =>  (d.t_citycountry!=null && list.Contains(d.T_CityCountryID.Value)) );
					 else
					 {
						predicateT_City = (d =>  (d.t_citycountry!=null && list.Contains(d.T_CityCountryID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_State" && mainEntity != "T_City")
        {
				var list = DbContext.T_States.Select(p => p.Id).ToList();
				//DbContext.T_Citys = new FilteredDbSet<T_City>(DbContext, d => (d.t_citystate!=null && list.Contains(d.T_CityStateID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_City = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_City = predicateT_City.Or(d =>  (d.t_citystate!=null && list.Contains(d.T_CityStateID.Value)) );
					 else
					 {
						predicateT_City = (d =>  (d.t_citystate!=null && list.Contains(d.T_CityStateID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_City)
		DbContext.T_Citys = new FilteredDbSet<T_City>(DbContext, predicateT_City);
		Expression<Func<T_Address, bool>> predicateT_Address = n => false;
		bool flagT_Address = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Address").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Address"))
					break;
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_Country" && mainEntity != "T_Address")
        {
				var list = DbContext.T_Countrys.Select(p => p.Id).ToList();
				//DbContext.T_Addresss = new FilteredDbSet<T_Address>(DbContext, d => (d.t_addresscountry!=null && list.Contains(d.T_AddressCountryID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Address = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Address = predicateT_Address.Or(d =>  (d.t_addresscountry!=null && list.Contains(d.T_AddressCountryID.Value)) );
					 else
					 {
						predicateT_Address = (d =>  (d.t_addresscountry!=null && list.Contains(d.T_AddressCountryID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_State" && mainEntity != "T_Address")
        {
				var list = DbContext.T_States.Select(p => p.Id).ToList();
				//DbContext.T_Addresss = new FilteredDbSet<T_Address>(DbContext, d => (d.t_addressstate!=null && list.Contains(d.T_AddressStateID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Address = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Address = predicateT_Address.Or(d =>  (d.t_addressstate!=null && list.Contains(d.T_AddressStateID.Value)) );
					 else
					 {
						predicateT_Address = (d =>  (d.t_addressstate!=null && list.Contains(d.T_AddressStateID.Value)) );
						break;
					 }
				 }
        }
		if (!_ubs.IsMainEntity && _ubs.TargetEntityName == "T_City" && mainEntity != "T_Address")
        {
				var list = DbContext.T_Citys.Select(p => p.Id).ToList();
				//DbContext.T_Addresss = new FilteredDbSet<T_Address>(DbContext, d => (d.t_addresscity!=null && list.Contains(d.T_AddressCityID.Value)) );
				 if(list.Count()>0)
				 {
					 flagT_Address = true;
					 if (_ubs.TargetEntityName != mainEntity)
						predicateT_Address = predicateT_Address.Or(d =>  (d.t_addresscity!=null && list.Contains(d.T_AddressCityID.Value)) );
					 else
					 {
						predicateT_Address = (d =>  (d.t_addresscity!=null && list.Contains(d.T_AddressCityID.Value)) );
						break;
					 }
				 }
        }
	}
	if(flagT_Address)
		DbContext.T_Addresss = new FilteredDbSet<T_Address>(DbContext, predicateT_Address);
		Expression<Func<T_Employeetype, bool>> predicateT_Employeetype = n => false;
		bool flagT_Employeetype = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Employeetype").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Employeetype"))
					break;
	}
	if(flagT_Employeetype)
		DbContext.T_Employeetypes = new FilteredDbSet<T_Employeetype>(DbContext, predicateT_Employeetype);
		Expression<Func<T_Employeestatus, bool>> predicateT_Employeestatus = n => false;
		bool flagT_Employeestatus = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Employeestatus").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Employeestatus"))
					break;
	}
	if(flagT_Employeestatus)
		DbContext.T_Employeestatuss = new FilteredDbSet<T_Employeestatus>(DbContext, predicateT_Employeestatus);
		Expression<Func<T_Organization, bool>> predicateT_Organization = n => false;
		bool flagT_Organization = false;
		foreach (var _ubs in UBS.UserBasedSecurities.Where(p => p.EntityName == "T_Organization").OrderByDescending(p => p.IsMainEntity).ThenByDescending(p=>p.Id))
		{
				if (_ubs.RolesToIgnore != null && user.IsInRole(_ubs.RolesToIgnore.Split(",".ToCharArray())))
                        continue;
				if(!user.CanView("T_Organization"))
					break;
	}
	if(flagT_Organization)
		DbContext.T_Organizations = new FilteredDbSet<T_Organization>(DbContext, predicateT_Organization);
			//document security
			if(UBS.UserBasedSecurities.Count()>0)
			{
				doclist.AddRange(DbContext.T_Employees.Select(p => p.T_Picture).ToList());

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
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var dynamicRoles = db.DynamicRoleMappings.ToList();
            foreach (var adr in dynamicRoles)
            {
                if (roles.Contains(adr.RoleId)) continue;
				if (adr.EntityName == "T_Employee")
                {
                    if (adr.UserRelation == "T_EmployeeUserLoginID")
                    {
                        var _Object = db.T_Employees.FirstOrDefault(p => p.T_EmployeeUserLoginID == userid);
                        if (_Object != null)
                        {
                           if(CheckCondition(_Object,"T_Employee",adr.Condition,adr.Value))
                                roles = (roles ?? Enumerable.Empty<string>()).Concat(Enumerable.Repeat(adr.RoleId, 1)).ToArray();
                        }
                    }
                }
            }
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
if (EntityName == "T_Employee")
{
			 var entresult = true;
                if (OwnerAssociationName.Contains("T_AssociatedEmployeeType"))
                {
                    Object original = getObject(user, EntityName, obj, FromContext, "T_AssociatedEmployeeType");
					if (original != null)
						entresult = entresult && (CheckUserCondition(user, original));
                }
                if (OwnerAssociationName.Contains("T_AssociatedEmployeeStatus"))
                {
                    Object original = getObject(user, EntityName, obj, FromContext, "T_AssociatedEmployeeStatus");
					if (original != null)
						entresult = entresult && (CheckUserCondition(user, original));
                }
                if (OwnerAssociationName.Contains("T_EmployeeUserLogin"))
                {
                    Object original = getObject(user, EntityName, obj, FromContext, "T_EmployeeUserLogin");
					if (original != null)
						entresult = entresult && (CheckUserCondition(user, original));
                }
                if (OwnerAssociationName.Contains("T_EmployeeAddress"))
                {
                    Object original = getObject(user, EntityName, obj, FromContext, "T_EmployeeAddress");
					if (original != null)
						entresult = entresult && (CheckUserCondition(user, original));
                }
 return entresult;
}
 if (EntityName == "T_EmployeeOrganizationAssociation")
{
			var entresult = true;
                if (OwnerAssociationName.Contains("T_Employee"))
                {
					Object original = getObject(user, EntityName, obj, FromContext, "T_Employee");
                    var relationobj = db.T_Employees.Find(original);
                    if (original != null)
                    {
				     if (relationobj != null)
					 {
												foreach (var str in user.OwnerAssociation("T_Employee").Split(",".ToArray()))
                            {
                                var UserName = getObject(user, "T_Employee", relationobj, false, str);
                                if (UserName != null)
                                    entresult = entresult && (CheckUserCondition(user, UserName));
                            }
							//else (CheckUserCondition(user, relationobj.T_EmployeeUserLoginID) );
										}
				  }
                }
                if (OwnerAssociationName.Contains("T_Organization"))
                {
					Object original = getObject(user, EntityName, obj, FromContext, "T_Organization");
                    var relationobj = db.T_Organizations.Find(original);
                    if (original != null)
                    {
				     if (relationobj != null)
					 {
												foreach (var str in user.OwnerAssociation("T_Organization").Split(",".ToArray()))
                            {
                                var UserName = getObject(user, "T_Organization", relationobj, false, str);
                                if (UserName != null)
                                    entresult = entresult && (CheckUserCondition(user, UserName));
                            }
							//else ();
										}
				  }
                }
	return entresult;
}
            return result;
        }
    }
    //End Owner level permission
}
