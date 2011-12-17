using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using EPiServer.Editor.TinyMCE;
using EPiServer.SpecializedProperties;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.Fakes;
using Refraction;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.PropertySettingsSynchronization
{
    //TODO: Add spec for validating that new settings are created with GetDefaultValues

    // Shared behaviors for:
    // when_a_page_type_property_is_annotated_with_Attribute_implementing_IPropertySettingsUpdater
    // when_a_non_public_page_type_property_is_annotated_with_Attribute_implementing_IPropertySettingsUpdater
    [Behaviors]
    public class PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors
    {
        public const string PropertyName = "MainBody";
        public const string PageTypeName = "PageTypeName";
        public const string SettingsClassName = "MyTinyMceSettings";

        protected static InMemoryContext SyncContext;

        It should_assign_PageDefinitions_SettingsId =
            () =>
            SyncContext.PageDefinitionRepository.List().First().SettingsID.ShouldNotBeNull();

        It should_assign_PageDefinitions_SettingsId_to_non_empty_value =
            () =>
            SyncContext.PageDefinitionRepository.List().First().SettingsID.ShouldNotEqual(Guid.Empty);

        It should_create_a_new_PropertySettingsContainer_with_the_same_Id_as_the_PageDefinitions_SettingsId =
            () =>
            GetPageDefinitionsPropertySettingsContainer().ShouldNotBeNull();

        It should_create_a_PropertySettingsContainer_with_settings_of_the_type_specified_as_type_parameter_to_IPropertySettingsUpdater =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof (TinyMCESettings).FullName].PropertySettings.
                ShouldBeOfType<TinyMCESettings>();

        It should_create_PropertySettings_of_the_type_specified_as_type_parameter_to_IPropertySettingsUpdater =
            () =>
            SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(PropertyName, PageTypeName)
                .ShouldNotBeNull();

        static PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer()
        {
            PropertySettingsContainer container;
            SyncContext.PropertySettingsRepository.TryGetContainer(GetPageDefinitionsPropertySettingsId(), out container);
            return container;
        }

        static Guid GetPageDefinitionsPropertySettingsId()
        {
            return SyncContext.PageDefinitionRepository.List().First().SettingsID;
        }
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_Attribute_implementing_IPropertySettingsUpdater
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                var settingsUpdater = with.TinyMceSettingsAttribute(
                    updateSettingsImplementation: "settings.Width = int.MaxValue",
                    matchMethodBody: "return settings.Width == int.MaxValue",
                    className: PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.SettingsClassName,
                    overwriteExistingSettings: false);

                with.Class(PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.PageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.PropertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith(new CodeTypeReference(settingsUpdater.Name), new object()));
            });
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        Behaves_like<PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors> an_annotated_page_type_property;
    }

    [Subject("Synchronization")]
    public class when_a_non_public_page_type_property_is_annotated_with_Attribute_implementing_IPropertySettingsUpdater
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                var settingsUpdater = with.TinyMceSettingsAttribute(
                    updateSettingsImplementation: "settings.Width = int.MaxValue",
                    matchMethodBody: "return settings.Width == int.MaxValue",
                    className: PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.SettingsClassName,
                    overwriteExistingSettings: false);

                with.Class(PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.PageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        Protected(x.Named(PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors.PropertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith(new CodeTypeReference(settingsUpdater.Name), new object())));
            });
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        Behaves_like<PageTypePropertyAnnotatedWithIPropertySettingsUpdaterAttributeBehaviors> an_annotated_page_type_property;

        // TODO: Should better be part of Refraction's CodeMemberPropertyExtensions
        public static CodeMemberProperty Protected(/* this */ CodeMemberProperty property)
        {
            property.Attributes = property.Attributes | MemberAttributes.Family;
            return property;
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
            var assembly = Create.Assembly(with =>
            {
                var settingsUpdater = with.TinyMceSettingsAttribute(
                    updateSettingsImplementation: "settings.Width = int.MaxValue",
                    matchMethodBody: "return settings.Width == int.MaxValue",
                    overwriteExistingSettings: false);

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(propertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith(new CodeTypeReference(settingsUpdater.Name), new object()));
            });
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_settings_modified_by_the_attributes_Update_method =
            () =>
            SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName).Width.ShouldEqual(int.MaxValue);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_IPropertySettingsUpdater_instance_returning_different_HashCodes_for_the_settings_before_and_after_update_and_OverWriteExistingSettings_set_to_true
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";
        static string settingsClassName = "MyTinyMceSettings";
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                var settingsUpdater = with.TinyMceSettingsAttribute(
                    updateSettingsImplementation: "settings.Width = int.MaxValue",
                    matchMethodBody: "return settings.Width == int.MaxValue",
                    className: settingsClassName);

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(propertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith(new CodeTypeReference(settingsUpdater.Name), new object()));
            });
            SyncContext.AssemblyLocator.Add(assembly);

            var existingPageType = new FakePageType(SyncContext.PageDefinitionRepository);
            existingPageType.Name = pageTypeName;
            SyncContext.PageTypeRepository.Save(existingPageType);

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.SettingsID = Guid.NewGuid();
            SyncContext.PageDefinitionRepository.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeRepository.Save(existingPageType);

            
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
            SyncContext.GetPageDefinitionsPropertySettings<TinyMCESettings>(propertyName, pageTypeName).Width.ShouldEqual(int.MaxValue);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_IPropertySettingsUpdater_instance_returning_different_HashCodes_for_the_settings_before_and_after_update_and_OverWriteExistingSettings_set_to_false
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "NameOfPageType";
        static string settingsClassName = "MyTinyMceSettings";
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                var settingsUpdater = with.TinyMceSettingsAttribute(
                    updateSettingsImplementation: "settings.Width = int.MaxValue",
                    matchMethodBody: "return settings.Width == int.MaxValue",
                    className: settingsClassName,
                    overwriteExistingSettings: false);

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(propertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith(new CodeTypeReference(settingsUpdater.Name), new object()));
            });
            SyncContext.AssemblyLocator.Add(assembly);

            var existingPageType = new FakePageType(SyncContext.PageDefinitionRepository);
            existingPageType.Name = pageTypeName;
            SyncContext.PageTypeRepository.Save(existingPageType);

            var existingPageDefinition = new PageDefinition();
            existingPageDefinition.Type = SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType<PropertyXhtmlString>();
            existingPageDefinition.PageTypeID = existingPageType.ID;
            existingPageDefinition.Name = propertyName;
            existingPageDefinition.EditCaption = propertyName;
            existingPageDefinition.SettingsID = Guid.NewGuid();
            SyncContext.PageDefinitionRepository.Save(existingPageDefinition);

            existingPageType.Definitions.Add(existingPageDefinition);
            SyncContext.PageTypeRepository.Save(existingPageType);

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
        static string settingsDisplayName = Guid.NewGuid().ToString();
        Establish context = () =>
        {
            var assembly = Create.Assembly(with => {
                var settingsUpdater = with.GlobalPropertySettingsClass(
                    className: "MyGlobalTinyMceSettings",
                    displayName: settingsDisplayName);

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(propertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith<UseGlobalSettingsAttribute>(new object(), settingsUpdater));
            });
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_set_the_propertys_PropertySettingsContainer_to_use_matching_global_settings_for_the_type =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof(TinyMCESettings).FullName]
            .Id.ShouldEqual(SyncContext.PropertySettingsRepository.GetGlobals(typeof(TinyMCESettings))
            .Where(w => w.DisplayName.Equals(settingsDisplayName)).First().Id);

        static PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer()
        {
            PropertySettingsContainer container;
            SyncContext.PropertySettingsRepository.TryGetContainer(GetPageDefinitionsPropertySettingsId(), out container);
            return container;
        }

        static Guid GetPageDefinitionsPropertySettingsId()
        {
            return SyncContext.PageDefinitionRepository.List().First().SettingsID;
        }
    }

    //TODO: Should validate that no exception is thrown when settings already assigned
    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_UseGlobalSettingsAttribute2
        : SynchronizationSpecs
    {
        static string propertyName = "MainBody";
        static string pageTypeName = "PageTypeName";
        static string settingsDisplayName = Guid.NewGuid().ToString();

        Establish context = () =>
        {
            var assembly = Create.Assembly(with => {
                var settingsUpdater = with.GlobalPropertySettingsClass(
                    className: "MyGlobalTinyMceSettings",
                    displayName: settingsDisplayName);

                with.Class(pageTypeName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>()
                    .AutomaticProperty<string>(x =>
                        x.Named(propertyName)
                         .AnnotatedWith<PageTypePropertyAttribute>()
                         .AnnotatedWith<UseGlobalSettingsAttribute>(new object(), settingsUpdater));
            });
            SyncContext.AssemblyLocator.Add(assembly);
        };

        private Because of =
            () =>
                {
                    SyncContext.PageTypeSynchronizer.SynchronizePageTypes();
                    SyncContext.PageTypeSynchronizer.SynchronizePageTypes();
                };

        It should_assign_PageDefinitions_SettingsId =
            () =>
            SyncContext.PageDefinitionRepository.List().First().SettingsID.ShouldNotBeNull();

        It should_assign_PageDefinitions_SettingsId_to_non_empty_value =
            () =>
            SyncContext.PageDefinitionRepository.List().First().SettingsID.ShouldNotEqual(Guid.Empty);

        It should_set_the_propertys_PropertySettingsContainer_to_use_matching_global_settings_for_the_type =
            () =>
            GetPageDefinitionsPropertySettingsContainer().Settings[typeof(TinyMCESettings).FullName]
            .Id.ShouldEqual(SyncContext.PropertySettingsRepository.GetGlobals(typeof(TinyMCESettings))
            .Where(w => w.DisplayName.Equals(settingsDisplayName)).First().Id);

        static PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer()
        {
            PropertySettingsContainer container;
            SyncContext.PropertySettingsRepository.TryGetContainer(GetPageDefinitionsPropertySettingsId(), out container);
            return container;
        }

        static Guid GetPageDefinitionsPropertySettingsId()
        {
            return SyncContext.PageDefinitionRepository.List().First().SettingsID;
        }
    }
}
