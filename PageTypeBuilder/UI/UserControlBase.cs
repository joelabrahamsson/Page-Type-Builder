using System.Globalization;
using EPiServer;
using EPiServer.Core;

namespace PageTypeBuilder.UI
{
    public abstract class UserControlBase<T> : UserControlBase where T : TypedPageData
    {
        private T _currentPage;

        public new T CurrentPage
        {
            get
            {
                if (_currentPage != null)
                    return _currentPage;

                PageData page = DataFactory.Instance.GetPage(PageBase.CurrentPage.PageLink);

                if (!page.InheritsFromType<T>())
                    throw new PageTypeBuilderException(
                        string.Format(CultureInfo.InvariantCulture, "The current page is not of type {0}.", typeof(T).Name));

                _currentPage = (T) page;
                return _currentPage;
            }
        }
    }
}
