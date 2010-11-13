using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageDefinitionTypeFactory
    {
        PageDefinitionType GetPageDefinitionType(int id);
        PageDefinitionType GetPageDefinitionType(string typeName, string assemblyName);
    }
}
