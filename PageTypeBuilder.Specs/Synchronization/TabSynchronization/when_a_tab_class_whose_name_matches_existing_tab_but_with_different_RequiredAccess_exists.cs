using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization
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

            SyncContext.TabFactory.SaveTabDefinition(existingTab);
            existingTabDefinitionId = existingTab.ID;
            SyncContext.TabFactory.ResetNumberOfSaves();

            TabClassFactory.CreateTabClass(
                "NameOfClass",
                existingTab.Name,
                accessLevelSpecifiedInTheClass,
                existingTab.SortIndex);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_TabDefinition_to_have_the_RequiredAccess_specified_by_the_class = () =>
            SyncContext.TabFactory.List().First(t => t.ID == existingTabDefinitionId).RequiredAccess
                .ShouldEqual(accessLevelSpecifiedInTheClass);
    }
}
