using System.Linq;
using EPiServer.Core;

namespace PageTypeBuilder
{
    public static class PageDataCollectionExtensions
    {
        public static PageDataCollection AsTyped(this PageDataCollection pages)
        {
            return new PageDataCollection(pages.Where(page => page != null).Select(PageTypeResolver.Instance.ConvertToTyped));
        }
    }
}
