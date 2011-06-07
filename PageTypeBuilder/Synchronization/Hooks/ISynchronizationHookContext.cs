using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Synchronization.Hooks
{
    public interface ISynchronizationHookContext
    {
        IAssemblyLocator AssemblyLocator { get; }
    }
}
