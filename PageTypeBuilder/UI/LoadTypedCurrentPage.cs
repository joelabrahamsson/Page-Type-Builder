using System;
using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.PageExtensions;

namespace PageTypeBuilder.UI
{

    /// <summary>
    /// To be used as CurrentPageHandler for strongly typed pages.
    /// </summary>
    /// <typeparam name="T">Type of TypedPageData</typeparam>
    public class LoadTypedCurrentPage<T> : ICurrentPage
        where T : TypedPageData
    {
        private PageData _pageData;

        public LoadTypedCurrentPage(PageBase page)
        {
            this.Page = page;
        }

        public PageBase Page { get; private set; }

        public virtual PageData CurrentPage
        {
            get
            {
                if (_pageData == null && PageReference.IsValue(Page.CurrentPageLink))
                {
                    _pageData = Page.GetPage(Page.CurrentPageLink);

                    if (!_pageData.InheritsFromType<T>())
                        throw new PageTypeBuilderException(
                            string.Format(CultureInfo.InvariantCulture, "The current page is not of type {0}. This often occurs when no id query string parameter is specified.", typeof(T).Name));

                }
                return _pageData;
            }
            set
            {
                if (value != null && !value.InheritsFromType<T>())
                    throw new PageTypeBuilderException(
                        string.Format(CultureInfo.InvariantCulture, "The current page is not of type {0}.", typeof(T).Name));

                _pageData = value;
            }
        }

    }

}