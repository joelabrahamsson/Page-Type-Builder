using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using Refraction;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Updating
{
    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_matches_existing_settings
        : SynchronizationSpecs
    {
        static string settingsDisplayName = "settings display name";
        static string settingsDescription = "settings description";
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(
                displayName: settingsDisplayName, 
                description: settingsDescription,
                isDefault: true,
                updateSettingsImplementation: "settings.Width = int.MaxValue;"));
            SyncContext.AssemblyLocator.Add(assembly);

            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(settingsDisplayName, "",
                                                              false, true, settings);
            SyncContext.PropertySettingsRepository.SaveGlobal(existingWrapper);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_settings_using_the_classes_Update_method =
            () =>
            ((TinyMCESettings)
             SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(settingsDisplayName).First().
                 PropertySettings).Width.ShouldEqual(int.MaxValue);

        It should_update_the_PropertySettingsWrappers_description =
            () =>
            SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(settingsDisplayName).First()
                .Description.ShouldEqual(settingsDescription);

        It should_update_the_PropertySettingsWrappers_IsDefault =
            () =>
            SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(settingsDisplayName).First()
                .IsDefault.ShouldBeTrue();
    }

    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_matches_existing_settings_and_returns_false_for_OverwriteExistingsSettings
        : SynchronizationSpecs
    {
        static string settingsDisplayName = Guid.NewGuid().ToString();
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(
                displayName: settingsDisplayName,
                overwriteExistingSettings: false,
                updateSettingsImplementation: "settings.Width = int.MaxValue;"));
            SyncContext.AssemblyLocator.Add(assembly);

            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(settingsDisplayName, "",
                                                              true, true, settings);
            SyncContext.PropertySettingsRepository.SaveGlobal(existingWrapper);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        static IEnumerable<PropertySettingsWrapper> GetGlobalWrappers(Type settingsType, string displayName)
        {
            return
                SyncContext.PropertySettingsRepository.GetGlobals(settingsType).Where(
                    w => w.DisplayName.Equals(displayName));
        }

        It should_not_update_settings =
            () =>
            ((TinyMCESettings) GetGlobalWrappers(typeof (TinyMCESettings), settingsDisplayName).First().PropertySettings)
                .Width
                .ShouldNotEqual(int.MaxValue);
    }

    [Subject("Synchronization")]
    public class when_settings_already_exists_for_the_type_and_no_class_implementing_IUpdateGlobalPropertySettings_match
        : GlobalPropertySettingsSpecsBase
    {
        static string displayName = Guid.NewGuid().ToString();
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(
                updateSettingsImplementation: "settings.Width = int.MaxValue;",
                matchMethodBody: "return false;"));
            SyncContext.AssemblyLocator.Add(assembly);

            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(displayName, "",
                                                              true, true, settings);
            SyncContext.PropertySettingsRepository.SaveGlobal(existingWrapper);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_update_the_settings =
            () =>
            ((TinyMCESettings) GetGlobalWrappersByDisplayName<TinyMCESettings>(displayName).First().PropertySettings)
                .Width.ShouldNotEqual(int.MaxValue);
    }

    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_returns_null_for_IsDefault_and_matching_settings_exists
        : GlobalPropertySettingsSpecsBase
    {
        static string settingsDisplayName = Guid.NewGuid().ToString();
        static string existingsDescription = "Existing wrapper's description";
        static bool existingsIsDefault = true;
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(
                displayName: settingsDisplayName,
                description: null,
                isDefault: null,
                matchMethodBody: "return true;"));
            SyncContext.AssemblyLocator.Add(assembly);

            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(settingsDisplayName, existingsDescription,
                                                              existingsIsDefault, true, settings);
            SyncContext.PropertySettingsRepository.SaveGlobal(existingWrapper);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_update_the_description =
            () =>
            GetGlobalWrappersByDisplayName<TinyMCESettings>(settingsDisplayName).First()
                .Description.ShouldEqual(existingsDescription);

        It should_not_update_the_wrappers_IsDefault_property =
            () =>
            GetGlobalWrappersByDisplayName<TinyMCESettings>(settingsDisplayName).First()
                .IsDefault.ShouldEqual(existingsIsDefault);
    }
}
