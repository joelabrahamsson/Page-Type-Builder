using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.Validation.PropertyValidation.InvalidTabInPageTypePropertyAttribute
{
    [Subject("Synchronization")]
    public class when_a_page_type_property_has_a_Tab_specified_in_the_attribute_that_is_not_a_valid_Tab_class
        : PropertyValidationSpecs
    {
        Establish context = () =>
        {
            PageTypePropertyAttribute propertyAttribute;
            propertyAttribute = new PageTypePropertyAttribute();
            propertyAttribute.Tab = typeof(string);

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                });
            });
        };

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }
}
