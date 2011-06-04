using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeUpdaterFactory
    {
        public static PageTypeUpdater Create(IPageTypeDefinitionLocator pageDefinitionLocator, IPageTypeRepository pageTypeRepository)
        {
            return new PageTypeUpdater(
                pageDefinitionLocator,
                pageTypeRepository,
                new PageTypeValueExtractor(),
                new PageTypeLocator(pageTypeRepository));
        }

        public static PageTypeUpdater Create(IPageTypeDefinitionLocator pageDefinitionLocator)
        {
            return Create(pageDefinitionLocator, new PageTypeRepository());
        }

        public static PageTypeUpdater Create()
        {
            return Create(PageTypeDefinitionLocatorFactory.Create());
        }

        public static PageTypeUpdater Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<PageTypeUpdater>(
                PageTypeDefinitionLocatorFactory.Stub(), 
                new PageTypeRepository(),
                new PageTypeValueExtractor(),
                new PageTypeLocator(new PageTypeRepository()));
        }
    }
}
