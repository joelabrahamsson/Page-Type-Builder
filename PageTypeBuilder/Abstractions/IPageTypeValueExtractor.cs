using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageTypeValueExtractor
    {
        int GetDefaultFrameId(PageType pageType);
        string GetFileName(PageType pageType);
    }
}
