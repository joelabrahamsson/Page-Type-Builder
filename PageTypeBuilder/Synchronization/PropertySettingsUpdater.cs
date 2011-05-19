using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Synchronization
{
    internal class PropertySettingsUpdater
    {
        private object invokationTarget;

        public PropertySettingsUpdater(Type settingsType, object invokationTarget)
        {
            SettingsType = settingsType;
            this.invokationTarget = invokationTarget;
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
            return (int) hashCodeMethod.Invoke(invokationTarget, new object[] { settings });
        }

        public bool OverWriteExisting()
        {
            var overWriteExistingMethod = typeof(IUpdatePropertySettings<>).MakeGenericType(SettingsType).GetMethod("OverWriteExistingSettings", new Type[] { }).MakeGenericMethod(new Type[] { SettingsType});
            return (bool) overWriteExistingMethod.Invoke(invokationTarget, new object[] { });
        }

    }
}