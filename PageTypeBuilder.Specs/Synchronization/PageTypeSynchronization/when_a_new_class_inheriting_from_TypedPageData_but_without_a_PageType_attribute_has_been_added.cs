using System.Linq;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_class_inheriting_from_TypedPageData_but_without_a_PageType_attribute_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static int numberOfPageTypesBeforeSynchronization;

        Establish context = () => SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
            });

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => SyncContext.PageTypeFactory.List().Count()
                .ShouldEqual(numberOfPageTypesBeforeSynchronization);
    }
}
