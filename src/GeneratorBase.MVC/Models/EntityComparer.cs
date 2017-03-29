using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public static class EntityCopy
    {
        public static void CopyValues(object source, object destination, string ColumnMapping, string CodePrefix)
        {
            var mapping = ColumnMapping.Split(",".ToCharArray());
            foreach (var item in mapping)
            {
                var map = item.Split("-".ToCharArray());
                var target = CodePrefix + map[0];
                var src = CodePrefix + map[1];
                var targetProp = destination.GetType().GetProperty(target);
                var sourceProp = source.GetType().GetProperty(src);
                if (sourceProp == null && targetProp == null)
                {
                    targetProp = destination.GetType().GetProperty(target + "ID");
                    sourceProp = source.GetType().GetProperty(src + "ID");
                }
                if (sourceProp != null && targetProp != null)
                    if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(destination, sourceProp.GetValue(
                            source, new object[] { }), new object[] { });
                    }
            }
        }
        public static void CopyValuesForSameObjectType(object source, object destination, string ColumnMapping)
        {
            var mapping = ColumnMapping.Split(",".ToCharArray());
            foreach (var item in mapping)
            {
                var target = item;
                var src = item;
                var targetProp = destination.GetType().GetProperty(target);
                var sourceProp = source.GetType().GetProperty(src);
                if (sourceProp == null && targetProp == null)
                {
                    targetProp = destination.GetType().GetProperty(target + "ID");
                    sourceProp = source.GetType().GetProperty(src + "ID");
                }
                if (sourceProp != null && targetProp != null)
                    if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(destination, sourceProp.GetValue(
                            source, new object[] { }), new object[] { });
                    }
            }
        }
        public static void CopyValuesForSameObjectType(object source, object destination)
        {

            foreach (var item1 in source.GetType().GetProperties())
            {
                var item = item1.Name;
                if (item == "Id") continue;
                var target = item;
                var src = item;
                var targetProp = destination.GetType().GetProperty(target);
                var sourceProp = source.GetType().GetProperty(src);
                if (sourceProp == null && targetProp == null)
                {
                    targetProp = destination.GetType().GetProperty(target + "ID");
                    sourceProp = source.GetType().GetProperty(src + "ID");
                }
                if (sourceProp != null && targetProp != null)
                    if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(destination, sourceProp.GetValue(
                            source, new object[] { }), new object[] { });
                    }
            }
        }
    }
    public static class EntityComparer
    {
        public static IEnumerable<string> EnumeratePropertyDifferences<T>(this T obj1, T obj2, string state, string EntityName)
        {
            PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            List<string> changes = new List<string>();
            string id = Convert.ToString(typeof(T).GetProperty("Id").GetValue(obj2, null));
            string dispValue = Convert.ToString(typeof(T).GetProperty("DisplayValue").GetValue(obj2, null));
            foreach (PropertyInfo pi in properties)
            {
                if (pi.Name == "DisplayValue" || pi.Name == "ConcurrencyKey" || typeof(T).GetProperty(pi.Name).PropertyType.Name == "ICollection`1") continue;
                object value1 = typeof(T).GetProperty(pi.Name).GetValue(obj1, null);
                object value2 = typeof(T).GetProperty(pi.Name).GetValue(obj2, null);
                if (value1 != value2 && (value1 == null || !value1.Equals(value2)))
                {
                    using (var db = new JournalEntryContext())
                    {
                        JournalEntry Je = new JournalEntry();
                        Je.DateTimeOfEntry = DateTime.Now;
                        Je.EntityName = EntityName;
                        Je.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        Je.Type = state;
                        var displayValue = dispValue;
                        Je.RecordInfo = displayValue;
                        Je.PropertyName = pi.Name;
                        var assolist = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Associations.Where(p => p.AssociationProperty == pi.Name).ToList();
                        if (assolist.Count() > 0)
                        {
                            Je.PropertyName = assolist[0].DisplayName;
                            if (value1 != null)
                                Je.OldValue = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value1));
                            if (value2 != null)
                                Je.NewValue = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value2));
                        }
                        else
                        {
                            Je.OldValue = Convert.ToString(value1);
                            Je.NewValue = Convert.ToString(value2);
                        }
                        Je.RecordId = Convert.ToInt64(id);
                        db.JournalEntries.Add(Je);
                        db.SaveChanges();
                    }
                }
            }
            return changes;
        }
        public static string GetDisplayValueForAssociation(string EntityName, string id)
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
        public static string EnumeratePropertyValues<T>(this T obj1, string EntityName, string[] excludeProps)
        {
            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            string result = "";
            var style = "background-color: #CC9999; color: black;";
            foreach (PropertyInfo pi in properties)
            {
                // if (pi.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() > 0) continue;
                if (pi.Name == "Id" || pi.Name == "ConcurrencyKey" || pi.Name == "DisplayValue" || pi.Name == "m_OffSet" || (obj1.GetType()).GetProperty(pi.Name).PropertyType.Name == "ICollection`1") continue;
                if (pi.Name == "WFeMailNotification" || pi.Name.EndsWith("WorkFlowInstanceId")) continue;
                if (excludeProps != null && excludeProps.Contains(pi.Name)) continue;
                var DisplayName = pi.Name;
                var prefix = "";
                object value1 = (obj1.GetType()).GetProperty(pi.Name).GetValue(obj1, null);
                var entity = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0];
                var assolist = entity.Associations.Where(p => p.AssociationProperty == pi.Name).ToList();
                if (assolist.Count() > 0)
                {
                    if (assolist[0].Target == "IdentityUser")
                    {
                        try
                        {
                            ApplicationDbContext usercontext = new ApplicationDbContext();
                            string idvalue = Convert.ToString(value1);
                            var objuser = usercontext.Users.First(p => p.Id == idvalue);
                            if (objuser != null)
                                value1 = objuser.UserName;
                        }
                        catch { continue; }
                    }
                    else
                    {
                        value1 = GetDisplayValueForAssociation(assolist[0].Target, Convert.ToString(value1));
                    }
                    DisplayName = assolist[0].DisplayName;
                }
                else
                {
                    var proplist = entity.Properties;
                    var prop = proplist.FirstOrDefault(p => p.Name == DisplayName);
                    if (prop != null)
                    {
                        if (prop.DataTypeAttribute == "Currency")
                            prefix = "$";
                        if (prop.DataTypeAttribute == "Date")
                            value1 = value1 != null ? ((DateTime)value1).ToShortDateString() : value1;
                    }
                    DisplayName = (prop == null ? DisplayName : prop.DisplayName);
                }


                if (value1 != null && !string.IsNullOrEmpty(value1.ToString()))
                {
                    if (style == "background-color: #ffffff; color: black;")
                        style = "background-color: #eeeeee; color: black;";
                    else style = "background-color: #ffffff; color: black;";
                    result += "<tr style=\"" + style + "\"><td width=200>" + (DisplayName + " </td><td> " + prefix + value1.ToString() + "</td></tr>");
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                var classvalue = " <style>.MailTable {margin: 0px 0px 20px 0px;padding: 0;width: 97%;"
                            + "}.MailTable table { border-collapse: collapse;border-spacing: 0;width: 100%;height: 100%;margin: 0px;padding: 0px;}"
                            + ".MailTable td {vertical-align: middle;border: 1px solid #c1c1c1;border-width: 1px 1px 1px 1px;text-align: left;padding: 5px;font-size: 12px;font-family: Arial;font-weight: normal;color: #333333;}"
                            + ".MailTable td:first-child {font-weight: bold;}</style>";
                return classvalue + " <div class=\"MailTable\">" + "<table>" + result + "</table></div>";
            }
            else
                return result;
        }
    }
    public static class ApplyRule
    {
        public static bool CheckRule<T>(this T obj1, BusinessRule br, string EntityName)
        {
            if (EntityName != br.EntityName) return false;
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            bool result = true;
            var ruleconditionsdb = new ConditionContext().Conditions.Where(p => p.RuleConditionsID == br.Id);
            bool containsCondition = false;
            var ruleactionsdb = new RuleActionContext();
            var ruleactions = ruleactionsdb.RuleActions.Where(p => p.RuleActionID == br.Id && p.AssociatedActionTypeID == 1);

            string lstConditions = "";
            foreach (var condition in ruleconditionsdb)
            {
                containsCondition = true;
                var PropName = condition.PropertyName;
                var ConditionOperator = condition.Operator;
                var ConditionValue = condition.Value;
                object oldPropValue = null;
                bool isPropertyValueChanged = true;

                if (br.AssociatedBusinessRuleTypeID == 4)
                    isPropertyValueChanged = IsPropertyValueChanged(obj1, EntityName, PropName);

                if (PropName == "Id" && ConditionOperator == ">" && ConditionValue == "0")
                {
                    result = true && isPropertyValueChanged;
                }
                else
                {
                    string propDataType = "String";
                    var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
                    if (PropInfo != null)
                        propDataType = PropInfo.DataType;
                    var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                    if (PropName.StartsWith("[") && PropName.EndsWith("]"))
                    {
                        PropName = PropName.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                        if (PropName.Contains("."))
                        {
                            var targetProperties = PropName.Split(".".ToCharArray());
                            if (targetProperties.Length > 1)
                            {
                                AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                                if (AssociationInfo != null)
                                {
                                    var targetEntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                                    PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                                    propDataType = PropInfo.DataType;
                                    var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                                    if (assocInfo != null)
                                        propDataType = "String";
                                    PropName = targetProperties[0];
                                }
                            }
                        }
                    }
                    string targetValue = GetTargetConditionValue(obj1, ConditionValue, EntityName);
                    if (!string.IsNullOrEmpty(targetValue))
                        ConditionValue = targetValue;

                    string DataType = propDataType;
                    PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == PropName);
                    object PropValue = Property.GetValue(obj1, null);
                    oldPropValue = GetOldValueOfEntity(obj1, EntityName, PropName);
                    if (AssociationInfo != null)
                    {
                        if (PropValue != null)
                            PropValue = GetValueForAssociationProperty(AssociationInfo.Target, Convert.ToString(PropValue), PropInfo.Name);
                        //PropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(PropValue));

                        if (ruleactions != null && ruleactions.Count() > 0)
                        {
                            oldPropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(oldPropValue));
                        }

                        DataType = "Association";
                    }
                    if (ruleactions != null && ruleactions.Count() > 0)
                    {
                        bool isRuleValid = ValidateValueAgainstRule(oldPropValue, DataType, ConditionOperator, ConditionValue, Property, propDataType);
                        result = isRuleValid && isPropertyValueChanged;
                    }
                    else
                    {
                        bool isRuleValid = ValidateValueAgainstRule(PropValue, DataType, ConditionOperator, ConditionValue, Property, propDataType);
                        result = isRuleValid && isPropertyValueChanged;
                    }
                    if (br.AssociatedBusinessRuleTypeID == 4 && ConditionOperator == "Changes to anything")
                        result = isPropertyValueChanged;
                }
                lstConditions += Convert.ToString(result) + " " + condition.LogicalConnector + " ";
            }
            result = CheckRuleWithLogicalConnectors(lstConditions);

            if (!containsCondition)
                return false;
            return result;
        }
        public static bool CheckRuleWithLogicalConnectors(string lstConditions)
        {
            if (string.IsNullOrEmpty(lstConditions))
                return true;
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("", typeof(Boolean));
            lstConditions = lstConditions.Trim();
            if (lstConditions.EndsWith("AND"))
                lstConditions = lstConditions.Remove(lstConditions.LastIndexOf("AND"));
            else if (lstConditions.EndsWith("OR"))
                lstConditions = lstConditions.Remove(lstConditions.LastIndexOf("OR"));
            table.Columns[0].Expression = lstConditions;
            System.Data.DataRow r = table.NewRow();
            table.Rows.Add(r);
            return (Boolean)r[0]; ;
        }
        public static string GetTargetConditionValue(object obj1, string targetProperty, string EntityName)
        {
            string result = "";
            ModelReflector.Property PropInfo = null;
            string PropName = "";
            string targetPropertyName = "";
            string propDataType = "String";
            var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
            if (targetProperty.StartsWith("[") && targetProperty.EndsWith("]"))
            {
                //var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                PropName = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (PropName.Contains("."))
                {
                    var targetProperties = PropName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            var targetEntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                            if (assocInfo != null)
                                propDataType = "String";
                            targetPropertyName = targetProperties[0];

                            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                            var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                            object PropValue = Property.GetValue(obj1, null);
                            result = GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                        }
                    }
                }
                else
                {
                    targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                    PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                    object PropValue = Property.GetValue(obj1, null);
                    result = Convert.ToString(PropValue);
                }
            }
            else if (targetProperty.StartsWith("[") && (targetProperty.ToLower().EndsWith("days") || targetProperty.ToLower().EndsWith("months") || targetProperty.ToLower().EndsWith("weeks") || targetProperty.ToLower().EndsWith("years")))
            {
                var propNameArr = targetProperty.Split(" ".ToArray());
                PropName = propNameArr[0].TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (PropName.Contains("."))
                {
                    var targetProperties = PropName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            var targetEntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                            if (assocInfo != null)
                                propDataType = "String";
                            targetPropertyName = targetProperties[0];

                            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                            var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                            object PropValue = Property.GetValue(obj1, null);
                            result = GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                            if (!string.IsNullOrEmpty(result))
                            {
                                DateTime dateValue = Convert.ToDateTime(result);
                                result = dateValue.ToString("MM/dd/yyyy");
                            }
                        }
                    }
                }
                else
                {
                    targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                    PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                    object PropValue = Property.GetValue(obj1, null);
                    DateTime dateValue = Convert.ToDateTime(PropValue);
                    result = dateValue.ToString("MM/dd/yyyy");
                }
                var propTarget = targetProperty.Replace("[", "").Replace("]", "").Trim();
                result = EvaluateDateTime(result, propTarget.Replace(PropName, result));
            }
            return result;
        }
        public static string GetPropertyChangeValue(object obj1, string targetProperty, string EntityName)
        {
            string result = "";
            ModelReflector.Property PropInfo = null;
            string PropName = "";
            string targetPropertyName = "";
            string propDataType = "String";
            if (targetProperty.StartsWith("[") && targetProperty.EndsWith("]"))
            {
                var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                PropName = targetProperty.TrimStart("[".ToArray()).TrimEnd("]".ToArray());
                if (PropName.Contains("."))
                {
                    var targetProperties = PropName.Split(".".ToCharArray());
                    if (targetProperties.Length > 1)
                    {
                        var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == targetProperties[0]);
                        if (AssociationInfo != null)
                        {
                            var targetEntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == AssociationInfo.Target);
                            PropInfo = targetEntityInfo.Properties.FirstOrDefault(p => p.Name == targetProperties[1]);
                            var assocInfo = targetEntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropInfo.Name);
                            if (assocInfo != null)
                                propDataType = "String";
                            targetPropertyName = targetProperties[0];

                            PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                            var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                            object PropValue = Property.GetValue(obj1, null);
                            result = GetValueForAssociationProperty(targetEntityInfo.Name, Convert.ToString(PropValue), PropInfo.Name);
                        }
                    }
                }
                else
                {
                    targetPropertyName = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName).Name;
                    PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                    var Property = properties.FirstOrDefault(p => p.Name == targetPropertyName);
                    object PropValue = Property.GetValue(obj1, null);
                    result = Convert.ToString(PropValue);
                }
            }
            else
            {
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var property = properties.FirstOrDefault(p => p.Name == targetProperty);
                object Value = property.GetValue(obj1, null);
                result = Convert.ToString(Value);
            }
            return result;
        }
        public static bool IsPropertyValueChanged(object newentity, string entityName, string propName)
        {
            //old value
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            MethodInfo mc = controller.GetMethod("GetRecordById");

            PropertyInfo[] newproperties = (newentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var idProperty = newproperties.FirstOrDefault(p => p.Name == "Id");
            object idPropValue = idProperty.GetValue(newentity, null);
            if (Convert.ToInt64(idPropValue) != 0)
            {
                object newValue = GetPropertyChangeValue(newentity, propName, entityName);
                //var newproperty = newproperties.FirstOrDefault(p => p.Name == propName);
                //object newValue = newproperty.GetValue(newentity, null);

                object[] MethodParams = new object[] { Convert.ToString(idPropValue) };
                var oldentity = mc.Invoke(objController, MethodParams);

                object oldValue = GetPropertyChangeValue(oldentity, propName, entityName);
                //PropertyInfo[] oldproperties = (oldentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                //var oldproperty = oldproperties.FirstOrDefault(p => p.Name == propName);
                //object oldValue = oldproperty.GetValue(oldentity, null);

                if (Convert.ToString(oldValue) == Convert.ToString(newValue))
                    return false;
                else
                    return true;
            }
            return true;
        }
        public static object GetOldValueOfEntity(object newentity, string entityName, string propName)
        {
            Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + entityName + "Controller");
            object objController = Activator.CreateInstance(controller, null);
            MethodInfo mc = controller.GetMethod("GetRecordById");

            PropertyInfo[] newproperties = (newentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
            var idProperty = newproperties.FirstOrDefault(p => p.Name == "Id");
            object idPropValue = idProperty.GetValue(newentity, null);
            if (Convert.ToInt64(idPropValue) != 0)
            {
                var newproperty = newproperties.FirstOrDefault(p => p.Name == propName);
                object newValue = newproperty.GetValue(newentity, null);

                object[] MethodParams = new object[] { Convert.ToString(idPropValue) };
                var oldentity = mc.Invoke(objController, MethodParams);
				if (oldentity == null)
                    return null;
                PropertyInfo[] oldproperties = (oldentity.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var oldproperty = oldproperties.FirstOrDefault(p => p.Name == propName);
                object oldValue = oldproperty.GetValue(oldentity, null);

                return oldValue;
            }
            return null;
        }
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
        private static bool ValidateValueAgainstRule(object PropValue, string DataType, string condition, string value, PropertyInfo property, string AssocDataType)
        {
            if (PropValue == null && value.ToLower().Trim() != "null")
                return false;

            if (PropValue != null && string.IsNullOrEmpty(value) && condition == "Changes to anything")
                return false;

            if (value.ToLower().Trim() == "null")
            {
                if (CheckNullCondition(PropValue, value.ToLower().Trim(), condition))
                    return true;
                else
                    return false;
            }
            bool result = false;
            Type targetType = property.PropertyType;
            if (property.PropertyType.IsGenericType)
                targetType = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType;
            if (DataType == "Association")
            {
                if (ValidateValueForAssociation(PropValue, condition, value, AssocDataType))
                    return true;
                else
                    return false;

            }

            if (DataType == "DateTime")
            {
                if (value.Trim().ToLower() == "today")
                    value = Convert.ToString(DateTime.Now.Date);
                else if (value.Trim().ToLower().Contains("days") || value.Trim().ToLower().Contains("weeks")
                    || value.Trim().ToLower().Contains("months") || value.Trim().ToLower().Contains("years"))
                {
                    value = EvaluateDateTime(Convert.ToString(PropValue).Trim(), value.Trim());
                }
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
                case "Regular Expression": if (IsRegularExpressionMatch(value1, value2)) return true; break;
            }
            return result;
        }
        public static bool ValidateValueForAssociation(object PropValue, string condition, string value, string AssocDataType)
        {
            if (PropValue == null && value.ToLower().Trim() != "null")
                return false;
            if (Convert.ToString(PropValue) == "")
                return false;
            bool flag = false;
            var lstconditions = "";
            //Type targetType = typeof(System.String);
            Type targetType = getTypeForAssocProperty(AssocDataType);
            PropValue = Convert.ToString(PropValue).Trim();
            dynamic value1 = Convert.ChangeType(PropValue, targetType);
            dynamic value2 = Convert.ChangeType(value.Trim(), targetType);

            var arrValue = Convert.ToString(value2).Split("OR".ToCharArray());
            foreach (var item in arrValue)
            {
                dynamic itemVal = Convert.ChangeType(item.Trim(), targetType);
                if (!string.IsNullOrEmpty(item))
                {
                    switch (condition)
                    {
                        case "=": if (value1 == itemVal) flag = true; else flag = false; break;
                        case ">": if (value1 > itemVal) flag = true; else flag = false; break;
                        case "<": if (value1 < itemVal) flag = true; else flag = false; break;
                        case "<=": if (value1 <= itemVal) flag = true; else flag = false; break;
                        case ">=": if (value1 >= itemVal) flag = true; else flag = false; break;
                        case "!=": if (value1 != itemVal) flag = true; else flag = false; break;
                        case "Contains": if ((Convert.ToString(value1)).Contains(itemVal.ToString())) return true; break;
                        case "Regular Expression": if (IsRegularExpressionMatch(value1, itemVal)) return true; break;
                    }
                    lstconditions += Convert.ToString(flag) + " OR ";
                }
            }

            if (CheckRuleWithLogicalConnectors(lstconditions))
                return true;
            else
                return false;
        }
        public static Type getTypeForAssocProperty(string AssocDataType)
        {
            if (AssocDataType == "Int32")
                return typeof(System.Int32);
            else if (AssocDataType == "Int64")
                return typeof(System.Int64);
            else if (AssocDataType == "Boolean")
                return typeof(System.Boolean);
            else if (AssocDataType == "Decimal")
                return typeof(System.Decimal);
            else if (AssocDataType == "Double")
                return typeof(System.Double);
            else if (AssocDataType == "DateTime")
                return typeof(System.DateTime);
            return typeof(System.String);
        }
        public static bool IsRegularExpressionMatch(string value, string expression)
        {
            Regex regex = new Regex(expression.Replace("&#63;", "?").Replace("&#644;", ","));
            var match = regex.Match(value);
            return match.Success;
        }
        public static bool CheckNullCondition(object PropValue, string value, string condition)
        {

            if (condition == "=")
            {
                if (PropValue == null || PropValue == "")
                    return true;
                else
                    return false;
            }
            else if (condition == "!=")
            {
                if (PropValue != null && PropValue != "")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static string EvaluateDateTime(string propValue, string conditionValue)
        {
            string targetDate = "";

            var valuearr = conditionValue.Split(' ');
            int count = Convert.ToInt32(valuearr[2]);
            string interval = valuearr[3].ToLower().Trim();
            double noOfDays = count;
            DateTime pValue = new DateTime();
            if (!string.IsNullOrEmpty(propValue))
                pValue = Convert.ToDateTime(propValue);
            DateTime conditionDateValue = DateTime.Now.Date;
            if (valuearr[0].ToLower() != "today")
                if (!string.IsNullOrEmpty(valuearr[0]))
                    conditionDateValue = Convert.ToDateTime(valuearr[0]);
                else
                    conditionDateValue = pValue;


            if (interval == "days")
            {
                if (valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddDays(noOfDays));
                else if (valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.Subtract(TimeSpan.FromDays(noOfDays)));
            }
            else if (interval == "weeks")
            {
                if (valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddDays(noOfDays * 7));
                else if (valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.Subtract(TimeSpan.FromDays(noOfDays * 7)));
            }
            else if (interval == "months")
            {
                if (valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddMonths(Convert.ToInt32(noOfDays)));
                else if (valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.AddMonths(-Convert.ToInt32(noOfDays)));
            }
            else if (interval == "years")
            {
                if (valuearr[1] == "+")
                    targetDate = Convert.ToString(conditionDateValue.AddYears(Convert.ToInt32(noOfDays)));
                else if (valuearr[1] == "-")
                    targetDate = Convert.ToString(conditionDateValue.AddYears(-Convert.ToInt32(noOfDays)));
            }
            return targetDate;
        }
        public static string EvaluateDateForActionInTarget(string finalvalue)
        {
            string targetDate = "";
            string currentDate = DateTime.Now.Date.ToString("MM/dd/yyyy");
            string strFinalValue = Convert.ToString(finalvalue).ToLower();

            if (Convert.ToString(finalvalue).ToLower() == "today")
                targetDate = currentDate;
            else if (Convert.ToString(finalvalue).ToLower().Contains("today"))
            {
                string strDuration = strFinalValue.Substring("today".Length);
                targetDate = ApplyRule.EvaluateDateTime(currentDate, strDuration);
            }
            else
            {
                DateTime dateTimeTemp;
                if (DateTime.TryParseExact(strFinalValue, "hh:mm tt", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out dateTimeTemp))
                {
                    targetDate = strFinalValue;
                }
                else
                {
                    string[] strarr = strFinalValue.Split(' ');
                    string strDuration = strFinalValue.Substring(strarr[0].Length);
                    targetDate = ApplyRule.EvaluateDateTime(strarr[0], strDuration);
                }
            }
            return targetDate;
        }
        private static string GetValueForAssociationProperty(string EntityName, string id, string propName)
        {
            try
            {
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                MethodInfo mc = controller.GetMethod("GetFieldValueByEntityId");
                object[] MethodParams = new object[] { Convert.ToInt64(id), propName };
                var obj = mc.Invoke(objController, MethodParams);

                var entityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                var assocInfo = entityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == propName);

                if (assocInfo != null)
                {
                    EntityName = assocInfo.Target;
                    propName = "DisplayValue";
                    Type controllerAssoc = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                    object objControllerAssoc = Activator.CreateInstance(controllerAssoc, null);
                    MethodInfo mcAssoc = controllerAssoc.GetMethod("GetFieldValueByEntityId");
                    object[] MethodParamsAssoc = new object[] { Convert.ToInt64(obj), propName };
                    obj = mcAssoc.Invoke(objControllerAssoc, MethodParamsAssoc);
                }

                return Convert.ToString(obj == null ? "" : obj);
            }
            catch { return id; }
        }
    }
}