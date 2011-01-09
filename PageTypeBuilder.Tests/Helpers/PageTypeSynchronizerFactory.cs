using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeSynchronizerFactory
    {
        public static PageTypeSynchronizer Create(PageTypePropertyUpdater pageTypePropertyUpdater, IPageTypeLocator pageTypeLocator)
        {
            return new PageTypeSynchronizer(
                new PageTypeDefinitionLocator(),
                new PageTypeBuilderConfiguration(),
                pageTypePropertyUpdater,
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())),
                PageTypeResolver.Instance,
                pageTypeLocator,
                PageTypeUpdaterFactory.Create(),
                new TabDefinitionUpdater(),
                new TabLocator());
        }

        public static PageTypeSynchronizer Create(IPageTypeLocator pageTypeLocator)
        {
            return Create(new PageTypePropertyUpdater(), pageTypeLocator);
        }

        public static PageTypeSynchronizer Create()
        {
            return Create(new PageTypeLocator(new PageTypeFactory()));
        }
    }
}
