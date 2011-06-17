using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Migrations
{
    public interface IMigrationContext
    {
        IPageTypeRepository PageTypeRepository { get; }
        IPageDefinitionRepository PageDefinitionRepository { get; }
        IPageDefinitionTypeRepository PageDefinitionTypeRepository { get; }
        ITabDefinitionRepository TabDefinitionRepository { get; }
        INativePageDefinitionsMap NativePageDefinitionsMap { get; }
    }
}