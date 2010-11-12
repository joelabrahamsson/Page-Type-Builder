using EPiServer;
using EPiServer.Web.PageExtensions;

namespace PageTypeBuilder.UI
{
    public abstract class PageBase<T> : PageBase where T : TypedPageData
    {
        protected PageBase(int enable)
            : this(enable, 0) { }

        protected PageBase(int enable, int disable)
            : base(enable, LoadCurrentPage.OptionFlag | disable)
        {
            CurrentPageHandler = new LoadTypedCurrentPage<T>(this);
        }

        public new T CurrentPage
        {
            get { return (T)base.CurrentPage; }
        }
    }
}
