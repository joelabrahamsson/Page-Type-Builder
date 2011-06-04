using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;

namespace PageTypeBuilder.Specs.Hooks
{
    [Subject("Synchronization")]
    public class given_a_class_implementing_IPreSynchronizationHook
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(ExamplePreSynchronizationHook).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_invoke_the_class_PreSynchronization_method
            = () => ExamplePreSynchronizationHook.PreSynchronizationHasBeenInvoked.ShouldBeTrue();
    }

    [Subject("Synchronization")]
    public class given_a_class_implementing_IPreSynchronizationHook_with_constructor_parameter_known_by_container
        : SynchronizationSpecs
    {
        Establish context = () =>
        {
            SyncContext.AssemblyLocator.Add(typeof(ExamplePreSynchronizationHook).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_pass_instance_from
            = () => ExamplePreSynchronizationHook.PreSynchronizationHasBeenInvoked.ShouldBeTrue();
    }

    public class ExamplePreSynchronizationHook : IPreSynchronizationHook
    {
        public static bool PreSynchronizationHasBeenInvoked { get; set; }

        public void PreSynchronization()
        {
            PreSynchronizationHasBeenInvoked = true;    
        }
    }

    public class ExamplePreSynchronizationHookWithConstructorParameter : IPreSynchronizationHook
    {
        public bool nonNullConstructorParameterPassed;

        public ExamplePreSynchronizationHookWithConstructorParameter(IAssemblyLocator assemblyLocator)
        {
            nonNullConstructorParameterPassed = assemblyLocator != null;
        }
        
        public void PreSynchronization()
        {
        }
    }
}
