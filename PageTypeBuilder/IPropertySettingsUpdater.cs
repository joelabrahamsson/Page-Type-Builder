using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    public interface IPropertySettingsUpdater<T> where T : IPropertySettings, new()
    {
        void UpdateSettings(T settings);
        int GetSettingsHashCode(T settings);
        bool OverWriteExistingSettings<T>();
    }
}