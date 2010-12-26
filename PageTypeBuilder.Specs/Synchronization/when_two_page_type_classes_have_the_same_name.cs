using System;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization
{
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_the_same_name
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "aPageType";

        Establish context = () =>
        {
            SyncContext.AddPageTypeClass(type =>
            {
                type.Name = pageTypeName;
            });

            SyncContext.AddPageTypeClass(type =>
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
}
