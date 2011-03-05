using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using EPiServer.Editor.TinyMCE;
using EPiServer.SpecializedProperties;
using Machine.Specifications;
using PageTypeBuilder.PropertySettings;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.PropertySettingsSynchronization
{
    //Test width, height, ContentCss, NonVisualPlugins, OverWriteExistingSettings
    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_TinyMceSettingsAttribute
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                    prop.AddAttributeTemplate(settingsAttribute);
                });
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_assign_PageDefinitions_SettingsId =
            () =>
            SyncContext.PageDefinitionFactory.List().First().SettingsID.ShouldNotBeNull();

        It should_create_a_new_PropertySettingsContainer_with_the_same_Id_as_the_PageDefinitions_SettingsId =
            () =>
            GetPageDefinitionsPropertySettingsContainer().ShouldNotBeNull();

        It should_create_a_PropertySettingsContainer_with_settings_of_type_TinyMCESettings =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof (TinyMCESettings).FullName].PropertySettings.
                ShouldBeOfType<TinyMCESettings>();

        It should_create_TinyMCESettings_with_no_ToolbarRows =
            () =>
                SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ToolbarRows.Count.ShouldEqual(0);

        static PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer()
        {
            PropertySettingsContainer container;
            SyncContext.PropertySettingsRepository.TryGetContainer(GetPageDefinitionsPropertySettingsId(), out container);
            return container;
        }

        static Guid GetPageDefinitionsPropertySettingsId()
        {
            return SyncContext.PageDefinitionFactory.List().First().SettingsID;
        }
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_TinyMceSettingsAttribute_with_bold_specified_in_the_first_ToolbarRow
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.FirstToolbarRow = new[] { "bold" };
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                    prop.AddAttributeTemplate(settingsAttribute);
                });
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_TinyMCESettings_with_a_bold_button_in_the_first_ToolbarRow =
            () =>
                SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ToolbarRows.First().Buttons.ShouldContainOnly("bold");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_TinyMceSettingsAttribute_with_only_bold_specified_in_the_third_ToolbarRow
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";
        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.ThirdToolbarRow = new[] { "bold" };
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                {
                    type.Name = pageTypeName;
                    type.AddProperty(prop =>
                        {
                            prop.Name = propertyName;
                            prop.Type = typeof (string);
                            prop.AddAttributeTemplate(propertyAttribute);
                            prop.AddAttributeTemplate(settingsAttribute);
                        });
                });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_TinyMCESettings_with_a_bold_button_in_the_first_ToolbarRow =
            () =>
                SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ToolbarRows.First().Buttons.ShouldContainOnly("bold");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_annotated_with_TinyMceSettingsAttribute_with_one_ToolbarRow_and_OverWriteExistingSettings_set_to_true_already_has_TinyMceSettings_with_two_ToolbarRows
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.FirstToolbarRow = new[] { "bold" };
            settingsAttribute.OverWriteExistingSettings = true;
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                {
                    type.Name = pageTypeName;

                    type.AddProperty(prop =>
                        {
                            prop.Name = propertyName;
                            prop.Type = typeof (string);
                            prop.AddAttributeTemplate(propertyAttribute);
                            prop.AddAttributeTemplate(settingsAttribute);
                        });
                });

            var existingPageType = new FakePageType();
            existingPageType.Name = pageTypeName;
            SyncContext.PageTypeFactory.Save(existingPageType);

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeFactory.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.SettingsID = Guid.NewGuid();
            SyncContext.PageDefinitionFactory.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeFactory.Save(existingPageType);

            
            var existingSettings = new TinyMCESettings();
            existingSettings.ToolbarRows = new List<ToolbarRow>();

            var existingFirstToolbarRow = new ToolbarRow();
            existingFirstToolbarRow.Buttons.Add("unlink");
            existingSettings.ToolbarRows.Add(existingFirstToolbarRow);
            
            var existingSecondToolbarRow = new ToolbarRow();
            existingSecondToolbarRow.Buttons.Add("separator");
            existingSettings.ToolbarRows.Add(existingSecondToolbarRow);
            
            var existingPropertySettingsWrapper = new PropertySettingsWrapper();
            existingPropertySettingsWrapper.PropertySettings = existingSettings;

            var existingContainer = new PropertySettingsContainer(existingPageDefinition.SettingsID);
            existingContainer.Settings.Add(typeof(TinyMCESettings).FullName, existingPropertySettingsWrapper);
            
            SyncContext.PropertySettingsRepository.Save(existingContainer);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_existing_settings_to_have_a_single_ToolbarRow =
            () =>
                SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ToolbarRows.Count.ShouldEqual(1);

        It should_update_existing_settings_to_have_a_bold_button_in_the_first_ToolbarRow =
            () =>
                SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ToolbarRows.First().Buttons.ShouldContainOnly("bold");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_annotated_with_TinyMceSettingsAttribute_already_has_TinyMceSettings_settings_matching_the_attribute
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.FirstToolbarRow = new[] { "bold" };
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;

                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                    prop.AddAttributeTemplate(settingsAttribute);
                });
            });

            var existingPageType = new FakePageType();
            existingPageType.Name = pageTypeName;
            SyncContext.PageTypeFactory.Save(existingPageType);

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeFactory.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.SettingsID = Guid.NewGuid();
            SyncContext.PageDefinitionFactory.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeFactory.Save(existingPageType);

            var existingSettings = new TinyMCESettings();
            existingSettings.ToolbarRows = new List<ToolbarRow>();

            var existingFirstToolbarRow = new ToolbarRow();
            existingFirstToolbarRow.Buttons.Add("bold");
            existingSettings.ToolbarRows.Add(existingFirstToolbarRow);

            var existingPropertySettingsWrapper = new PropertySettingsWrapper();
            existingPropertySettingsWrapper.PropertySettings = existingSettings;

            var existingContainer = new PropertySettingsContainer(existingPageDefinition.SettingsID);
            existingContainer.Settings.Add(typeof(TinyMCESettings).FullName, existingPropertySettingsWrapper);

            SyncContext.PropertySettingsRepository.Save(existingContainer);
            SyncContext.PropertySettingsRepository.ResetNumberOfSaves();
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_save_the_settings =
            () =>
            SyncContext.PropertySettingsRepository.GetNumberOfSaves(
                SyncContext.GetPageDefinition(propertyName, pageTypeName).SettingsID).ShouldEqual(0);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_annotated_with_TinyMceSettingsAttribute_with_OverWriteExistingSettings_set_to_false_already_has_TinyMceSettings_with_different_values
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.FirstToolbarRow = new[] { "bold" };
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;

                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                    prop.AddAttributeTemplate(settingsAttribute);
                });
            });

            var existingPageType = new FakePageType();
            existingPageType.Name = pageTypeName;
            SyncContext.PageTypeFactory.Save(existingPageType);

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeFactory.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.SettingsID = Guid.NewGuid();
            SyncContext.PageDefinitionFactory.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeFactory.Save(existingPageType);

            var existingSettings = new TinyMCESettings();
            existingSettings.ToolbarRows = new List<ToolbarRow>();

            var existingFirstToolbarRow = new ToolbarRow();
            existingFirstToolbarRow.Buttons.Add("unlink");
            existingSettings.ToolbarRows.Add(existingFirstToolbarRow);

            var existingPropertySettingsWrapper = new PropertySettingsWrapper();
            existingPropertySettingsWrapper.PropertySettings = existingSettings;

            var existingContainer = new PropertySettingsContainer(existingPageDefinition.SettingsID);
            existingContainer.Settings.Add(typeof(TinyMCESettings).FullName, existingPropertySettingsWrapper);

            SyncContext.PropertySettingsRepository.Save(existingContainer);
            SyncContext.PropertySettingsRepository.ResetNumberOfSaves();
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_save_the_settings =
            () =>
            SyncContext.PropertySettingsRepository.GetNumberOfSaves(
                SyncContext.GetPageDefinition(propertyName, pageTypeName).SettingsID).ShouldEqual(0);
    }
}
