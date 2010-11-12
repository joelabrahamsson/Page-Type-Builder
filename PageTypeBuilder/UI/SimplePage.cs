using EPiServer;
using EPiServer.Web.PageExtensions;

namespace PageTypeBuilder.UI
{
    public abstract class SimplePage<T> : SimplePage where T : TypedPageData
    {
        protected SimplePage()
            : this(0, 0) { }

        protected SimplePage(int options)
            : this(options, 0) { }

        protected SimplePage(int options, int disable)
            : base(options, LoadCurrentPage.OptionFlag | disable)
        {
            CurrentPageHandler = new LoadTypedCurrentPage<T>(this);
        }

        public new T CurrentPage
        {
            get { return (T)base.CurrentPage; }
        }

    }
}
