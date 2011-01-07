using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
            {
                pageTypeAttribute = new PageTypeAttribute();

                SyncContext.AddTypeInheritingFromTypedPageData(type =>
                {
                    type.Name = className;
                    type.AddAttributeTemplate(pageTypeAttribute);
                });   
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_class =
            () => SyncContext.PageTypeFactory.Load(className).ShouldNotBeNull();
    }
}
