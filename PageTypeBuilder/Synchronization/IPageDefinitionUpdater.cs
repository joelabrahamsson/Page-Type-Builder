using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageDefinitionUpdater
    {
        void UpdateExistingPageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition);
        void CreateNewPageDefinition(PageTypePropertyDefinition propertyDefinition);
    }
}