using System;
using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    [Subject("Synchronization")]
    public class when_a_class_inheriting_from_an_abstract_page_type_class_in_another_assembly_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                Type abstractClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                    {
                        type.Name = "BaseClass";
                        type.TypeAttributes = TypeAttributes.Abstract | TypeAttributes.Public;
                        type.Attributes.Add(new PageTypeAttribute());
                    });
                SyncContext.AssemblyLocator.Add(abstractClass.Assembly);

                Type pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                {
                    type.Name = className;
                    type.ParentType = abstractClass;
                });
                SyncContext.AssemblyLocator.Add(pageTypeClass.Assembly);
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_class =
            () => SyncContext.PageTypeFactory.Load(className).ShouldNotBeNull();
    }
}
