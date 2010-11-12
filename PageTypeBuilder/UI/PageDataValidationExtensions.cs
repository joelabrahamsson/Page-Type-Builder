using EPiServer.Core;

namespace PageTypeBuilder.UI
{
    public static class PageDataValidationExtensions
    {
        public static bool InheritsFromType<T>(this PageData page) where T : TypedPageData
        {
            return typeof(T).IsAssignableFrom(page.GetType());
        }
    }
}
