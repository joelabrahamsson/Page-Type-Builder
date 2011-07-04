namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.ValueSetting
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Security;
    using EPiServer.SpecializedProperties;
    using Machine.Specifications;
    using Abstractions;
    using Helpers;
    using Helpers.TypeBuildingDsl;
    using Refraction;

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

    [Subject("Synchronization")]
    public class when_a_page_type_property_matches_existing_page_type_property_but_everything_specified_in_its_attribute_is_different_than_corresponding_values_in_the_page_type_property_and_no_properties_are_marked_as_being_set
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static string pageTypeName = "PageTypeName";
        static PageTypePropertyAttribute propertyAttribute;
        static string tabName = "NameOfTheTab";

        Establish context = () =>
        {
            var tabClass = TabClassFactory.CreateTabClass(
            "NameOfClass", tabName, AccessLevel.Undefined, 0);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);

            PageTypeAttribute pageTypeAttribute = AttributeHelper.CreatePageTypeAttributeWithOnlyGuidAndNameSpecified(SyncContext);
            pageTypeAttribute.AvailablePageTypes = new Type[] {};

            var assembly = Create.Assembly(with =>
                {
                    with.Class(pageTypeName)
                        .Inheriting<TypedPageData>()
                        .AnnotatedWith<PageTypeAttribute>(
                            new { Name = pageTypeAttribute.Name },
                            pageTypeAttribute.Guid.ToString())
                        .AutomaticProperty<string>(x =>
                                                    x.Named(propertyName)
                                                        .AnnotatedWith
                                                        <PageTypePropertyAttribute>());
                });

            SyncContext.AssemblyLocator.Add(assembly);

            IPageType existingPageType = PageTypeMother.CreatePageTypeWithSameValuesAsAttribute(SyncContext, pageTypeAttribute);

            SyncContext.PageTypeRepository.Save(existingPageType);

            IEnumerable<Assembly> assemblies = SyncContext.AssemblyLocator.GetAssemblies();
            Type pageTypeType = assemblies.ElementAt(1).GetTypes()[0];
            PropertyInfo property = pageTypeType.GetProperty(propertyName);
            propertyAttribute = property.GetCustomAttributes(typeof(PageTypePropertyAttribute), false).First() as PageTypePropertyAttribute;

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.HelpText = "Help text";
            existingPageDefinition.Tab = SyncContext.TabDefinitionRepository.GetTabDefinition(tabName);
            existingPageDefinition.Required = !propertyAttribute.Required;
            existingPageDefinition.Searchable = !propertyAttribute.Searchable;
            existingPageDefinition.DefaultValue = "asdf";
            existingPageDefinition.DefaultValueType = DefaultValueType.Inherit;
            existingPageDefinition.DisplayEditUI = !propertyAttribute.DisplayInEditMode;
            existingPageDefinition.FieldOrder = propertyAttribute.SortOrder + 100;
            existingPageDefinition.LanguageSpecific = !propertyAttribute.UniqueValuePerLanguage;
            SyncContext.PageDefinitionRepository.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeRepository.Save(existingPageType);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_update_page_definition_Type =
            () => SyncContext.PageDefinitionRepository.List().First().Type.ShouldNotBeNull();

        It should_not_update_page_definition_EditCaption =
            () => SyncContext.PageDefinitionRepository.List().First().EditCaption.ShouldNotEqual(propertyAttribute.EditCaption);

        It should_not_update_page_definition_HelpText =
            () => SyncContext.PageDefinitionRepository.List().First().HelpText.ShouldNotEqual(propertyAttribute.HelpText);

        It should_not_update_page_definition_Tab =
            () => SyncContext.PageDefinitionRepository.List().First().Tab.ShouldNotBeNull();

        It should_not_update_page_definition_Required =
            () => SyncContext.PageDefinitionRepository.List().First().Required.ShouldNotEqual(propertyAttribute.Required);

        It should_not_update_page_definition_Searchable =
            () => SyncContext.PageDefinitionRepository.List().First().Searchable.ShouldNotEqual(propertyAttribute.Searchable);

        It should_not_update_page_definition_DefaultValue =
            () => SyncContext.PageDefinitionRepository.List().First().DefaultValue.ShouldNotEqual(propertyAttribute.DefaultValue);

        It should_not_update_page_definition_DefaultValueType =
            () => SyncContext.PageDefinitionRepository.List().First().DefaultValueType.ShouldNotEqual(propertyAttribute.DefaultValueType);

        It should_not_update_page_definition_LanguageSpecific =
            () => SyncContext.PageDefinitionRepository.List().First().LanguageSpecific.ShouldNotEqual(propertyAttribute.UniqueValuePerLanguage);

        It should_not_update_page_definition_DisplayEditUI =
            () => SyncContext.PageDefinitionRepository.List().First().DisplayEditUI.ShouldNotEqual(propertyAttribute.DisplayInEditMode);

        It should_not_update_page_definition_FieldOrder =
            () => SyncContext.PageDefinitionRepository.List().First().FieldOrder.ShouldNotEqual(propertyAttribute.SortOrder);

    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_matches_existing_page_type_property_but_everything_specified_in_its_attribute_is_different_than_corresponding_values_in_the_page_type_property_and_all_properties_are_marked_as_being_set
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static string pageTypeName = "PageTypeName";
        static PageTypePropertyAttribute propertyAttribute;
        static string tabName = "TabOne";
        static string secondTabName = "TabTwo";

        Establish context = () =>
        {
            TabDefinition tabOne = new TabDefinition
            {
                Name = tabName,
                RequiredAccess = AccessLevel.Publish,
                SortIndex = 0,
            };

            SyncContext.TabDefinitionRepository.SaveTabDefinition(tabOne);

            TabDefinition tabTwo = new TabDefinition
            {
                Name = secondTabName,
                RequiredAccess = AccessLevel.Publish,
                SortIndex = 0,
            };

            SyncContext.TabDefinitionRepository.SaveTabDefinition(tabTwo);

            PageTypeAttribute pageTypeAttribute = AttributeHelper.CreatePageTypeAttributeWithOnlyGuidAndNameSpecified(SyncContext);
            pageTypeAttribute.AvailablePageTypes = new Type[] { };

            var assembly = Create.Assembly(with =>
            {
                var tab = with.Class(secondTabName)
                    .Inheriting<Tab>()
                    .Property<string>(x => x.Named("Name").GetterBody(string.Format("return \"{0}\";", secondTabName)).IsOverride())
                    .Property<AccessLevel>(x => x.Named("RequiredAccess").GetterBody(string.Format("return {0};", 0)).IsOverride())
                    .Property<int>(x => x.Named("SortIndex").GetterBody(string.Format("return {0};", 100)).IsOverride());

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(
                        new { Name = pageTypeAttribute.Name },
                        pageTypeAttribute.Guid.ToString())
                    .AutomaticProperty<string>(x =>
                                                x.Named(propertyName)
                                                    .AnnotatedWith
                                                    <PageTypePropertyAttribute>(new
                                                    {
                                                        Type = typeof(PropertyUrl),
                                                        EditCaption = propertyName + "New",
                                                        HelpText = "This is the updated help text",
                                                        Tab = tab,
                                                        Required = true,
                                                        Searchable = true,
                                                        DefaultValue = "This is the new default value",
                                                        DefaultValueType = DefaultValueType.Value,
                                                        DisplayInEditMode = true,
                                                        SortOrder = 200,
                                                        UniqueValuePerLanguage = true
                                                    }));
            });

            SyncContext.AssemblyLocator.Add(assembly);

            IPageType existingPageType = PageTypeMother.CreatePageTypeWithSameValuesAsAttribute(SyncContext, pageTypeAttribute);

            SyncContext.PageTypeRepository.Save(existingPageType);

            IEnumerable<Assembly> assemblies = SyncContext.AssemblyLocator.GetAssemblies();
            Type pageTypeType = assemblies.ElementAt(assemblies.Count() - 1).GetTypes()[1];
            PropertyInfo property = pageTypeType.GetProperty(propertyName);
            propertyAttribute = property.GetCustomAttributes(typeof(PageTypePropertyAttribute), false).First() as PageTypePropertyAttribute;

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType<PropertyString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.HelpText = "Help text";
            existingPageDefinition.Tab = SyncContext.TabDefinitionRepository.GetTabDefinition(tabName);
            existingPageDefinition.Required = !propertyAttribute.Required;
            existingPageDefinition.Searchable = !propertyAttribute.Searchable;
            existingPageDefinition.DefaultValue = "asdf";
            existingPageDefinition.DefaultValueType = DefaultValueType.Inherit;
            existingPageDefinition.DisplayEditUI = !propertyAttribute.DisplayInEditMode;
            existingPageDefinition.FieldOrder = propertyAttribute.SortOrder + 100;
            existingPageDefinition.LanguageSpecific = !propertyAttribute.UniqueValuePerLanguage;
            SyncContext.PageDefinitionRepository.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeRepository.Save(existingPageType);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_page_definition_Type =
            () => SyncContext.PageDefinitionRepository.List().First().Type.ID.ShouldEqual(SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType(propertyAttribute.Type).ID);

        It should_update_page_definition_EditCaption =
            () => SyncContext.PageDefinitionRepository.List().First().EditCaption.ShouldEqual(propertyAttribute.EditCaption);

        It should_update_page_definition_HelpText =
            () => SyncContext.PageDefinitionRepository.List().First().HelpText.ShouldEqual(propertyAttribute.HelpText);

        It should_update_page_definition_Tab =
            () => SyncContext.PageDefinitionRepository.List().First().Tab.ID.ShouldEqual(SyncContext.TabDefinitionRepository.GetTabDefinition(secondTabName).ID);

        It should_update_page_definition_Required =
            () => SyncContext.PageDefinitionRepository.List().First().Required.ShouldEqual(propertyAttribute.Required);

        It should_update_page_definition_Searchable =
            () => SyncContext.PageDefinitionRepository.List().First().Searchable.ShouldEqual(propertyAttribute.Searchable);

        It should_update_page_definition_DefaultValue =
            () => SyncContext.PageDefinitionRepository.List().First().DefaultValue.ShouldEqual(propertyAttribute.DefaultValue);

        It should_update_page_definition_DefaultValueType =
            () => SyncContext.PageDefinitionRepository.List().First().DefaultValueType.ShouldEqual(propertyAttribute.DefaultValueType);

        It should_update_page_definition_LanguageSpecific =
            () => SyncContext.PageDefinitionRepository.List().First().LanguageSpecific.ShouldEqual(propertyAttribute.UniqueValuePerLanguage);

        It should_update_page_definition_DisplayEditUI =
            () => SyncContext.PageDefinitionRepository.List().First().DisplayEditUI.ShouldEqual(propertyAttribute.DisplayInEditMode);

        It should_update_page_definition_FieldOrder =
            () => SyncContext.PageDefinitionRepository.List().First().FieldOrder.ShouldEqual(propertyAttribute.SortOrder);
    }
}
