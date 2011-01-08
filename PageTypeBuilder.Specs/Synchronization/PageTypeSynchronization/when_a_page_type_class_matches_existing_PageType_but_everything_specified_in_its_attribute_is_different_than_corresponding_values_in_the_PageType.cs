using System;
using System.Linq;
using EPiServer.Core;
using EPiServer.Filters;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.Fakes;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_matches_existing_PageType_but_everything_specified_in_its_attribute_is_different_than_corresponding_values_in_the_PageType
        : SynchronizationSpecs
    {
        static string pageTypeName = "NameOfTheClass";
        static PageTypeAttribute pageTypeAttribute;
        static Guid guidInAttribute;
        static int idOfExistingPageType;

        Establish context = () =>
        {
            var anotherPageTypeClass = SyncContext.CreateAndAddPageTypeClassToAppDomain(type => {});

            pageTypeAttribute = new PageTypeAttribute();
            pageTypeAttribute.AvailablePageTypes = new [] { anotherPageTypeClass };
            pageTypeAttribute.AvailableInEditMode = false;
            pageTypeAttribute.Description = "A description";
            pageTypeAttribute.SortOrder = 123;
            pageTypeAttribute.DefaultArchiveToPageID = 567;
            pageTypeAttribute.DefaultPageName = "Default page name";
            pageTypeAttribute.DefaultStartPublishOffsetMinutes = 1234;
            pageTypeAttribute.DefaultStopPublishOffsetMinutes = 12345;
            pageTypeAttribute.DefaultVisibleInMenu = false;
            pageTypeAttribute.DefaultChildSortOrder = FilterSortOrder.Alphabetical;
            pageTypeAttribute.DefaultSortIndex = 345;
            pageTypeAttribute.DefaultFrameID = 1;
            pageTypeAttribute.Filename = "~/TemplateForThePageType.aspx";
            pageTypeAttribute.Name = pageTypeName;

            guidInAttribute = Guid.NewGuid();
            
            var attributeSpecification = AttributeHelper.CreatePageTypeAttributeSpecification(guidInAttribute.ToString());
            attributeSpecification.Template = pageTypeAttribute;
            
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = pageTypeName;
                type.Attributes.Add(attributeSpecification);
            });

            var existingPageType = new FakePageType();
            existingPageType.GUID = guidInAttribute;
            existingPageType.AllowedPageTypes = new int[0];
            existingPageType.Description = pageTypeAttribute.Description + " more text";
            existingPageType.IsAvailable = !pageTypeAttribute.AvailableInEditMode;
            existingPageType.DefaultArchivePageLink = 
                new PageReference(pageTypeAttribute.DefaultArchiveToPageID + 1);
            existingPageType.SortOrder = pageTypeAttribute.SortOrder + 1;
            existingPageType.DefaultPageName = pageTypeAttribute.DefaultPageName + " more text";
            existingPageType.DefaultStartPublishOffset = 
                (pageTypeAttribute.DefaultStartPublishOffsetMinutes + 1).Minutes();
            existingPageType.DefaultStopPublishOffset =
                (pageTypeAttribute.DefaultStopPublishOffsetMinutes + 1).Minutes();
            existingPageType.DefaultVisibleInMenu = !pageTypeAttribute.DefaultVisibleInMenu;
            existingPageType.DefaultPeerOrder = pageTypeAttribute.DefaultSortIndex + 1;
            existingPageType.DefaultChildOrderRule = FilterSortOrder.Index;
            existingPageType.DefaultFrameID = pageTypeAttribute.DefaultFrameID + 1;
            existingPageType.FileName = "~/OldTemplateForThePageType.aspx";
            existingPageType.Name = pageTypeAttribute.Name + " more text";

            SyncContext.PageTypeFactory.Save(existingPageType);
            SyncContext.PageTypeFactory.ResetNumberOfSaves();

            idOfExistingPageType = existingPageType.ID;
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_save_the_PageType = () =>
            SyncContext.PageTypeFactory.GetNumberOfSaves(idOfExistingPageType).ShouldEqual(1);

        It should_update_the_PageType_to_have_the_same_Name_as_the_attibutes_Name_property = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).Name
            .ShouldEqual(pageTypeAttribute.Name);

        It should_update_the_PageType_so_that_its_AllowedPageTypes_contains_exactly_the_ids_of_the_page_types_matching_the_page_type_classes_in_the_attributes_AvailablePageTypes = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).AllowedPageTypes
            .ShouldContainOnly(pageTypeAttribute.AvailablePageTypes
                .Select(type => SyncContext.PageTypeResolver.GetPageTypeID(type).Value));

        It should_update_the_PageType_to_have_IsAvailable_set_to_the_value_of_the_attibutes_AvailableInEditMode = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).IsAvailable
            .ShouldEqual(pageTypeAttribute.AvailableInEditMode);

        It should_update_the_PageType_to_have_the_same_Description_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).Description
            .ShouldEqual(pageTypeAttribute.Description);

        It should_update_the_PageType_to_have_the_same_SortOrder_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).SortOrder
            .ShouldEqual(pageTypeAttribute.SortOrder);

        It should_update_the_PageType_to_have_the_same_DefaultPageName_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultPageName
            .ShouldEqual(pageTypeAttribute.DefaultPageName);

        It should_update_the_PageType_so_that_its_DefaultStartPublishOffsets_converted_to_minutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultStartPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStartPublishOffsetMinutes);

        It should_update_the_PageType_so_that_its_DefaultStopPublishOffsets_converted_to_minutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultStopPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStopPublishOffsetMinutes);

        It should_update_the_PageType_to_have_the_same_DefaultVisibleInMenu_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultVisibleInMenu
            .ShouldEqual(pageTypeAttribute.DefaultVisibleInMenu);

        It should_update_the_PageType_so_that_its_DefaultChildSortOrderRule_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultChildOrderRule
            .ShouldEqual(pageTypeAttribute.DefaultChildSortOrder);

        It should_update_the_PageType_so_that_its_DefaultSortIndex_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultPeerOrder
            .ShouldEqual(pageTypeAttribute.DefaultSortIndex);

        It should_update_the_PageType_so_that_its_FileName_equals_the_attributes_Filename = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).FileName
            .ShouldEqual(pageTypeAttribute.Filename);

        It should_update_the_PageType_to_have_the_same_DefaultFrameID_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeName).DefaultFrameID
            .ShouldEqual(pageTypeAttribute.DefaultFrameID);

        It  should_update_the_PageType_so_that_its_DefaultArchivePageLink_has_an_ID_equal_to_the_attributes_DefaultArchiveToPageID = () =>
            SyncContext.PageTypeFactory.Load(pageTypeName).DefaultArchivePageLink.ID
            .ShouldEqual(pageTypeAttribute.DefaultArchiveToPageID);
    }
}
