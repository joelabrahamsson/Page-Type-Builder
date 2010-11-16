using System.Collections.Generic;

namespace PageTypeBuilder.Discovery
{
    public interface IPageTypeDefinitionLocator
    {
        List<PageTypeDefinition> GetPageTypeDefinitions();
    }
}
