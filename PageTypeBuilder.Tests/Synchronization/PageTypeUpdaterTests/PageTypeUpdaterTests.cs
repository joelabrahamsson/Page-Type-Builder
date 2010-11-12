using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public class PageTypeUpdaterTests
    {
        [Fact]
        public void WhenConstructorCalled_SetsPageTypeFactoryPropertyToInstanceOfPageTypeFactory()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            Assert.NotNull(pageTypeUpdater.PageTypeFactory);
            Assert.Equal<Type>(typeof(PageTypeFactory), pageTypeUpdater.PageTypeFactory.GetType());
        }

        private PageTypeUpdater CreatePageTypeUpdater()
        {
            return new PageTypeUpdater(null);
        }

        [Fact]
        public void WhenPageTypeFactoryPropertySet_SetsPageTypeFactoryPropertyValue()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            PageTypeFactory setPageTypeFactory = new PageTypeFactory();
            pageTypeUpdater.PageTypeFactory = setPageTypeFactory;

            Assert.Equal<PageTypeFactory>(setPageTypeFactory, pageTypeUpdater.PageTypeFactory);
        }

        [Fact]
        public void WhenDefaultFilenameNotSet_DefaultsToDefaultPageTypeFilenameConstant()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            Assert.Equal<string>(PageTypeUpdater.DefaultPageTypeFilename, pageTypeUpdater.DefaultFilename);
        }

        [Fact]
        public void WhenDefaultFilenameSet_ReturnsSpecifiedString()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            pageTypeUpdater.DefaultFilename = TestValueUtility.CreateRandomString();

            string defaultFilename = pageTypeUpdater.DefaultFilename;

            Assert.Equal<string>(defaultFilename, pageTypeUpdater.DefaultFilename);
        }
    }
}