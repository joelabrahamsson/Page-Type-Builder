using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Synchronization
{
    internal class PropertySettingsUpdateCommand
    {
        private object invokationTarget;
        private PropertySettingsContainer container;
        private IPropertySettingsRepository repository;
        private Type settingsType;

        public PropertySettingsUpdateCommand(Type settingsType, object invokationTarget, PropertySettingsContainer container, IPropertySettingsRepository repository)
        {
            this.settingsType = settingsType;
            this.invokationTarget = invokationTarget;
            this.container = container;
            this.repository = repository;
        }

        public void Update()
        {
            var wrapper = container.GetSetting(settingsType);
            if (wrapper == null)
            {
                wrapper = new PropertySettingsWrapper();
                container.Settings.Add(settingsType.FullName, wrapper);
            }

            bool settingsAlreadyExists = true;
            if (wrapper.PropertySettings == null)
            {
                wrapper.PropertySettings = (IPropertySettings)Activator.CreateInstance(settingsType);
                settingsAlreadyExists = false;
            }

            if (settingsAlreadyExists && !OverWriteExisting)
                return;

            int hashBeforeUpdate = GetSettingsHashCode(wrapper.PropertySettings);
            UpdateSettings(wrapper.PropertySettings);
            int hashAfterUpdate = GetSettingsHashCode(wrapper.PropertySettings);
            if (hashBeforeUpdate != hashAfterUpdate || !settingsAlreadyExists)
            {
                repository.Save(container);
            }
        }

        private void UpdateSettings(IPropertySettings settings)
        {
            var updateMethod = typeof(IUpdatePropertySettings<>).MakeGenericType(settingsType).GetMethod("UpdateSettings", new[] { settingsType });
            updateMethod.Invoke(invokationTarget, new object[] { settings });
        }
            
        private int GetSettingsHashCode(IPropertySettings settings)
        {
            var hashCodeMethod = typeof(IUpdatePropertySettings<>).MakeGenericType(settingsType).GetMethod("GetSettingsHashCode", new[] { settingsType });
            return (int) hashCodeMethod.Invoke(invokationTarget, new object[] { settings });
        }

        private bool OverWriteExisting
        {
            get
            {
                var overWriteExistingMethod =
                    typeof (IUpdatePropertySettings<>).MakeGenericType(settingsType).GetMethod(
                        "OverWriteExistingSettings", new Type[] {}).MakeGenericMethod(new [] {settingsType});
                return (bool) overWriteExistingMethod.Invoke(invokationTarget, new object[] {});
            }
        }
    }
}