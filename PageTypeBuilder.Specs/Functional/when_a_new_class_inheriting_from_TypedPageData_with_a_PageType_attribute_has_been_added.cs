using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string className = "MyPageTypeClass";
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
            {
                pageTypeAttribute = new PageTypeAttribute
                {
                    Description = "A description of the page type"
                };

                syncContext.AddTypeInheritingFromTypedPageData(type =>
                {
                    type.Name = className;
                    type.Attributes.Add(pageTypeAttribute);
                });   
            };

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_class =
            () => syncContext.PageTypeFactory.Load(className).ShouldNotBeNull();

        It should_create_a_new_page_type_with_the_description_entered_in_the_PageType_attribute =
            () => syncContext.PageTypeFactory.Load(className).Description.ShouldEqual(pageTypeAttribute.Description);
    }
}
