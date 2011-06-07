using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_SetsTabFactoryPropertyValue()
        {
            TabDefinitionUpdater tabUpdater = TabDefinitionUpdaterFactory.Create();

            Assert.NotNull(tabUpdater.TabDefinitionRepository);
            Assert.Equal<Type>(typeof(TabDefinitionRepository), tabUpdater.TabDefinitionRepository.GetType());
        }
    }
}
