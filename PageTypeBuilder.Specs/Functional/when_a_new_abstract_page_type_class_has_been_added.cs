using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_abstract_page_type_class_has_been_added
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string className = "MyPageTypeClass";

        Establish context = () => syncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
                type.TypeAttributes = TypeAttributes.Abstract;
                type.Attributes.Add(new PageTypeAttribute());
            });

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => syncContext.PageTypeFactory.List().ShouldBeEmpty();
    }
}
