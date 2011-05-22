using System;

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
    }
}
