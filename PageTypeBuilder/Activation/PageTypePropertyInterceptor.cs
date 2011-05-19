namespace PageTypeBuilder.Activation
{
    using Castle.DynamicProxy;
    using Reflection;

    internal class PageTypePropertyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypedPageData page;
            string propertyName = invocation.Method.GetPropertyName();

            if (invocation.InvocationTarget is PageTypePropertyGroup)
            {
                PageTypePropertyGroup propertyGroup = invocation.InvocationTarget as PageTypePropertyGroup;
                propertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroup.Hierarchy.Value, propertyName);
                page = propertyGroup.TypedPageData;
            }
            else
                page = invocation.InvocationTarget as TypedPageData;

            if (invocation.Method.IsGetter())
            {
                invocation.ReturnValue = page[propertyName];
                
                if (invocation.ReturnValue == null && invocation.Method.ReturnType == typeof(bool))
                    invocation.ReturnValue = false;
            }
            else
                page.SetValue(propertyName, invocation.Arguments[0]);
        }
    }
}
