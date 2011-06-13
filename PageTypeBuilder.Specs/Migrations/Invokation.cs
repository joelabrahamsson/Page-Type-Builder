using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EPiServer.Data.Dynamic;
using Machine.Specifications;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Synchronization.Hooks;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations
{
    public class when_a_new_migration_exists
    {
        static Assembly assembly;
        static MigrationsHook hook;
        static ISynchronizationHookContext hookContext;
        static string migrationName = "Migration1";
        static string verificationPropertyName = "ExecuteInvoked";

        Establish context = () =>
            {
                DynamicDataStoreFactory.Instance = new InMemoryDynamicDataStoreFactory();

                assembly = Create.Assembly(with =>
                    with.Class(migrationName)
                        .Inheriting<Migration>()
                        .AutomaticProperty<bool>(x =>
                            x.Named(verificationPropertyName)
                             .Static())
                        .PublicMethod(x =>
                            x.Named("Execute")
                             .IsOverride()
                             .Body("ExecuteInvoked = true")));

                var assemblyLocator = new InMemoryAssemblyLocator();
                assemblyLocator.Add(assembly);

                hookContext = new SynchronizationHookContext(assemblyLocator);

                hook = new MigrationsHook();
            };

        Because of = () => hook.PreSynchronization(hookContext);

        It should_invoke_the_migrations_Execute_method
            = () => 
                assembly.GetTypeNamed(migrationName)
                        .GetPublicStaticPropertyValue<bool>(verificationPropertyName)
                        .ShouldBeTrue();
    }

    public class when_a_migration_has_previously_been_executed
    {
        static Assembly assembly;
        static MigrationsHook hook;
        static ISynchronizationHookContext hookContext;
        static string migrationName = "Migration1";
        static string verificationPropertyName = "ExecuteInvoked";

        Establish context = () =>
        {
            DynamicDataStoreFactory.Instance = new InMemoryDynamicDataStoreFactory();

            assembly = Create.Assembly(with =>
                with.Class(migrationName)
                    .Inheriting<Migration>()
                    .AutomaticProperty<bool>(x =>
                        x.Named(verificationPropertyName)
                         .Static())
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()
                         .Body("ExecuteInvoked = true")));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();

            hook.PreSynchronization(hookContext);

            assembly.GetTypeNamed(migrationName)
                .SetPublicStaticPropertyValue(verificationPropertyName, false);
        };

        Because of = () => hook.PreSynchronization(hookContext);

        It should_not_invoke_the_migrations_Execute_method
            = () =>
              assembly.GetTypeNamed(migrationName)
                        .GetPublicStaticPropertyValue<bool>(verificationPropertyName)
                        .ShouldBeFalse();
    }
}
