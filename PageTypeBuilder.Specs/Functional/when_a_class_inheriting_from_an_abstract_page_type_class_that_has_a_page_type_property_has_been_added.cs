using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_class_inheriting_from_an_abstract_page_type_class_that_has_a_page_type_property_has_been_added
    {
        static InMemoryContext syncContext = new InMemoryContext();
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
                                property.Attributes.Add(propertyAttribute);
                            });
                    });

                syncContext.AssemblyLocator.Add(abstractClass.Assembly);

                var concrete = ((ModuleBuilder)abstractClass.Module).CreateClass(type =>
                {
                    type.Name = className;
                    type.ParentType = abstractClass;
                });
            };

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_page_definition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => syncContext.PageDefinitionFactory.List().First().PageTypeID
                .ShouldEqual(syncContext.PageTypeFactory.List().First().ID);
    }
}
