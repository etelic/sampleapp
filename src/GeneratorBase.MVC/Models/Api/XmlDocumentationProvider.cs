using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;

namespace GeneratorBase.MVC.Models
{
    public class XmlDocumentationProvider : IDocumentationProvider
    {

        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";

        private XPathNavigator _documentNavigator;
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentationProvider"/> class.
        /// </summary>
        /// <param name="documentPath">The physical path to XML document.</param>
        public XmlDocumentationProvider(string documentPath)
        {
            if (documentPath == null)
            {
                throw new ArgumentNullException("documentPath");
            }
            XPathDocument xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            if (methodNode != null)
            {
                XPathNavigator summaryNode = methodNode.SelectSingleNode("summary");
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }
            }

            return null;
        }

        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            ReflectedHttpParameterDescriptor reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                XPathNavigator methodNode = GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                if (methodNode != null)
                {
                    string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    XPathNavigator parameterNode = methodNode.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                    if (parameterNode != null)
                    {
                        return parameterNode.Value.Trim();
                    }
                }
            }

            return null;
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                return _documentNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", method.DeclaringType.FullName, method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string typeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                return String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", typeName, String.Join(",", argumentTypeNames));
            }

            return type.FullName;
        }
        private static string GetPropertyName(PropertyInfo property)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", property.DeclaringType.FullName, property.Name);
            return name;
        }

        public IDictionary<string, TypeDocumentation> GetModelDocumentation(HttpActionDescriptor actionDescriptor)
        {
            var retDictionary = new Dictionary<string, TypeDocumentation>();
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                foreach (var parameterDescriptor in reflectedActionDescriptor.GetParameters())
                {
                    if (!parameterDescriptor.ParameterType.IsValueType)
                    {
                        TypeDocumentation typeDocs = new TypeDocumentation();


                        string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, GetTypeName(parameterDescriptor.ParameterType));
                        var typeNode = _documentNavigator.SelectSingleNode(selectExpression);

                        if (typeNode != null)
                        {
                            XPathNavigator summaryNode;
                            summaryNode = typeNode.SelectSingleNode("summary");
                            if (summaryNode != null)
                                typeDocs.Summary = summaryNode.Value;
                        }

                        foreach (var prop in parameterDescriptor.ParameterType.GetProperties())
                        {
                            string propName = prop.Name;
                            string propDocs = string.Empty;
                            string propExpression = String.Format(CultureInfo.InvariantCulture, PropertyExpression, GetPropertyName(prop));
                            var propNode = _documentNavigator.SelectSingleNode(propExpression);
                            if (propNode != null)
                            {
                                XPathNavigator summaryNode;
                                summaryNode = propNode.SelectSingleNode("summary");
                                if (summaryNode != null) propDocs = summaryNode.Value;
                            }
                            typeDocs.PropertyDocumentation.Add(new PropertyDocumentation(propName, prop.PropertyType.Name, propDocs));

                        }
                        retDictionary.Add(parameterDescriptor.ParameterName, typeDocs);
                    }

                }

            }

            return retDictionary;
        }
    }
}
