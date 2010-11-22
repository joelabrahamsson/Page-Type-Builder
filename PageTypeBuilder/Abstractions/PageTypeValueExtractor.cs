using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageTypeValueExtractor : IPageTypeValueExtractor
    {
        public virtual int GetDefaultFrameId(PageType pageType)
        {
            return pageType.DefaultFrameID;
        }

        public virtual string GetFileName(PageType pageType)
        {
            return pageType.FileName;
        }
    }
}
