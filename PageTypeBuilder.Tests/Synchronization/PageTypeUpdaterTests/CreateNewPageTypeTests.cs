using System;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public class CreateNewPageTypeTests
    {
        [Fact]
        public void GivenDefinitionWithNoSpecifiedName_CreateNewPageType_ReturnsNewPageTypeWithNameSetToTheNameOfTheTypeName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);

            PageType returnedPageType = pageTypeUpdater.CreateNewPageType(definition);

            Assert.Equal(typeof(object).Name, returnedPageType.Name);
        }

        private PageTypeUpdater CreatePageTypeUpdater()
        {
            return new PageTypeUpdater(null);
        }

        private void SetupPageTypeUpdaterWithFakePageTypeFactory(PageTypeUpdater pageTypeUpdater)
        {
            MockRepository mocks = new MockRepository();
            PageTypeFactory fakePageTypeFactory = mocks.Stub<PageTypeFactory>();
            fakePageTypeFactory.Stub(factory => factory.Save(Arg<PageType>.Is.NotNull));
            fakePageTypeFactory.Replay();
            pageTypeUpdater.PageTypeFactory = fakePageTypeFactory;
        }

        [Fact]
        public void GivenDefinitionWithASpecifiedName_CreateNewPageType_ReturnsPageTypeWithThatName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            definition.Attribute.Name = TestValueUtility.CreateRandomString();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);

            PageType returnedPageType = pageTypeUpdater.CreateNewPageType(definition);

            Assert.Equal(definition.Attribute.Name, returnedPageType.Name);
        }

        [Fact]
        public void GivenDefinitionWithASpecifiedGuid_CreateNewPageType_ReturnsPageTypeWithThatGuid()
        {
            PageTypeDefinition definition = new PageTypeDefinition
                                                {
                                                    Type = typeof(object),
                                                    Attribute = new PageTypeAttribute(Guid.NewGuid().ToString())
                                                };
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);

            PageType returnedPageType = pageTypeUpdater.CreateNewPageType(definition);

            Assert.Equal<Guid?>(definition.Attribute.Guid, returnedPageType.GUID);
        }

        [Fact]
        public void GivenDefinitionWithASpecifiedFilename_CreateNewPageType_ReturnsPageTypeWithThatFilename()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            definition.Attribute.Filename = TestValueUtility.CreateRandomString();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);

            PageType returnedPageType = pageTypeUpdater.CreateNewPageType(definition);

            Assert.Equal<string>(definition.Attribute.Filename, returnedPageType.FileName);
        }

        [Fact]
        public void Given_definition_without_a_specified_filename_CreateNewPageType_returns_PageType_with_default_filename()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);
            //Override the default filename to something without a slash in the beginning as 
            //the PageType.Filename set method requires that EPiServer is started if
            //the filename begins with a slash
            pageTypeUpdater.DefaultFilename = TestValueUtility.CreateRandomString();

            PageType returnedPageType = pageTypeUpdater.CreateNewPageType(definition);

            Assert.Equal<string>(pageTypeUpdater.DefaultFilename, returnedPageType.FileName);
        }

        [Fact]
        public void WhenCreateNewPageTypeCalled_CallsPageTypeFactorySave()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            SetupPageTypeUpdaterWithFakePageTypeFactory(pageTypeUpdater);

            pageTypeUpdater.CreateNewPageType(definition);

            pageTypeUpdater.PageTypeFactory.AssertWasCalled(factory => factory.Save(Arg<PageType>.Is.NotNull));
        }
    }
}