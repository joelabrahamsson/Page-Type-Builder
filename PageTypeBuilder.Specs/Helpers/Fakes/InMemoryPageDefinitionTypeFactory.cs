using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageDefinitionTypeFactory : IPageDefinitionTypeFactory
    {
        private List<PageDefinitionType> pageDefinitions;

        public InMemoryPageDefinitionTypeFactory()
        {
            pageDefinitions = new List<PageDefinitionType>();

            pageDefinitions.Add(new PageDefinitionType(1, PropertyDataType.String, "PropertyXHtmlString", "EPiServer.SpecializedProperties.PropertyXhtmlString", "EPiServer"));
        }

        public PageDefinitionType GetPageDefinitionType(int id)
        {
            return pageDefinitions.FirstOrDefault(def => def.ID == id);
        }

        public PageDefinitionType GetPageDefinitionType(string typeName, string assemblyName)
        {
            return pageDefinitions.FirstOrDefault(def => def.TypeName == typeName && def.AssemblyName == assemblyName);
        }
    }
}
