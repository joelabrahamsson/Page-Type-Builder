using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
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
            PageTypeSynchronizer synchronizer = new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new List<PageTypeDefinition>());
            PageType pageType = new PageType();
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(definition)).Return(pageType);
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            pageTypeUpdater.Replay();
            PageTypePropertyUpdater pageTypePropertyUpdater = fakes.Stub<PageTypePropertyUpdater>();
            pageTypePropertyUpdater.Stub(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
            pageTypePropertyUpdater.Replay();
            synchronizer.PageTypePropertyUpdater = pageTypePropertyUpdater;
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };

            synchronizer.UpdatePageTypePropertyDefinitions(definitions);

            pageTypePropertyUpdater.AssertWasCalled(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
        }
    }
}
