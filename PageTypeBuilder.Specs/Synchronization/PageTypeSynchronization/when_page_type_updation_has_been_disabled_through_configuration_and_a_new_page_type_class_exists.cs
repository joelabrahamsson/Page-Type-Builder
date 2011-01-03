using System.Linq;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_page_type_updation_has_been_disabled_through_configuration_and_a_new_page_type_class_exists
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AddPageTypeClassToAppDomain(type =>
                {
                    type.Name = "NameOfThePageTypeClass";
                });

            SyncContext.Configuration.SetDisablePageTypeUpdation(true);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => SyncContext.PageTypeFactory.List().Count().ShouldEqual(0);
    }
}
