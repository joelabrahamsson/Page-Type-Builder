using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using Rhino.Mocks;

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

        public static PageTypeSynchronizer PartialMock(
            MockRepository fakesRepository, 
            IPageTypeDefinitionLocator definitionLocator, 
            PageTypeBuilderConfiguration configuration)
        {
            return fakesRepository.PartialMock<PageTypeSynchronizer>(
                definitionLocator,
                configuration,
                PageTypePropertyUpdaterFactory.Create(),
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())),
                new PageTypeResolver(),
                new PageTypeLocator(new PageTypeFactory()),
                PageTypeUpdaterFactory.Create(),
                TabDefinitionUpdaterFactory.Create(),
                TabLocatorFactory.Create());
        }
    }
}
