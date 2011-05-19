using System.Collections.Generic;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class UpdatePageTypesTests
    {
        [Fact]
        public void GivenPageType_UpdatePageTypes_CallsPageTypeUpdaterUpdatePageType()
        {
            PageTypeSynchronizer synchronizer = PageTypeSynchronizerFactory.Create();
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Stub(fakes);
            PageTypeDefinition definition = new PageTypeDefinition();   
            pageTypeUpdater.Stub(updater => updater.UpdatePageType(definition));
            pageTypeUpdater.Replay();
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };

            synchronizer.UpdatePageTypes(definitions);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdatePageType(definition));
        }
    }
}
