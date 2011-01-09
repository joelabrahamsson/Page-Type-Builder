using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.NoSaving
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_with_everything_specified_in_the_attribute_matches_an_existing_page_type_and_the_values_in_the_attribute_is_the_same_as_corresponding_values_in_the_PageType
        : SynchronizationSpecs
    {
        static string pageTypeName = "NameOfThePageType";
        static int pageTypeId;

        Establish context = () =>
        {
            var pageTypeAttribute = AttributeHelper.CreatePageTypeAttributeWithEverythingSpeficied(SyncContext);
            var existingPageType = PageTypeMother.CreatePageTypeWithSameValuesAsAttribute(SyncContext, pageTypeAttribute);
            existingPageType = SyncContext.PageTypeFactory.CreateNew();
            SyncContext.PageTypeFactory.Save(existingPageType);
            pageTypeId = existingPageType.ID;
            SyncContext.PageTypeFactory.ResetNumberOfSaves();

            var attributeSpecification =
                AttributeHelper.CreatePageTypeAttributeSpecification(pageTypeAttribute.Guid.ToString());
            attributeSpecification.Template = pageTypeAttribute;

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = pageTypeName;
                type.Attributes.Add(attributeSpecification);
            });
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_save_the_PageType = () =>
            SyncContext.PageTypeFactory.GetNumberOfSaves(pageTypeId).ShouldEqual(0);
    }

    [Subject("Synchronization")]
    public class when_page_type_updation_has_been_disabled_through_configuration_and_a_new_page_type_class_exists
        : SynchronizationSpecs
    {
        static int numberOfPageTypesBeforeSynchronization;

        Establish context = () =>
        {
            numberOfPageTypesBeforeSynchronization =
                    SyncContext.PageTypeFactory.List().Count();

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = "NameOfThePageTypeClass";
            });

            SyncContext.Configuration.SetDisablePageTypeUpdation(true);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_page_type =
            () => SyncContext.PageTypeFactory.List().Count()
                .ShouldEqual(numberOfPageTypesBeforeSynchronization);
    }

    [Subject("Synchronization")]
    public class when_page_type_updation_has_been_disabled_through_configuration_and_a_page_type_class_has_the_same_name_as_an_existing_PageType_but_a_different_SortOrder
        : SynchronizationSpecs
    {
        static int pageTypeId;
        static int originalSortOrder;

        Establish context = () =>
        {
            IPageType existingPageType = SyncContext.PageTypeFactory.CreateNew();
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
