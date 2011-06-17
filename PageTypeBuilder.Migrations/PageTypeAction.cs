using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public class PageTypeAction
    {
        IPageType pageType;
        IPageTypeRepository pageTypeRepository;

        public PageTypeAction(IPageType pageType, IPageTypeRepository pageTypeRepository)
        {
            this.pageType = pageType;
            this.pageTypeRepository = pageTypeRepository;
        }

        public void Delete()
        {
            if(pageType.IsNull())
            {
                return;
            }

            pageTypeRepository.Delete(pageType);
        }
    }
}
