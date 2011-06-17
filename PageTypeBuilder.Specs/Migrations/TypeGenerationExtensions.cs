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
                             x.Parameter<IMigrationContext>("context")
                                 .PassToBase("context"));
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
            IMigrationContext migrationContext)
        {
            return (Migration) assembly.GetTypeInstance(DefaultMigrationName,
                                                        migrationContext);
        }
    }
}