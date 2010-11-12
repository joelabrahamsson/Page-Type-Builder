using EPiServer.DataAbstraction;
using Xunit;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class TabDefinitionShouldBeUpdatedTests
    {
        [Fact]
        public void GivenTabDefinitionWithAllValuesEqualToTab_TabDefinitionShouldBeUpdated_ReturnsFalse()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            bool shouldBeUpdated = tabDefinitionUpdater.TabDefinitionShouldBeUpdated(tabDefinition, tab);

            Assert.False(shouldBeUpdated);
        }

        [Fact]
        public void GivenTabDefinitionWithDifferentName_TabDefinitionShouldBeUpdated_ReturnsTrue()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.Name = TestValueUtility.CreateRandomString();
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            bool shouldBeUpdated = tabDefinitionUpdater.TabDefinitionShouldBeUpdated(tabDefinition, tab);

            Assert.True(shouldBeUpdated);
        }

        [Fact]
        public void GivenTabDefinitionWithDifferentRequiredAccess_TabDefinitionShouldBeUpdated_ReturnsTrue()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.RequiredAccess = tabDefinition.RequiredAccess + 1;
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            bool shouldBeUpdated = tabDefinitionUpdater.TabDefinitionShouldBeUpdated(tabDefinition, tab);

            Assert.True(shouldBeUpdated);
        }

        [Fact]
        public void GivenTabDefinitionWithDifferentSortIndex_TabDefinitionShouldBeUpdated_ReturnsTrue()
        {
            Tab tab = new TestTab();
            TabDefinition tabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(tab);
            tabDefinition.SortIndex++;
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();

            bool shouldBeUpdated = tabDefinitionUpdater.TabDefinitionShouldBeUpdated(tabDefinition, tab);

            Assert.True(shouldBeUpdated);
        }
    }
}
