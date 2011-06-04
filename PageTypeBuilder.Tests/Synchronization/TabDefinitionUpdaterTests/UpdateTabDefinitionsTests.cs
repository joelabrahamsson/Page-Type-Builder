using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Tests.Helpers;
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
            TabDefinitionUpdater tabDefinitionUpdater = TabDefinitionUpdaterFactory.PartialMock(fakes);
            List<Tab> tabs = new List<Tab>();
            Tab newTab = new TestTab();
            tabs.Add(newTab);
            tabDefinitionUpdater.Stub(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.Anything, Arg<Tab>.Is.Anything));
            tabDefinitionUpdater.Replay();
            TabDefinitionRepository fakeTabDefinitionRepository = fakes.Stub<TabDefinitionRepository>();
            tabDefinitionUpdater.TabDefinitionRepository = fakeTabDefinitionRepository;
            tabDefinitionUpdater.TabDefinitionRepository.Stub(factory => factory.GetTabDefinition(newTab.Name)).Return(null);
            tabDefinitionUpdater.TabDefinitionRepository.Stub(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
            tabDefinitionUpdater.TabDefinitionRepository.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.AssertWasCalled(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.NotNull, Arg<Tab>.Matches(tab => tab == newTab)));
            tabDefinitionUpdater.TabDefinitionRepository.AssertWasCalled(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.NotNull));
        }

        [Fact]
        public void GivenExistingTab_UpdateTabDefinitions_DoesNotSaveTab()
        {
            MockRepository fakes = new MockRepository();
            TabDefinitionUpdater tabDefinitionUpdater = TabDefinitionUpdaterFactory.Create();
            List<Tab> tabs = new List<Tab>();
            Tab existingTab = new TestTab();
            tabs.Add(existingTab);
            TabDefinitionRepository fakeTabDefinitionRepository = fakes.Stub<TabDefinitionRepository>();
            tabDefinitionUpdater.TabDefinitionRepository = fakeTabDefinitionRepository;
            TabDefinition existingTabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(existingTab);
            tabDefinitionUpdater.TabDefinitionRepository.Stub(factory => factory.GetTabDefinition(existingTab.Name)).Return(existingTabDefinition);
            tabDefinitionUpdater.TabDefinitionRepository.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.TabDefinitionRepository.AssertWasNotCalled(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
        }

        [Fact]
        public void GivenExistingTabThatShouldBeUpdated_UpdateTabDefinitions_UpdatesTab()
        {
            MockRepository fakes = new MockRepository();
            TabDefinitionUpdater tabDefinitionUpdater = TabDefinitionUpdaterFactory.PartialMock(fakes);
            List<Tab> tabs = new List<Tab>();
            Tab existingTab = new TestTab();
            tabs.Add(existingTab);
            tabDefinitionUpdater.Stub(updater => updater.UpdateTabDefinition(Arg<TabDefinition>.Is.Anything, Arg<Tab>.Is.Anything));
            TabDefinitionRepository fakeTabDefinitionRepository = fakes.Stub<TabDefinitionRepository>();
            tabDefinitionUpdater.TabDefinitionRepository = fakeTabDefinitionRepository;
            TabDefinition existingTabDefinition = TabDefinitionUpdaterTestsUtility.CreateTabDefinition(existingTab);
            tabDefinitionUpdater.TabDefinitionRepository.Stub(factory => factory.GetTabDefinition(existingTab.Name)).Return(existingTabDefinition);
            tabDefinitionUpdater.TabDefinitionRepository.Stub(factory => factory.SaveTabDefinition(Arg<TabDefinition>.Is.Anything));
            tabDefinitionUpdater.TabDefinitionRepository.Replay();
            tabDefinitionUpdater.Stub(updater => updater.TabDefinitionShouldBeUpdated(existingTabDefinition, existingTab)).Return(true);
            tabDefinitionUpdater.Replay();

            tabDefinitionUpdater.UpdateTabDefinitions(tabs);

            tabDefinitionUpdater.AssertWasCalled(updater => updater.UpdateTabDefinition(existingTabDefinition, existingTab));
            tabDefinitionUpdater.TabDefinitionRepository.AssertWasCalled(factory => factory.SaveTabDefinition(existingTabDefinition));
        }
    }
}