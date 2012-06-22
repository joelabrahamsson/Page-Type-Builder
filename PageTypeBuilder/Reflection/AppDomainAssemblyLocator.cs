using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PageTypeBuilder.Configuration;

namespace PageTypeBuilder.Reflection
{
    public class AppDomainAssemblyLocator : IAssemblyLocator
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            ScanAssemblyCollection scanAssemblies = PageTypeBuilderConfiguration.GetConfiguration().ScanAssemblies;

            if (scanAssemblies == null || scanAssemblies.Count == 0)
                return AppDomain.CurrentDomain.GetAssemblies();

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(c => scanAssemblies.Cast<ScanAssemblyElement>().Any(a => string.Equals(c.GetName().Name, a.AssemblyName)));
        }
    }
}