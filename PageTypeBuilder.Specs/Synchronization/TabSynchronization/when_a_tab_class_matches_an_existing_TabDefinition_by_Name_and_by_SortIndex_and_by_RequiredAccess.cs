using EPiServer.DataAbstraction;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization
{
    [Subject("Synchronization")]
    public class when_a_tab_class_matches_an_existing_TabDefinition_by_Name_and_by_SortIndex_and_by_RequiredAccess
        : SynchronizationSpecs
    {
        static int existingTabDefinitionId;

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

            var tabClass = TabClassFactory.CreateTabClass(
                "NameOfClass",
                existingTab.Name,
                existingTab.RequiredAccess,
                existingTab.SortIndex);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_save_the_existing_TabDefinition= () =>
            SyncContext.TabFactory.GetNumberOfSaves(existingTabDefinitionId).ShouldEqual(0);
    }
}
