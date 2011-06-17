using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations.Helpers
{
    [Subject("Migration helper methods")]
    public abstract class MigrationHelpersSpec
    {
        protected static IPageTypeRepository pageTypeRepository;
        protected static IPageDefinitionRepository pageDefinitionRepository;
        protected static ITabDefinitionRepository tabDefinitionRepository;
        protected static InMemoryPageDefinitionTypeRepository pageDefinitionTypeRepository;
        protected static INativePageDefinitionsMap nativePageDefinitionsMap;
        protected static Migration migration;

        Establish context = () =>
            {
                pageDefinitionRepository = new InMemoryPageDefinitionRepository();
                pageTypeRepository = new InMemoryPageTypeRepository(pageDefinitionRepository);
                tabDefinitionRepository = new InMemoryTabDefinitionRepository();
                pageDefinitionTypeRepository = new InMemoryPageDefinitionTypeRepository();
                nativePageDefinitionsMap = new NativePageDefinitionsMap();
            };

        protected static Migration GetMigrationInstance(Assembly assembly)
        {
            var context = new MigrationContext(
                pageTypeRepository,
                pageDefinitionRepository,
                pageDefinitionTypeRepository,
                tabDefinitionRepository,
                nativePageDefinitionsMap);

            return assembly.GetMigrationInstance(context);
        }

        protected static Migration MigrationWithExecuteMethod(string methodBody)
        {
            var assembly = Create.Assembly(with =>
                                           TypeGenerationExtensions.WithExecuteMethod(with.MigrationClass(), methodBody));

            return GetMigrationInstance(assembly);
        }
    }
}