using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.ExampleImplementations;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Creation;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Updating
{
    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_matches_existing_settings
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettingsWithIsDefaultReturningTrue).Assembly);
            var instance = new GlobalTinyMceSettingsWithIsDefaultReturningTrue();
            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(instance.DisplayName, "",
                                                              !instance.IsDefault.Value, true, settings);
            SyncContext.PropertySettingsRepository.SaveGlobal(existingWrapper);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_settings_using_the_classes_Update_method =
            () =>
            GlobalTinyMceSettings.MatchesUpdatedSettings((TinyMCESettings)SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(new GlobalTinyMceSettingsWithIsDefaultReturningTrue().DisplayName).First().PropertySettings)
            .ShouldBeTrue();

        It should_update_the_PropertySettingsWrappers_description =
            () =>
            SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(new GlobalTinyMceSettingsWithIsDefaultReturningTrue().DisplayName).First()
                .Description.ShouldEqual(new GlobalTinyMceSettingsWithIsDefaultReturningTrue().Description);

        It should_update_the_PropertySettingsWrappers_IsDefault =
            () =>
            SyncContext.PropertySettingsRepository.GetGlobalWrappers<TinyMCESettings>(new GlobalTinyMceSettingsWithIsDefaultReturningTrue().DisplayName).First()
                .IsDefault.ShouldEqual(new GlobalTinyMceSettingsWithIsDefaultReturningTrue().IsDefault.Value);
    }

    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_matches_existing_settings_and_returns_false_for_OverwriteExistingsSettings
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettingsWithOverwriteExistingSettingsReturningFalse).Assembly);
            var instance = new GlobalTinyMceSettingsWithOverwriteExistingSettingsReturningFalse();
            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(instance.DisplayName, instance.Description,
                                                              instance.IsDefault.GetValueOrDefault(), true, settings);
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
            GlobalTinyMceSettings.MatchesUpdatedSettings((TinyMCESettings)GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettingsWithOverwriteExistingSettingsReturningFalse().DisplayName).First().PropertySettings)
            .ShouldBeFalse();
    }

    [Subject("Synchronization")]
    public class when_settings_already_exists_for_the_type_and_no_class_implementing_IUpdateGlobalPropertySettings_does_not_match
        : SynchronizationSpecs
    {
        static string displayName;
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettings).Assembly);
            var instance = new GlobalTinyMceSettings();
            var settings = new TinyMCESettings();
            displayName = instance.DisplayName + "some more text";
            var existingWrapper = new PropertySettingsWrapper(displayName, instance.Description,
                                                              instance.IsDefault.GetValueOrDefault(), true, settings);
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

        It should_not_update_the_settings =
            () =>
            GlobalTinyMceSettings.MatchesUpdatedSettings((TinyMCESettings)GetGlobalWrappers(typeof(TinyMCESettings), displayName).First().PropertySettings)
            .ShouldBeFalse();
    }

    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_returns_null_for_IsDefault_and_matching_settings_exists
        : SynchronizationSpecs
    {
        static string existingsDescription = "Existing wrapper's description";
        static bool existingsIsDefault = true;
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettingsWithNullValues).Assembly);
            var instance = new GlobalTinyMceSettingsWithNullValues();
            var settings = new TinyMCESettings();
            var existingWrapper = new PropertySettingsWrapper(instance.DisplayName, existingsDescription,
                                                              existingsIsDefault, true, settings);
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

        It should_not_update_the_description =
            () =>
            GetGlobalWrappers(typeof (TinyMCESettings),
                              new GlobalTinyMceSettingsWithNullValues().DisplayName).First()
                .Description.ShouldEqual(existingsDescription);

        It should_not_update_the_wrappers_IsDefault_property =
            () =>
            GetGlobalWrappers(typeof(TinyMCESettings),
                              new GlobalTinyMceSettingsWithNullValues().DisplayName).First()
                .IsDefault.ShouldEqual(existingsIsDefault);
    }
}
