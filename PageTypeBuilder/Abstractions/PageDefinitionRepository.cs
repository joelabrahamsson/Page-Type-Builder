using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageDefinitionRepository : IPageDefinitionRepository
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
