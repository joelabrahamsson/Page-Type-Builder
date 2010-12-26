using System;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.Validation
{
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
                type.AddAttributeTemplate(new PageTypeAttribute() { Name = nameInAttribute});
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
}
