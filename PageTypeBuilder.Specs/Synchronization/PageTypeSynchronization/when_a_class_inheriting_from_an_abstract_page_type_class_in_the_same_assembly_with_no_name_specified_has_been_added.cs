using System;
using System.Reflection;
using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_class_inheriting_from_an_abstract_page_type_class_in_the_same_assembly_with_no_name_specified_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                Type abstractClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                    {
                        type.Name = "BaseClass";
                        type.TypeAttributes = TypeAttributes.Abstract;
                        type.AddAttributeTemplate(new PageTypeAttribute());
                    });

                SyncContext.AssemblyLocator.Add(abstractClass.Assembly);

                ((ModuleBuilder)abstractClass.Module).CreateClass(type =>
                {
                    type.Name = className;
                    type.ParentType = abstractClass;
                });
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_concrete_class =
            () => SyncContext.PageTypeFactory.Load(className).ShouldNotBeNull();
    }
}
