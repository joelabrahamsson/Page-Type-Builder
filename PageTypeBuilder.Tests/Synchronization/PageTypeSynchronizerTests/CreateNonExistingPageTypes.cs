using System.Collections.Generic;
using EPiServer.DataAbstraction;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using PageTypeBuilder.Tests.Helpers;
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
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Stub(fakes);
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Replay();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(new NativePageType());
            pageTypeLocator.Replay();
            PageTypeSynchronizer synchronizer = CreateSynchronizer(pageTypeLocator);

            synchronizer.CreateNonExistingPageTypes(definitions);

            pageTypeUpdater.AssertWasNotCalled(updater => updater.CreateNewPageType(Arg<PageTypeDefinition>.Is.Anything));
        }

        private PageTypeSynchronizer CreateSynchronizer(IPageTypeLocator pageTypeLocator)
        {
            return PageTypeSynchronizerFactory.Create(pageTypeLocator);
        }

        [Fact]
        public void GivenExistingPageTypeNotFound_CreateNonExistingPageTypes_PageTypeUpdaterCreateNewPageTypeCalled()
        {
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = PageTypeUpdaterFactory.Stub(fakes);
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Stub(updater => updater.CreateNewPageType(definition)).Return(new NativePageType());
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
