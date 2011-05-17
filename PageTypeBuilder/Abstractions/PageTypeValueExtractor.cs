using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageTypeValueExtractor : IPageTypeValueExtractor
    {
        public virtual int GetDefaultFrameId(IPageType pageType)
        {
            return pageType.DefaultFrameID;
        }

        public virtual string GetFileName(IPageType pageType)
        {
            return pageType.FileName;
        }
    }
}
