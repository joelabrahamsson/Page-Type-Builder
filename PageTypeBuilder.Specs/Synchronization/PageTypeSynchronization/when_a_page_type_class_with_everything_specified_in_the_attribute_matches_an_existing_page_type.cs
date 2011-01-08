using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
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
}
