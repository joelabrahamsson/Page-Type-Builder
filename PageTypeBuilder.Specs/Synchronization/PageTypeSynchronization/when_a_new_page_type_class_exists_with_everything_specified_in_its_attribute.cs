using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_exists_with_everything_specified_in_its_attribute
        : SynchronizationSpecs
    {
        static string className = "NameOfTheClass";
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
        {
            pageTypeAttribute = new PageTypeAttribute();
            pageTypeAttribute.Description = "A description";
            pageTypeAttribute.SortOrder = 123;
            pageTypeAttribute.DefaultPageName = "Default page name";
            pageTypeAttribute.DefaultStartPublishOffsetMinutes = 1234;
            pageTypeAttribute.DefaultStopPublishOffsetMinutes = 12345;
            pageTypeAttribute.DefaultVisibleInMenu = false;
            pageTypeAttribute.DefaultChildSortOrder = FilterSortOrder.Alphabetical;
            pageTypeAttribute.DefaultSortIndex = 345;
            //pageTypeAttribute.DefaultFrameID
            //Kvar: Filename, DefaultFrameID, DefaultArchiveToPageId, AvailablePageTypes, GUID

            var attributeSpecification = new AttributeSpecification(pageTypeAttribute);

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
                type.Attributes.Add(attributeSpecification);
            });
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_PageType_with_the_same_Description_as_in_the_attibute= () =>
            SyncContext.PageTypeFactory.Load(className).Description
            .ShouldEqual(pageTypeAttribute.Description);

        It should_create_a_new_PageType_with_the_same_SortOrder_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(className).SortOrder
            .ShouldEqual(pageTypeAttribute.SortOrder);

        It should_create_a_new_PageType_with_the_same_DefaultPageName_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultPageName
            .ShouldEqual(pageTypeAttribute.DefaultPageName);

        It should_create_a_new_PageType_whose_DefaultStartPublishOffsets_TotalMinutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultStartPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStartPublishOffsetMinutes);

        It should_create_a_new_PageType_whose_DefaultStopPublishOffsets_TotalMinutes_is_equal_to_DefaultStartPublishOffsetMinutes_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultStopPublishOffset.TotalMinutes
            .ShouldEqual(pageTypeAttribute.DefaultStopPublishOffsetMinutes);

        It should_create_a_new_PageType_with_the_same_DefaultVisibleInMenu_as_in_the_attibute = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultVisibleInMenu
            .ShouldEqual(pageTypeAttribute.DefaultVisibleInMenu);

        It should_create_a_new_PageType_whose_DefaultChildSortOrderRule_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultChildOrderRule
            .ShouldEqual(pageTypeAttribute.DefaultChildSortOrder);

        It DefaultSortIndexshould_create_a_new_PageType_whose_DefaultSortIndex_equals_the_attributes_DefaultChildSortOrder = () =>
            SyncContext.PageTypeFactory.Load(className).DefaultPeerOrder
            .ShouldEqual(pageTypeAttribute.DefaultSortIndex);
    }
}
