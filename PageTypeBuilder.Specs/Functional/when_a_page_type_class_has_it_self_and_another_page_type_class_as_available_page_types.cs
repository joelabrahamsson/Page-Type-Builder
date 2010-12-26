using System;
using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_page_type_class_has_it_self_and_another_page_type_class_as_available_page_types
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";

        Establish context = () =>
            {
                var module = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");
                var anotherModule = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AnotherAssembly");
                var another = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(anotherModule, type =>
                    {
                        type.Name = otherClassName;
                        type.Attributes.Add(new PageTypeAttribute());
                    });
                Type typeBuilder = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(module, type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.BeforeAttributeIsAddedToType = (attribute, t) =>
                    {
                        if (!(attribute is PageTypeAttribute))
                            return;

                        ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t, another };


                    };
                    type.Attributes.Add(new PageTypeAttribute());
                });

                syncContext.AssemblyLocator.Add(typeBuilder.Assembly);
                syncContext.AssemblyLocator.Add(another.Assembly);    
            };

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_both_page_types_in_its_AllowedPageTypes_property =
            () => syncContext.PageTypeFactory.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    syncContext.PageTypeFactory.Load(className).ID,
                    syncContext.PageTypeFactory.Load(otherClassName).ID);
    }
}
