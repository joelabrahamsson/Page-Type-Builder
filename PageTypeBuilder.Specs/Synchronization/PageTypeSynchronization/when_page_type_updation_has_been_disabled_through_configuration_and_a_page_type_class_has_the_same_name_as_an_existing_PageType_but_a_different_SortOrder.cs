using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_page_type_updation_has_been_disabled_through_configuration_and_a_page_type_class_has_the_same_name_as_an_existing_PageType_but_a_different_SortOrder
        : SynchronizationSpecs
    {
        static int pageTypeId;
        static int originalSortOrder;

        Establish context = () =>
        {
            IPageType existingPageType = new NativePageType();
            existingPageType.Name = "NameOfThePageType";
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.SortOrder = originalSortOrder;
            
            SyncContext.PageTypeFactory.Save(existingPageType);
            pageTypeId = existingPageType.ID;
            SyncContext.PageTypeFactory.ResetNumberOfSaves();
            
            var attribute = new PageTypeAttribute();
            attribute.SortOrder = 2;
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = existingPageType.Name;
                type.AddAttributeTemplate(attribute);
            });

            SyncContext.Configuration.SetDisablePageTypeUpdation(true);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_update_the_existing_page_types_SortOrder = () =>
            SyncContext.PageTypeFactory.Load(pageTypeId).SortOrder.ShouldEqual(originalSortOrder);

        It should_not_save_the_PageType = () =>
            SyncContext.PageTypeFactory.GetNumberOfSaves(pageTypeId).ShouldEqual(0);
    }
}
