using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageDefinitionTypeRepository
    {
        PageDefinitionType GetPageDefinitionType(int id);
        PageDefinitionType GetPageDefinitionType(string typeName, string assemblyName);
    }
}
