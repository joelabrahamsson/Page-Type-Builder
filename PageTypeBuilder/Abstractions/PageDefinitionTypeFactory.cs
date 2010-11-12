using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class PageDefinitionTypeFactory
    {
        public virtual PageDefinitionType GetPageDefinitionType(int id)
        {
            return PageDefinitionType.Load(id);
        }

        public virtual PageDefinitionType GetPageDefinitionType(string typeName, string assemblyName)
        {
            return PageDefinitionType.Load(typeName, assemblyName);
        }
    }
}
