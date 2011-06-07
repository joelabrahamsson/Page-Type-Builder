using System;
using System.Collections.Generic;
using System.Linq;
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

        void ValidateMigrations(IEnumerable<Type> migrations)
        {
            if (migrations.GroupBy(m => m.Namespace).Count() > 1)
            {
                throw new PageTypeBuilderException();
            }
        }

        void ExecuteMigrations(IEnumerable<Type> migrations)
        {
            throw new NotImplementedException();
        }
    }
}
