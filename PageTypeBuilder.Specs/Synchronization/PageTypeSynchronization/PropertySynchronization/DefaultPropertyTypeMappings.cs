using System;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization
{
    public class DefaultPropertyTypeMappingSpecs : SynchronizationSpecs
    {
        protected static string PropertyName = "NameOfTheProperty";

        protected static void AddPageTypeClassWithAPropertyOfTypeToAppDomain<TProperty>()
        {
            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                type.AddProperty(prop =>
                {
                    prop.Name = PropertyName;
                    prop.Type = typeof(TProperty);
                    prop.AddAttributeTemplate(new PageTypePropertyAttribute());
                }));
        }

        protected static PageDefinition GetCreatedPageDefinition()
        {
            return SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => p.Name == PropertyName);
        }
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_string_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<string>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyXhtmlString =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyXHtmlString");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_int_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<int>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyNumber =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyNumber");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_nullable_int_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<int?>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyNumber =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyNumber");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_bool_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<bool>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyBoolean =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyBoolean");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_nullable_bool_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<bool?>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyBoolean =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyBoolean");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_DateTime_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<DateTime>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyDate =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyDate");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_nullable_DateTime_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<DateTime?>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyDate =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyDate");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_float_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<float>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyFloatNumber =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyFloatNumber");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_nullable_float_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<float?>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyFloatNumber =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyFloatNumber");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_PageType_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<PageType>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyPageType =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyPageType");
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_PageReference_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<PageReference>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyPageReference =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyPageReference");
    }

    [Ignore]
    [Subject("Synchronization")]
    public class when_a_page_type_property_of_type_LinkItemCollection_doesnt_have_a_type_specified_in_the_attribute
        : DefaultPropertyTypeMappingSpecs
    {
        Establish context = AddPageTypeClassWithAPropertyOfTypeToAppDomain<LinkItemCollection>;

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition_of_type_PropertyLinkCollection =
            () =>
            GetCreatedPageDefinition().Type.Name.ShouldEqual("PropertyLinkCollection");
    }
}
