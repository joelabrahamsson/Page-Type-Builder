using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageTypeLocator
    {
        PageType GetExistingPageType(PageTypeDefinition definition);
    }
}
