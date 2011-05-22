using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                //TODO: Check existing
                var settings = (IPropertySettings) Activator.CreateInstance(updater.SettingsType);
                var wrapper = new PropertySettingsWrapper(updater.DisplayName, updater.Description, updater.IsDefault.GetValueOrDefault(), true, settings);
                propertySettingsRepository.SaveGlobal(wrapper);
            }
        }
    }
}
