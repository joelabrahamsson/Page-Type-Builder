using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_exists_with_everything_specified_in_its_attribute
        : SynchronizationSpecs
    {
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
        {
            pageTypeAttribute = AttributeHelper
                .CreatePageTypeAttributeWithEverythingSpeficied(SyncContext);

            var attributeSpecification = AttributeHelper.CreatePageTypeAttributeSpecification(pageTypeAttribute.Guid.Value.ToString());
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

        It should_create_a_new_PageType_with_the_same_Description_as_in_the_attribute= () =>
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
}
