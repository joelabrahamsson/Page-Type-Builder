using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.Validation
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_has_a_name_longer_than_50_characters_and_no_name_specified_in_the_attribute
        : SynchronizationSpecs
    {
        static Exception thrownException;

        Establish context = () =>
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                type.Name = 51.CharactersLongAlphanumericString());

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
