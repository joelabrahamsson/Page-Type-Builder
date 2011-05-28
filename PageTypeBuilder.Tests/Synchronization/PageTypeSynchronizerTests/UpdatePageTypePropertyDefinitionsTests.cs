using System.Collections.Generic;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
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
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Stub(fakes);
            IPageType pageType = new NativePageType();
            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = PageDefinitionSynchronizationEngineFactory.Stub(fakes);
            pageDefinitionSynchronizationEngine.Stub(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
            pageDefinitionSynchronizationEngine.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(pageType);
            pageTypeLocator.Replay();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };
            PageTypeSynchronizer synchronizer =
                PageTypeSynchronizerFactory.Create(pageDefinitionSynchronizationEngine, pageTypeLocator);
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            synchronizer.UpdatePageTypePropertyDefinitions(definitions);

            pageDefinitionSynchronizationEngine.AssertWasCalled(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
        }
    }
}
