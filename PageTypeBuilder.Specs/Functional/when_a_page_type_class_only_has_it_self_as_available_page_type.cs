using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_page_type_class_only_has_it_self_as_available_page_type
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                 var pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                    {
                        type.Name = "MyPageTypeClass";
                        type.BeforeAttributeIsAddedToType = (attribute, t) =>
                            {
                                if (!(attribute is PageTypeAttribute))
                                    return;

                                ((PageTypeAttribute) attribute).AvailablePageTypes = new[] {t};


                            };
                        type.Attributes.Add(new PageTypeAttribute());
                    });

                 syncContext.AssemblyLocator.Add(pageTypeClass.Assembly);
            };


        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_page_type_that_has_only_itself_in_its_AllowedPageTypes_property =
            () => syncContext.PageTypeFactory.Load(className).AllowedPageTypes
                .ShouldContainOnly(syncContext.PageTypeFactory.Load(className).ID);
    }
}
