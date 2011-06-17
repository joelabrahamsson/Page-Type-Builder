using System;
using System.Reflection;
using EPiServer.Data.Dynamic;
using Machine.Specifications;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;
using Refraction;

namespace PageTypeBuilder.Specs.Migrations.Validation
{
    public class given_two_migrations_in_different_namespaces
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
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

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }

    public class given_two_migrations_whose_names_are_not_in_consecutive_order
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            DynamicDataStoreFactory.Instance = new InMemoryDynamicDataStoreFactory();
            assembly = Create.Assembly(with =>
            {
                with.Class("Migration1")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                         x.Named("Execute")
                          .IsOverride());

                with.Class("Migration3")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                         x.Named("Execute")
                          .IsOverride());
            });

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }

    public class given_a_migration_whose_name_starts_with_lowercase_m
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            assembly = Create.Assembly(with => 
                with.Class("migration1")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }

    public class given_a_migration_whose_number_starts_with_0
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class("Migration01")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }

    public class given_a_migration_whose_name_doesnt_end_with_a_number
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            assembly = Create.Assembly(with =>
                with.Class("Migration1a")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }

    public class given_an_abstract_migration_with_an_invalid_name
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            DynamicDataStoreFactory.Instance = new InMemoryDynamicDataStoreFactory();

            assembly = Create.Assembly(with =>
                with.Class("migration01a")
                    .Inheriting<Migration>()
                    .Abstract()
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            hookContext = new SynchronizationHookContext(assemblyLocator);

            hook = new MigrationsHook();
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_not_throw_an_exception
            = () => thrownException.ShouldBeNull();
    }

    public class given_an_executed_migration_doesnt_exist_in_the_application_domain
    {
        static Assembly assembly;
        static ISynchronizationHookContext hookContext;
        static IPreSynchronizationHook hook;
        static Exception thrownException;

        Establish context = () =>
        {
            DynamicDataStoreFactory.Instance = new InMemoryDynamicDataStoreFactory();

            assembly = Create.Assembly(with =>
                with.Class("Migration1")
                    .Inheriting<Migration>()
                    .PublicMethod(x =>
                        x.Named("Execute")
                         .IsOverride()));

            var assemblyLocator = new InMemoryAssemblyLocator();
            assemblyLocator.Add(assembly);

            var firstRunHookContext = new SynchronizationHookContext(assemblyLocator);
            hookContext = new SynchronizationHookContext(new InMemoryAssemblyLocator());
            hook = new MigrationsHook();
            hook.PreSynchronization(firstRunHookContext);
        };

        Because of =
            () => thrownException = Catch.Exception(() => hook.PreSynchronization(hookContext));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
