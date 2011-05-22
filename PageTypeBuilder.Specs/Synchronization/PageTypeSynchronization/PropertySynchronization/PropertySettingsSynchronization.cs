using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using EPiServer.Editor.TinyMCE;
using EPiServer.SpecializedProperties;
using Machine.Specifications;
using PageTypeBuilder.Specs.ExampleImplementations;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.PropertySettingsSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_Attribute_implementing_IPropertySettingsUpdater
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

        It should_create_a_PropertySettingsContainer_with_settings_of_the_type_specified_as_type_parameter_to_IPropertySettingsUpdater =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof (TinyMCESettings).FullName].PropertySettings.
                ShouldBeOfType<TinyMCESettings>();

        It should_create_PropertySettings_of_the_type_specified_as_type_parameter_to_IPropertySettingsUpdater =
            () =>
            SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName)
                .ShouldNotBeNull();

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
    public class when_a_page_type_property_is_annotated_with_IPropertySettingsUpdater_instance_returning_different_HashCodes_for_the_settings_before_and_after_update
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.ModifyPropertySettings = true;
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

        It should_create_settings_modified_by_the_attributes_Update_method =
            () =>
            TinyMceSettingsAttribute.MatchesUpdatedSettings(SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName));
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_IPropertySettingsUpdater_instance_returning_different_HashCodes_for_the_settings_before_and_after_update_and_OverWriteExistingSettings_set_to_true
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.OverWriteExisting = true;
            settingsAttribute.ModifyPropertySettings = true;
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
            
            var existingPropertySettingsWrapper = new PropertySettingsWrapper();
            existingPropertySettingsWrapper.PropertySettings = existingSettings;

            var existingContainer = new PropertySettingsContainer(existingPageDefinition.SettingsID);
            existingContainer.Settings.Add(typeof(TinyMCESettings).FullName, existingPropertySettingsWrapper);
            
            SyncContext.PropertySettingsRepository.Save(existingContainer);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_settings_as_modified_by_the_attributes_Update_method =
            () =>
            TinyMceSettingsAttribute.MatchesUpdatedSettings(SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName));
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_IPropertySettingsUpdater_instance_returning_different_HashCodes_for_the_settings_before_and_after_update_and_OverWriteExistingSettings_set_to_false
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";

        Establish context = () =>
        {
            var propertyAttribute = new PageTypePropertyAttribute();
            var settingsAttribute = new TinyMceSettingsAttribute();
            settingsAttribute.OverWriteExisting = false;
            settingsAttribute.ModifyPropertySettings = true;
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
    public class when_a_page_type_property_is_annotated_with_UseGlobalSettingsAttribute
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettings).Assembly);

            var propertyAttribute = new PageTypePropertyAttribute();
            var useGlobalSettingsAttribute = new UseGlobalSettingsAttribute(typeof(GlobalTinyMceSettings));
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                    prop.Attributes.Add(new AttributeSpecification(useGlobalSettingsAttribute)
                        {
                            Constructor = typeof(UseGlobalSettingsAttribute).GetConstructor(new [] { typeof(Type)}),
                            ConstructorParameters = new object[] {typeof (GlobalTinyMceSettings)}
                        });
                });
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_set_the_propertys_PropertySettingsContainer_to_use_matching_global_settings_for_the_type =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof(TinyMCESettings).FullName]
            .Id.ShouldEqual(SyncContext.PropertySettingsRepository.GetGlobals(typeof(TinyMCESettings))
            .Where(w => w.DisplayName.Equals(new GlobalTinyMceSettings().DisplayName)).First().Id);

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
}
