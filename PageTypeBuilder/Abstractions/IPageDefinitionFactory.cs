using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageDefinitionFactory
    {
        void Save(PageDefinition pageDefinition);
    }
}
