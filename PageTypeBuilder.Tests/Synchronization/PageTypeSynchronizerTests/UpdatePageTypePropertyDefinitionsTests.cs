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
            PageTypePropertyUpdater pageTypePropertyUpdater = PageTypePropertyUpdaterFactory.Stub(fakes);
            pageTypePropertyUpdater.Stub(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
            pageTypePropertyUpdater.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(pageType);
            pageTypeLocator.Replay();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };
            PageTypeSynchronizer synchronizer =
                PageTypeSynchronizerFactory.Create(pageTypePropertyUpdater, pageTypeLocator);
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            synchronizer.UpdatePageTypePropertyDefinitions(definitions);

            pageTypePropertyUpdater.AssertWasCalled(updater => updater.UpdatePageTypePropertyDefinitions(pageType, definition));
        }
    }
}
