using System;
using System.Reflection;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.Validation
{
    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_but_not_virtual
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "NameOfThePropertysPageType";
        static string propertyName = "ThePropertysName";

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

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_an_Exception_whose_Message_should_contain_the_propertys_name = () =>
            thrownException.Message.ShouldContain(propertyName);

        It should_throw_an_Exception_whose_Message_should_contain_the_page_type_class_name = () =>
            thrownException.Message.ShouldContain(pageTypeName);
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_and_is_virtual
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "NameOfThePropertysPageType";
        static string propertyName = "ThePropertysName";

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

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_not_throw_an_Exception = () =>
            thrownException.ShouldBeNull();
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_with_a_private_setter
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "NameOfThePropertysPageType";
        static string propertyName = "ThePropertysName";

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

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        It should_throw_an_Exception_whose_Message_should_contain_the_propertys_name = () =>
            thrownException.Message.ShouldContain(propertyName);

        It should_throw_an_Exception_whose_Message_should_contain_the_page_type_class_name = () =>
            thrownException.Message.ShouldContain(pageTypeName);
    }

    [Subject("Synchronization")]
    public class when_a_property_in_a_page_type_class_has_a_PageTypePropertyAttribute_and_is_compiler_generated_with_a_private_getter
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string pageTypeName = "NameOfThePropertysPageType";
        static string propertyName = "ThePropertysName";

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

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

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
