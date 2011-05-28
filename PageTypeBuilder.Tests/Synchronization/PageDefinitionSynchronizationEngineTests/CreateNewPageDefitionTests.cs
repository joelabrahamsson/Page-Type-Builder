using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageDefinitionSynchronizationEngineTests
{
    public class CreateNewPageDefitionTests
    {
        private PageDefinitionSynchronizationEngine CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod()
        {
            MockRepository fakes = new MockRepository();
            PageDefinitionFactory fakePageDefinitionFactory = fakes.Stub<PageDefinitionFactory>();
            fakePageDefinitionFactory.Stub(factory => factory.Save(Arg<PageDefinition>.Is.Anything));
            fakePageDefinitionFactory.Replay();
            PageDefinitionSynchronizationEngine partiallyMockedUtility = PageDefinitionSynchronizationEngineFactory.PartialMock(fakes);
            partiallyMockedUtility.Stub(
                utility => utility.SetPageDefinitionType(
                    Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            partiallyMockedUtility.PageDefinitionFactory = fakePageDefinitionFactory;
            partiallyMockedUtility.Replay();

            return partiallyMockedUtility;
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_ReturnsPageDefinitionWithCorrectPageTypeID()
        {
            PageDefinitionSynchronizationEngine utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = utility.CreateNewPageDefinition(definition);

            Assert.Equal<int>(definition.PageType.ID, returnedPageDefintion.PageTypeID);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_ReturnsPageDefinitionWithCorrectName()
        {
            PageDefinitionSynchronizationEngine utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = utility.CreateNewPageDefinition(definition);

            Assert.Equal<string>(definition.Name, returnedPageDefintion.Name);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_CallsSetPageDefinitionTypeMethod()
        {
            PageDefinitionSynchronizationEngine partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            partiallyMockedUtility.AssertWasCalled(utility => utility.SetPageDefinitionType(returnedPageDefintion, pageTypePropertyDefinition));
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoEditCaption_CreateNewPageDefinition_SetsEditCaptionToDefinitionName()
        {
            PageDefinitionSynchronizationEngine partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();
            pageTypePropertyDefinition.Name = TestValueUtility.CreateRandomString();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            Assert.Equal<string>(pageTypePropertyDefinition.Name, returnedPageDefintion.EditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithEditCaption_CreateNewPageDefinition_SetsEditCaptionToDefinitionEditCaption()
        {
            PageDefinitionSynchronizationEngine partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();
            pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption = TestValueUtility.CreateRandomString();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            Assert.Equal<string>(pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption, returnedPageDefintion.EditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_ReturnsPageDefinitionWithCorrectType2()
        {
            PageDefinitionType expectedPageDefintionType = new PageDefinitionType(
                1, PropertyDataType.String, TestValueUtility.CreateRandomString());

            PageTypePropertyDefinition pageTypePropertyDefinition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();
            PageDefinition pageDefinition = new PageDefinition();
            MockRepository fakes = new MockRepository();
            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = PageDefinitionSynchronizationEngineFactory.PartialMock(fakes);
            pageDefinitionSynchronizationEngine.Stub(
                utility => utility.GetPageDefinitionType(pageTypePropertyDefinition)
                ).Return(expectedPageDefintionType);
            pageDefinitionSynchronizationEngine.Replay();

            pageDefinitionSynchronizationEngine.SetPageDefinitionType(pageDefinition, pageTypePropertyDefinition);

            Assert.Equal<PageDefinitionType>(expectedPageDefintionType, pageDefinition.Type);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_SavesPageDefinition()
        {
            PageDefinitionSynchronizationEngine utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefinition = utility.CreateNewPageDefinition(definition);

            utility.PageDefinitionFactory.AssertWasCalled(factory => factory.Save(returnedPageDefinition));
        }
    }
}
