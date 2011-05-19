using System;
using EPiServer.DataAbstraction;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public class GetExistingPageTypeTests
    {
        [Fact]
        public void GivenPageTypeWithGuid_GetExistingPageType_ReturnsPageTypeReturnedFromPageTypeFactoryLoad()
        {
            MockRepository mockRepository = new MockRepository();
            Type pageTypeType = typeof(object);
            Guid pageTypeGuid = Guid.NewGuid();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition
                                                        {
                                                            Type = pageTypeType,
                                                            Attribute = new PageTypeAttribute(pageTypeGuid.ToString())
                                                        };
            PageTypeFactory fakePageTypeFactory = mockRepository.Stub<PageTypeFactory>();
            IPageType pageTypeFromFactory = new NativePageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeGuid)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Create(
                PageTypeDefinitionLocatorFactory.Stub(), fakePageTypeFactory);

            IPageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeGuid));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }

        [Fact]
        public void GivenPageTypeWithSpecifiedNameAndNoGuid_GetExistingPageType_ReturnsPageTypeReturnedFromPageTypeFactoryLoad()
        {
            MockRepository mockRepository = new MockRepository();
            Type pageTypeType = typeof(object);
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition
                                                        {
                                                            Type = pageTypeType,
                                                            Attribute = new PageTypeAttribute { Name = Guid.NewGuid().ToString() }
                                                        };
            PageTypeFactory fakePageTypeFactory = mockRepository.Stub<PageTypeFactory>();
            IPageType pageTypeFromFactory = new NativePageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeDefinition.Attribute.Name)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Create(
                PageTypeDefinitionLocatorFactory.Stub(), fakePageTypeFactory);

            IPageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeDefinition.Attribute.Name));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }

        [Fact]
        public void GivenPageTypeWithNoGuidOrSpecifiedName_GetExistingPageType_ReturnsPageTypeReturnedFromPageTypeFactoryLoad()
        {
            MockRepository mockRepository = new MockRepository();
            PageTypeDefinition pageTypeDefinition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeFactory fakePageTypeFactory = mockRepository.Stub<PageTypeFactory>();
            IPageType pageTypeFromFactory = new NativePageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeDefinition.Type.Name)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Create(
                PageTypeDefinitionLocatorFactory.Stub(), fakePageTypeFactory);

            IPageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeDefinition.Type.Name));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }
    }
}