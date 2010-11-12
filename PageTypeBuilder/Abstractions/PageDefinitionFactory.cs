using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageDefinitionFactory
    {
        public virtual void Save(PageDefinition pageDefinition)
        {
            pageDefinition.Save();
        }
    }
}
