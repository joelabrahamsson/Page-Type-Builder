using System;
using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations.Validation
{
    public class given_two_migrations_in_different_namespaces : SynchronizationSpecs
    {
        static Assembly assembly;
        static MigrationsHook migrationsHook;
        static Exception thrownException;

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                {
                    with.Namespace("Namespace1")
                        .Class("Migration1")
                        .Inheriting<Migration>()
                        .PublicMethod(x =>
                             x.Named("Execute")
                              .IsOverride());

                    with.Namespace("Namespace2")
                        .Class("Migration2")
                        .Inheriting<Migration>()
                        .PublicMethod(x =>
                             x.Named("Execute")
                              .IsOverride());
                });

            SyncContext.AssemblyLocator.Add(assembly);

            migrationsHook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => migrationsHook.PreSynchronization(new SynchronizationHookContext(SyncContext.AssemblyLocator)));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
