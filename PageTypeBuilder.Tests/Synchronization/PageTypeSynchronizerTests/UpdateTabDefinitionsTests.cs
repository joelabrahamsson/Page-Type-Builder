using System.Collections.Generic;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class UpdateTabDefinitionsTests
    {
        [Fact]
        public void UpdateTabDefinitions_CallsTabDefinitionUpdaterWithDefinedTabs()
        {
            PageTypeSynchronizer pageTypeSynchronizer = new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
            MockRepository fakes = new MockRepository();
            TabLocator fakeTabLocator = fakes.Stub<TabLocator>();
            List<Tab> tabs = new List<Tab> { new TestTab() };
            fakeTabLocator.Stub(locator => locator.GetDefinedTabs()).Return(tabs);
            fakeTabLocator.Replay();
            pageTypeSynchronizer.TabLocator = fakeTabLocator;
            TabDefinitionUpdater fakeTabDefinitionUpdater = fakes.Stub<TabDefinitionUpdater>();
            fakeTabDefinitionUpdater.Stub(updater => updater.UpdateTabDefinitions(Arg<List<Tab>>.Is.Anything));
            fakeTabDefinitionUpdater.Replay();
            pageTypeSynchronizer.TabDefinitionUpdater = fakeTabDefinitionUpdater;

            pageTypeSynchronizer.UpdateTabDefinitions();

            pageTypeSynchronizer.TabLocator.AssertWasCalled(updater => updater.GetDefinedTabs());
            pageTypeSynchronizer.TabDefinitionUpdater.AssertWasCalled(updater => updater.UpdateTabDefinitions(tabs));
        }
    }
}
