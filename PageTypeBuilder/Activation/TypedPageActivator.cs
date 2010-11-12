using System;
using Castle.DynamicProxy;
using EPiServer.Core;

namespace PageTypeBuilder.Activation
{
    public class TypedPageActivator
    {
        private ProxyGenerator _generator;
        private ProxyGenerationOptions _options;
        private IInterceptor[] _interceptors;

        public TypedPageActivator()
        {
            _generator = new ProxyGenerator();
            _options = new ProxyGenerationOptions(new PageTypePropertiesProxyGenerationHook());
            _interceptors = new IInterceptor[] 
                               {
                                   new PageTypePropertyInterceptor()
                               };
        }

        public virtual TypedPageData CreateAndPopulateTypedInstance(PageData originalPage, Type typedType)
        {
            TypedPageData typedPage = CreateInstance(typedType);
            TypedPageData.PopuplateInstance(originalPage, typedPage);
            return typedPage;
        }

        public virtual TypedPageData CreateInstance(Type typedType)
        {
            return CreateInstance(typedType, new object[] {});
        }

        protected virtual TypedPageData CreateInstance(Type typedType, object[] ctorArguments)
        {
            return (TypedPageData)_generator.CreateClassProxy(typedType, new Type[] {}, _options, ctorArguments, _interceptors);
        }
    }
}