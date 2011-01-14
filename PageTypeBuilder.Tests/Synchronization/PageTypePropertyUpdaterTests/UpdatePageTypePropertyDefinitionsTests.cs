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
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdater(definitions);
            IPageType pageType = new NativePageType();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();

            pageTypePropertyUpdater.UpdatePageTypePropertyDefinitions(pageType, pageTypeDefinition);

            pageTypePropertyUpdater.PageTypePropertyDefinitionLocator.AssertWasCalled(
                locator => locator.GetPageTypePropertyDefinitions(
                               pageType, pageTypeDefinition.Type));
        }

        private PageTypePropertyUpdater CreatePageTypePropertyUpdater(
            List<PageTypePropertyDefinition> definitionsToReturnFromGetPageTypePropertyDefinitions)
        {
            MockRepository fakes = new MockRepository();
            PageTypePropertyUpdater pageTypePropertyUpdater = PageTypePropertyUpdaterFactory.PartialMock(fakes);
            PageTypePropertyDefinitionLocator definitionLocator = fakes.Stub<PageTypePropertyDefinitionLocator>();
            definitionLocator.Stub(
                locator => locator.GetPageTypePropertyDefinitions(
                               Arg<IPageType>.Is.Anything, Arg<Type>.Is.Anything))
                .Return(definitionsToReturnFromGetPageTypePropertyDefinitions);
            definitionLocator.Replay();
            pageTypePropertyUpdater.Replay();
            pageTypePropertyUpdater.PageTypePropertyDefinitionLocator = definitionLocator;

            return pageTypePropertyUpdater;
        }

        [Fact]
        public void GivenPageType_UpdatePageTypePropertyDefinitions_CallsGetExistingPageDefinition()
        {
            List<PageTypePropertyDefinition> definitions = new List<PageTypePropertyDefinition>();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definitions.Add(pageTypePropertyDefinition);
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdater(definitions);
            IPageType pageType = new NativePageType();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            pageTypePropertyUpdater.Stub(utility => utility.GetExistingPageDefinition(
                                                        pageType, pageTypePropertyDefinition)).Return(new PageDefinition());
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinition(
                               Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            pageTypePropertyUpdater.Replay();

            pageTypePropertyUpdater.UpdatePageTypePropertyDefinitions(pageType, pageTypeDefinition);

            pageTypePropertyUpdater.AssertWasCalled(
                utility => utility.GetExistingPageDefinition(
                               pageType, pageTypePropertyDefinition));
        }

        [Fact]
        public void GivenNoExistingPageDefinition_UpdatePageTypePropertyDefinitions_CallsCreateNewPageDefition()
        {
            List<PageTypePropertyDefinition> definitions = new List<PageTypePropertyDefinition>();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definitions.Add(pageTypePropertyDefinition);
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdater(definitions);
            IPageType pageType = new NativePageType();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            pageTypePropertyUpdater.Stub(utility => utility.GetExistingPageDefinition(
                                                        pageType, pageTypePropertyDefinition)).Return(null);
            pageTypePropertyUpdater.Stub(
                utility => utility.CreateNewPageDefinition(pageTypePropertyDefinition)).Return(new PageDefinition());
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinition(
                               Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            pageTypePropertyUpdater.Replay();

            pageTypePropertyUpdater.UpdatePageTypePropertyDefinitions(pageType, pageTypeDefinition);

            pageTypePropertyUpdater.AssertWasCalled(
                utility => utility.CreateNewPageDefinition(pageTypePropertyDefinition));
        }

        [Fact]
        public void GivenPageType_UpdatePageTypePropertyDefinitions_CallsUpdatePageDefinition()
        {
            List<PageTypePropertyDefinition> definitions = new List<PageTypePropertyDefinition>();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definitions.Add(pageTypePropertyDefinition);
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdater(definitions);
            IPageType pageType = new NativePageType();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            PageDefinition existingPageDefinition = new PageDefinition();
            pageTypePropertyUpdater.Stub(utility => utility.GetExistingPageDefinition(
                                                        pageType, pageTypePropertyDefinition)).Return(existingPageDefinition);
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinition(
                               Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            pageTypePropertyUpdater.Replay();

            pageTypePropertyUpdater.UpdatePageTypePropertyDefinitions(pageType, pageTypeDefinition);

            pageTypePropertyUpdater.AssertWasCalled(
                utility => utility.UpdatePageDefinition(
                               existingPageDefinition, pageTypePropertyDefinition));
        }
    }
}