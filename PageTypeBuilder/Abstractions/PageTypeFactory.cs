using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageTypeFactory
    {
        public virtual PageType Load(string name)
        {
            return PageType.Load(name);
        }

        public virtual PageType Load(System.Guid guid)
        {
            return PageType.Load(guid);
        }

        public virtual PageType Load(int id)
        {
            return PageType.Load(id);
        }

        public virtual void Save(PageType pageTypeToSave)
        {
            pageTypeToSave.Save();
        }
    }
}