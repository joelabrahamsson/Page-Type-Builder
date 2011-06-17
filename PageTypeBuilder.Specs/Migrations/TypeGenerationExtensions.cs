using System.CodeDom;
using System.Reflection;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Migrations;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations.Helpers
{
    public static class TypeGenerationExtensions
    {
        public static string DefaultMigrationName = "Migration1";

        public static CodeTypeDeclaration MigrationClass(this AssemblyDefinition assembly)
        {
            return assembly.Class(DefaultMigrationName)
                .Inheriting<Migration>()
                .Constructor(x =>
                             x.Parameter<IPageTypeRepository>("pageTypeRepository")
                                 .PassToBase("pageTypeRepository")
                                 .Parameter<IPageDefinitionRepository>("pageDefinitionRepository")
                                 .PassToBase("pageDefinitionRepository")
                                 .Parameter<ITabDefinitionRepository>("tabDefinitionRepository")
                                 .PassToBase("tabDefinitionRepository"));
        }

        public static CodeTypeDeclaration WithExecuteMethod(
            this CodeTypeDeclaration migrationClass,
            string methodBody)
        {
            return migrationClass.PublicMethod(x =>
                                               x.Named("Execute")
                                                   .IsOverride()
                                                   .Body(methodBody));
        }

        public static Migration GetMigrationInstance(
            this Assembly assembly,
            IPageTypeRepository pageTypeRepository,
            IPageDefinitionRepository pageDefinitionRepository,
            ITabDefinitionRepository tabDefinitionRepository)
        {
            return (Migration) assembly.GetTypeInstance(DefaultMigrationName,
                                                        pageTypeRepository, 
                                                        pageDefinitionRepository,
                                                        tabDefinitionRepository);
        }
    }
}