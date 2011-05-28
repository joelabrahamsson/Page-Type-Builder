using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageDefinitionSynchronizationEngineFactory
    {
        public static PageDefinitionSynchronizationEngine Create()
        {
            return new PageDefinitionSynchronizationEngine(
                new PageDefinitionFactory(),
                new PageDefinitionUpdater(new PageDefinitionFactory(), new PageDefinitionTypeFactory(), new TabFactory()),
                new PageTypePropertyDefinitionLocator(), 
                new PageDefinitionTypeMapper(new PageDefinitionTypeFactory()), 
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }

        public static PageDefinitionSynchronizationEngine PartialMock(MockRepository fakesRepository)
        {
            return fakesRepository.PartialMock<PageDefinitionSynchronizationEngine>(
                new PageDefinitionFactory(),
                new PageDefinitionUpdater(new PageDefinitionFactory(), new PageDefinitionTypeFactory(), new TabFactory()),
                new PageTypePropertyDefinitionLocator(),
                new PageDefinitionTypeMapper(new PageDefinitionTypeFactory()), 
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }

        public static PageDefinitionSynchronizationEngine Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<PageDefinitionSynchronizationEngine>(
                new PageDefinitionFactory(),
                new PageDefinitionUpdater(new PageDefinitionFactory(), new PageDefinitionTypeFactory(), new TabFactory()),
                new PageTypePropertyDefinitionLocator(),
                new PageDefinitionTypeMapper(new PageDefinitionTypeFactory()), 
                new PropertySettingsRepository(),
                new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()));
        }
    }
}
