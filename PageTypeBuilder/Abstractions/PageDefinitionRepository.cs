using System;
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

        public void Delete(PageDefinition pageDefinition)
        {
            pageDefinition.Delete();
        }

        public PageDefinition Load(int id)
        {
            return PageDefinition.Load(id);
        }
    }
}
