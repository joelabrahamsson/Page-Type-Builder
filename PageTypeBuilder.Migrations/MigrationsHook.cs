using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EPiServer.Data.Dynamic;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization.Hooks;

namespace PageTypeBuilder.Migrations
{
    public class MigrationsHook : IPreSynchronizationHook
    {
        public void PreSynchronization(ISynchronizationHookContext context)
        {
            var migrations = FindMigrations(context.AssemblyLocator);
            ValidateMigrations(migrations);
            ExecuteMigrations(migrations);
        }

        IEnumerable<Type> FindMigrations(IAssemblyLocator assemblyLocator)
        {
            return assemblyLocator.AssembliesWithReferenceToAssemblyOf<Migration>().TypesAssignableTo<Migration>().Concrete();
        }

        static Regex validMigrationPattern = new Regex("Migration[1-9][0-9]*$", RegexOptions.Compiled);

        void ValidateMigrations(IEnumerable<Type> migrations)
        {
            if (migrations.GroupBy(m => m.Namespace).Count() > 1)
            {
                throw new PageTypeBuilderException();
            }

            var withInvalidNames = migrations.Where(migration => !validMigrationPattern.IsMatch(migration.Name));
            if(withInvalidNames.Count() > 0)
            {
                var errorMessage = string.Format(
                    "Migration(s) with invalid name found: {0}. " 
                    + "Migrations should be named MigrationX where X is " 
                    + "a number which doesn't start with 0.",
                    withInvalidNames.Select(type => type.Name)
                        .Aggregate((accumulate, name) => accumulate + name));
                throw new PageTypeBuilderException(errorMessage);
            }
        }

        void ExecuteMigrations(IEnumerable<Type> migrations)
        {
            var store = GetStore();
            var lastApplied = store.Items<ExecutedMigration>()
                .OrderByDescending(migration => migration.Number)
                .Select(migration => migration.Number)
                .FirstOrDefault();

            migrations.Select(type => (Migration) Activator.CreateInstance(type))
                .Where(migration => migration.Number() > lastApplied)
                .ToList().ForEach(migration =>
                    {
                        migration.Execute();
                        var executed = new ExecutedMigration(migration);
                        store.Save(executed);
                    });
        }

        static DynamicDataStore GetStore()
        {
            var type = typeof (ExecutedMigration);
            var storeName = "PageTypeBuilderMigrations";
            return DynamicDataStoreFactory.Instance.GetStore(storeName) ??
                DynamicDataStoreFactory.Instance.CreateStore(storeName, type);
        }
    }
}
