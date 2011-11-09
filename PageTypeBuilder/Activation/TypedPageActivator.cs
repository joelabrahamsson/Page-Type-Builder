namespace PageTypeBuilder.Activation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using EPiServer.Core;
    using Reflection;

    public class TypedPageActivator
    {
        private ProxyGenerator _generator;
        private ProxyGenerationOptions _options;
        private IInterceptor[] _interceptors;

        public TypedPageActivator()
            : this(CreateDefaultProxyInterceptors(), CreateDefaultProxyGenerationOptions(), CreateDefaultProxyGenerator())
        {
        }

        public TypedPageActivator(IInterceptor[] interceptors)
            : this(interceptors, CreateDefaultProxyGenerationOptions())
        {
        }

        public TypedPageActivator(ProxyGenerationOptions options)
            : this(CreateDefaultProxyInterceptors(), options, CreateDefaultProxyGenerator())
        {
        }
        
        public TypedPageActivator(IInterceptor[] interceptors, ProxyGenerationOptions options)
            : this(interceptors, options, CreateDefaultProxyGenerator())
        {
        }

        public TypedPageActivator(IInterceptor[] interceptors, ProxyGenerationOptions options, ProxyGenerator generator)
        {
            _generator = generator;
            _options = options;
            _interceptors = interceptors;
        }

        protected static IInterceptor[] CreateDefaultProxyInterceptors()
        {
            return new IInterceptor[]
            {
                new PageTypePropertyInterceptor()
            };
        }

        protected static ProxyGenerationOptions CreateDefaultProxyGenerationOptions()
        {
            return new ProxyGenerationOptions(new PageTypePropertiesProxyGenerationHook());
        }

        protected static ProxyGenerator CreateDefaultProxyGenerator()
        {
            return new ProxyGenerator();
        }

        public virtual TypedPageData CreateAndPopulateTypedInstance(PageData originalPage, Type typedType)
        {
            TypedPageData typedPage = CreateInstance(typedType);
            TypedPageData.PopuplateInstance(originalPage, typedPage);
            PropertyInfo[] properties = typedPage.GetType().GetPublicOrPrivateProperties();
            CreateAndPopulateNestedPropertyGroupInstances(typedPage, typedPage, properties, string.Empty);
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

        public virtual PageTypePropertyGroup CreatePropertyGroupInstance(Type typedType)
        {
            return CreatePropertyGroupInstance(typedType, new object[] { });
        }

        protected virtual PageTypePropertyGroup CreatePropertyGroupInstance(Type typedPropertyGroup, object[] ctorArguments)
        {
            return (PageTypePropertyGroup)_generator.CreateClassProxy(typedPropertyGroup, new Type[] { }, _options, ctorArguments, _interceptors);
        }

        internal void CreateAndPopulateNestedPropertyGroupInstances(TypedPageData typedPage, object classInstance,
            IEnumerable<PropertyInfo> properties, string hierarchy)
        {
            foreach (PropertyInfo property in properties.Where(current => current.PropertyType.BaseType == typeof(PageTypePropertyGroup)))
            {
                PageTypePropertyGroup propertyGroup = CreatePropertyGroupInstance(property.PropertyType);
                string propertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(hierarchy, property.Name);

                propertyGroup.PopuplateInstance(typedPage, propertyName);
                property.SetValue(classInstance, propertyGroup, null);
            }
        }
    }
}