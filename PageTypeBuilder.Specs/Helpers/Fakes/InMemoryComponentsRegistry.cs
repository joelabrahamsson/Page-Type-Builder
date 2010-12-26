using System;
using Moq;
using PageTypeBuilder.Abstractions;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

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
