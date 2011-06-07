using System;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public class PageTypeUpdaterTests
    {
        [Fact]
        public void WhenConstructorCalled_SetsPageTypeFactoryPropertyToInstanceOfPageTypeFactory()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            Assert.NotNull(pageTypeUpdater.PageTypeRepository);
            Assert.Equal<Type>(typeof(PageTypeRepository), pageTypeUpdater.PageTypeRepository.GetType());
        }

        private PageTypeUpdater CreatePageTypeUpdater()
        {
            return PageTypeUpdaterFactory.Create(
                PageTypeDefinitionLocatorFactory.Stub());
        }

        [Fact]
        public void WhenPageTypeFactoryPropertySet_SetsPageTypeFactoryPropertyValue()
        {
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            PageTypeRepository setPageTypeRepository = new PageTypeRepository();
            pageTypeUpdater.PageTypeRepository = setPageTypeRepository;

            Assert.Equal<IPageTypeRepository>(setPageTypeRepository, pageTypeUpdater.PageTypeRepository);
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