using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class when_a_new_class_inheriting_from_TypedPageData_but_without_a_PageType_attribute_has_been_added
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string className = "MyPageTypeClass";

        Establish context = () => syncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
            });

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => syncContext.PageTypeFactory.List().ShouldBeEmpty();
    }
}
