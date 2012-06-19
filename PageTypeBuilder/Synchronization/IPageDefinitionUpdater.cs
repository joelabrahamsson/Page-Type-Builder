using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageDefinitionUpdater
    {
        List<int> UpdatedPageDefinitions { get; }
        void UpdateExistingPageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition);
        void CreateNewPageDefinition(PageTypePropertyDefinition propertyDefinition);
    }
}