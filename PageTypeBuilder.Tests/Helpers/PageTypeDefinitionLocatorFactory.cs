using Moq;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeDefinitionLocatorFactory
    {
        public static PageTypeDefinitionLocator Create()
        {
            return new PageTypeDefinitionLocator(new AppDomainAssemblyLocator());
        }

        public static IPageTypeDefinitionLocator Stub()
        {
            return Mock().Object;
        }

        public static Mock<IPageTypeDefinitionLocator> Mock()
        {
            return new Mock<IPageTypeDefinitionLocator>();
        }
    }
}
