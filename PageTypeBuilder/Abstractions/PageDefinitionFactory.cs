using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageDefinitionFactory : IPageDefinitionFactory
    {
        public virtual void Save(PageDefinition pageDefinition)
        {
            pageDefinition.Save();
        }

        public PageDefinitionCollection List(int pageTypeId)
        {
            return PageDefinition.List(pageTypeId);
        }
    }
}
