using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PageTypeBuilder.Reflection
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetPublicOrPrivateProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static IEnumerable<PropertyInfo> GetPageTypePropertiesOnClass(this Type pageTypeType)
        {
            return pageTypeType.GetPublicOrPrivateProperties().Where(propertyInfo => propertyInfo.HasAttribute(typeof(PageTypePropertyAttribute)));
        }

        internal static IEnumerable<PropertyInfo> GetPageTypePropertyGroupProperties(this Type pageTypeType)
        {
            return pageTypeType.GetPublicOrPrivateProperties().Where(propertyInfo => propertyInfo.HasAttribute(typeof(PageTypePropertyGroupAttribute)));
        }

        public static IEnumerable<Type> AssignableTo(this IEnumerable<Type> types, Type superType)
        {
            return types.Where(superType.IsAssignableFrom);
        }

        public static IEnumerable<Type> Concrete(this IEnumerable<Type> types)
        {
            return types.Where(type => !type.IsAbstract);
        }

        public static bool IsNullableType(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static bool CanBeNull(this Type type)
        {
            return !type.IsValueType || type.IsNullableType();
        }

        public static IEnumerable<Type> WithAttribute<T>(this IEnumerable<Type> types)
            where T : Attribute
        {
            foreach (var type in types)
            {
                object[] attributes = GetAttributes(type);
                foreach (object attribute in attributes)
                {
                    if (typeof(T).IsAssignableFrom(attribute.GetType()))
                        yield return type;
                }
            }
        }

        private static object[] GetAttributes(Type type)
        {
            return type.GetCustomAttributes(true); ;
        }

        public static T GetAttribute<T>(this Type type)
            where T : Attribute
        {
            T attribute = null;

            object[] attributes = type.GetCustomAttributes(true);
            foreach (object attributeInType in attributes)
            {
                if (typeof(T).IsAssignableFrom(attributeInType.GetType()))
                    attribute = (T)attributeInType;
            }

            return attribute;
        }
    }
}
