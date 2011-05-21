using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder
{
    interface IUpdateGlobalPropertySettings<T> : IUpdatePropertySettings<T>
        where T : IPropertySettings, new()
    {
        string DisplayName<T>();
    }
}
