using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_class_inheriting_from_Tab_has_been_added
        : SynchronizationSpecs
    {
        static string tabName = "ValueOfNameProperty";
        static AccessLevel requiredAccess = AccessLevel.Publish;
        static int sortIndex = 12;

        Establish context = () =>
            {
                var tabClass =TabClassFactory.CreateTabClass(
                    "NameOfClass",
                    tabName,
                    requiredAccess,
                    sortIndex);
            
                SyncContext.AssemblyLocator.Add(tabClass.Assembly);
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_TabDefinition_with_the_name_returned_by_the_class_Name_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).ShouldNotBeNull();

        It should_create_a_TabDefinition_with_RequiredAccess_equal_to_the_returned_value_by_the_class_RequiredAccess_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).RequiredAccess.ShouldEqual(requiredAccess);

        It should_create_a_TabDefinition_with_SortIndex_equal_to_the_value_returned_by_the_class_SortIndex_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).SortIndex.ShouldEqual(sortIndex);
    }
}
