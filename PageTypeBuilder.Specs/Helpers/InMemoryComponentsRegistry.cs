using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageTypeBuilder.Abstractions;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace PageTypeBuilder.Specs.Helpers
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
