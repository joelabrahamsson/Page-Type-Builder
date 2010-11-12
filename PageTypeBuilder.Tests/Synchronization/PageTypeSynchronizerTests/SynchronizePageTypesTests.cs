using System.Collections.Generic;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class SynchronizePageTypesTests
    {
        [Fact]
        public void SynchronizePageTypes_CallsPageTypeDefinitionLocatorGetPageTypeDefinitions()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeDefinitionLocator.AssertWasCalled(locator => locator.GetPageTypeDefinitions(), options => options.Repeat.AtLeastOnce());
        }

        private PageTypeDefinitionLocator CreatePageTypeDefinitionLocatorStub(List<PageTypeDefinition> pageTypeDefinitions)
        {
            MockRepository fakes = new MockRepository();
            PageTypeDefinitionLocator definitionLocator = fakes.Stub<PageTypeDefinitionLocator>();
            definitionLocator.Stub(updater => updater.GetPageTypeDefinitions()).Return(pageTypeDefinitions);
            definitionLocator.Replay();

            return definitionLocator;
        }

        private PageTypeSynchronizer GetPageTypePartiallyMockedSynchronizer(PageTypeDefinitionLocator definitionLocator)
        {
            return GetPageTypePartiallyMockedSynchronizer(definitionLocator, new PageTypeBuilderConfiguration());
        }

        private PageTypeSynchronizer GetPageTypePartiallyMockedSynchronizer(PageTypeDefinitionLocator definitionLocator, PageTypeBuilderConfiguration configuration)
        {
            MockRepository fakes = new MockRepository();
            PageTypeSynchronizer pageTypeSynchronizer = fakes.PartialMock<PageTypeSynchronizer>(definitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.UpdateTabDefinitions());
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.ValidatePageTypeDefinitions(Arg<List<PageTypeDefinition>>.Is.Anything));
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.CreateNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything));
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.UpdatePageTypes(Arg<List<PageTypeDefinition>>.Is.Anything));
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.UpdatePageTypePropertyDefinitions(Arg<List<PageTypeDefinition>>.Is.Anything));
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.AddPageTypesToResolver(Arg<List<PageTypeDefinition>>.Is.Anything));
            pageTypeSynchronizer.Replay();

            return pageTypeSynchronizer;
        }

        [Fact]
        public void SynchronizePageTypes_UpdatesTabs()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.UpdateTabDefinitions());
        }

        [Fact]
        public void SynchronizePageTypes_ValidatesPageTypeDefinitions()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.ValidatePageTypeDefinitions(pageTypeDefinitions));
        }

        [Fact]
        public void SynchronizePageTypes_CreatesNonExistingPageTypes()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.CreateNonExistingPageTypes(pageTypeDefinitions));
        }

        [Fact]
        public void SynchronizePageTypes_AddsPageTypesToResolver()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.AddPageTypesToResolver(pageTypeDefinitions));
        }

        [Fact]
        public void SynchronizePageTypes_UpdatesPageTypes()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.UpdatePageTypes(pageTypeDefinitions));
        }

        [Fact]
        public void SynchronizePageTypes_UpdatesPageTypePropertyDefinitionsForPageTypes()
        {
            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(pageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator);

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.UpdatePageTypePropertyDefinitions(pageTypeDefinitions));
        }

        [Fact]
        public void GivenPageTypeUpdationDisabled_SynchronizePageTypes_OnlyAddsExistingPageTypesToResolver()
        {
            MockRepository fakes = new MockRepository();
            List<PageTypeDefinition> allPageTypeDefinitions = new List<PageTypeDefinition>
                                                                  {
                                                                      new PageTypeDefinition(),
                                                                      new PageTypeDefinition()
                                                                  };
            List<PageTypeDefinition> nonExistingPageTypesDefinitions = new List<PageTypeDefinition> { allPageTypeDefinitions[0] };
            PageTypeBuilderConfiguration configuration = fakes.Stub<PageTypeBuilderConfiguration>();
            configuration.Stub(config => config.DisablePageTypeUpdation).Return(true);
            configuration.Replay();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(allPageTypeDefinitions);
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer => synchronizer.GetNonExistingPageTypes(allPageTypeDefinitions)).Return(nonExistingPageTypesDefinitions);
            pageTypeSynchronizer.Replay();
            
            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasCalled(synchronizer => synchronizer.AddPageTypesToResolver(Arg<List<PageTypeDefinition>>.Matches(list => list.Count == 1)));
        }

        [Fact]
        public void GivenPageTypeUpdationDisabled_SynchronizePageTypes_DoesNotUpdateTabDefinitions()
        {
            MockRepository fakes = new MockRepository();
            PageTypeBuilderConfiguration configuration = fakes.Stub<PageTypeBuilderConfiguration>();
            configuration.Stub(config => config.DisablePageTypeUpdation).Return(true);
            configuration.Replay();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(new List<PageTypeDefinition>());
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer =>
                synchronizer.GetNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything)).Return(new List<PageTypeDefinition>());

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasNotCalled(synchronizer => synchronizer.UpdateTabDefinitions());
        }

        [Fact]
        public void GivenPageTypeUpdationDisabled_SynchronizePageTypes_DoesNotCreateNewPageTypes()
        {
            MockRepository fakes = new MockRepository();
            PageTypeBuilderConfiguration configuration = fakes.Stub<PageTypeBuilderConfiguration>();
            configuration.Stub(config => config.DisablePageTypeUpdation).Return(true);
            configuration.Replay();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(new List<PageTypeDefinition>());
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer => 
                synchronizer.GetNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything)).Return(new List<PageTypeDefinition>());

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasNotCalled(synchronizer => synchronizer.CreateNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything));
        }

        [Fact]
        public void GivenPageTypeUpdationDisabled_SynchronizePageTypes_DoesNotUpdatePageTypes()
        {
            MockRepository fakes = new MockRepository();
            PageTypeBuilderConfiguration configuration = fakes.Stub<PageTypeBuilderConfiguration>();
            configuration.Stub(config => config.DisablePageTypeUpdation).Return(true);
            configuration.Replay();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(new List<PageTypeDefinition>());
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer =>
                synchronizer.GetNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything)).Return(new List<PageTypeDefinition>());

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasNotCalled(synchronizer => synchronizer.UpdatePageTypes(Arg<List<PageTypeDefinition>>.Is.Anything));
        }

        [Fact]
        public void GivenPageTypeUpdationDisabled_SynchronizePageTypes_DoesNotUpdatePageTypePropertyDefinitions()
        {
            MockRepository fakes = new MockRepository();
            PageTypeBuilderConfiguration configuration = fakes.Stub<PageTypeBuilderConfiguration>();
            configuration.Stub(config => config.DisablePageTypeUpdation).Return(true);
            configuration.Replay();
            PageTypeDefinitionLocator pageTypeDefinitionLocator = CreatePageTypeDefinitionLocatorStub(new List<PageTypeDefinition>());
            PageTypeSynchronizer pageTypeSynchronizer = GetPageTypePartiallyMockedSynchronizer(pageTypeDefinitionLocator, configuration);
            pageTypeSynchronizer.Stub(synchronizer =>
                synchronizer.GetNonExistingPageTypes(Arg<List<PageTypeDefinition>>.Is.Anything)).Return(new List<PageTypeDefinition>());

            pageTypeSynchronizer.SynchronizePageTypes();

            pageTypeSynchronizer.AssertWasNotCalled(synchronizer => synchronizer.UpdatePageTypePropertyDefinitions(Arg<List<PageTypeDefinition>>.Is.Anything));
        }
    }
}
