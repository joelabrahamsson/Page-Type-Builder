using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_page_type_class_only_has_it_self_as_available_page_type
    {
        static InMemoryContext environmentContext = new InMemoryContext();
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                environmentContext.AddTypeInheritingFromTypedPageData(type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.BeforeAttributeIsAddedToType = (attribute, t) =>
                    {
                        if (!(attribute is PageTypeAttribute))
                            return;

                        ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t };


                    };
                    type.Attributes.Add(new PageTypeAttribute());
                });
            };

        Because of =
            () => environmentContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_page_type_that_has_only_itself_in_its_AllowedPageTypes_property =
            () => environmentContext.PageTypeFactory.Load(className).AllowedPageTypes
                .ShouldContainOnly(environmentContext.PageTypeFactory.Load(className).ID);
    }
}
