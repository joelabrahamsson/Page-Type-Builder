using System;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
{
    public class UpdatePageDefinitionTests
    {
        [Fact]
        public void GivenPropertyDefinition_UpdatePageDefinition_CallsUpdatePageDefinitionValues()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = 
                CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakeUpdatePageDefinitionValuesMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

            pageTypePropertyUpdater.UpdatePageDefinition(pageDefinitionToUpdate, propertyDefinition);

            pageTypePropertyUpdater.AssertWasCalled(
                utility => utility.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition));
        }

        private PageTypePropertyUpdater CreatePageTypePropertyUpdaterWithFakePageDefinitionFactoryAndFakeUpdatePageDefinitionValuesMethod()
        {
            MockRepository fakes = new MockRepository();
            PageTypePropertyUpdater pageTypePropertyUpdater = fakes.PartialMock<PageTypePropertyUpdater>();
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinitionValues(
                               Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            pageTypePropertyUpdater.Replay();
            PageDefinitionFactory fakeFactory = fakes.Stub<PageDefinitionFactory>();
            fakeFactory.Stub(factory => factory.Save(Arg<PageDefinition>.Is.Anything));
            fakeFactory.Replay();
            pageTypePropertyUpdater.PageDefinitionFactory = fakeFactory;
            return pageTypePropertyUpdater;
        }

        [Fact]
        public void GivenUpdatedPropertyDefinition_UpdatePageDefinition_CallsPageDefinitionFactorySave()
        {
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            MockRepository fakes = new MockRepository();
            PageTypePropertyUpdater pageTypePropertyUpdater = fakes.PartialMock<PageTypePropertyUpdater>();
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinitionValues(
                               Arg<PageDefinition>.Is.Anything, Arg<PageTypePropertyDefinition>.Is.Anything));
            PageDefinitionFactory fakeFactory = fakes.Stub<PageDefinitionFactory>();
            fakeFactory.Stub(factory => factory.Save(Arg<PageDefinition>.Is.Anything));
            fakeFactory.Replay();
            pageTypePropertyUpdater.PageDefinitionFactory = fakeFactory;
            pageTypePropertyUpdater.Stub(updater => updater.SerializeValues(pageDefinitionToUpdate)).Return(Guid.NewGuid().ToString()).Repeat.Once();
            pageTypePropertyUpdater.Stub(updater => updater.SerializeValues(pageDefinitionToUpdate)).Return(Guid.NewGuid().ToString());
            pageTypePropertyUpdater.Replay();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

            pageTypePropertyUpdater.UpdatePageDefinition(pageDefinitionToUpdate, propertyDefinition);

            pageTypePropertyUpdater.PageDefinitionFactory.AssertWasCalled(factory => factory.Save(pageDefinitionToUpdate));
        }

        [Fact]
        public void GivenPropertyDefinitionWithNoEditCaptionAndNameOtherThanPageDefinitionEditCaption_UpdatedPageDefinitionEditCaption()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
            string newEditCaption = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.Name = newEditCaption;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivenPropertyDefinitionWithNewEditCaption_UpdatePageDefinitionValues_UpdatedPageDefinitionEditCaption()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
            string newEditCaption = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = newEditCaption;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivenPropertyDefinitionWithNoHelpText_UpdatePageDefinitionValues_SetsHelpTextToEmptyString()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(string.Empty, pageDefinitionToUpdate.HelpText);
        }

        private PageTypePropertyUpdater CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod()
        {
            MockRepository fakes = new MockRepository();
            PageTypePropertyUpdater pageTypePropertyUpdater = fakes.PartialMock<PageTypePropertyUpdater>();
            pageTypePropertyUpdater.Stub(
                utility => utility.UpdatePageDefinitionTab(Arg<PageDefinition>.Is.Anything, 
                                                           Arg<PageTypePropertyAttribute>.Is.Anything));
            pageTypePropertyUpdater.Replay();
            return pageTypePropertyUpdater;
        }

        private PageTypePropertyDefinition CreatePageTypePropertyDefinition()
        {
            PageTypePropertyDefinition propertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            return propertyDefinition;
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionEditCaption()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = TestValueUtility.CreateRandomString();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.EditCaption, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivePropertyDefinitionWithNoEditCaption_UpdatePageDefinitionValues_SetsPageDefinitionEditCaptionToName()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = null;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.Name, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionHelpText()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.HelpText = TestValueUtility.CreateRandomString();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.HelpText, pageDefinitionToUpdate.HelpText);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionRequired(bool required)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.Required = required;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Required, pageDefinitionToUpdate.Required);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionSearchable(bool searchable)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.Searchable = searchable;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Searchable, pageDefinitionToUpdate.Searchable);
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDefaultValue()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DefaultValue = TestValueUtility.CreateRandomString();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.DefaultValue.ToString(), pageDefinitionToUpdate.DefaultValue);
        }

        [Theory]
        [InlineData(DefaultValueType.Inherit)]
        [InlineData(DefaultValueType.None)]
        [InlineData(DefaultValueType.Value)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDefaultValueType(DefaultValueType defaultValueType)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DefaultValueType = defaultValueType;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<DefaultValueType>(propertyDefinition.PageTypePropertyAttribute.DefaultValueType, pageDefinitionToUpdate.DefaultValueType);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionLanguageSpecific(bool uniqueValuePerLanguage)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage = uniqueValuePerLanguage;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage, pageDefinitionToUpdate.LanguageSpecific);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDisplayEditMode(bool displayInEditMode)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode = displayInEditMode;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode, pageDefinitionToUpdate.DisplayEditUI);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionFieldOrder(int sortOrder)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.SortOrder = sortOrder;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<int>(propertyDefinition.PageTypePropertyAttribute.SortOrder, pageDefinitionToUpdate.FieldOrder);
        }

        [Theory]
        [InlineData(EditorToolOption.All)]
        [InlineData(EditorToolOption.Bold)]
        [InlineData(EditorToolOption.Bold | EditorToolOption.Bullets)]
        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionLongStringSettings(
            EditorToolOption longStringSettings)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.LongStringSettings = longStringSettings;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<EditorToolOption>(
                propertyDefinition.PageTypePropertyAttribute.LongStringSettings, 
                pageDefinitionToUpdate.LongStringSettings);
        }

        [Fact]
        public void GivePropertyDefinitionWithNoLongStringSettingsAndMatchingPageDefinitionWithSetting_UpdatePageDefinitionValues_DoesNotUpdateLongStringSettings()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            pageDefinitionToUpdate.LongStringSettings = EditorToolOption.SpellCheck;
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<EditorToolOption>(
                EditorToolOption.SpellCheck,
                pageDefinitionToUpdate.LongStringSettings);
        }

        [Theory]
        [InlineData(EditorToolOption.All)]
        [InlineData(EditorToolOption.Bold)]
        [InlineData(EditorToolOption.Bold | EditorToolOption.Bullets)]
        public void GivePropertyDefinitionWithLongStringSettingsAndClearAllLongStringSettings_UpdatePageDefinitionValues_SetsPageDefinitionLongStringSettings(
            EditorToolOption longStringSettings)
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.LongStringSettings = longStringSettings;
            propertyDefinition.PageTypePropertyAttribute.ClearAllLongStringSettings = true;

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<EditorToolOption>(
                propertyDefinition.PageTypePropertyAttribute.LongStringSettings,
                pageDefinitionToUpdate.LongStringSettings);
        }        

        [Fact]
        public void GivenPropertyDefinition_UpdatePageDefinitionValues_CallsUpdatePageDefinitionTab()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

            pageTypePropertyUpdater.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

            pageTypePropertyUpdater.AssertWasCalled(
                utility => utility.UpdatePageDefinitionTab(pageDefinitionToUpdate, propertyDefinition.PageTypePropertyAttribute));
        }
    }
}