using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Discovery
{
    public class PropertySettingsUpdater
    {
        protected object invokationTarget;

        public PropertySettingsUpdater(Type settingsType, object updater)
        {
            invokationTarget = updater;
            SettingsType = settingsType;
        }

        public Type SettingsType { get; private set; }

        public void UpdateSettings(IPropertySettings settings)
        {
            var updateMethod = typeof(IUpdatePropertySettings<>).MakeGenericType(SettingsType).GetMethod("UpdateSettings", new[] { SettingsType });
            updateMethod.Invoke(invokationTarget, new object[] { settings });
        }

        public int GetSettingsHashCode(IPropertySettings settings)
        {
            var hashCodeMethod = typeof(IUpdatePropertySettings<>).MakeGenericType(SettingsType).GetMethod("GetSettingsHashCode", new[] { SettingsType });
            return (int)hashCodeMethod.Invoke(invokationTarget, new object[] { settings });
        }

        public bool OverWriteExisting
        {
            get
            {
                var overWriteExistingMethod =
                    typeof(IUpdatePropertySettings<>).MakeGenericType(SettingsType).GetProperty(
                        "OverWriteExistingSettings").GetGetMethod();
                return (bool)overWriteExistingMethod.Invoke(invokationTarget, new object[] { });
            }
        }
    }
}
