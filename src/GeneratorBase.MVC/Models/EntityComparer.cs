using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                if (pi.Name == "Id" || pi.Name == "ConcurrencyKey" || pi.Name == "DisplayValue" || (obj1.GetType()).GetProperty(pi.Name).PropertyType.Name == "ICollection`1") continue;
                if (pi.Name == "WFeMailNotification" || pi.Name.EndsWith("WorkFlowInstanceId")) continue;
                if (excludeProps != null && excludeProps.Contains(pi.Name)) continue;
                var DisplayName = pi.Name;
                var prefix = "";
                object value1 = (obj1.GetType()).GetProperty(pi.Name).GetValue(obj1, null);
                var assolist = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Associations.Where(p => p.AssociationProperty == pi.Name).ToList();
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
                    var prop = ModelReflector.Entities.Where(p => p.Name == EntityName).ToList()[0].Properties.FirstOrDefault(p => p.Name == DisplayName);
                    if (prop.DataTypeAttribute == "Currency")
                        prefix = "$";
                    if (prop.DataTypeAttribute == "Date")
                        value1 = value1 != null ? ((DateTime)value1).ToShortDateString():value1;

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
            var ruleconditionsdb = new ConditionContext();
            bool containsCondition = false;
            foreach (var condition in ruleconditionsdb.Conditions.Where(p => p.RuleConditionsID == br.Id))
            {
                containsCondition = true;
                var PropName = condition.PropertyName;
                var ConditionOperator = condition.Operator;
                var ConditionValue = condition.Value;
                var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
                var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                string DataType = PropInfo.DataType;
                //PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                PropertyInfo[] properties = (obj1.GetType()).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                object PropValue = Property.GetValue(obj1, null);
                if (AssociationInfo != null)
                {
                    if (PropValue != null)
                        PropValue = GetDisplayValueForAssociation(AssociationInfo.Target, Convert.ToString(PropValue));
                    DataType = "Association";
                }
                if (ValidateValueAgainstRule(PropValue, DataType, ConditionOperator, ConditionValue, Property))
                    result = result && true;
                else result = result && false;
            }
            if (!containsCondition)
                return false;
            return result;
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
    }
}