using System;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.Validation
{
    [Subject("Synchronization")]
    //[Ignore]
    public class when_a_page_type_class_with_the_name_of_an_existing_page_type_exists_and_a_page_type_class_with_another_name_but_the_same_GUID_as_the_same_existing_page_type_exists
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string nameOfTheFirstPageTypeClass = "ClassThatMatchesPageTypeByName";
        static string nameOfTheSecondPageTypeClass = "ClassThatMatchesPageTypeByGUID";


        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            PageType existingPageType = new PageType();
            existingPageType.Name = nameOfTheFirstPageTypeClass;
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeFactory.Save(existingPageType);

            SyncContext.AddPageTypeClassToAppDomain(type =>
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
