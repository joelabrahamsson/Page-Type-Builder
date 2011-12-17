using System;
using Autofac;
using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Bootstrapping
{
    public class DefaultBootstrapper
    {
        [Fact]
        public void AfterConfiguration_CanResolvePageTypeSynchronizer()
        {
            var containerBuilder = new ContainerBuilder();
            var bootstrapper = new PageTypeBuilder.DefaultBootstrapper();
            bootstrapper.Configure(containerBuilder);
            var container = containerBuilder.Build();

            var exception = Record.Exception(() => container.Resolve<PageTypeSynchronizer>());

            Assert.Null(exception);
        }

        [Fact]
        public void AfterConfiguration_CanResolvePropertySettingsRepositoryFactoryMethods()
        {
            var containerBuilder = new ContainerBuilder();
            var bootstrapper = new PageTypeBuilder.DefaultBootstrapper();
            bootstrapper.Configure(containerBuilder);
            var container = containerBuilder.Build();

            var exception = Record.Exception(() => container.Resolve<Func<IPropertySettingsRepository>>());

            Assert.Null(exception);
        }
    }
}
