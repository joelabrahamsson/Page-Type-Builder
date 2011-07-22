using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageTypeRepository : IPageTypeRepository
    {
        public virtual IPageType Load(string name)
        {
            var pageType = PageType.Load(name);
            if (pageType == null)
                return null;
            return new WrappedPageType(pageType);
        }

        public virtual IPageType Load(System.Guid guid)
        {
            var pageType = PageType.Load(guid);
            if (pageType == null)
                return null;
            return new WrappedPageType(pageType);
        }

        public virtual IPageType Load(int id)
        {
            var pageType = PageType.Load(id);
            if (pageType == null)
                return null;
            return new WrappedPageType(pageType);
        }

        public virtual IEnumerable<IPageType> List()
        {
            return PageType.List().Select(pageType => new WrappedPageType(pageType)).Cast<IPageType>();
        }

        public virtual void Save(IPageType pageTypeToSave)
        {
            pageTypeToSave.Save();
        }

        public virtual IPageType CreateNew()
        {
            return new NativePageType();
        }

        public void Delete(IPageType pageType)
        {
            pageType.Delete();
        }
    }
}