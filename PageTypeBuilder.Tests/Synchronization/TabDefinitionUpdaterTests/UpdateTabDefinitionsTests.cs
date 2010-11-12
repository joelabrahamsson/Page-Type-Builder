using System.Collections.Generic;
using EPiServer.DataAbstraction;
using Rhino.Mocks;
using Xunit;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class TabDefinitionUpdaterTests
    {
        [Fact]
        public void GivenNewTab_UpdateTabDefinitions_SavesNewTabDefinition()
        {
            MockRepository fakes = new MockRepository();
            TabDefinitionUpdater tabDefinitionUpdater = fakes.PartialMock<TabDefinitionUpdater>();
            List<Tab> tabs = new List<Tab>();
            Tab newTab = new TestTab();
            tabs.Add(newTab);
            tabDefinitionUpdater.Stub(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.Anything, Arg<Tab>.Is.Anything));
            tabDefinitionUpdater.Replay();
            TabFactory fakeTabFactory = fakes.Stub<TabFactory>();
            tabDefinitionUpdater.TabFactory = fakeTabFactory;
            tabDefinitionUpdater.TabFactory.Stub(factory => factory.GetTabDefinition(newTab.Name)).Return(null);
            tabDefinitionUpdater.TabFactory.Stub(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
            tabDefinitionUpdater.TabFactory.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.AssertWasCalled(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.NotNull, Arg<Tab>.Matches(tab => tab == newTab)));
            tabDefinitionUpdater.TabFactory.AssertWasCalled(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.NotNull));
        }

        [Fact]
        public void GivenExistingTab_UpdateTabDefinitions_DoesNotSaveTab()
        {
            MockRepository fakes = new MockRepository();
            TabDefinitionUpdater tabDefinitionUpdater = new TabDefinitionUpdater();
            List<Tab> tabs = new List<Tab>();
            Tab existingTab = new TestTab();
            tabs.Add(existingTab);
            TabFactory fakeTabFactory = fakes.Stub<TabFactory>();
            tabDefinitionUpdater.TabFactory = fakeTabFactory;
            TabDefinition existingTabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(existingTab);
            tabDefinitionUpdater.TabFactory.Stub(factory => factory.GetTabDefinition(existingTab.Name)).Return(existingTabDefinition);
            tabDefinitionUpdater.TabFactory.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.TabFactory.AssertWasNotCalled(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
        }

        [Fact]
        public void GivenExistingTabThatShouldBeUpdated_UpdateTabDefinitions_UpdatesTab()
        {
            MockRepository fakes = new MockRepository();
            TabDefinitionUpdater tabDefinitionUpdater = fakes.PartialMock<TabDefinitionUpdater>();
            List<Tab> tabs = new List<Tab>();
            Tab existingTab = new TestTab();
            tabs.Add(existingTab);
            tabDefinitionUpdater.Stub(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.Anything, Arg<Tab>.Is.Anything));
            TabFactory fakeTabFactory = fakes.Stub<TabFactory>();
            tabDefinitionUpdater.TabFactory = fakeTabFactory;
            TabDefinition existingTabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(existingTab);
            tabDefinitionUpdater.TabFactory.Stub(factory => factory.GetTabDefinition(existingTab.Name)).Return(existingTabDefinition);
            tabDefinitionUpdater.TabFactory.Stub(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
            tabDefinitionUpdater.TabFactory.Replay();
            tabDefinitionUpdater.Stub(updater => updater.TabDefinitionShouldBeUpdated(existingTabDefinition, existingTab)).Return(true);
            tabDefinitionUpdater.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.AssertWasCalled(updater => updater.UpdateTabDefinition(existingTabDefinition, existingTab));
            tabDefinitionUpdater.TabFactory.AssertWasCalled(factory => factory.SaveTabDefinition(existingTabDefinition));
        }
    }
}