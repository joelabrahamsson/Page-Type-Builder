using System;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
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
            PageType pageTypeFromFactory = new PageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeGuid)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            pageTypeUpdater.PageTypeFactory = fakePageTypeFactory;

            PageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeGuid));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }

        private PageTypeUpdater CreatePageTypeUpdater()
        {
            return new PageTypeUpdater(null);
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
            PageType pageTypeFromFactory = new PageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeDefinition.Attribute.Name)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            pageTypeUpdater.PageTypeFactory = fakePageTypeFactory;

            PageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeDefinition.Attribute.Name));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }

        [Fact]
        public void GivenPageTypeWithNoGuidOrSpecifiedName_GetExistingPageType_ReturnsPageTypeReturnedFromPageTypeFactoryLoad()
        {
            MockRepository mockRepository = new MockRepository();
            PageTypeDefinition pageTypeDefinition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeFactory fakePageTypeFactory = mockRepository.Stub<PageTypeFactory>();
            PageType pageTypeFromFactory = new PageType();
            pageTypeFromFactory.ID = 1;
            fakePageTypeFactory.Expect(factory => factory.Load(pageTypeDefinition.Type.Name)).Return(pageTypeFromFactory);
            fakePageTypeFactory.Replay();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            pageTypeUpdater.PageTypeFactory = fakePageTypeFactory;

            PageType returnedPageType = pageTypeUpdater.GetExistingPageType(pageTypeDefinition);

            fakePageTypeFactory.AssertWasCalled(factory => factory.Load(pageTypeDefinition.Type.Name));
            Assert.Equal<int>(pageTypeFromFactory.ID, returnedPageType.ID);
        }
    }
}