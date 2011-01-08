using System.Linq;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added
        : SynchronizationSpecs
    {
        static PageTypeAttribute pageTypeAttribute;
        static int numberOfPageTypesBeforeSynchronization;

        Establish context = () =>
            {
                pageTypeAttribute = new PageTypeAttribute();

                SyncContext.AddTypeInheritingFromTypedPageData(type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.AddAttributeTemplate(pageTypeAttribute);
                });   
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type =
            () => SyncContext.PageTypeFactory.List().Count()
                .ShouldEqual(numberOfPageTypesBeforeSynchronization + 1);
    }
}
