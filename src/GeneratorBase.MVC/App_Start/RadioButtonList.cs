using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
namespace GeneratorBase.MVC
{
    public static class HtmlExtensions
    {
        #region RadioButtonList
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name)
        {
            return RadioButtonList(htmlHelper, name, null /* inputList */, null /* htmlAttributes */);
        }
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList)
        {
            return RadioButtonList(htmlHelper, name, inputList, null /* htmlAttributes */);
        }
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return RadioButtonList(htmlHelper, name, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
        {
            return RadioButtonListHelper(htmlHelper, name, inputList, htmlAttributes);
        }
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList)
        {
            return RadioButtonListFor(htmlHelper, expression, inputList, null /* htmlAttributes */);
        }
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return RadioButtonListFor(htmlHelper, expression, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return RadioButtonListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), inputList, htmlAttributes);
        }
        private static MvcHtmlString RadioButtonListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
        {
            return InputInternal(htmlHelper, expression, inputList, false /* allowMultiple */, htmlAttributes);
        }
        #endregion
        #region Helper methods
        private static MvcHtmlString InputInternal(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Null or Empty", "name");
            }
            var usedViewData = false;
            var IsRequired = false;
            if (htmlAttributes != null)
            {
                IsRequired = htmlAttributes.Keys.Any(k => k.ToLower().Trim() == "required");
                //htmlAttributes.Remove("required");
            }
            if (inputList == null)
            {
                inputList = htmlHelper.GetInputData(fullName);
                usedViewData = true;
            }
            var defaultValue = GetDefaultValue(htmlHelper, fullName, allowMultiple, usedViewData);
            if (defaultValue != null)
            {
                inputList = GetListWithDefaultValue(inputList, defaultValue, allowMultiple);
            }
            var listItemBuilder = new StringBuilder();
            if (inputList != null)
            {
                foreach (var item in inputList)
                    listItemBuilder.AppendLine(InputItemToHtml(item, allowMultiple, fullName, IsRequired));
            }
            if (inputList == null || inputList.Count() == 0)
            {
                var builder = new TagBuilder("input");
                builder.Attributes["id"] = name;
                builder.Attributes["name"] = name;
                builder.Attributes["type"] = "text";
                builder.Attributes.Add("style", "width:0px; height:0px; border:0px solid #fff !important;");
                if (IsRequired)
                    builder.Attributes["required"] = "required";
                listItemBuilder.AppendLine(builder.ToString(TagRenderMode.Normal));
            }
            var tagBuilder = new TagBuilder("ul")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.Attributes.Add("style", "padding:0");
            tagBuilder.Attributes.Add("name", "ul" + fullName);
            if (htmlAttributes != null)
                tagBuilder.MergeAttributes(htmlAttributes, true /* replaceExisting */);
            tagBuilder.GenerateId("ul" + fullName);
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }
            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));
            return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
        }
        private static IEnumerable<SelectListItem> GetInputData(this HtmlHelper htmlHelper, string name)
        {
            object inputObject = null;
            if (htmlHelper.ViewData != null)
            {
                inputObject = htmlHelper.ViewData.Eval(name);
            }
            if (inputObject != null)
            {
                var inputList = inputObject as IEnumerable<SelectListItem>;
                if (inputList == null)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                            "The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.",
                            name,
                            inputObject.GetType().FullName,
                            "IEnumerable<SelectListItem>"));
                }
                return inputList;
            }
            return null;
        }
        private static object GetDefaultValue(HtmlHelper htmlHelper, string fullName, bool allowMultiple, bool usedViewData)
        {
            var defaultValue = (allowMultiple) ?
                               GetModelStateValue(htmlHelper, fullName, typeof(string[])) :
                               GetModelStateValue(htmlHelper, fullName, typeof(string));
            if (defaultValue == null && !usedViewData)
            {
                defaultValue = htmlHelper.ViewData.Eval(fullName);
            }
            return defaultValue;
        }
        private static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
                if (modelState.Value != null)
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
            return null;
        }
        private static IEnumerable<SelectListItem> GetListWithDefaultValue(IEnumerable<SelectListItem> inputList, object defaultValue, bool allowMultiple)
        {
            var defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
            IEnumerable<string> values = from object value in defaultValues
                                         select Convert.ToString(value, CultureInfo.CurrentCulture);
            var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            var newInputList = new List<SelectListItem>();
            foreach (var item in inputList)
            {
                // item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                newInputList.Add(item);
            }
            return newInputList;
        }
        internal static string InputItemToHtml(SelectListItem item, bool allowMultiple, string name, bool IsRequired)
        {
            var listItemBuilder = new StringBuilder();
            listItemBuilder.AppendLine(GetInputItem(item, allowMultiple, name, IsRequired));
            listItemBuilder.AppendLine(CoverWithTag("span", HttpUtility.HtmlEncode(item.Text)));
            var label = CoverWithTag("label", listItemBuilder.ToString());
            return CoverWithLi(label);
        }
        internal static string GetInputItem(SelectListItem item, bool allowMultiple, string name, bool IsRequired)
        {
            var builder = new TagBuilder("input");
            builder.Attributes["name"] = name;
            if (allowMultiple)
                builder.Attributes["type"] = "checkbox";
            else
                builder.Attributes["type"] = "radio";
            if (item.Value != null)
                builder.Attributes["value"] = item.Value;
            else
                builder.Attributes["value"] = item.Text;
            if (item.Selected)
                builder.Attributes["checked"] = "checked";
            if (IsRequired)
                builder.Attributes["required"] = "required";
            return builder.ToString(TagRenderMode.Normal);
        }
        private static string CoverWithTag(string tagName, string html)
        {
            var builder = new TagBuilder(tagName) { InnerHtml = html };
            return builder.ToString(TagRenderMode.Normal);
        }
        internal static string CoverWithLi(string html)
        {
            var builder = new TagBuilder("li") { InnerHtml = html };
            builder.Attributes.Add("style", "list-style-type:none;");
            return builder.ToString(TagRenderMode.Normal);
        }
        private static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            Debug.Assert(tagBuilder != null);
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }
        #endregion
    }
}