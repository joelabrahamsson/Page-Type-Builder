using System;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.Validation.PropertyValidation
{
    public abstract class PropertyValidationSpecs : SynchronizationSpecs
    {
        protected static Exception thrownException;
        protected static string pageTypeName = "NameOfThePropertysPageType";
        protected static string propertyName = "ThePropertysName";

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());
    }

    [Behaviors]
    public class InvalidPageTypePropertyBehavior
    {
        protected static Exception thrownException;
        protected static string pageTypeName;
        protected static string propertyName;

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_an_Exception_whose_Message_should_contain_the_propertys_name = () =>
            thrownException.Message.ShouldContain(propertyName);

        It should_throw_an_Exception_whose_Message_should_contain_the_page_type_class_name = () =>
            thrownException.Message.ShouldContain(pageTypeName);
    }
}
