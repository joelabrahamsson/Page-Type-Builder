using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageTypeValueExtractor
    {
        int GetDefaultFrameId(IPageType pageType);
        string GetFileName(IPageType pageType);
    }
}
