using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface ITabFactory
    {
        TabDefinition GetTabDefinition(string name);
        void SaveTabDefinition(TabDefinition tabDefinition);
        TabDefinitionCollection List();
    }
}
