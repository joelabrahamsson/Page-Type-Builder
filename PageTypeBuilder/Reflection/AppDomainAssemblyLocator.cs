using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.Configuration;
using PageTypeBuilder.Configuration;

namespace PageTypeBuilder.Reflection
{
    public class AppDomainAssemblyLocator : IAssemblyLocator
    {
        private readonly PageTypeBuilderConfiguration _config = PageTypeBuilderConfiguration.GetConfiguration();

        public IEnumerable<Assembly> GetAssemblies()
        {
            if (this._config.ScanAssembly != null && this._config.ScanAssembly.Count > 0)
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => this._config.ScanAssembly.Cast<AssemblyElement>()
                                    .Any(ae => ae.Assembly.Equals(a.GetName().Name, StringComparison.InvariantCultureIgnoreCase)));
            }

            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
