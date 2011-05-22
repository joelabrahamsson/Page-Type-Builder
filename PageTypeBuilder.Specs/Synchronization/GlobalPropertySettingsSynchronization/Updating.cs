using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.ExampleImplementations;
using PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Creation;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Updating
{
    [Subject("Synchronization")]
    public class when_a_class_implementing_IUpdateGlobalPropertySettings_matches_existing_settings
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(GlobalTinyMceSettings).Assembly);
            var instance = new GlobalTinyMceSettings();
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

        It should_update_settings_using_the_classes_Update_method =
            () =>
            GlobalTinyMceSettings.MatchesUpdatedSettings((TinyMCESettings)GetGlobalWrappers(typeof(TinyMCESettings), new GlobalTinyMceSettings().DisplayName).First().PropertySettings)
            .ShouldBeTrue();
    }
}
