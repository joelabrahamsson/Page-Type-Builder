using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.ValueSetting
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
            propertyAttribute.DefaultValue = "Specified default value";
            propertyAttribute.DefaultValueType = DefaultValueType.Value;
            propertyAttribute.DisplayInEditMode = false;
            propertyAttribute.EditCaption = "Property's Edit Caption";
            propertyAttribute.HelpText = "Property's help text";
            propertyAttribute.Required = true;
            propertyAttribute.Searchable = true;
            propertyAttribute.SortOrder = 123;
            propertyAttribute.UniqueValuePerLanguage = true;

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                }));
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_PageDefinition =
            () => SyncContext.PageDefinitionRepository.List().ShouldNotBeEmpty();

        It should_create_a_PageDefinition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => SyncContext.PageDefinitionRepository.List().First().PageTypeID
                .ShouldEqual(SyncContext.PageTypeRepository.List().First().ID);

        It should_create_a_PageDefinition_whose_name_equals_the_propertys_name =
            () => SyncContext.PageDefinitionRepository.List().First().Name.ShouldEqual(propertyName);

        It should_create_a_PageDefinition_whose_HelpText_equals_the_attributes_HelpText =
            () => SyncContext.PageDefinitionRepository.List().First()
                .HelpText.ShouldEqual(propertyAttribute.HelpText);

        It should_create_a_PageDefinition_whose_EditCaption_equals_the_attributes_EditCaption =
            () => SyncContext.PageDefinitionRepository.List().First().EditCaption.ShouldEqual(propertyAttribute.EditCaption);

        It should_create_a_PageDefinition_whose_FieldOrder_equals_the_attributes_SortOrder =
            () => SyncContext.PageDefinitionRepository.List().First().FieldOrder.ShouldEqual(propertyAttribute.SortOrder);

        It should_create_a_PageDefinition_whose_DefaultValue_equals_the_attributes_DefaultValue =
            () => SyncContext.PageDefinitionRepository.List().First()
                .DefaultValue.ShouldEqual(propertyAttribute.DefaultValue);

        It should_create_a_PageDefinition_whose_DefaultValueType_equals_the_attributes_DefaultValueType =
            () => SyncContext.PageDefinitionRepository.List().First()
                .DefaultValueType.ShouldEqual(propertyAttribute.DefaultValueType);

        It should_create_a_PageDefinition_whose_DisplayEditUI_equals_the_attributes_DisplayInEditMode =
            () => SyncContext.PageDefinitionRepository.List().First()
                .DisplayEditUI.ShouldEqual(propertyAttribute.DisplayInEditMode);

        It should_create_a_PageDefinition_whose_Required_property_equals_the_attributes_Required_property =
            () => SyncContext.PageDefinitionRepository.List().First()
                .Required.ShouldEqual(propertyAttribute.Required);

        It should_create_a_PageDefinition_whose_Searchable_property_equals_the_attributes_Searchable_property =
            () => SyncContext.PageDefinitionRepository.List().First()
                .Searchable.ShouldEqual(propertyAttribute.Searchable);

        It should_create_a_PageDefinition_whose_LanguageSpecific_property_equals_the_attributes_UniqueValuePerLanguage_property =
            () => SyncContext.PageDefinitionRepository.List().First()
                .LanguageSpecific.ShouldEqual(propertyAttribute.UniqueValuePerLanguage);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_has_a_property_with_a_tab_specified
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static string tabName = "NameOfTheTab";

        Establish context = () =>
        {
            var tabClass = TabClassFactory.CreateTabClass(
            "NameOfClass", tabName, AccessLevel.Undefined, 0);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);
            PageTypePropertyAttribute propertyAttribute;
            propertyAttribute = new PageTypePropertyAttribute();
            propertyAttribute.Tab = tabClass;

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                }));
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_page_definition_whose_Tab_property_matches_the_specified_Tab =
            () => SyncContext.PageDefinitionRepository.List().First().Tab.ID.ShouldEqual(SyncContext.TabDefinitionRepository.GetTabDefinition(tabName).ID);
    }
}
