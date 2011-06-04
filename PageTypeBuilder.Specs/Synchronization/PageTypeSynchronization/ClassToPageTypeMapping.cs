using System;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.ClassToPageTypeMapping
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_with_a_different_class_name_but_the_same_GUID_as_an_existing_page_type_exists
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static int idOfExistingPageType;

        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            IPageType existingPageType = SyncContext.PageTypeRepository.CreateNew();
            existingPageType.Name = "ADifferentName";
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeRepository.Save(existingPageType);
            idOfExistingPageType = existingPageType.ID;

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
                type.Attributes.Add(AttributeHelper.CreatePageTypeAttributeSpecification(guid.ToString()));
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_page_type_to_have_the_name_of_the_class =
            () => SyncContext.PageTypeRepository.Load(idOfExistingPageType).Name.ShouldEqual(className);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_with_a_name_specified_in_the_attribute_but_the_same_GUID_as_an_existing_page_type_exists
        : SynchronizationSpecs
    {
        static string nameInAttribute = "NameInPageTypeAttribute";
        static int idOfExistingPageType;

        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            IPageType existingPageType = SyncContext.PageTypeRepository.CreateNew();
            existingPageType.Name = "ADifferentName";
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeRepository.Save(existingPageType);
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
            () => SyncContext.PageTypeRepository.Load(idOfExistingPageType).Name.ShouldEqual(nameInAttribute);
    }

    //It appears this is not how it currently works, should be fixed
    //Currently the PageTypeLocator class will return the page type that matches
    //the class name of the page type class that has the same name as the one with
    //a name in the attribute resulting in the one with the attribute not being
    //created.
    [Ignore]
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_the_same_name_but_one_has_a_different_name_in_the_attribute
        : SynchronizationSpecs
    {
        static string commonClassName = "SharedNameForBothClasses";
        static string nameInAttribute = "PageTypeNameInAttribute";
        Establish context = () =>
        {
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = commonClassName;
            });

            SyncContext.CreateAndAddPageTypeClassToAppDomain((type, attribute) =>
            {
                type.Name = commonClassName;
                attribute.Name = nameInAttribute;
            });
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_PageType_with_the_common_name = () =>
            SyncContext.PageTypeRepository.Load(commonClassName).
                ShouldNotBeNull();

        It should_create_a_PageType_with_the_name_in_the_attribute = () =>
            SyncContext.PageTypeRepository.Load(nameInAttribute).
                ShouldNotBeNull();
    }
}
