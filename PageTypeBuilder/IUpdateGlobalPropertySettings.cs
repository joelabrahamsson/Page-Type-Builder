using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    public interface IUpdateGlobalPropertySettings<T> : IUpdatePropertySettings<T>
        where T : IPropertySettings, new()
    {
        string DisplayName { get; }
        string Description { get; }
        bool? IsDefault { get; }
        bool Match(PropertySettingsWrapper propertySettingsWrapper);
    }
}
