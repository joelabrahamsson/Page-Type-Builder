using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypePropertyUpdaterFactory
    {
        public static PageTypePropertyUpdater Create()
        {
            return new PageTypePropertyUpdater(
                new PageDefinitionFactory(),
                new PageDefinitionTypeFactory(),
                new TabFactory(),
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }

        public static PageTypePropertyUpdater PartialMock(MockRepository fakesRepository)
        {
            return fakesRepository.PartialMock<PageTypePropertyUpdater>(
                new PageDefinitionFactory(),
                new PageDefinitionTypeFactory(),
                new TabFactory(),
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }

        public static PageTypePropertyUpdater Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<PageTypePropertyUpdater>(
                new PageDefinitionFactory(),
                new PageDefinitionTypeFactory(),
                new TabFactory(),
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }
    }
}
