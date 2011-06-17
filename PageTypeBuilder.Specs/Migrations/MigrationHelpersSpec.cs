using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Helpers.Fakes;
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
        protected static Migration migration;

        Establish context = () =>
            {
                pageDefinitionRepository = new InMemoryPageDefinitionRepository();
                pageTypeRepository = new InMemoryPageTypeRepository(pageDefinitionRepository);
                tabDefinitionRepository = new InMemoryTabDefinitionRepository();
                pageDefinitionTypeRepository = new InMemoryPageDefinitionTypeRepository();
            };

        protected static Migration GetMigrationInstance(Assembly assembly)
        {
            return assembly.GetMigrationInstance(
                pageTypeRepository, 
                pageDefinitionRepository, 
                pageDefinitionTypeRepository, 
                tabDefinitionRepository);
        }

        protected static Migration MigrationWithExecuteMethod(string methodBody)
        {
            var assembly = Create.Assembly(with =>
                                           TypeGenerationExtensions.WithExecuteMethod(with.MigrationClass(), methodBody));

            return GetMigrationInstance(assembly);
        }
    }
}