using System.Linq;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.NoSaving
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_property_exists_for_an_existing_PageType_but_page_type_updation_has_been_disabled_through_configuration
        : SynchronizationSpecs
    {
        static int numberOfPageDefinitionsBeforeSynchronization;

        Establish context = () => SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
        {
            numberOfPageDefinitionsBeforeSynchronization =
                SyncContext.PageDefinitionRepository.List().Count();

            IPageType existingPageType = new NativePageType();
            existingPageType.Name = "NameOfExistingPageType";
            {
                type.Name = existingPageType.Name;
                type.AddProperty(property =>
                {
                    property.Name = "NameOfTheProperty";
                    property.Type = typeof(string);
                    property.AddAttributeTemplate(new PageTypePropertyAttribute());
                });
            }

            SyncContext.Configuration.SetDisablePageTypeUpdation(true);
        });

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_PageDefinition =
            () => SyncContext.PageDefinitionRepository.List().Count()
                      .ShouldEqual(numberOfPageDefinitionsBeforeSynchronization);
    }
}
