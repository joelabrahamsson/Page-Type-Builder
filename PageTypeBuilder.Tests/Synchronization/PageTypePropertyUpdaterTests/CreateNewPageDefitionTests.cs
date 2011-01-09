using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.PageTypePropertyUpdaterTests
{
    public class CreateNewPageDefitionTests
    {
        private PageTypePropertyUpdater CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod()
        {
            MockRepository fakes = new MockRepository();
            PageDefinitionFactory fakePageDefinitionFactory = fakes.Stub<PageDefinitionFactory>();
            fakePageDefinitionFactory.Stub(factory => factory.Save(Arg<PageDefinition>.Is.Anything));
            fakePageDefinitionFactory.Replay();
            PageTypePropertyUpdater partiallyMockedUtility = PageTypePropertyUpdaterFactory.PartialMock(fakes);
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
            PageTypePropertyUpdater utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = utility.CreateNewPageDefinition(definition);

            Assert.Equal<int>(definition.PageType.ID, returnedPageDefintion.PageTypeID);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_ReturnsPageDefinitionWithCorrectName()
        {
            PageTypePropertyUpdater utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = utility.CreateNewPageDefinition(definition);

            Assert.Equal<string>(definition.Name, returnedPageDefintion.Name);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_CallsSetPageDefinitionTypeMethod()
        {
            PageTypePropertyUpdater partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            partiallyMockedUtility.AssertWasCalled(utility => utility.SetPageDefinitionType(returnedPageDefintion, pageTypePropertyDefinition));
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoEditCaption_CreateNewPageDefinition_SetsEditCaptionToDefinitionName()
        {
            PageTypePropertyUpdater partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            pageTypePropertyDefinition.Name = TestValueUtility.CreateRandomString();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            Assert.Equal<string>(pageTypePropertyDefinition.Name, returnedPageDefintion.EditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithEditCaption_CreateNewPageDefinition_SetsEditCaptionToDefinitionEditCaption()
        {
            PageTypePropertyUpdater partiallyMockedUtility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption = TestValueUtility.CreateRandomString();

            PageDefinition returnedPageDefintion = partiallyMockedUtility.CreateNewPageDefinition(pageTypePropertyDefinition);

            Assert.Equal<string>(pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption, returnedPageDefintion.EditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_ReturnsPageDefinitionWithCorrectType2()
        {
            PageDefinitionType expectedPageDefintionType = new PageDefinitionType(
                1, PropertyDataType.String, TestValueUtility.CreateRandomString());

            PageTypePropertyDefinition pageTypePropertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            PageDefinition pageDefinition = new PageDefinition();
            MockRepository fakes = new MockRepository();
            PageTypePropertyUpdater pageTypePropertyUpdater = PageTypePropertyUpdaterFactory.PartialMock(fakes);
            pageTypePropertyUpdater.Stub(
                utility => utility.GetPageDefinitionType(pageTypePropertyDefinition)
                ).Return(expectedPageDefintionType);
            pageTypePropertyUpdater.Replay();

            pageTypePropertyUpdater.SetPageDefinitionType(pageDefinition, pageTypePropertyDefinition);

            Assert.Equal<PageDefinitionType>(expectedPageDefintionType, pageDefinition.Type);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_CreateNewPageDefinition_SavesPageDefinition()
        {
            PageTypePropertyUpdater utility = CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakedSetPageDefinitionTypeMethod();
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();

            PageDefinition returnedPageDefinition = utility.CreateNewPageDefinition(definition);

            utility.PageDefinitionFactory.AssertWasCalled(factory => factory.Save(returnedPageDefinition));
        }
    }
}
