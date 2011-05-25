using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
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
    }
}
