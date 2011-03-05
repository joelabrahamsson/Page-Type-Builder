using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    public class PropertySettingsUpdateResult
    {
        public PropertySettingsUpdateResult(IPropertySettings settings, bool updated)
        {
            Settings = settings;
            Updated = updated;
        }

        public IPropertySettings Settings { get; private set; }

        public bool Updated { get; private set; }
    }
}
