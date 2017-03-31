using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
namespace GeneratorBase.MVC
{
    /// <summary>
    /// Uses reflection to create lists of entities and their properties.
    /// </summary>
    public class ModelReflector
    {
        public static List<Entity> Entities { get; private set; }
        static ModelReflector()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = GetTypesWithTableAttribute(assembly);
            Entities = types.Select(t =>
                new Entity()
                {
                    Name = t.Name,
                    DisplayName = GetDisplayName(t) ?? t.Name,
                    IsDefault = GetDisplayName(t) == null || Enum.IsDefined(typeof(IgnoreEntities), t.Name) ? true : false,
                    IsExternalEntity = GetDBTableAttribute(t),
                    Properties = GetDisplayPropertiesForEntity(t).OrderBy(prop => prop.DisplayName).ToList(),
                    Associations = GetAssociationsForEntity(t).OrderBy(asso => asso.DisplayName).ToList(),
                    //code for verb action security
                    Verbs = GetVerbsForEntity(t).OrderBy(verb => verb.DisplayName).ToList(),
                    //
                    IsAdminEntity = (Enum.IsDefined(typeof(AdminEntities), t.Name))
                }
            ).OrderBy(p => p.IsDefault).ThenBy(p => p.DisplayName).ToList();
        }
        private static bool GetDBTableAttribute(Type t)
        {
            var result = false;
            var attr = t.GetCustomAttribute<TableAttribute>(false);
            if (attr != null)
            {
                result = false;
            }
            else result = true;
            return result;
        }
        private static string GetDisplayName(Type t)
        {
            string result = string.Empty;
            var attr = t.GetCustomAttribute<DisplayNameAttribute>(false);
            if (attr != null)
            {
                result = attr.DisplayName;
            }
            return string.IsNullOrEmpty(result) ? null : result;
        }
        private static IEnumerable<Type> GetTypesWithTableAttribute(Assembly assembly)
        {
            var tableAttr = typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == "UserBasedSecurity" || type.Name == "UserDefinePages" || type.Name == "UserDefinePagesRole") continue;
                var attr = type.GetCustomAttribute<TableAttribute>(true);
                var attr1 = type.GetCustomAttribute<DisplayNameAttribute>(true);
                if (attr != null || attr1 != null)
                {
                    yield return type;
                }
            }
        }
        private static IEnumerable<Property> GetDisplayPropertiesForEntity(Type type)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !p.Name.ToLower().EndsWith("workflowinstanceid")))
            {
                var attr = property.GetCustomAttribute<DisplayNameAttribute>(false);
                Attribute[] allattr = (Attribute[])Attribute.GetCustomAttributes(property, false);// property.GetCustomAttribute<System.ComponentModel.DataAnnotations.DataTypeAttribute>(false);
                var datatypeattr = allattr.FirstOrDefault(p => p.GetType() == typeof(System.ComponentModel.DataAnnotations.DataTypeAttribute));

                if (attr != null)
                {
                    yield return new Property()
                    {
                        Name = property.Name,
                        DisplayName = attr.DisplayName,
                        DataType = (property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? property.PropertyType.GetGenericArguments()[0].Name : property.PropertyType.Name,
                        IsRequired = property.IsDefined(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false),
                        IsEmailValidation = property.IsDefined(typeof(System.ComponentModel.DataAnnotations.EmailAddressAttribute), false),
                        DataTypeAttribute = datatypeattr!=null?((System.ComponentModel.DataAnnotations.DataTypeAttribute)datatypeattr).DataType.ToString() :""
                    };
                }
            }
        }
        private static IEnumerable<Association> GetAssociationsForEntity(Type type)
        {
            var proplist = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in proplist.Where(p => p.GetCustomAttribute<DisplayNameAttribute>(false) == null))
            {
                if (property.Name == "Id") continue;
                var targetProperty = proplist.Where(p => p.Name.ToLower() == property.Name.ToLower() + "id").ToList();
                if (targetProperty.Count > 0)
                {
                    var name = targetProperty[0].Name;
                    yield return new Association()
                    {
                        Name = name.TrimEnd("ID".ToCharArray()),
                        DisplayName = targetProperty[0].GetCustomAttribute<DisplayNameAttribute>(false).DisplayName,
                        AssociationProperty = name,
                        Target = property.PropertyType.Name
                    };
                }
            }
        }
        //code for verb action security
        private static IEnumerable<Verb> GetVerbsForEntity(Type type)
        {

            string controllerName = "GeneratorBase.MVC.Controllers." + type.Name + "Controller";
            Type VerbTypeCls = Type.GetType(controllerName);

            if (VerbTypeCls != null)
            {
                MethodInfo VerbTypeMethod = VerbTypeCls.GetMethod("getVerbsName");
                if (VerbTypeMethod != null)
                {
                    object clsInstance = Activator.CreateInstance(VerbTypeCls);
                    string[] result = (string[])VerbTypeMethod.Invoke(clsInstance, null);
                    foreach (var verbName in result)
                    {
                        var name = verbName;
                        yield return new Verb()
                        {
                            Name = name,
                            DisplayName = name,
                        };
                    }
                }

            }
        }
        //
        [DebuggerDisplay("{Name,nq}")]
        public class Entity
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public List<Property> Properties { get; set; }
            public List<Association> Associations { get; set; }
            //code for verb action security
            public List<Verb> Verbs { get; set; }
            //
            public bool IsAdminEntity { get; set; }
            public bool IsDefault { get; set; }
            public bool IsExternalEntity { get; set; }
        }
        [DebuggerDisplay("{DisplayName,nq}")]
        public class Property
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string DataType { get; set; }
            public bool IsRequired { get; set; }
            public bool IsEmailValidation { get; set; }
            public string DataTypeAttribute { get; set; }
            
        }
        public class Association
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string AssociationProperty { get; set; }
            public string Target { get; set; }
        }

        //code for verb action security
        public class Verb
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }

        }
        //
        public enum AdminEntities
        {
            ActionArgs,
            ActionType,
            BusinessRule,
            BusinessRuleType,
            Condition,
            Permission,
            RuleAction
        }
        public enum IgnoreEntities
        {
            PropertyMapping,
            T_RecurrenceDays,
            T_RecurringEndType,
            T_RecurringFrequency,
            T_RecurringScheduleDetailstype,
            T_RepeatOn,
            T_Schedule,
            T_Scheduletype,
            DataSourceParameters,
            Document,
            EntityDataSource,
            T_MonthlyRepeatType,
 	    ApiToken,
            MultiTenantExtraAccess,
            MultiTenantLoginSelected,
        }
    }
}