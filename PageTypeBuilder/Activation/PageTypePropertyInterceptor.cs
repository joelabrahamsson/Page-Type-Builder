using Castle.DynamicProxy;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Activation
{
    internal class PageTypePropertyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypedPageData page = (TypedPageData)invocation.InvocationTarget;
           
            string propertyName = invocation.Method.GetPropertyName();

            if (invocation.Method.IsGetter())
            {
                invocation.ReturnValue = page[propertyName];
                if (invocation.ReturnValue == null && invocation.Method.ReturnType == typeof(bool))
                    invocation.ReturnValue = false;
            }
            else
            {
                page.SetValue(propertyName, invocation.Arguments[0]);
            }
        }
    }
}
