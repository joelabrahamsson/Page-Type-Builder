using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageDefinitionSynchronizationEngineFactory
    {
        public static PageDefinitionSynchronizationEngine Create()
        {
            return new PageDefinitionSynchronizationEngine(
                new PageDefinitionUpdater(new PageDefinitionRepository(), new TabDefinitionRepository(), new PageDefinitionTypeMapper(new PageDefinitionTypeRepository(), new NativePageDefinitionsMap())),
                new PageTypePropertyDefinitionLocator(),
                new PageDefinitionSpecificPropertySettingsUpdater(new PropertySettingsRepository(), new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()), new PageDefinitionRepository()),
                new PageTypeRepository());
        }

        public static PageDefinitionSynchronizationEngine PartialMock(MockRepository fakesRepository)
        {
            return fakesRepository.PartialMock<PageDefinitionSynchronizationEngine>(
                new PageDefinitionUpdater(new PageDefinitionRepository(), new TabDefinitionRepository(), new PageDefinitionTypeMapper(new PageDefinitionTypeRepository(), new NativePageDefinitionsMap())),
                new PageTypePropertyDefinitionLocator(),
                new PageDefinitionSpecificPropertySettingsUpdater(new PropertySettingsRepository(), new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()), new PageDefinitionRepository()));
        }

        public static PageDefinitionSynchronizationEngine Stub(MockRepository fakesRepository)
        {
            return fakesRepository.Stub<PageDefinitionSynchronizationEngine>(
                new PageDefinitionUpdater(new PageDefinitionRepository(), new TabDefinitionRepository(), new PageDefinitionTypeMapper(new PageDefinitionTypeRepository(), new NativePageDefinitionsMap())),
                new PageTypePropertyDefinitionLocator(), 
                new PageDefinitionSpecificPropertySettingsUpdater(new PropertySettingsRepository(), new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator()), new PageDefinitionRepository()),
                new PageTypeRepository());
        }
    }
}
