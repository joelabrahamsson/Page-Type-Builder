using System.Collections.Generic;
using EPiServer.DataAbstraction;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class CreateNonExistingPageTypes
    {
        [Fact]
        public void GivenExistingPageTypeFound_CreateNonExistingPageTypes_PageTypeUpdaterCreateNewPageTypeNotCalled()
        {
            
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new Mock<IPageTypeDefinitionLocator>().Object, new PageTypeFactory());
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(new PageType());
            pageTypeLocator.Replay();
            PageTypeSynchronizer synchronizer = CreateSynchronizer(pageTypeLocator);

            synchronizer.CreateNonExistingPageTypes(definitions);

            pageTypeUpdater.AssertWasNotCalled(updater => updater.CreateNewPageType(Arg<PageTypeDefinition>.Is.Anything));
        }

        private PageTypeSynchronizer CreateSynchronizer()
        {
            return new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
        }

        private PageTypeSynchronizer CreateSynchronizer(IPageTypeLocator pageTypeLocator)
        {
            return new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration(), new PageTypeFactory(), new PageTypePropertyUpdater(), new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())), new PageTypeValueExtractor(), new PageTypeResolver(), pageTypeLocator);
        }

        [Fact]
        public void GivenExistingPageTypeNotFound_CreateNonExistingPageTypes_PageTypeUpdaterCreateNewPageTypeCalled()
        {
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new Mock<IPageTypeDefinitionLocator>().Object, new PageTypeFactory());
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Stub(updater => updater.CreateNewPageType(definition)).Return(new PageType());
            pageTypeUpdater.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(null);
            pageTypeLocator.Replay();
            PageTypeSynchronizer synchronizer = CreateSynchronizer(pageTypeLocator);
            synchronizer.PageTypeUpdater = pageTypeUpdater;

            synchronizer.CreateNonExistingPageTypes(definitions);

            pageTypeUpdater.AssertWasCalled(updater => updater.CreateNewPageType(definition));
        }
    }
}
