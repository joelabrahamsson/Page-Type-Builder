using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageDefinitionUpdater
    {
        void UpdatePageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition);
    }
}