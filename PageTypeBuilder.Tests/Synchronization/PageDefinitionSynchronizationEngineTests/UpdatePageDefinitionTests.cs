﻿using System.Collections.Generic;
using EPiServer.DataAbstraction;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using Xunit;
using Xunit.Extensions;

namespace PageTypeBuilder.Tests.Synchronization.pageDefinitionUpdaterTests
{
    public class UpdatePageDefinitionTests
    {
        [Fact]
        public void GivenPropertyDefinitionWithNoEditCaptionAndNameOtherThanPageDefinitionEditCaption_UpdatedPageDefinitionEditCaption()
        {
            PageDefinitionUpdater pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
            string newEditCaption = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.Name = newEditCaption;
            pageDefinitionToUpdate.EditCaption = null;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
        }

        private PageDefinitionUpdater GetPageDefinitionUpdater()
        {
            var pageDefinitionFactory = new Mock<IPageDefinitionRepository>();
            var tabFactory = new Mock<ITabDefinitionRepository>();
            tabFactory.Setup(f => f.List()).Returns(new TabDefinitionCollection {new TabDefinition(1, "Tab")});
            var pageDefinitionTypeRepository = new Mock<IPageDefinitionTypeRepository>();
            pageDefinitionTypeRepository.Setup(x => x.GetPageDefinitionType(It.IsAny<string>(), It.IsAny<string>())).
                Returns(new PageDefinitionType());
            return new PageDefinitionUpdater(
                pageDefinitionFactory.Object,
                tabFactory.Object,
                new PageDefinitionTypeMapper(pageDefinitionTypeRepository.Object, new NativePageDefinitionsMap()));
        }

        [Fact]
        public void GivenPropertyDefinitionWithNewEditCaption_UpdatePageDefinitionValues_UpdatedPageDefinitionEditCaption()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            pageDefinitionToUpdate.EditCaption = TestValueUtility.CreateRandomString();
            string newEditCaption = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = newEditCaption;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(newEditCaption, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivenPropertyDefinitionWithNoHelpText_UpdatePageDefinition_SetsHelpTextToEmptyString()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.HelpText = null;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(string.Empty, pageDefinitionToUpdate.HelpText);
        }

        //private pageDefinitionUpdater GetPageDefinitionUpdater()
        //{
        //    MockRepository fakes = new MockRepository();
        //    pageDefinitionUpdater pageDefinitionUpdater = PageTypePropertyUpdaterFactory.PartialMock(fakes);
        //    pageDefinitionUpdater.Stub(
        //        utility => utility.UpdatePageDefinitionTab(Arg<PageDefinition>.Is.Anything,
        //                                                   Arg<PageTypePropertyAttribute>.Is.Anything));
        //    pageDefinitionUpdater.Replay();
        //    return pageDefinitionUpdater;
        //}

        private PageTypePropertyDefinition CreatePageTypePropertyDefinition()
        {
            PageTypePropertyDefinition propertyDefinition = PageDefinitionSynchronizationEngineTests.PageDefinitionSynchronizationEngineTestsUtility.CreatePageTypePropertyDefinition();
            return propertyDefinition;
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionEditCaption()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = TestValueUtility.CreateRandomString();

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.EditCaption, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivePropertyDefinitionWithNoEditCaption_UpdatePageDefinition_SetsPageDefinitionEditCaptionToName()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.EditCaption = null;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.Name, pageDefinitionToUpdate.EditCaption);
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionHelpText()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.HelpText = TestValueUtility.CreateRandomString();

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.HelpText, pageDefinitionToUpdate.HelpText);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionRequired(bool required)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.Required = required;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Required, pageDefinitionToUpdate.Required);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionSearchable(bool searchable)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.Searchable = searchable;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.Searchable, pageDefinitionToUpdate.Searchable);
        }

        [Fact]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionDefaultValue()
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DefaultValue = "true";// TestValueUtility.CreateRandomString();

            // As we have had to modifiy how default value and value types are handled in definition update
            // we now have to specify a default value to satisfy this test
            propertyDefinition.PageTypePropertyAttribute.DefaultValueType = DefaultValueType.Value;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<string>(propertyDefinition.PageTypePropertyAttribute.DefaultValue.ToString(), pageDefinitionToUpdate.DefaultValue);
        }

        [Theory]
        [InlineData(DefaultValueType.Inherit)]
        [InlineData(DefaultValueType.None)]
        [InlineData(DefaultValueType.Value)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionDefaultValueType(DefaultValueType defaultValueType)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DefaultValueType = defaultValueType;

            // As we have had to modifiy how default value and value types are handled in definition update
            // we now have to specify a default value to satisfy this test
            propertyDefinition.PageTypePropertyAttribute.DefaultValue = "true";

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<DefaultValueType>(propertyDefinition.PageTypePropertyAttribute.DefaultValueType, pageDefinitionToUpdate.DefaultValueType);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionLanguageSpecific(bool uniqueValuePerLanguage)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage = uniqueValuePerLanguage;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.UniqueValuePerLanguage, pageDefinitionToUpdate.LanguageSpecific);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionDisplayEditMode(bool displayInEditMode)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode = displayInEditMode;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<bool>(propertyDefinition.PageTypePropertyAttribute.DisplayInEditMode, pageDefinitionToUpdate.DisplayEditUI);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivePropertyDefinition_UpdatePageDefinition_UpdatesPageDefinitionFieldOrder(int sortOrder)
        {
            var pageDefinitionUpdater = GetPageDefinitionUpdater();
            PageDefinition pageDefinitionToUpdate = new PageDefinition();
            PageTypePropertyDefinition propertyDefinition = CreatePageTypePropertyDefinition();
            propertyDefinition.PageTypePropertyAttribute.SortOrder = sortOrder;

            pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinitionToUpdate, propertyDefinition);

            Assert.Equal<int>(propertyDefinition.PageTypePropertyAttribute.SortOrder, pageDefinitionToUpdate.FieldOrder);
        }
    }
}