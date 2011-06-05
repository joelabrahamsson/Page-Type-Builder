using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Synchronization.Hooks
{
    public class SynchronizationHookContext : ISynchronizationHookContext
    {
        public SynchronizationHookContext(IAssemblyLocator assemblyLocator)
        {
            AssemblyLocator = assemblyLocator;
        }

        public virtual IAssemblyLocator AssemblyLocator
        {
            get; private set;
        }
    }
}
