using System;
using Castle.DynamicProxy;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Activation
{
    internal class PageTypePropertiesProxyGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonVirtualMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo memberInfo)
        {
            return memberInfo.IsGetterOrSetterForPropertyWithAttribute(typeof(PageTypePropertyAttribute))
                && !memberInfo.IsGetterOrSetterForPropertyWithAttribute(typeof(PageTypePropertyGroupPropertyOverrideAttribute))
                && memberInfo.IsCompilerGenerated();
        }

        public void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }

        public override bool Equals(object obj)
        {
            return ((obj != null) && (obj.GetType() == typeof(PageTypePropertiesProxyGenerationHook)));
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
