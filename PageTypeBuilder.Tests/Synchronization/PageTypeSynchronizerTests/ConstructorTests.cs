using System;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Synchronization.Validation;
using Xunit;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_SetsPageTypeResolverPropertyToInstanceOfPageTypeResolver()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.PageTypeResolver);
            Assert.Equal<Type>(typeof(PageTypeResolver), synchronizer.PageTypeResolver.GetType());
            Assert.Equal<PageTypeResolver>(PageTypeResolver.Instance, synchronizer.PageTypeResolver);
        }

        private PageTypeSynchronizer CreatePageTypeSynchronizer()
        {
            return new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
        }

        [Fact]
        public void Constructor_SetsTabLocatorPropertyToInstanceOfTabLocator()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.TabLocator);
            Assert.Equal<Type>(typeof(TabLocator), synchronizer.TabLocator.GetType());
        }

        [Fact]
        public void Constructor_SetsTabDefinitionUpdaterPropertyToInstanceOfTabDefinitionUpdater()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.TabDefinitionUpdater);
            Assert.Equal<Type>(typeof(TabDefinitionUpdater), synchronizer.TabDefinitionUpdater.GetType());
        }

        [Fact]
        public void Constructor_SetsPageTypeUpdaterPropertyToInstanceOfPageTypeUpdater()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.PageTypeUpdater);
            Assert.Equal<Type>(typeof(PageTypeUpdater), synchronizer.PageTypeUpdater.GetType());
        }

        [Fact]
        public void Constructor_SetsPageTypePropertyUpdaterPropertyToInstanceOfPageTypePropertyUpdater()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.PageTypePropertyUpdater);
            Assert.Equal<Type>(typeof(PageTypePropertyUpdater), synchronizer.PageTypePropertyUpdater.GetType());
        }

        [Fact]
        public void Constructor_SetsPageTypeDefinitionValidatorPropertyToInstanceOfPageTypeDefinitionValidator()
        {
            PageTypeSynchronizer synchronizer = CreatePageTypeSynchronizer();

            Assert.NotNull(synchronizer.PageTypeDefinitionValidator);
            Assert.Equal<Type>(typeof(PageTypeDefinitionValidator), synchronizer.PageTypeDefinitionValidator.GetType());
        }
    }
}
