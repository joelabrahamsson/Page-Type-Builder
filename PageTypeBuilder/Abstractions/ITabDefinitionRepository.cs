using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface ITabDefinitionRepository
    {
        TabDefinition GetTabDefinition(string name);
        void SaveTabDefinition(TabDefinition tabDefinition);
        TabDefinitionCollection List();
    }
}
