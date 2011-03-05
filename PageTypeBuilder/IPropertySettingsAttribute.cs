using System;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    //Global?
    //Should be validated is attribute if not abstract
    public interface IPropertySettingsAttribute
    {
        bool UpdateSettings(IPropertySettings settings);

        //Should be validated is IPropertySettings
        Type SettingType { get; }

        bool OverWriteExistingSettings { get; }
    }
}
