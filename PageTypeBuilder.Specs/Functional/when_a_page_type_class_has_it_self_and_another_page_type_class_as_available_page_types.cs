using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_page_type_class_has_it_self_and_another_page_type_class_as_available_page_types
    {
        static InMemoryContext environmentContext = new InMemoryContext();
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
                TypeBuilder typeBuilder = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(module, type =>
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

                ((AssemblyBuilder)another.Assembly).Save("AnotherAssembly.dll");
                environmentContext.AssemblyLocator.Add(typeBuilder.Assembly);
                environmentContext.AssemblyLocator.Add(another.Assembly);    
            };

        Because of =
            () => environmentContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_both_page_types_in_its_AllowedPageTypes_property =
            () => environmentContext.PageTypeFactory.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    environmentContext.PageTypeFactory.Load(className).ID,
                    environmentContext.PageTypeFactory.Load(otherClassName).ID);
    }
}
