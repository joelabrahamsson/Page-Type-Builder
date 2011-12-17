using Autofac;
using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder
{
    public class DefaultBootstrapper
    {
        public void Configure(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<AppDomainAssemblyLocator>()
                .As<IAssemblyLocator>();
            containerBuilder
                .RegisterType<PageTypeRepository>()
                .As<IPageTypeRepository>();
            containerBuilder
                .RegisterType<PageDefinitionRepository>()
                .As<IPageDefinitionRepository>();
            containerBuilder
                .RegisterType<PageDefinitionTypeRepository>()
                .As<IPageDefinitionTypeRepository>();
            containerBuilder
                .RegisterType<PageTypeLocator>()
                .As<IPageTypeLocator>();
            containerBuilder
                .RegisterType<PageDefinitionTypeRepository>()
                .As<IPageDefinitionTypeRepository>();
            containerBuilder
                .RegisterType<PageTypeDefinitionLocator>()
                .As<IPageTypeDefinitionLocator>();
            containerBuilder
                .RegisterType<PageDefinitionTypeMapper>()
                .As<PageDefinitionTypeMapper>();
            containerBuilder
                .RegisterType<PageTypeValueExtractor>()
                .As<IPageTypeValueExtractor>();
            containerBuilder
                .RegisterType<GlobalPropertySettingsLocator>()
                .As<IGlobalPropertySettingsLocator>();
            containerBuilder
                .RegisterType<GlobalPropertySettingsSynchronizer>()
                .As<GlobalPropertySettingsSynchronizer>();
            containerBuilder
                .RegisterType<PropertySettingsRepository>()
                .As<IPropertySettingsRepository>();
            containerBuilder
                .RegisterType<PageTypeUpdater>()
                .As<PageTypeUpdater>();
            containerBuilder
                .RegisterType<TabDefinitionUpdater>()
                .As<TabDefinitionUpdater>();
            containerBuilder
                .RegisterType<TabLocator>()
                .As<TabLocator>();
            containerBuilder
                .RegisterType<PageDefinitionSynchronizationEngine>()
                .As<PageDefinitionSynchronizationEngine>();
            containerBuilder
                .RegisterType<TabDefinitionRepository>()
                .As<ITabDefinitionRepository>();
            containerBuilder
                .RegisterType<PageTypeSynchronizer>()
                .As<PageTypeSynchronizer>();
            containerBuilder
                .RegisterType<PageTypeDefinitionValidator>()
                .As<PageTypeDefinitionValidator>();
            containerBuilder
                .Register(context => PageTypeBuilderConfiguration.GetConfiguration())
                .As<PageTypeBuilderConfiguration>();
            containerBuilder
                .RegisterInstance(PageTypeResolver.Instance)
                .As<PageTypeResolver>();
            containerBuilder
                .RegisterType<PageDefinitionUpdater>()
                .As<IPageDefinitionUpdater>();
            containerBuilder
                .RegisterType<PageTypePropertyDefinitionLocator>()
                .As<PageTypePropertyDefinitionLocator>();
            containerBuilder
                .RegisterType<NativePageDefinitionsMap>()
                .As<INativePageDefinitionsMap>();
            containerBuilder
                .RegisterType<PageDefinitionSpecificPropertySettingsUpdater>()
                .As<PageDefinitionSpecificPropertySettingsUpdater>();
            containerBuilder
                .RegisterType<HooksHandler>()
                .As<IHooksHandler>();
        }
    }
}
