using System.Collections.Generic;

namespace PageTypeBuilder.Discovery
{
    public interface IPageTypeDefinitionLocator
    {
        IEnumerable<PageTypeDefinition> GetPageTypeDefinitions();
    }
}
