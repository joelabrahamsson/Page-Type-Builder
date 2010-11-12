using EPiServer;
using EPiServer.Web.PageExtensions;

namespace PageTypeBuilder.UI
{
    public abstract class TemplatePage<T> : TemplatePage where T : TypedPageData
    {
        protected TemplatePage()
            : this(0, 0) { }

        protected TemplatePage(int options)
            : this(options, 0) { }

        protected TemplatePage(int enable, int disable)
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
