using System.Collections.Generic;
using System.Reflection;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Specs.Helpers
{
    public class InMemoryAssemblyLocator : IAssemblyLocator
    {
        private List<Assembly> assemblies;

        public InMemoryAssemblyLocator()
        {
            assemblies = new List<Assembly>();
        }

        public void Add(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return assemblies;
        }
    }
}
