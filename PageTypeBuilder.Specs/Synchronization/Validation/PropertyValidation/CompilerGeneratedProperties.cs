using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.Validation.PropertyValidation.CompilerGeneratedProperties
{
    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_but_not_virtual
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.AddAttributeTemplate(new PageTypePropertyAttribute());
                property.Type = typeof(string);
                property.AnnotateAsCompilerGenerated = true;
            });
        });

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_and_is_virtual
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.AddAttributeTemplate(new PageTypePropertyAttribute());
                property.Type = typeof(string);
                property.AnnotateAsCompilerGenerated = true;
                property.GetterAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                property.SetterAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
            });
        });

        It should_not_throw_an_Exception = () =>
            thrownException.ShouldBeNull();
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_with_a_private_setter
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.AddAttributeTemplate(new PageTypePropertyAttribute());
                property.Type = typeof(string);
                property.AnnotateAsCompilerGenerated = true;
                property.GetterAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                property.SetterAttributes = MethodAttributes.Private | MethodAttributes.Virtual;
            });
        });

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_with_a_private_getter
        : PropertyValidationSpecs
    {
        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            type.Name = pageTypeName;
            type.AddProperty(property =>
            {
                property.Name = propertyName;
                property.AddAttributeTemplate(new PageTypePropertyAttribute());
                property.Type = typeof(string);
                property.AnnotateAsCompilerGenerated = true;
                property.GetterAttributes = MethodAttributes.Private | MethodAttributes.Virtual;
                property.SetterAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
            });
        });

        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
    }
}
