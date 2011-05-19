using System;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public class TinyMceSettingsAttribute : Attribute, IUpdatePropertySettings<TinyMCESettings>
    {
        public bool ModifyPropertySettings { get; set; }
        public void UpdateSettings(TinyMCESettings settings)
        {
            if(ModifyPropertySettings)
            {
                settings.Width = int.MaxValue;
            }
        }

        public static bool MatchesUpdatedSettings(TinyMCESettings settings)
        {
            return settings.Width == int.MaxValue;
        }

        public int GetSettingsHashCode(TinyMCESettings tinyMceSettings)
        {
            return tinyMceSettings.Width;
        }

        public bool OverWriteExisting { get; set; }

        public bool OverWriteExistingSettings<TinyMCESettings>()
        {
            return OverWriteExisting;
        }
    }
}
