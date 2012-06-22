using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class GlobalPropertySettingsSynchronizer
    {
        private Func<IPropertySettingsRepository> _propertySettingsRepositoryMethod;
        private IGlobalPropertySettingsLocator globalPropertySettingsLocator;
        internal List<Guid> globalSettingsIds = new List<Guid>();

        public GlobalPropertySettingsSynchronizer(Func<IPropertySettingsRepository> propertySettingsRepositoryMethod, IGlobalPropertySettingsLocator globalPropertySettingsLocator)
        {
            this._propertySettingsRepositoryMethod = propertySettingsRepositoryMethod;
            this.globalPropertySettingsLocator = globalPropertySettingsLocator;
        }

        public void Synchronize()
        {
            var updaters = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters();

            foreach (var updater in updaters)
            {
                var existingWrappers = _propertySettingsRepositoryMethod().GetGlobals(updater.SettingsType);
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
                        _propertySettingsRepositoryMethod().SaveGlobal(wrapper);
                        globalSettingsIds.Add(wrapper.Id);
                        //globalSettingsIds.Add(wrapper.PropertySettings.Id);
                    }
                });

                if (matchingWrappers.Count() == 0)
                {
                    var settings = ((IPropertySettings)Activator.CreateInstance(updater.SettingsType)).GetDefaultValues();
                    updater.UpdateSettings(settings);
                    var newWrapper = new PropertySettingsWrapper(updater.DisplayName, updater.Description, updater.IsDefault.GetValueOrDefault(), true, settings);
                    UpdateWrapperValues(updater, newWrapper);
                    _propertySettingsRepositoryMethod().SaveGlobal(newWrapper);
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
