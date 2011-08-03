using System;
using System.Linq.Expressions;
using EPiServer.Core;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder
{
    public static class PageTypePropertyGroupExtensionMethods
    {
        public static TProperty GetPropertyGroupPropertyValue<TPageTypePropertyGroup, TProperty>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, TProperty>> expression)
            where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            return propertyGroup.GetPropertyGroupPropertyValue(expression, true);
        }

        public static TProperty GetPropertyGroupPropertyValue<TPageTypePropertyGroup, TProperty>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, TProperty>> expression,
            bool usePropertyGetHandler) where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            MemberExpression memberExpression = GetMemberExpression(expression);
            string propertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroup.Hierarchy.Value, memberExpression.Member.Name);

            object value;
            if (usePropertyGetHandler)
                value = propertyGroup.TypedPageData[propertyName];
            else
                value = propertyGroup.TypedPageData.GetValue(propertyName);

            return ConvertToRequestedType<TProperty>(value);
        }

        private static MemberExpression GetMemberExpression<TPageTypePropertyGroup, TProperty>(Expression<Func<TPageTypePropertyGroup, TProperty>> expression)
        {
            MemberExpression memberExpression = null;

            if (expression.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)expression.Body;
            }
            else if (expression.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            if (memberExpression == null)
                throw new PageTypeBuilderException("The body of the expression must be either a MemberExpression of a UnaryExpression.");

            return memberExpression;
        }

        private static TProperty ConvertToRequestedType<TProperty>(object value)
        {
            if (value != null)
                return (TProperty)value;

            if (typeof(TProperty) == typeof(bool))
                return default(TProperty);

            if (!typeof(TProperty).CanBeNull())
                throw new PageTypeBuilderException(@"The property value is null and the requested type is a value type. 
                    Consider using nullable as type or make the property mandatory.");

            return (TProperty)value;
        }

        public static PropertyData GetPropertyGroupProperty<TPageTypePropertyGroup>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression)
            where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            return GetPropertyGroupProperty<TPageTypePropertyGroup, PropertyData>(propertyGroup, expression);
        }

        public static TPropertyData GetPropertyGroupProperty<TPageTypePropertyGroup, TPropertyData>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression)
            where TPageTypePropertyGroup : PageTypePropertyGroup
            where TPropertyData : PropertyData
        {
            MemberExpression memberExpression = GetMemberExpression(expression);
            string propertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroup.Hierarchy.Value, memberExpression.Member.Name);
            PropertyData propertyData = propertyGroup.TypedPageData.Property[propertyName];
            return (TPropertyData)propertyData;
        }

        public static TProperty GetPropertyGroupPropertyValue<TPageTypePropertyGroup, TProperty>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression)
            where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            return (TProperty)propertyGroup.GetPropertyGroupPropertyValue(expression, false);
        }

        public static TProperty GetPropertyGroupPropertyValue<TPageTypePropertyGroup, TProperty>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression,
            bool usePropertyGetHandler) where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            MemberExpression memberExpression = GetMemberExpression(expression);
            string propertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroup.Hierarchy.Value, memberExpression.Member.Name);

            object value;
            if (usePropertyGetHandler)
                value = (TProperty)propertyGroup.TypedPageData[propertyName];
            else
                value = propertyGroup.TypedPageData.GetValue(propertyName);

            return ConvertToRequestedType<TProperty>(value);
        }

        public static void SetPropertyGroupPropertyValue<TPageTypePropertyGroup>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression, object value) 
            where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            string propertyName = GetPropertyGroupPropertyName(propertyGroup, expression);
            propertyGroup.TypedPageData[propertyName] = value;
        }

        public static string GetPropertyGroupPropertyName<TPageTypePropertyGroup>(this TPageTypePropertyGroup propertyGroup, Expression<Func<TPageTypePropertyGroup, object>> expression)
            where TPageTypePropertyGroup : PageTypePropertyGroup
        {
            return PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroup.Hierarchy.Value, GetMemberExpression(expression).Member.Name);
        }
    }
}
