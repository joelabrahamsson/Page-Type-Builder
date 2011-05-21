using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Creation
{
    [Subject("Synchronization")]
    public class when_a_new_class_implementing_IUpdateGlobalPropertySettings_has_been_added
        : SynchronizationSpecs
    {
        static string DisplayName = "Settings display name";

        Establish context = () =>
        {
            var moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AssemblyWithGlobalSettings");
            var settingsClass = moduleBuilder.CreateClass(type =>
            {
                type.Name = "NameOfTheSettingsClass";
                type.Interfaces.Add(typeof(IUpdateGlobalPropertySettings<TinyMCESettings>));
            });

            SyncContext.AssemblyLocator.Add(settingsClass.Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_PropertySettingsWrapper_with_a_matching_DisplayName  = () =>
            SyncContext.PropertySettingsRepository.GetGlobals(typeof(TinyMCESettings))
                .ShouldContain(w => w.DisplayName.Equals(DisplayName));
    }
}
