using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_has_it_self_and_another_page_type_class_as_available_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";

        Establish context = () =>
            {
                var module = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");
                var anotherModule = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AnotherAssembly");
                var another = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(anotherModule, type =>
                    {
                        type.Name = otherClassName;
                        type.AddAttributeTemplate(new PageTypeAttribute());
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
                    type.AddAttributeTemplate(new PageTypeAttribute());
                });

                SyncContext.AssemblyLocator.Add(typeBuilder.Assembly);
                SyncContext.AssemblyLocator.Add(another.Assembly);    
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_both_page_types_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeFactory.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeFactory.Load(className).ID,
                    SyncContext.PageTypeFactory.Load(otherClassName).ID);
    }
}
