using System;
using System.Reflection;
using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class when_a_class_inheriting_from_an_abstract_page_type_class_in_the_same_assembly_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static PageTypeAttribute attribute;

        Establish context = () =>
            {
                attribute = new PageTypeAttribute();
                attribute.Description = "Description in base class' attribute";

                Type abstractClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                    {
                        type.Name = "BaseClass";
                        type.TypeAttributes = TypeAttributes.Abstract;
                        type.Attributes.Add(attribute);
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

        It should_create_a_new_page_type_with_a_description_from_the_abstract_class_page_type_attribute =
            () => SyncContext.PageTypeFactory.Load(className).Description.ShouldEqual(attribute.Description);
    }
}
