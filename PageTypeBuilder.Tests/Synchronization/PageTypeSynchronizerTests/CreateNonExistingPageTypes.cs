using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class CreateNonExistingPageTypes
    {
        [Fact]
        public void GivenExistingPageTypeFound_CreateNonExistingPageTypes_PageTypeUpdaterCreateNewPageTypeNotCalled()
        {
            PageTypeSynchronizer synchronizer = CreateSynchronizer();
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new List<PageTypeDefinition>());
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(definition)).Return(new PageType());
            pageTypeUpdater.Replay();
            synchronizer.PageTypeUpdater = pageTypeUpdater;

            synchronizer.CreateNonExistingPageTypes(definitions);

            pageTypeUpdater.AssertWasNotCalled(updater => updater.CreateNewPageType(Arg<PageTypeDefinition>.Is.Anything));
        }

        private PageTypeSynchronizer CreateSynchronizer()
        {
            return new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
        }

        [Fact]
        public void GivenExistingPageTypeNotFound_CreateNonExistingPageTypes_PageTypeUpdaterCreateNewPageTypeCalled()
        {
            PageTypeSynchronizer synchronizer = CreateSynchronizer();
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new List<PageTypeDefinition>());
            PageTypeDefinition definition = new PageTypeDefinition();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            definitions.Add(definition);
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(definition)).Return(null);
            pageTypeUpdater.Stub(updater => updater.CreateNewPageType(definition)).Return(new PageType());
            pageTypeUpdater.Replay();
            synchronizer.PageTypeUpdater = pageTypeUpdater;

            synchronizer.CreateNonExistingPageTypes(definitions);

            pageTypeUpdater.AssertWasCalled(updater => updater.CreateNewPageType(definition));
        }
    }
}
