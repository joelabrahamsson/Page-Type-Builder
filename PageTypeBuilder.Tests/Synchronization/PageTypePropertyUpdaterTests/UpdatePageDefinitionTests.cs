//using System;
//using EPiServer.DataAbstraction;
//using EPiServer.Editor;
//using PageTypeBuilder.Abstractions;
//using PageTypeBuilder.Discovery;
//using PageTypeBuilder.Synchronization;
//using PageTypeBuilder.Tests.Helpers;
//using Rhino.Mocks;
//using Xunit;
//using Xunit.Extensions;

//namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
//{
//    public class UpdatePageDefinitionTests
//    {
//        [Fact]
//        public void GivenUpdatedPropertyDefinition_UpdatePageDefinition_CallsPageDefinitionFactorySave()
//        {
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            MockRepository fakes = new MockRepository();
//            PageDefinitionFactory fakeFactory = fakes.Stub<PageDefinitionFactory>();
//            fakeFactory.Stub(factory => factory.Save(Arg<PageDefinition>.Is.Anything));
//            fakeFactory.Replay();
//            var pageDefinitionUpdater = new PageDefinitionUpdater(
//                fakeFactory, 
//                new PageDefinitionTypeFactory(),
//                new TabFactory());
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

//            pageDefinitionUpdater.UpdatePageDefinition(pageDefinitionToUpdate, propertyDefinition);

//            fakeFactory.AssertWasCalled(factory => factory.Save(pageDefinitionToUpdate));
//        }

//        [Fact]
//        public void GivenPropertyDefinitionWithNoEditCaptionAndNameOtherThanPageDefinitionEditCaption_UpdatedPageDefinitionEditCaption()
//        {
//            var pageDefinitionUpdater = PageDefinitionUpdaterFactory.Create();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
//            string newEditCaption = TestValueUtility.CreateRandomString();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.Name = newEditCaption;

//            pageDefinitionUpdater.UpdatePageDefinition(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
//        }

//        [Fact]
//        public void GivenPropertyDefinitionWithNewEditCaption_UpdatePageDefinitionValues_UpdatedPageDefinitionEditCaption()
//        {
//            var pageDefinitionUpdater = PageDefinitionUpdaterFactory.Create();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
//            string newEditCaption = TestValueUtility.CreateRandomString();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.EditCaption = newEditCaption;

//            pageDefinitionUpdater.UpdatePageDefinition(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
//        }

//        [Fact]
//        public void GivenPropertyDefinitionWithNoHelpText_UpdatePageDefinitionValues_SetsHelpTextToEmptyString()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(string.Empty, pageDefinitionToUpdate.HelpText);
//        }

//        private PageDefinitionSynchronizationEngine CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod()
//        {
//            MockRepository fakes = new MockRepository();
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = PageTypePropertyUpdaterFactory.PartialMock(fakes);
//            pageDefinitionSynchronizationEngine.Stub(
//                utility => utility.UpdatePageDefinitionTab(Arg<PageDefinition>.Is.Anything, 
//                                                           Arg<PageTypePropertyAttribute>.Is.Anything));
//            pageDefinitionSynchronizationEngine.Replay();
//            return pageDefinitionSynchronizationEngine;
//        }

//        private PageTypePropertyDefinition CreatePageTypePropertyDefinition()
//        {
//            PageTypePropertyDefinition propertyDefinition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
//            return propertyDefinition;
//        }

//        [Fact]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionEditCaption()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.EditCaption = TestValueUtility.CreateRandomString();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.EditCaption, pageDefinitionToUpdate.EditCaption);
//        }

//        [Fact]
//        public void GivePropertyDefinitionWithNoEditCaption_UpdatePageDefinitionValues_SetsPageDefinitionEditCaptionToName()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.EditCaption = null;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(propertyDefinition.Name, pageDefinitionToUpdate.EditCaption);
//        }

//        [Fact]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionHelpText()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.HelpText = TestValueUtility.CreateRandomString();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.HelpText, pageDefinitionToUpdate.HelpText);
//        }

//        [Theory]
//        [InlineData(false)]
//        [InlineData(true)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionRequired(bool required)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.Required = required;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Required, pageDefinitionToUpdate.Required);
//        }

//        [Theory]
//        [InlineData(false)]
//        [InlineData(true)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionSearchable(bool searchable)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.Searchable = searchable;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Searchable, pageDefinitionToUpdate.Searchable);
//        }

//        [Fact]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDefaultValue()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.DefaultValue = TestValueUtility.CreateRandomString();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.DefaultValue.ToString(), pageDefinitionToUpdate.DefaultValue);
//        }

//        [Theory]
//        [InlineData(DefaultValueType.Inherit)]
//        [InlineData(DefaultValueType.None)]
//        [InlineData(DefaultValueType.Value)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDefaultValueType(DefaultValueType defaultValueType)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.DefaultValueType = defaultValueType;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<DefaultValueType>(propertyDefinition.PageTypePropertyAttribute.DefaultValueType, pageDefinitionToUpdate.DefaultValueType);
//        }

//        [Theory]
//        [InlineData(false)]
//        [InlineData(true)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionLanguageSpecific(bool uniqueValuePerLanguage)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage = uniqueValuePerLanguage;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage, pageDefinitionToUpdate.LanguageSpecific);
//        }

//        [Theory]
//        [InlineData(false)]
//        [InlineData(true)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionDisplayEditMode(bool displayInEditMode)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode = displayInEditMode;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode, pageDefinitionToUpdate.DisplayEditUI);
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(1)]
//        public void GivePropertyDefinition_UpdatePageDefinitionValues_UpdatesPageDefinitionFieldOrder(int sortOrder)
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
//            propertyDefinition.PageTypePropertyAttribute.SortOrder = sortOrder;

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<int>(propertyDefinition.PageTypePropertyAttribute.SortOrder, pageDefinitionToUpdate.FieldOrder);
//        }

//        [Fact]
//        public void GivePropertyDefinitionWithNoLongStringSettingsAndMatchingPageDefinitionWithSetting_UpdatePageDefinitionValues_DoesNotUpdateLongStringSettings()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            pageDefinitionToUpdate.LongStringSettings = EditorToolOption.SpellCheck;
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            Assert.Equal<EditorToolOption>(
//                EditorToolOption.SpellCheck,
//                pageDefinitionToUpdate.LongStringSettings);
//        }

//        [Fact]
//        public void GivenPropertyDefinition_UpdatePageDefinitionValues_CallsUpdatePageDefinitionTab()
//        {
//            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine = CreatePageTypePropertyUpdaterWithFakeUpdatePageDefinitionTabMethod();
//            PageDefinition pageDefinitionToUpdate = new PageDefinition();
//            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();

//            pageDefinitionSynchronizationEngine.UpdatePageDefinitionValues(pageDefinitionToUpdate, propertyDefinition);

//            pageDefinitionSynchronizationEngine.AssertWasCalled(
//                utility => utility.UpdatePageDefinitionTab(pageDefinitionToUpdate, propertyDefinition.PageTypePropertyAttribute));
//        }
//    }
//}