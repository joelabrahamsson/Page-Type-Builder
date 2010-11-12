using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace PageTypeBuilder.Activation.StructureMap
{
    public class StructureMapTypedPageActivator : TypedPageActivator
    {
        private readonly IContainer _container;

        public StructureMapTypedPageActivator(IContainer container)
        {
            _container = container;
        }

        public override TypedPageData CreateInstance(Type typedType)
        {
            ParameterInfo[] expectedParameters = GetCtorParameters(typedType);
            
            object[] ctorParameters = GetResolvedParameters(expectedParameters);

            return base.CreateInstance(typedType, ctorParameters);
        }

        private static ParameterInfo[] GetCtorParameters(Type typedType)
        {
            ConstructorInfo constructor = GetConstructorWithMostParameters(typedType);

            if(constructor == default(ConstructorInfo))
                throw new PageTypeBuilderException(string.Format("Unable to find a public constructor for type {0}.", typedType.Name));

            return constructor.GetParameters();
        }

        private static ConstructorInfo GetConstructorWithMostParameters(Type typedType)
        {
            return typedType.GetConstructors()
                .Where(ctor => ctor.IsPublic && !ctor.IsStatic)
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .FirstOrDefault();
        }

        private object[] GetResolvedParameters(ParameterInfo[] expectedParameters)
        {
            object[] ctorParameters = new object[expectedParameters.Length];
            for (int i = 0; i < expectedParameters.Length; i++)
                ctorParameters[i] = ResolveParameter(expectedParameters[i]);
            return ctorParameters;
        }

        protected virtual object ResolveParameter(ParameterInfo parameterInfo)
        {
            return _container.GetInstance(parameterInfo.ParameterType);
        }
    }
}
