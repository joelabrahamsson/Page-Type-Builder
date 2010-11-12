using EPiServer.DataAbstraction;
using EPiServer.Security;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class UpdateTabDefinitionTests
    {
        [Fact]
        public void GivenTab_UpdateTabDefinition_UpdatesTabDefinitionsName()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.Name = TestValueUtility.CreateRandomString();
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            tabDefinitionUpdater.UpdateTabDefinition(tabDefinition, tab);

            Assert.Equal<string>(tab.Name, tabDefinition.Name);
        }

        [Fact]
        public void GivenTab_UpdateTabDefinition_UpdatesTabDefinitionsRequiredAccess()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.RequiredAccess = tab.RequiredAccess + 1;
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            tabDefinitionUpdater.UpdateTabDefinition(tabDefinition, tab);

            Assert.Equal<AccessLevel>(tab.RequiredAccess, tabDefinition.RequiredAccess);
        }

        [Fact]
        public void GivenTab_UpdateTabDefinition_UpdatesTabDefinitionsSortIndex()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.SortIndex = tab.SortIndex + 1;
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            tabDefinitionUpdater.UpdateTabDefinition(tabDefinition, tab);

            Assert.Equal<int>(tab.SortIndex, tabDefinition.SortIndex);
        }
    }
}
