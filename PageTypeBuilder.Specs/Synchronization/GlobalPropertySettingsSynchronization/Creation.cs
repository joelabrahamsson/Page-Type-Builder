using System;
using System.Linq;
using EPiServer.Editor.TinyMCE;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using Refraction;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization.Creation
{
    [Subject("Synchronization")]
    public class when_an_abstract_class_implementing_IUpdateGlobalPropertySettings_has_been_added
        : SynchronizationSpecs
    {
        static Exception thrownExcpetion;
        Establish context = () =>
            {
                var assembly =
                    Create.Assembly(with =>
                        with.Class("AbstractClass")
                            .Abstract()
                            .AutoImplementing<IUpdateGlobalPropertySettings<TinyMCESettings>>());

                SyncContext.AssemblyLocator.Add(assembly);
            };

        Because of = () => thrownExcpetion = Catch.Exception(() => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_not_throw_an_exception 
            = () => thrownExcpetion.ShouldBeNull();
        
    }
    [Subject("Synchronization")]
    public class when_a_new_class_implementing_IUpdateGlobalPropertySettings_has_been_added
        : GlobalPropertySettingsSpecsBase
    {
        static string displayName = Guid.NewGuid().ToString();
        static string description = Guid.NewGuid().ToString();
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(
                displayName: displayName,
                description: description,
                updateSettingsImplementation: "settings.Width = int.MaxValue;"));

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_global_PropertySettingsWrapper_with_a_matching_DisplayName  = () =>
            GetGlobalWrappersByDisplayName<TinyMCESettings>(displayName).ShouldNotBeEmpty();

        It should_create_a_global_PropertySettingsWrapper_with_a_matching_Description = () =>
            GetGlobalWrappersByDisplayName<TinyMCESettings>(displayName)
            .First().Description.ShouldEqual(description);

        It should_create_property_settings_with_default_values = () =>
            HasDefaultValuesExceptUpdated((TinyMCESettings)GetGlobalWrappersByDisplayName<TinyMCESettings>(displayName).First().PropertySettings).ShouldBeTrue();

        It should_create_settings_modified_by_the_classes_Update_method =
            () =>
            ((TinyMCESettings)
             GetGlobalWrappersByDisplayName<TinyMCESettings>(displayName).First().
                 PropertySettings).Width.ShouldEqual(int.MaxValue);

        static bool HasDefaultValuesExceptUpdated(TinyMCESettings settings)
        {
            var defaultValues = (TinyMCESettings)new TinyMCESettings().GetDefaultValues();
            defaultValues.Width = int.MaxValue;
            return settings.Serialize().Equals(defaultValues.Serialize());
        }
    }

    [Subject("Synchronization")]
    public class when_a_new_class_implementing_IUpdateGlobalPropertySettings_with_IsDefault_returning_true_has_been_added
        : GlobalPropertySettingsSpecsBase
    {
        static string settingsDisplayName = Guid.NewGuid().ToString();
        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                with.GlobalPropertySettingsClass(displayName: settingsDisplayName, isDefault: true));
            SyncContext.AssemblyLocator.Add(assembly);
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_global_PropertySettingsWrapper_with_IsDefault_set_to_true = () =>
            GetGlobalWrappersByDisplayName<TinyMCESettings>(settingsDisplayName).First().IsDefault.ShouldBeTrue();
    }
}
