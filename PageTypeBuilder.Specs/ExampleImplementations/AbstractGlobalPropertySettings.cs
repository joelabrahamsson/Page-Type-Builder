using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public abstract class AbstractGlobalPropertySettings : IUpdateGlobalPropertySettings<TinyMCESettings>
    {
        public void UpdateSettings(TinyMCESettings settings)
        {
        }

        public abstract int GetSettingsHashCode(TinyMCESettings tinyMceSettings);

        public virtual bool OverWriteExistingSettings
        {
            get { return true; }
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

        public abstract bool Match(EPiServer.Core.PropertySettings.PropertySettingsWrapper propertySettingsWrapper);
    }
}
