using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.DefaultValues
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_property_with_no_EditCaption_specified_in_the_attribute_exists
        : SynchronizationSpecs
    {
        static string propertyName = "NameOfTheProperty";

        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                type.AddProperty(property =>
                {
                    property.Name = propertyName;
                    property.Type = typeof(string);
                    property.AddAttributeTemplate(new PageTypePropertyAttribute());
                })
            );

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_new_PageDefinition_with_an_EditCaption_equal_to_the_propertys_name =
            () => SyncContext.PageDefinitionFactory.List().First()
                .EditCaption.ShouldEqual(propertyName);
    }
}
