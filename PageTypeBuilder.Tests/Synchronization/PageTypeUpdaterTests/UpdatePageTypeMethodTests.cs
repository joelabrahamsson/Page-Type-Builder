using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public class UpdatePageTypeMethodTests
    {
        [Fact]
        public void UpdatePageType_gets_existing_PageType()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.GetExistingPageType(definition));
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateName(pageType, definition));
        }

        [Fact]
        public void GivenNoNameSetInAttribute_WhenUpdatePageTypeCalled_UpdatesPageTypeNameWithName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            string name = definition.Type.Name;

            pageTypeUpdater.UpdateName(pageType, definition);

            Assert.Equal<string>(name, pageType.Name);
        }

        private PageTypeUpdater CreatePageTypeUpdater()
        {
            return PageTypeUpdaterFactory.Create(new Mock<PageTypeDefinitionLocator>().Object);
        }

        [Fact]
        public void GivenNameSetInAttribute_WhenUpdatePageTypeCalled_UpdatesPageTypeNameWithName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            string name = TestValueUtility.CreateRandomString();
            definition.Attribute.Name = name;

            pageTypeUpdater.UpdateName(pageType, definition);

            Assert.Equal<string>(name, pageType.Name);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateFilename()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateFilename(pageType, definition.Attribute));
        }

        [Fact]
        public void GivenNoFilenameSetInAttribute_WhenUpdatePageTypeCalled_UpdatesPageTypeFilenameWith_Filename()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();
            pageTypeUpdater.DefaultFilename = TestValueUtility.CreateRandomString();

            pageTypeUpdater.UpdateFilename(pageType, attribute);

            Assert.Equal<string>(pageTypeUpdater.DefaultFilename, pageType.FileName);
        }

        [Fact]
        public void GivenFilenameSetInAttribute_WhenUpdatePageTypeCalled_UpdatesPageTypeFilenameWith_Filename()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.Filename = TestValueUtility.CreateRandomString();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateFilename(pageType, attribute);

            Assert.Equal<string>(attribute.Filename, pageType.FileName);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateSortOrder()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateSortOrder(pageType, definition.Attribute));
        }

        [Fact]
        public void GivenSortOrder_WhenUpdateSortOrderCalled_UpdatesPageTypeSortOrder()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.SortOrder = 1;
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateSortOrder(pageType, attribute);

            Assert.Equal<int>(attribute.SortOrder, pageType.SortOrder);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDescription()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDescription(pageType, definition.Attribute));
        }

        [Fact]
        public void GivenDescription_UpdateDescription_UpdatesPageTypeDescription()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.Description = TestValueUtility.CreateRandomString();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDescription(pageType, attribute);

            Assert.Equal<string>(attribute.Description, pageType.Description);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateIsAvailable()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateIsAvailable(pageType, definition.Attribute));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivenValue_WhenUpdateIsAvailableCalled_UpdatesPageTypeAvailableInEditMode(bool availableInEditMode)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.AvailableInEditMode = availableInEditMode;
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateIsAvailable(pageType, attribute);

            Assert.Equal<bool>(attribute.AvailableInEditMode, pageType.IsAvailable);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultArchiveToPageID()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultArchivePageLink(pageType, definition.Attribute));
        }

        [Fact]
        public void GivenAttribueDefaultArchiveToPageIDPageTypeIDIsNotSet_WhenUpdateDefaultArchiveToPageIDCalled_UpdatesPageTypeDefaultArchivePageLink()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            IPageType pageType = new NativePageType();
            pageType.DefaultArchivePageLink = new PageReference(1);
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultArchivePageLink(pageType, attribute);

            Assert.Equal<PageReference>(null, pageType.DefaultArchivePageLink);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GivenAttribueDefaultArchiveToPageIDIsSet_WhenUpdateDefaultArchiveToPageIDCalled_UpdatesPageTypeDefaultArchivePageLink(int archiveToPageID)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultArchiveToPageID = archiveToPageID;
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultArchivePageLink(pageType, attribute);

            Assert.Equal<int>(attribute.DefaultArchiveToPageID, pageType.DefaultArchivePageLink.ID);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultChildOrderRule()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultChildOrderRule(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(FilterSortOrder.Alphabetical)]
        [InlineData(FilterSortOrder.ChangedDescending)]
        [InlineData(null)]
        public void GivenFilterSortOrder_WhenUpdatePageTypeCalled_UpdatesPageTypeDefaultChildOrderRule(FilterSortOrder sortOrder)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultChildSortOrder = sortOrder;
            IPageType pageType = new NativePageType();
            pageType.DefaultChildOrderRule = FilterSortOrder.PublishedAscending;
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultChildOrderRule(pageType, attribute);

            Assert.Equal<FilterSortOrder>(sortOrder, pageType.DefaultChildOrderRule);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultPageName()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultPageName(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("The default page name")]
        public void GivenName_WhenUpdateDefaultNameCalled_UpdatesPageTypeDefaultPageName(string defaultPageName)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultPageName = defaultPageName;
            IPageType pageType = new NativePageType();
            pageType.DefaultPageName = TestValueUtility.CreateRandomString();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultPageName(pageType, attribute);

            Assert.Equal<string>(defaultPageName, pageType.DefaultPageName);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultPeerOrderRule()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultPeerOrder(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivenPageType_WhenUpdateDefaultPeerOrderRuleCalled_UpdatesPageTypeDefaultPeerOrderRule(int defaultSortIndex)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultSortIndex = defaultSortIndex;
            IPageType pageType = new NativePageType();
            pageType.DefaultPeerOrder = 1;
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultPeerOrder(pageType, attribute);

            Assert.Equal<int>(defaultSortIndex, pageType.DefaultPeerOrder);
        }

        [Fact]
        public void GivenNoDefaultPeerOrderRuleInAttribute_WhenUpdateDefaultPeerOrderRuleCalled_SetsPageTypeDefaultPeerOrderRuleToDefaultValue()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            IPageType pageType = new NativePageType();
            pageType.DefaultPeerOrder = 1;
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultPeerOrder(pageType, attribute);

            Assert.Equal<int>(PageTypeUpdater.DefaultDefaultPageTypePeerOrder, pageType.DefaultPeerOrder);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultStartPublishOffset()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultStartPublishOffset(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivenAPageTypeAndANumberOfMinutes_WhenUpdateDefaultStartPublishOffsetIsCalled_ItUpdatesPageTypeDefaultStartPublishOffset(int defaultOffsetMinutes)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultStartPublishOffsetMinutes = defaultOffsetMinutes;
            IPageType pageType = new NativePageType();
            pageType.DefaultStartPublishOffset = new TimeSpan(0, 0, 1, 0);
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultStartPublishOffset(pageType, attribute);

            TimeSpan expectedOffset = new TimeSpan(0, 0, defaultOffsetMinutes, 0);
            Assert.Equal<TimeSpan>(expectedOffset, pageType.DefaultStartPublishOffset);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateDefaultStopPublishOffset()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultStopPublishOffset(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivenAPageTypeAndANumberOfMinutes_WhenUpdateDefaultStopPublishOffsetIsCalled_ItUpdatesPageTypeDefaultStopPublishOffset(int defaultOffsetMinutes)
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.DefaultStopPublishOffsetMinutes = defaultOffsetMinutes;
            IPageType pageType = new NativePageType();
            pageType.DefaultStopPublishOffset = new TimeSpan(0, 0, 1, 0);
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultStopPublishOffset(pageType, attribute);

            TimeSpan expectedOffset = new TimeSpan(0, 0, defaultOffsetMinutes, 0);
            Assert.Equal<TimeSpan>(expectedOffset, pageType.DefaultStopPublishOffset);
        }

        [Fact]
        public void WhenUpdatePageTypeCalls_CallsUpdateDefaultVisibleInMenu()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateDefaultVisibleInMenu(existingPageType, definition.Attribute));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GivenValue_UpdateDefaultVisibleInMenu_UpdatesPageTypeDefaultVisibleInMenuWithValue(bool defaultVisibleInMenu)
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            definition.Attribute.DefaultVisibleInMenu = defaultVisibleInMenu;
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultVisibleInMenu(pageType, definition.Attribute);

            Assert.Equal<bool>(defaultVisibleInMenu, pageType.DefaultVisibleInMenu);
        }

        [Fact]
        public void GivenNoDefaultVisibleInMenuInAttribute_UpdateDefaultVisibleInMenuCalled_SetsPageTypeDefaultVisibleInMenuToDefaultValue()
        {
            IPageType pageType = new NativePageType();
            pageType.DefaultVisibleInMenu = !PageTypeUpdater.DefaultDefaultVisibleInMenu;
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateDefaultVisibleInMenu(pageType, new PageTypeAttribute());

            Assert.Equal<bool>(PageTypeUpdater.DefaultDefaultVisibleInMenu, pageType.DefaultVisibleInMenu);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateFrame()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            IPageType pageType = new NativePageType();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdateFrame(pageType, attribute);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateFrame(pageType, attribute));
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsUpdateAvailablePageTypes()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdateAvailablePageTypes(Arg<IPageType>.Is.Anything, Arg<Type[]>.Is.Anything));
        }

        [Fact]
        public void GivenTypeArray_WhenUpdateAvailablePageTypesCalled_SetsPageTypeAllowedPageTypes()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            definition.Attribute.AvailablePageTypes = new[] { typeof(object) };
            IPageType existingPageType = new NativePageType();
            MockRepository mocks = new MockRepository();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition definitionToReturn = new PageTypeDefinition
            {
                Type = typeof(object),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(definitionToReturn);
            var pageTypeDefinitionLocator = new Mock<IPageTypeDefinitionLocator>();
            pageTypeDefinitionLocator.Setup(locator => locator.GetPageTypeDefinitions()).Returns(definitions);
            PageTypeUpdater pageTypeUpdater = mocks.PartialMock<PageTypeUpdater>(pageTypeDefinitionLocator.Object, new PageTypeFactory());
            IPageType allowedPageType = new NativePageType();
            allowedPageType.ID = 1;
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(definitionToReturn)).Return(allowedPageType);
            pageTypeUpdater.Replay();

            pageTypeUpdater.UpdateAvailablePageTypes(existingPageType, definition.Attribute.AvailablePageTypes);

            Assert.Equal<int[]>(new[] { 1 }, existingPageType.AllowedPageTypes);
        }

        [Fact]
        public void GivenNoAvailablePageTypes_WhenUpdateAvailablePageTypesCalled_SetsPageTypeAllowedPageTypesToEmptyArray()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            IPageType existingPageType = new NativePageType();
            MockRepository mocks = new MockRepository();
            PageTypeUpdater pageTypeUpdater = CreatePageTypeUpdater();

            pageTypeUpdater.UpdateAvailablePageTypes(existingPageType, definition.Attribute.AvailablePageTypes);

            Assert.Equal<int[]>(new int[0], existingPageType.AllowedPageTypes);
        }

        [Fact]
        public void WhenUpdatePageTypeCalled_CallsPageTypeFactorySave()
        {
            PageTypeDefinition definition = PageTypeUpdaterTestsUtility.CreateBasicPageTypeDefinition();
            PageTypeUpdater pageTypeUpdater = CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs();

            pageTypeUpdater.UpdatePageType(definition);

            pageTypeUpdater.PageTypeFactory.AssertWasCalled(factory => factory.Save(Arg<IPageType>.Is.NotNull));
        }

        private PageTypeUpdater CreateFakePageTypeUpdaterWithUpdatePageTypeMethodHelperStubs()
        {
            MockRepository mocks = new MockRepository();
            PageTypeUpdater pageTypeUpdater = mocks.PartialMock<PageTypeUpdater>(new Mock<IPageTypeDefinitionLocator>().Object, new PageTypeFactory());
            PageTypeFactory pageTypeFactory = mocks.Stub<PageTypeFactory>();
            pageTypeFactory.Stub(factory => factory.Save(Arg<IPageType>.Is.Anything));
            pageTypeFactory.Replay();
            pageTypeUpdater.PageTypeFactory = pageTypeFactory;
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(Arg<PageTypeDefinition>.Is.Anything)).Return(new NativePageType());
            pageTypeUpdater.Stub(updater => updater.UpdateName(Arg<IPageType>.Is.Anything, Arg<PageTypeDefinition>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateFilename(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateSortOrder(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDescription(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateIsAvailable(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultArchivePageLink(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultChildOrderRule(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultPageName(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultPeerOrder(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultStartPublishOffset(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultStopPublishOffset(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateDefaultVisibleInMenu(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateFrame(Arg<IPageType>.Is.Anything, Arg<PageTypeAttribute>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.UpdateAvailablePageTypes(Arg<IPageType>.Is.Anything, Arg<Type[]>.Is.Anything));
            pageTypeUpdater.Stub(updater => updater.SerializeValues(Arg<IPageType>.Is.Anything)).Repeat.Once().Return(string.Empty);
            pageTypeUpdater.Stub(updater => updater.SerializeValues(Arg<IPageType>.Is.Anything)).Repeat.Once().Return("test");
            pageTypeUpdater.Replay();
            return pageTypeUpdater;
        }
    }
}