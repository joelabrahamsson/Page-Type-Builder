using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;
using Refraction;

namespace PageTypeBuilder.Specs.Hooks
{
    [Subject("Synchronization")]
    public class given_a_class_implementing_IPreSynchronizationHook
        : SynchronizationSpecs
    {
        static Assembly assembly;
        static string className = "ExamplePreSynchronizationHook";
        static string verificationPropertyName = "PreSynchronizationHasBeenInvoked";

        Establish context = () =>
        {
            assembly = Create.Assembly(with => 
                with.Class(className)
                    .Implementing<IPreSynchronizationHook>()
                    .AutomaticProperty<bool>(x => 
                        x.Named(verificationPropertyName)
                        .Static())
                    .PublicMethod(x => x.Named("PreSynchronization")
                        .Body("{0} = true;", verificationPropertyName)
                        .Parameter<ISynchronizationHookContext>("context")));
            
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_invoke_the_class_PreSynchronization_method
            = () => assembly.GetTypeNamed(className).InvokeMember<bool>(verificationPropertyName, BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public).ShouldBeTrue(); //ExamplePreSynchronizationHook.PreSynchronizationHasBeenInvoked.ShouldBeTrue();
    }
}
