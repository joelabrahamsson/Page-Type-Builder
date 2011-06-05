using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Synchronization.Hooks
{
    public class HooksHandler : IHooksHandler
    {
        private IAssemblyLocator assemblyLocator;

        public HooksHandler(IAssemblyLocator assemblyLocator)
        {
            this.assemblyLocator = assemblyLocator;
        }

        public void InvokePreSynchronizationHooks()
        {
            var hooks = assemblyLocator.AssembliesWithReferenceToAssemblyOf<IPreSynchronizationHook>()
                .TypesAssignableTo<IPreSynchronizationHook>()
                .Concrete()
                .Select(t => (IPreSynchronizationHook) Activator.CreateInstance(t))
                .ToList();

            hooks.ForEach(h => h.PreSynchronization(new SynchronizationHookContext(assemblyLocator)));
        }
    }
}
