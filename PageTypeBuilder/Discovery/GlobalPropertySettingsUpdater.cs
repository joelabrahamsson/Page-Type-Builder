using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Discovery
{
    public class GlobalPropertySettingsUpdater : PropertySettingsUpdater
    {
        public GlobalPropertySettingsUpdater(Type settingsType, object updater)
            : base(settingsType, updater)
        {
        }

        public string DisplayName
        {
            get
            {
                var getter =
                    typeof(IUpdateGlobalPropertySettings<>).MakeGenericType(SettingsType).GetProperty(
                        "DisplayName").GetGetMethod();
                return (string)getter.Invoke(invokationTarget, new object[] { });
            }
        }

        public string Description
        {
            get
            {
                var getter =
                    typeof(IUpdateGlobalPropertySettings<>).MakeGenericType(SettingsType).GetProperty(
                        "Description").GetGetMethod();
                return (string)getter.Invoke(invokationTarget, new object[] { });
            }
        }

        public bool? IsDefault
        {
            get
            {
                var getter =
                    typeof(IUpdateGlobalPropertySettings<>).MakeGenericType(SettingsType).GetProperty(
                        "IsDefault").GetGetMethod();
                return (bool?)getter.Invoke(invokationTarget, new object[] { });
            }
        }

        public bool Match(PropertySettingsWrapper propertySettingsWrapper)
        {
            var matchMethod = typeof(IUpdateGlobalPropertySettings<>).MakeGenericType(SettingsType).GetMethod("Match", new[] { typeof(PropertySettingsWrapper) });
            return (bool)matchMethod.Invoke(invokationTarget, new object[] { propertySettingsWrapper });
        }

        public Type WrappedInstanceType
        {
            get
            {
                return invokationTarget.GetType();
            }
        }
    }
}
