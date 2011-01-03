using System;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryComponentsRegistry : Registry
    { 
        public InMemoryComponentsRegistry()
        {
            Scan(scanner =>
                     {
                         scanner.AssemblyContainingType<InMemoryPageTypeFactory>();
                         scanner.Convention<InMemoryConvention>();
                     });

            For<PageTypeResolver>().Use(new PageTypeResolver());

            For<IPageTypeValueExtractor>().Use(new Mock<PageTypeValueExtractor>().Object);

            For<PageTypeBuilderConfiguration>()
                .LifecycleIs(new SingletonLifecycle())
                .Use<FakePageTypeBuilderConfiguration>();

            Scan(scanner =>
            {
                scanner.AssemblyContainingType<IPageTypeFactory>();
                scanner.WithDefaultConventions();
            });
        }
    }

    public class InMemoryConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (!type.Name.Contains("InMemory"))
                return;
            Type interfaceType = type.GetInterface(type.Name.Replace("InMemory", "I"));
            if(interfaceType != null)
                registry.For(interfaceType).LifecycleIs(InstanceScope.Singleton).Use(type);
        }
    }
}
