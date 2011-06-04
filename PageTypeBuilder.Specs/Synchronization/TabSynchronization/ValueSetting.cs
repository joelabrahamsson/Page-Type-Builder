using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization.ValueSetting
{
    [Subject("Synchronization")]
    public class when_a_tab_class_whose_name_matches_existing_tab_but_with_different_RequiredAccess_exists
        : SynchronizationSpecs
    {
        static int existingTabDefinitionId;
        static AccessLevel accessLevelSpecifiedInTheClass = AccessLevel.Edit;

        Establish context = () =>
        {
            TabDefinition existingTab = new TabDefinition
            {
                Name = "NameOfExistingTab",
                RequiredAccess = AccessLevel.Publish,
                SortIndex = 0,
            };

            SyncContext.TabDefinitionRepository.SaveTabDefinition(existingTab);
            existingTabDefinitionId = existingTab.ID;
            SyncContext.TabDefinitionRepository.ResetNumberOfSaves();

            var tabClass = TabClassFactory.CreateTabClass(
                "NameOfClass",
                existingTab.Name,
                accessLevelSpecifiedInTheClass,
                existingTab.SortIndex);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_TabDefinition_to_have_the_RequiredAccess_specified_by_the_class = () =>
            SyncContext.TabDefinitionRepository.List().First(t => t.ID == existingTabDefinitionId).RequiredAccess
                .ShouldEqual(accessLevelSpecifiedInTheClass);
    }

    [Subject("Synchronization")]
    public class when_a_tab_class_whose_name_matches_existing_tab_but_with_different_SortIndex_exists
        : SynchronizationSpecs
    {
        static int existingTabDefinitionId;
        static int sortIndexSpecifiedInTheClass = 1;

        Establish context = () =>
        {
            TabDefinition existingTab = new TabDefinition
            {
                Name = "NameOfExistingTab",
                RequiredAccess = AccessLevel.Publish,
                SortIndex = 0,
            };

            SyncContext.TabDefinitionRepository.SaveTabDefinition(existingTab);
            existingTabDefinitionId = existingTab.ID;
            SyncContext.TabDefinitionRepository.ResetNumberOfSaves();

            var tabClass = TabClassFactory.CreateTabClass(
                "NameOfClass",
                existingTab.Name,
                existingTab.RequiredAccess,
                sortIndexSpecifiedInTheClass);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_TabDefinition_to_have_the_SortIndex_specified_by_the_class = () =>
            SyncContext.TabDefinitionRepository.List().First(t => t.ID == existingTabDefinitionId).SortIndex
                .ShouldEqual(sortIndexSpecifiedInTheClass);
    }
}
