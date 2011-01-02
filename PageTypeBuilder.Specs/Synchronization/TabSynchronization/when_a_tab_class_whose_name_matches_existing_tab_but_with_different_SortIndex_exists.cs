using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization
{
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

            SyncContext.TabFactory.SaveTabDefinition(existingTab);
            existingTabDefinitionId = existingTab.ID;
            SyncContext.TabFactory.ResetNumberOfSaves();

            TabClassFactory.CreateTabClass(
                "NameOfClass",
                existingTab.Name,
                existingTab.RequiredAccess,
                sortIndexSpecifiedInTheClass);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_TabDefinition_to_have_the_SortIndex_specified_by_the_class = () =>
            SyncContext.TabFactory.List().First(t => t.ID == existingTabDefinitionId).SortIndex
                .ShouldEqual(sortIndexSpecifiedInTheClass);
    }
}
