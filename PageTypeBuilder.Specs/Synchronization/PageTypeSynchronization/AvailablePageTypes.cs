using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.AvailablePageTypes
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_only_has_it_self_as_available_page_type
        : SynchronizationSpecs
    {
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

                    ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t };


                };
                type.AddAttributeTemplate(new PageTypeAttribute());
            });

            SyncContext.AssemblyLocator.Add(pageTypeClass.Assembly);
        };


        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_page_type_that_has_only_itself_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeFactory.Load(className).AllowedPageTypes
                .ShouldContainOnly(SyncContext.PageTypeFactory.Load(className).ID);
    }

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

        It should_ensure_that_the_corresponding_page_type_has_the_IDs_of_those_two_page_types_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeFactory.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeFactory.Load(className).ID,
                    SyncContext.PageTypeFactory.Load(otherClassName).ID);
    }
}
