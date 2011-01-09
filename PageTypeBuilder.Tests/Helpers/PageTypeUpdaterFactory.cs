using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;

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
                new PageTypeLocator(pageTypeFactory),
                new FrameFacade());
        }


        public static PageTypeUpdater Create(IPageTypeDefinitionLocator pageDefinitionLocator)
        {
            return Create(pageDefinitionLocator, new PageTypeFactory());
        }

        public static PageTypeUpdater Create()
        {
            return Create(new PageTypeDefinitionLocator());
        }
    }
}
