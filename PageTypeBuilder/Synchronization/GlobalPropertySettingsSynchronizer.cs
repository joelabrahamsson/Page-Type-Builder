using System;
using System.Linq;
using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class GlobalPropertySettingsSynchronizer
    {
        private IPropertySettingsRepository propertySettingsRepository;
        private IGlobalPropertySettingsLocator globalPropertySettingsLocator;

        public GlobalPropertySettingsSynchronizer(IPropertySettingsRepository propertySettingsRepository, IGlobalPropertySettingsLocator globalPropertySettingsLocator)
        {
            this.propertySettingsRepository = propertySettingsRepository;
            this.globalPropertySettingsLocator = globalPropertySettingsLocator;
        }

        public void Synchronize()
        {
            var updaters = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters();
            foreach (var updater in updaters)
            {
                var existingWrappers = propertySettingsRepository.GetGlobals(updater.SettingsType);
                var matchingWrappers = existingWrappers.Where(wrapper => updater.Match(wrapper)).ToList();

                matchingWrappers.ForEach(wrapper =>
                {
                    var existingWrapperValues = WrapperValuesSerialized(updater, wrapper);
                    if (updater.OverWriteExisting)
                    {
                        updater.UpdateSettings(wrapper.PropertySettings);
                    }
                    UpdateWrapperValues(updater, wrapper);
                    var isChanged = !WrapperValuesSerialized(updater, wrapper).Equals(existingWrapperValues);
                    if (updater.OverWriteExisting && isChanged)
                    {
                        propertySettingsRepository.SaveGlobal(wrapper);
                    }
                });

                if (matchingWrappers.Count() == 0)
                {
                    var settings = ((IPropertySettings)Activator.CreateInstance(updater.SettingsType)).GetDefaultValues();
                    updater.UpdateSettings(settings);
                    var newWrapper = new PropertySettingsWrapper(updater.DisplayName, updater.Description, updater.IsDefault.GetValueOrDefault(), true, settings);
                    UpdateWrapperValues(updater, newWrapper);
                    propertySettingsRepository.SaveGlobal(newWrapper);
                }
            }
        }

        static string WrapperValuesSerialized(GlobalPropertySettingsUpdater updater, PropertySettingsWrapper wrapper)
        {
            var hashCode = updater.GetSettingsHashCode(wrapper.PropertySettings);
            return string.Concat((wrapper.Description ?? "null"), "||", wrapper.IsDefault, "||", hashCode);
        }

        private void UpdateWrapperValues(GlobalPropertySettingsUpdater updater, PropertySettingsWrapper wrapper)
        {
            if (updater.Description != null)
            {
                wrapper.Description = updater.Description;
            }
            if (updater.IsDefault.HasValue)
            {
                wrapper.IsDefault = updater.IsDefault.Value;
            }
        }
    }
}
