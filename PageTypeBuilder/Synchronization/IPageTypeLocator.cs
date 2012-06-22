using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public interface IPageTypeLocator
    {
        IPageTypeRepository PageTypeRepository { get; }
        IPageType GetExistingPageType(PageTypeDefinition definition);
    }
}
