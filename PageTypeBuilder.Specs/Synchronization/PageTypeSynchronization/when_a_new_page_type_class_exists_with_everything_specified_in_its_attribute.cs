using System;
using System.Linq;
using EPiServer.Filters;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_exists_with_everything_specified_in_its_attribute
        : SynchronizationSpecs
    {
        static string pageTypeName = "NameOfTheClass";
        static PageTypeAttribute pageTypeAttribute;
        static Guid guidInAttribute;
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
            //Requires abstraction of the Frame class
            //pageTypeAttribute.DefaultFrameID = 456;
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
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_PageType_with_the_same_GUID_as_the_attibutes_Guid_property = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).GUID
            .ShouldEqual(guidInAttribute);

        It should_create_a_new_PageType_with_the_same_Name_as_the_attibutes_Name_property = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).Name
            .ShouldEqual(pageTypeAttribute.Name);

        It should_create_a_new_PageType_whose_AllowedPageTypes_contains_exactly_the_ids_of_the_page_types_matching_the_page_type_classes_in_the_attributes_AvailablePageTypes = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).AllowedPageTypes
            .ShouldContainOnly(pageTypeAttribute.AvailablePageTypes
                .Select(type => SyncContext.PageTypeResolver.GetPageTypeID(type).Value));

        It should_create_a_new_PageType_with_IsAvailable_set_to_the_value_as_the_attibutes_AvailableInEditMode = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).IsAvailable
            .ShouldEqual(pageTypeAttribute.AvailableInEditMode);

        It should_create_a_new_PageType_with_the_same_Description_as_in_the_attibute= () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).Description
            .ShouldEqual(pageTypeAttribute.Description);

        It should_create_a_new_PageType_with_the_same_SortOrder_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).SortOrder
            .ShouldEqual(pageTypeAttribute.SortOrder);

        It should_create_a_new_PageType_with_the_same_DefaultPageName_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultPageName
            .ShouldEqual(pageTypeAttribute.DefaultPageName);

        It should_create_a_new_PageType_whose_DefaultStartPublishOffsets_TotalMinutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultStartPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStartPublishOffsetMinutes);

        It should_create_a_new_PageType_whose_DefaultStopPublishOffsets_TotalMinutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultStopPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStopPublishOffsetMinutes);

        It should_create_a_new_PageType_with_the_same_DefaultVisibleInMenu_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultVisibleInMenu
            .ShouldEqual(pageTypeAttribute.DefaultVisibleInMenu);

        It should_create_a_new_PageType_whose_DefaultChildSortOrderRule_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultChildOrderRule
            .ShouldEqual(pageTypeAttribute.DefaultChildSortOrder);

        It should_create_a_new_PageType_whose_DefaultSortIndex_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).DefaultPeerOrder
            .ShouldEqual(pageTypeAttribute.DefaultSortIndex);

        It should_create_a_new_PageType_whose_FileName_equals_the_attributes_Filename = () =>
            SyncContext.PageTypeFactory.Load(guidInAttribute).FileName
            .ShouldEqual(pageTypeAttribute.Filename);

        //Requires abstraction of the Frame class
        [Ignore]
        It should_create_a_new_PageType_with_the_same_DefaultFrameID_as_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(pageTypeName).DefaultFrameID
            .ShouldEqual(pageTypeAttribute.DefaultFrameID);

        It should_create_a_new_PageType_whose_DefaultArchivePageLink_has_an_ID_equal_to_the_attributes_DefaultArchiveToPageID = () =>
            SyncContext.PageTypeFactory.Load(pageTypeName).DefaultArchivePageLink.ID
            .ShouldEqual(pageTypeAttribute.DefaultArchiveToPageID);
    }
}
