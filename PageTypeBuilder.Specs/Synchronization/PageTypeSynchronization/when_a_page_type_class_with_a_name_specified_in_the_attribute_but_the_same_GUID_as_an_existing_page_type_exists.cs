using System;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_with_a_name_specified_in_the_attribute_but_the_same_GUID_as_an_existing_page_type_exists
        : SynchronizationSpecs
    {
        static string nameInAttribute = "NameInPageTypeAttribute";
        static int idOfExistingPageType;

        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            PageType existingPageType = new PageType();
            existingPageType.Name = "ADifferentName";
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeFactory.Save(existingPageType);
            idOfExistingPageType = existingPageType.ID;


            var attributeTemplate = new PageTypeAttribute();
            attributeTemplate.Name = nameInAttribute;
            var attributeSpecification = AttributeHelper.CreatePageTypeAttributeSpecification(guid.ToString());
            attributeSpecification.Template = attributeTemplate;
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "PageTypeClassName";
                type.Attributes.Add(attributeSpecification);
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_page_type_to_have_the_name_specified_in_the_attribute =
            () => SyncContext.PageTypeFactory.Load(idOfExistingPageType).Name.ShouldEqual(nameInAttribute);
    }
}
