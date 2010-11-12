using EPiServer.Core;

namespace PageTypeBuilder
{
    public abstract class TypedPageData : PageData
    {
        internal static void PopuplateInstance(PageData source, TypedPageData destination)
        {
            destination.ShallowCopy(source);
        }
    }
}