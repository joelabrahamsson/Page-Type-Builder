using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.ValueSetting
{
    class ValueSetting
    {
        //when_a_new_page_type_class_exists_with_everything_specified_in_its_attribute
        //when_a_page_type_class_matches_existing_PageType_but_everything_specified_in_its_attribute_is_different_than_corresponding_values_in_the_PageType
    }

    [Subject("Synchronization")]
    public class when_a_new_page_type_class_exists_with_everything_specified_in_its_attribute
        : SynchronizationSpecs
    {
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
        {
            pageTypeAttribute = AttributeHelper
                .CreatePageTypeAttributeWithEverythingSpeficied(SyncContext);

            var attributeSpecification = AttributeHelper
                .CreatePageTypeAttributeSpecification(pageTypeAttribute.Guid.Value.ToString());
            attributeSpecification.Template = pageTypeAttribute;

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "NameOfTheClass";
                type.Attributes.Add(attributeSpecification);
            });
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_PageType_with_the_same_GUID_as_the_attributes_Guid_property = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).GUID
            .ShouldEqual(pageTypeAttribute.Guid.Value);

        It should_create_a_new_PageType_with_the_same_Name_as_the_attributes_Name_property = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).Name
            .ShouldEqual(pageTypeAttribute.Name);

        It should_create_a_new_PageType_whose_AllowedPageTypes_contains_exactly_the_ids_of_the_page_types_matching_the_page_type_classes_in_the_attributes_AvailablePageTypes = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).AllowedPageTypes
            .ShouldContainOnly(pageTypeAttribute.AvailablePageTypes
                .Select(type => SyncContext.PageTypeResolver.GetPageTypeID(type).Value));

        It should_create_a_new_PageType_with_IsAvailable_set_to_the_value_of_the_attributes_AvailableInEditMode_property = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).IsAvailable
            .ShouldEqual(pageTypeAttribute.AvailableInEditMode);

        It should_create_a_new_PageType_with_the_same_Description_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).Description
            .ShouldEqual(pageTypeAttribute.Description);

        It should_create_a_new_PageType_with_the_same_SortOrder_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).SortOrder
            .ShouldEqual(pageTypeAttribute.SortOrder);

        It should_create_a_new_PageType_with_the_same_DefaultPageName_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultPageName
            .ShouldEqual(pageTypeAttribute.DefaultPageName);

        It should_create_a_new_PageType_whose_DefaultStartPublishOffsets_converted_to_minutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultStartPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStartPublishOffsetMinutes);

        It should_create_a_new_PageType_whose_DefaultStopPublishOffsets_converted_to_minutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultStopPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStopPublishOffsetMinutes);

        It should_create_a_new_PageType_with_the_same_value_of_DefaultVisibleInMenu_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultVisibleInMenu
            .ShouldEqual(pageTypeAttribute.DefaultVisibleInMenu);

        It should_create_a_new_PageType_whose_DefaultChildSortOrderRule_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultChildOrderRule
            .ShouldEqual(pageTypeAttribute.DefaultChildSortOrder);

        It should_create_a_new_PageType_whose_DefaultSortIndex_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultPeerOrder
            .ShouldEqual(pageTypeAttribute.DefaultSortIndex);

        It should_create_a_new_PageType_whose_FileName_equals_the_attributes_Filename = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).FileName
            .ShouldEqual(pageTypeAttribute.Filename);

        It should_create_a_new_PageType_with_the_same_DefaultFrameID_as_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultFrameID
            .ShouldEqual(pageTypeAttribute.DefaultFrameID);

        It should_create_a_new_PageType_whose_DefaultArchivePageLink_has_an_ID_equal_to_the_attributes_DefaultArchiveToPageID = () =>
            SyncContext.PageTypeFactory.Load(pageTypeAttribute.Guid.Value).DefaultArchivePageLink.ID
            .ShouldEqual(pageTypeAttribute.DefaultArchiveToPageID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_matches_existing_PageType_but_everything_specified_in_its_attribute_is_different_than_corresponding_values_in_the_PageType
        : SynchronizationSpecs
    {
        static string pageTypeName = "NameOfTheClass";
        static PageTypeAttribute pageTypeAttribute;
        static int idOfExistingPageType;

        Establish context = () =>
        {
            pageTypeAttribute = AttributeHelper
                .CreatePageTypeAttributeWithEverythingSpeficied(SyncContext);

            var attributeSpecification = AttributeHelper.CreatePageTypeAttributeSpecification(pageTypeAttribute.Guid.Value.ToString());
            attributeSpecification.Template = pageTypeAttribute;

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = pageTypeName;
                type.Attributes.Add(attributeSpecification);
            });

            IPageType existingPageType =
                PageTypeMother.CreatePageTypeWithEverythingButGuidDifferentThanAttribute(SyncContext, pageTypeAttribute);

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
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultFrameID
            .ShouldEqual(pageTypeAttribute.DefaultFrameID);

        It should_update_the_PageType_so_that_its_DefaultArchivePageLink_has_an_ID_equal_to_the_attributes_DefaultArchiveToPageID = () =>
            SyncContext.PageTypeFactory.Load(idOfExistingPageType).DefaultArchivePageLink.ID
            .ShouldEqual(pageTypeAttribute.DefaultArchiveToPageID);
    }
}
