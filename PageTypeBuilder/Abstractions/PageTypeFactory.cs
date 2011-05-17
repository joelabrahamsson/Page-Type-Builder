using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageTypeFactory : IPageTypeFactory
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

        public virtual void Save(IPageType pageTypeToSave)
        {
            pageTypeToSave.Save();
        }

        public virtual IPageType CreateNew()
        {
            return new NativePageType();
        }
    }
}