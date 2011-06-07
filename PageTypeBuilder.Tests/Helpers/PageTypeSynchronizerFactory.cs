﻿using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using PageTypeBuilder.Synchronization.Validation;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageTypeSynchronizerFactory
    {
        public static PageTypeSynchronizer Create(PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine, IPageTypeLocator pageTypeLocator)
        {
            return Create(pageDefinitionSynchronizationEngine, new PageTypeResolver(), pageTypeLocator);
        }

        public static PageTypeSynchronizer Create(PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine, PageTypeResolver pageTypeResolver, IPageTypeLocator pageTypeLocator)
        {
            return new PageTypeSynchronizer(
                PageTypeDefinitionLocatorFactory.Create(),
                new PageTypeBuilderConfiguration(),
                pageDefinitionSynchronizationEngine,
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeRepository(), new NativePageDefinitionsMap())),
                pageTypeResolver,
                pageTypeLocator,
                PageTypeUpdaterFactory.Create(),
                TabDefinitionUpdaterFactory.Create(),
                TabLocatorFactory.Create(),
                new GlobalPropertySettingsSynchronizer(new PropertySettingsRepository(), new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator())),
                new HooksHandler(new AppDomainAssemblyLocator()));
        }

        public static PageTypeSynchronizer Create(IPageTypeLocator pageTypeLocator)
        {
            return Create(PageDefinitionSynchronizationEngineFactory.Create(), pageTypeLocator);
        }

        public static PageTypeSynchronizer Create(PageTypeResolver pageTypeResolver, IPageTypeLocator pageTypeLocator)
        {
            return Create(PageDefinitionSynchronizationEngineFactory.Create(), pageTypeResolver, pageTypeLocator);
        }

        public static PageTypeSynchronizer Create()
        {
            return Create(new PageTypeLocator(new PageTypeRepository()));
        }

        public static PageTypeSynchronizer PartialMock(
            MockRepository fakesRepository, 
            IPageTypeDefinitionLocator definitionLocator, 
            PageTypeBuilderConfiguration configuration)
        {
            return fakesRepository.PartialMock<PageTypeSynchronizer>(
                definitionLocator,
                configuration,
                PageDefinitionSynchronizationEngineFactory.Create(),
                new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeRepository(), new NativePageDefinitionsMap())),
                new PageTypeResolver(),
                new PageTypeLocator(new PageTypeRepository()),
                PageTypeUpdaterFactory.Create(),
                TabDefinitionUpdaterFactory.Create(),
                TabLocatorFactory.Create(),
                new GlobalPropertySettingsSynchronizer(new PropertySettingsRepository(), new GlobalPropertySettingsLocator(new AppDomainAssemblyLocator())),
                new HooksHandler(new AppDomainAssemblyLocator()));
        }
    }
}
