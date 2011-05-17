using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageTypeLocator
    {
        IPageType GetExistingPageType(PageTypeDefinition definition);
    }
}
