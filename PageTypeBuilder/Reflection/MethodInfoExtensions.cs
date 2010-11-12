using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PageTypeBuilder.Reflection
{
    internal static class MethodInfoExtensions
    {
        public static bool IsGetterOrSetter(this MemberInfo memberInfo)
        {
            return memberInfo.IsGetter() || memberInfo.IsSetter();
        }

        public static bool IsGetter(this MemberInfo memberInfo)
        {
            return memberInfo.Name.StartsWith("get_");
        }

        public static bool IsSetter(this MemberInfo memberInfo)
        {
            return memberInfo.Name.StartsWith("set_");
        }

        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            return memberInfo.GetCustomAttributes(attributeType, false).Length > 0;
        }

        public static bool IsCompilerGenerated(this MemberInfo memberInfo)
        {
            return memberInfo.HasAttribute(typeof(CompilerGeneratedAttribute));
        }

        public static string GetPropertyName(this MethodInfo propertyGetterOrSetter)
        {
            return propertyGetterOrSetter.Name.Substring(4);
        }

        public static bool IsGetterOrSetterForPropertyWithAttribute(this MethodInfo propertyGetterOrSetter, Type attributeType)
        {
            if (!propertyGetterOrSetter.IsGetterOrSetter())
                return false;

            string propertyName = propertyGetterOrSetter.GetPropertyName();
            PropertyInfo property = propertyGetterOrSetter.DeclaringType.GetProperty(propertyName);

            if (property == null)
                return false;

            return property.HasAttribute(attributeType);
        }
    }
}