using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class TabDefinitionUpdaterFactory
    {
        public static TabDefinitionUpdater Create()
        {
            return new TabDefinitionUpdater(new TabFactory());
        }

        public static TabDefinitionUpdater Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<TabDefinitionUpdater>(new TabFactory());
        }

        public static TabDefinitionUpdater PartialMock(MockRepository fakesRepository)
        {
            return fakesRepository.PartialMock<TabDefinitionUpdater>(new TabFactory());
        }
    }
}
