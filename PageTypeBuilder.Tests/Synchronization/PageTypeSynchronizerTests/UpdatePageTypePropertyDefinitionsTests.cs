using System.Collections.Generic;
using EPiServer.DataAbstraction;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class UpdatePageTypePropertyDefinitionsTests
    {
        [Fact]
        public void GivenPageType_UpdatePageTypePropertyDefinitions_CallsPageTypePropertyUpdaterUpdatePageTypePropertyDefinitions()
        {
            PageTypeDefinition definition = new PageTypeDefinition();
            
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new Mock<IPageTypeDefinitionLocator>().Object, new PageTypeFactory());
            IPageType pageType = new NativePageType();
            PageTypePropertyUpdater pageTypePropertyUpdater = fakes.Stub<PageTypePropertyUpdater>();
            pageTypePropertyUpdater.Stub(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
            pageTypePropertyUpdater.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(pageType);
            pageTypeLocator.Replay();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };
            PageTypeSynchronizer synchronizer = new PageTypeSynchronizer(
                new PageTypeDefinitionLocator(),
                new PageTypeBuilderConfiguration(),
                pageTypePropertyUpdater,
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())),
                new PageTypeResolver(),
                pageTypeLocator,
                new PageTypeUpdater(new PageTypeDefinitionLocator(), new PageTypeFactory()), new TabDefinitionUpdater(),
                new TabLocator());
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            synchronizer.UpdatePageTypePropertyDefinitions(definitions);

            pageTypePropertyUpdater.AssertWasCalled(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
        }
    }
}
