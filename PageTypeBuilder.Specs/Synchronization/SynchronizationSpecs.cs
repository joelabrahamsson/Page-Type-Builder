using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class SynchronizationSpecs
    {
        protected static InMemoryContext SyncContext;

        Establish context = () =>
             SyncContext = new InMemoryContext();
    }
}
