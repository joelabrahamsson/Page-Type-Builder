using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Discovery
{
    public class PageTypeDefinitionLocator : IPageTypeDefinitionLocator
    {
        private IAssemblyLocator assemblyLocator;

        public PageTypeDefinitionLocator(IAssemblyLocator assemblyLocator)
        {
            this.assemblyLocator = assemblyLocator;
        }

        public virtual IEnumerable<PageTypeDefinition> GetPageTypeDefinitions()
        {
            return assemblyLocator
                .AssembliesWithReferenceToAssemblyOf<PageTypeAttribute>()
                .TypesWithAttribute<PageTypeAttribute>()
                .Concrete()
                .Select(type => new PageTypeDefinition
                    {
                        Type = type,
                        Attribute = type.GetAttribute<PageTypeAttribute>()
                    })
                .ToList();
        }
    }

    
}