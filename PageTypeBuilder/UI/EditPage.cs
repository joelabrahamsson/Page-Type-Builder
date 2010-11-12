using EPiServer;
using EPiServer.Web.PageExtensions;

namespace PageTypeBuilder.UI
{
    public abstract class EditPage<T> : EditPage where T : TypedPageData
    {
        protected EditPage()
            : this(0, 0) { }

        protected EditPage(int enable)
            : this(enable, 0) { }

        protected EditPage(int enable, int disable)
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
