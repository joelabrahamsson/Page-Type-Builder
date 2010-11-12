using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_SetsTabFactoryPropertyValue()
        {
            TabDefinitionUpdater tabUpdater = new TabDefinitionUpdater();

            Assert.NotNull(tabUpdater.TabFactory);
            Assert.Equal<Type>(typeof(TabFactory), tabUpdater.TabFactory.GetType());
        }
    }
}
