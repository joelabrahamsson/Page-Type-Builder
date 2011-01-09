using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeSynchronizerFactory
    {
        public static PageTypeSynchronizer Create(PageTypePropertyUpdater pageTypePropertyUpdater, IPageTypeLocator pageTypeLocator)
        {
            return new PageTypeSynchronizer(
                PageTypeDefinitionLocatorFactory.Create(),
                new PageTypeBuilderConfiguration(),
                pageTypePropertyUpdater,
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())),
                PageTypeResolver.Instance,
                pageTypeLocator,
                PageTypeUpdaterFactory.Create(),
                TabDefinitionUpdaterFactory.Create(),
                TabLocatorFactory.Create());
        }

        public static PageTypeSynchronizer Create(IPageTypeLocator pageTypeLocator)
        {
            return Create(PageTypePropertyUpdaterFactory.Create(), pageTypeLocator);
        }

        public static PageTypeSynchronizer Create()
        {
            return Create(new PageTypeLocator(new PageTypeFactory()));
        }
    }
}
