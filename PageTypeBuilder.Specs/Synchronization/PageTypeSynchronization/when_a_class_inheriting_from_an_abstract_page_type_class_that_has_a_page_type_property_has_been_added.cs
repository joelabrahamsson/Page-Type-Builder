using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_class_inheriting_from_an_abstract_page_type_class_that_has_a_page_type_property_has_been_added
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string propertyName = "PropertyInAbstractClass";
        static Type propertyType = typeof (string);
        static PageTypePropertyAttribute propertyAttribute;

        Establish context = () =>
            {
                propertyAttribute = new PageTypePropertyAttribute();
                Type abstractClass = PageTypeClassFactory.CreatePageTypeClass(type =>
                    {
                        type.TypeAttributes = TypeAttributes.Abstract;

                        type.AddProperty(property =>
                            {
                                property.Name = propertyName;
                                property.Type = propertyType;
                                property.AddAttributeTemplate(propertyAttribute);
                            });
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

        It should_create_a_page_definition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => SyncContext.PageDefinitionFactory.List().First().PageTypeID
                .ShouldEqual(SyncContext.PageTypeFactory.List().First().ID);

        It should_create_a_page_definition_whose_name_equals_the_propertys_name =
            () => SyncContext.PageDefinitionFactory.List().First().Name
                .ShouldEqual(propertyName);
    }
}
