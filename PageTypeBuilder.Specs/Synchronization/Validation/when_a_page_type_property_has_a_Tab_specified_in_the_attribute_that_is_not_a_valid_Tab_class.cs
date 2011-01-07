using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_property_has_a_Tab_specified_in_the_attribute_that_is_not_a_valid_Tab_class
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static string nameOfThePageTypeClass = "NameOfThePageTypeClass";
        static Exception thrownException;

        Establish context = () =>
        {
            PageTypePropertyAttribute propertyAttribute;
            propertyAttribute = new PageTypePropertyAttribute();
            propertyAttribute.Tab = typeof(string);

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = nameOfThePageTypeClass;
                type.AddProperty(prop =>
                    {
                        prop.Name = propertyName;
                        prop.Type = typeof (string);
                        prop.AddAttributeTemplate(propertyAttribute);
                    });
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_an_Exception_with_a_message_containing_the_name_of_the_page_type_class = () =>
            thrownException.Message.ShouldContain(nameOfThePageTypeClass);

        It should_throw_an_Exception_with_a_message_containing_the_name_of_the_property = () =>
            thrownException.Message.ShouldContain(propertyName);
    }
}
