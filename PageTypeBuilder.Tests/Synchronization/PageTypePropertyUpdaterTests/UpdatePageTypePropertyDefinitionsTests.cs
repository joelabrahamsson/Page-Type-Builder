using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
{
    public class UpdatePageTypePropertyDefinitionsTests
    {
        [Fact]
        public void GivenPageType_UpdatePageTypePropertyDefinitions_CallsGetPageTypePropertyDefinitions()
        {
            List<PageTypePropertyDefinition> definitions = new List<PageTypePropertyDefinition>();
            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdater(definitions);
            IPageType pageType = new NativePageType();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();

            pageDefinitionSynchronizationEngine.UpdatePageTypePropertyDefinitions(pageType, pageTypeDefinition);

            pageDefinitionSynchronizationEngine.PageTypePropertyDefinitionLocator.AssertWasCalled(
                locator => locator.GetPageTypePropertyDefinitions(
                               pageType, pageTypeDefinition.Type));
        }

        private PageDefinitionSynchronizationEngine CreatePageTypePropertyUpdater(
            List<PageTypePropertyDefinition> definitionsToReturnFromGetPageTypePropertyDefinitions)
        {
            MockRepository fakes = new MockRepository();
            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = PageDefinitionSynchronizationEngineFactory.PartialMock(fakes);
            pageDefinitionSynchronizationEngine.Stub(
                u =>
                u.UpdatePropertySettings(Arg<PageTypeDefinition>.Is.Anything,
                                         Arg<PageTypePropertyDefinition>.Is.Anything, Arg<PageDefinition>.Is.Anything));

            PageTypePropertyDefinitionLocator definitionLocator = fakes.Stub<PageTypePropertyDefinitionLocator>();
            definitionLocator.Stub(
                locator => locator.GetPageTypePropertyDefinitions(
                               Arg<IPageType>.Is.Anything, Arg<Type>.Is.Anything))
                .Return(definitionsToReturnFromGetPageTypePropertyDefinitions);
            definitionLocator.Replay();
            pageDefinitionSynchronizationEngine.Replay();
            pageDefinitionSynchronizationEngine.PageTypePropertyDefinitionLocator = definitionLocator;

            return pageDefinitionSynchronizationEngine;
        }
    }
}