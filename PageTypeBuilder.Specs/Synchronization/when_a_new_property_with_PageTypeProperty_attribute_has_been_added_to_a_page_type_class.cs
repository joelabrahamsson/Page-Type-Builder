using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    [Subject("Synchronization")]
    public class when_a_new_property_with_PageTypePropertyAttribute_has_been_added_to_a_page_type_class
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static PageTypePropertyAttribute propertyAttribute;

        Establish context = () =>
            {
                propertyAttribute = new PageTypePropertyAttribute();
                propertyAttribute.EditCaption = "Property's Edit Caption";
                propertyAttribute.HelpText = "Property's help text";
                propertyAttribute.EditCaption = "Property's edit caption";
                propertyAttribute.SortOrder = 123;

                SyncContext.AddPageTypeClass(type => 
                    type.AddProperty(prop =>
                    {
                        prop.Name = propertyName;
                        prop.Type = typeof (string);
                        prop.AddAttributeTemplate(propertyAttribute);
                    }));
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => SyncContext.PageDefinitionFactory.List().ShouldNotBeEmpty();

        It should_create_a_page_definition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => SyncContext.PageDefinitionFactory.List().First().PageTypeID
                .ShouldEqual(SyncContext.PageTypeFactory.List().First().ID);

        It should_create_a_page_definition_with_a_name_equal_to_the_propertys_name =
            () => SyncContext.PageDefinitionFactory.List().First().Name.ShouldEqual(propertyName);

        It should_create_a_page_definition_with_a_help_text_equal_to_the_attributes =
            () => SyncContext.PageDefinitionFactory.List().First().HelpText.ShouldEqual(propertyAttribute.HelpText);

        It should_create_a_page_definition_with_an_edit_caption_equal_to_the_attributes =
            () => SyncContext.PageDefinitionFactory.List().First().EditCaption.ShouldEqual(propertyAttribute.EditCaption);

        It should_create_a_page_definition_with_FieldOrder_equal_to_the_attributes_SortOrder =
            () => SyncContext.PageDefinitionFactory.List().First().FieldOrder.ShouldEqual(propertyAttribute.SortOrder);

    }
}
