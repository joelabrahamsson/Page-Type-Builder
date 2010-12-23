using System;
using System.Linq;
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
            AppDomain.CurrentDomain.AssemblyResolve +=
                    (object sender, ResolveEventArgs args) =>
                    {
                        if (assemblies.Count(assembly => assembly.FullName == args.Name) == 1)
                            return assemblies.First(assembly => assembly.FullName == args.Name);

                        return null;
                    };
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
