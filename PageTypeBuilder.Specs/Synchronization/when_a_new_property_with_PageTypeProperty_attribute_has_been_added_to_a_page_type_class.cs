using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class when_a_new_property_with_PageTypePropertyAttribute_has_been_added_to_a_page_type_class
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static string propertyName = "PropertyName";
        static PageTypePropertyAttribute propertyAttribute;

        Establish context = () =>
            {
                propertyAttribute = new PageTypePropertyAttribute();
                propertyAttribute.EditCaption = "Property's Edit Caption";
                propertyAttribute.HelpText = "Property's help text";
                propertyAttribute.EditCaption = "Property's edit caption";
                propertyAttribute.SortOrder = 123;

                syncContext.AddPageTypeClass(type => 
                    type.AddProperty(prop =>
                    {
                        prop.Name = propertyName;
                        prop.Type = typeof (string);
                        prop.Attributes = new List<Attribute> {propertyAttribute};
                    }));
            };

        Because of =
            () => syncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => syncContext.PageDefinitionFactory.List().ShouldNotBeEmpty();

        It should_create_a_page_definition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => syncContext.PageDefinitionFactory.List().First().PageTypeID
                .ShouldEqual(syncContext.PageTypeFactory.List().First().ID);

        It should_create_a_page_definition_with_a_name_equal_to_the_propertys_name =
            () => syncContext.PageDefinitionFactory.List().First().Name.ShouldEqual(propertyName);

        It should_create_a_page_definition_with_a_help_text_equal_to_the_attributes =
            () => syncContext.PageDefinitionFactory.List().First().HelpText.ShouldEqual(propertyAttribute.HelpText);

        It should_create_a_page_definition_with_an_edit_caption_equal_to_the_attributes =
            () => syncContext.PageDefinitionFactory.List().First().EditCaption.ShouldEqual(propertyAttribute.EditCaption);

        It should_create_a_page_definition_with_FieldOrder_equal_to_the_attributes_SortOrder =
            () => syncContext.PageDefinitionFactory.List().First().FieldOrder.ShouldEqual(propertyAttribute.SortOrder);

    }
}
