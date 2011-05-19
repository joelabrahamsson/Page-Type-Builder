using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeUpdaterFactory
    {
        public static PageTypeUpdater Create(IPageTypeDefinitionLocator pageDefinitionLocator, IPageTypeFactory pageTypeFactory)
        {
            return new PageTypeUpdater(
                pageDefinitionLocator,
                pageTypeFactory,
                new PageTypeValueExtractor(),
                new PageTypeLocator(pageTypeFactory));
        }

        public static PageTypeUpdater Create(IPageTypeDefinitionLocator pageDefinitionLocator)
        {
            return Create(pageDefinitionLocator, new PageTypeFactory());
        }

        public static PageTypeUpdater Create()
        {
            return Create(PageTypeDefinitionLocatorFactory.Create());
        }

        public static PageTypeUpdater Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<PageTypeUpdater>(
                PageTypeDefinitionLocatorFactory.Stub(), 
                new PageTypeFactory(),
                new PageTypeValueExtractor(),
                new PageTypeLocator(new PageTypeFactory()));
        }
    }
}
