using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class FilterApplicationDropdowns
    {
        public IQueryable<T> FilterDropdown<T>(IUser User, IQueryable<T> list, string EntityName, string caller)
        {
            var br = User.businessrules.Where(p => p.EntityName == EntityName);
            if (br != null && br.Count() > 0)
            {
                var ruleactiondb = new RuleActionContext();
                foreach (var _rule in br)
                {
                    foreach (var act in ruleactiondb.RuleActions.Where(p => p.RuleActionID == _rule.Id && p.AssociatedActionTypeID == 5))
                    {
                        ActionArgsContext actionargs = new ActionArgsContext();
                        var ruleconditionsdb = new ConditionContext();
                        var arg = actionargs.ActionArgss.FirstOrDefault(p => p.ActionArgumentsID == act.Id && p.ParameterName == caller);
                        if (arg != null)
                            foreach (var condition in ruleconditionsdb.Conditions.Where(p => p.RuleConditionsID == _rule.Id))
                            {
                                var PropName = condition.PropertyName;
                                var EntityInfo = ModelReflector.Entities.FirstOrDefault(p => p.Name == EntityName);
                                var PropInfo = EntityInfo.Properties.FirstOrDefault(p => p.Name == PropName);
                                var AssociationInfo = EntityInfo.Associations.FirstOrDefault(p => p.AssociationProperty == PropName);
                                string DataType = PropInfo.DataType;
                                PropertyInfo[] properties = (typeof(T)).GetProperties().Where(p => p.PropertyType.FullName.StartsWith("System")).ToArray();
                                var Property = properties.FirstOrDefault(p => p.Name == PropName);
                                if (AssociationInfo != null)
                                {
                                    Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + AssociationInfo.Target + "Controller");
                                    object objController = Activator.CreateInstance(controller, null);
                                    System.Reflection.MethodInfo mc = controller.GetMethod("GetIdFromDisplayValue");
                                    object[] MethodParams = new object[] { condition.Value };
                                    var obj = mc.Invoke(objController, MethodParams);
                                    object PropValue = obj;
                                    IQueryable query = list;
                                    Type[] exprArgTypes = { query.ElementType };
                                    string propToWhere = condition.PropertyName;// filterCriteria.DropdownProperty;
                                    ParameterExpression p1 = Expression.Parameter(typeof(T), "p");
                                    MemberExpression member = Expression.PropertyOrField(p1, propToWhere);
                                    Type targetType = typeof(System.Int64?);
                                    dynamic PropertyValue = PropValue;//Convert.ChangeType(PropValue, targetType);
                                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                                    IQueryable q = query.Provider.CreateQuery(methodCall);
                                    list = ((IQueryable<T>)q);
                                }
                                else
                                {
                                    object PropValue = condition.Value;// filterCriteria.PropertyValue;
                                    IQueryable query = list;
                                    Type[] exprArgTypes = { query.ElementType };
                                    string propToWhere = condition.PropertyName;// filterCriteria.DropdownProperty;
                                    ParameterExpression p1 = Expression.Parameter(typeof(T), "p");
                                    MemberExpression member = Expression.PropertyOrField(p1, propToWhere);
                                    Type targetType = Property.PropertyType;
                                    if (Property.PropertyType.IsGenericType)
                                        targetType = Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(Property.PropertyType) : Property.PropertyType;
                                    dynamic PropertyValue = Convert.ChangeType(PropValue, targetType);
                                    LambdaExpression lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    if (condition.Operator == ">")
                                    {
                                        lambda = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if (condition.Operator == "<")
                                    {
                                        lambda = Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if (condition.Operator == "<=")
                                    {
                                        lambda = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if (condition.Operator == ">=")
                                    {
                                        lambda = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    if (condition.Operator == "!=")
                                    {
                                        lambda = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, Expression.Convert(Expression.Constant(PropertyValue), targetType)), p1);
                                    }
                                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                                    IQueryable q = query.Provider.CreateQuery(methodCall);
                                    list = ((IQueryable<T>)q);
                                }
                            }
                    }
                }
            }
            return list;
        }
    }
}