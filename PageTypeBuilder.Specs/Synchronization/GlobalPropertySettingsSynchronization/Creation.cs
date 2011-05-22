using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.ExampleImplementations;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Creation
{
    [Subject("Synchronization")]
    public class when_a_new_class_implementing_IUpdateGlobalPropertySettings_has_been_added
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettings).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_global_PropertySettingsWrapper_with_a_matching_DisplayName  = () =>
            GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettings().DisplayName).ShouldNotBeEmpty();

        It should_create_a_global_PropertySettingsWrapper_with_a_matching_Description = () =>
            GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettings().DisplayName).First().Description.ShouldEqual(new GlobalTinyMceSettings().Description);

        static IEnumerable<PropertySettingsWrapper> GetGlobalWrappers(Type settingsType, string displayName)
        {
            return
                SyncContext.PropertySettingsRepository.GetGlobals(settingsType).Where(
                    w => w.DisplayName.Equals(displayName));
        }

        It should_create_settings_modified_by_the_classes_Update_method =
            () =>
            GlobalTinyMceSettings.MatchesUpdatedSettings((TinyMCESettings) GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettings().DisplayName).First().PropertySettings)
            .ShouldBeTrue();
    }

    [Subject("Synchronization")]
    public class when_a_new_class_implementing_IUpdateGlobalPropertySettings_with_IsDefault_returning_true_has_been_added
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettingsWithIsDefaultReturningTrue).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_global_PropertySettingsWrapper_with_IsDefault_set_to_true = () =>
            GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettingsWithIsDefaultReturningTrue().DisplayName).First().IsDefault.ShouldBeTrue();

        static IEnumerable<PropertySettingsWrapper> GetGlobalWrappers(Type settingsType, string displayName)
        {
            return
                SyncContext.PropertySettingsRepository.GetGlobals(settingsType).Where(
                    w => w.DisplayName.Equals(displayName));
        }
    }
}
