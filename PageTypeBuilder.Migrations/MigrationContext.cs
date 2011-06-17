using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;

namespace PageTypeBuilder.Migrations
{
    public class MigrationContext : IMigrationContext
    {
        public MigrationContext()
            : this(new PageTypeRepository(), 
            new PageDefinitionRepository(), 
            new PageDefinitionTypeRepository(), 
            new TabDefinitionRepository(),
            new NativePageDefinitionsMap())
        {}

        public MigrationContext(
            IPageTypeRepository pageTypeRepository, 
            IPageDefinitionRepository pageDefinitionRepository,
            IPageDefinitionTypeRepository pageDefinitionTypeRepository,
            ITabDefinitionRepository tabDefinitionRepository,
            INativePageDefinitionsMap nativePageDefinitionsMap)
        {
            PageTypeRepository = pageTypeRepository;
            PageDefinitionRepository = pageDefinitionRepository;
            PageDefinitionTypeRepository = pageDefinitionTypeRepository;
            TabDefinitionRepository = tabDefinitionRepository;
            NativePageDefinitionsMap = nativePageDefinitionsMap;
        }

        public virtual IPageTypeRepository PageTypeRepository { get; private set; }
        public virtual IPageDefinitionRepository PageDefinitionRepository { get; private set; }
        public virtual IPageDefinitionTypeRepository PageDefinitionTypeRepository { get; private set; }
        public virtual ITabDefinitionRepository TabDefinitionRepository { get; private set; }
        public virtual INativePageDefinitionsMap NativePageDefinitionsMap { get; private set; }
    }
}
