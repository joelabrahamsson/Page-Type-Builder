using Machine.Specifications;
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

    public class ExamplePreSynchronizationHook : IPreSynchronizationHook
    {
        public static bool PreSynchronizationHasBeenInvoked { get; set; }

        public void PreSynchronization(ISynchronizationHookContext context)
        {
            PreSynchronizationHasBeenInvoked = true;    
        }
    }
}
