using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.Validation.PropertyValidation.InvalidPropertyType
{
    public class SomeClassThatDefinitelyIsNotAProperty { }

    [Subject("Synchronization")]
    public class when_a_page_type_property_has_a_type_that_does_not_match_a_default_mapping_and_with_no_type_specified_in_the_attribute
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.AddAttributeTemplate(new PageTypePropertyAttribute());
                property.Type = typeof(SomeClassThatDefinitelyIsNotAProperty);
            });
        });

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_has_an_invalid_type_specified_in_the_attribute
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.Type = typeof(string);
                property.AddAttributeTemplate(
                    new PageTypePropertyAttribute
                    {
                        Type = typeof(SomeClassThatDefinitelyIsNotAProperty)
                    });
            });
        });

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }
}