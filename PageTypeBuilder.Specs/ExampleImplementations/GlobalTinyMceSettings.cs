using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public class GlobalTinyMceSettings : IUpdateGlobalPropertySettings<TinyMCESettings>
    {
        public void UpdateSettings(TinyMCESettings settings)
        {
            settings.Width = int.MaxValue;
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

        public bool OverWriteExistingSettings
        {
            get { return OverWriteExisting; }
        }

        public virtual string DisplayName
        {
            get { return "Global TinyMCE settings"; }
        }

        public string Description
        {
            get { return "Description for the settings"; }
        }

        public virtual bool? IsDefault { get; set; }


        public bool Match(EPiServer.Core.PropertySettings.PropertySettingsWrapper propertySettingsWrapper)
        {
            throw new System.NotImplementedException();
        }
    }
}