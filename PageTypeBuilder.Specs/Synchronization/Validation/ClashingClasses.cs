using System;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.Validation.ClashingClasses
{
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_same_guid
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string firstPageTypeName = "aPageType";
        static string secondPageTypeName = "anotherPageType";

        Establish context = () =>
        {
            string guid = new Guid().ToString();
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = firstPageTypeName;
                type.Attributes.Add(AttributeHelper.CreatePageTypeAttributeSpecification(guid));
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = secondPageTypeName;
                type.Attributes.Add(AttributeHelper.CreatePageTypeAttributeSpecification(guid));
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        [Ignore]
        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        [Ignore]
        It should_throw_Exception_containing_the_first_page_types_name = () =>
            thrownException.Message.ShouldContain(firstPageTypeName);

        [Ignore]
        It should_throw_Exception_containing_the_second_page_types_name = () =>
            thrownException.Message.ShouldContain(secondPageTypeName);
    }

    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_the_same_name
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "aPageType";

        Establish context = () =>
        {
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
            });

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_Exception_containing_the_page_types_name = () =>
            thrownException.Message.ShouldContain(pageTypeName);
    }

    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_the_same_name_in_the_attribute
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string firstPageTypeName = "aPageType";
        static string secondPageTypeName = "anotherPageType";
        static string nameInAttribute = "nameInAttribute";

        Establish context = () =>
        {
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = firstPageTypeName;
                type.AddAttributeTemplate(new PageTypeAttribute() { Name = nameInAttribute });
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = secondPageTypeName;
                type.AddAttributeTemplate(new PageTypeAttribute() { Name = nameInAttribute });
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_Exception_containing_the_first_page_types_name = () =>
            thrownException.Message.ShouldContain(firstPageTypeName);

        It should_throw_Exception_containing_the_second_page_types_name = () =>
            thrownException.Message.ShouldContain(secondPageTypeName);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_with_the_name_of_an_existing_page_type_exists_and_a_page_type_class_with_another_name_but_the_same_GUID_as_the_same_existing_page_type_exists
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string nameOfTheFirstPageTypeClass = "ClassThatMatchesPageTypeByName";
        static string nameOfTheSecondPageTypeClass = "ClassThatMatchesPageTypeByGUID";


        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            IPageType existingPageType = SyncContext.PageTypeRepository.CreateNew();
            existingPageType.Name = nameOfTheFirstPageTypeClass;
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeRepository.Save(existingPageType);

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = nameOfTheFirstPageTypeClass;
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = nameOfTheSecondPageTypeClass;
                type.Attributes.Add(AttributeHelper.CreatePageTypeAttributeSpecification(guid.ToString()));
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        [Ignore]
        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        [Ignore]
        It should_throw_an_Exception_with_a_message_containing_the_name_of_the_first_class = () =>
            thrownException.Message.ShouldContain(nameOfTheFirstPageTypeClass);

        [Ignore]
        It should_throw_an_Exception_with_a_message_containing_the_name_of_the_second_class = () =>
            thrownException.Message.ShouldContain(nameOfTheSecondPageTypeClass);
    }
}
