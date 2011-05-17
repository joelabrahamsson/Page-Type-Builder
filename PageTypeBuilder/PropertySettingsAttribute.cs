using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    //Global?
    //Should be validated is attribute if not abstract
    public abstract class PropertySettingsAttribute : Attribute
    {
        public abstract bool UpdateSettings(IPropertySettings settings);

        //Should be validated is IPropertySettings
        public abstract Type SettingType { get; }

        public abstract bool OverWriteExistingSettings { get; set; }
    }
}
