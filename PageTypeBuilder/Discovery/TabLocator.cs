using System;
using System.Collections.Generic;
using System.Reflection;
using PageTypeBuilder.Reflection;
using System.Linq;

namespace PageTypeBuilder.Discovery
{
    public class TabLocator
    {
        private IAssemblyLocator _assemblyLocator;

        public TabLocator(IAssemblyLocator assemblyLocator)
        {
            _assemblyLocator = assemblyLocator;
        }

        public virtual IEnumerable<Tab> GetDefinedTabs()
        {
            List<Tab> definedTabs = new List<Tab>();
            IEnumerable<Type> tabTypes = GetTabTypesInApplicationDomain();

            foreach (Type tabType in tabTypes)
            {
                definedTabs.Add((Tab)Activator.CreateInstance(tabType));
            }

            return definedTabs;
        }

        private IEnumerable<Type> GetTabTypesInApplicationDomain()
        {
            string tabTypeAssemblyName = typeof(Tab).Assembly.GetName().Name;
            List<Type> tabTypes = new List<Type>();
            var assemblies = _assemblyLocator.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if(assembly.GetReferencedAssemblies().Count(a => a.Name == tabTypeAssemblyName) == 0)
                    continue;

                IEnumerable<Type> tabTypesInAssembly = assembly.GetTypes();
                tabTypesInAssembly = tabTypesInAssembly.AssignableTo(typeof(Tab)).Concrete();
                tabTypes.AddRange(tabTypesInAssembly);
                
            }
            return tabTypes;
        }
    }
}
