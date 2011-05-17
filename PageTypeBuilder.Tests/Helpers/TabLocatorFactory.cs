using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class TabLocatorFactory
    {
        public static TabLocator Create()
        {
            return new TabLocator(new AppDomainAssemblyLocator());
        }

        public static TabLocator Stub(MockRepository fakeRepository)
        {
            return fakeRepository.Stub<TabLocator>(new AppDomainAssemblyLocator());
        }
    }
}
