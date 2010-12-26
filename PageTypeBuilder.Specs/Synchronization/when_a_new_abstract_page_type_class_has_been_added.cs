using System.Reflection;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class when_a_new_abstract_page_type_class_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";

        Establish context = () => SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
                type.TypeAttributes = TypeAttributes.Abstract;
                type.Attributes.Add(new PageTypeAttribute());
            });

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => SyncContext.PageTypeFactory.List().ShouldBeEmpty();
    }
}
