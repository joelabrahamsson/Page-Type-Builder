using System.Linq;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_property_exists_for_an_existing_PageType_but_page_type_updation_has_been_disabled_through_configuration
        : SynchronizationSpecs
    {
        static int numberOfPageDefinitionsBeforeSynchronization;

        Establish context = () => SyncContext.AddPageTypeClassToAppDomain(type =>
            {
                numberOfPageDefinitionsBeforeSynchronization = 
                    SyncContext.PageDefinitionFactory.List().Count();

                PageType existingPageType = new PageType();
                existingPageType.Name = "NameOfExistingPageType";
                {
                    type.Name = existingPageType.Name;
                    type.AddProperty(property =>
                        {
                            property.Name = "NameOfTheProperty";
                            property.Type = typeof (string);
                            property.AddAttributeTemplate(new PageTypePropertyAttribute());
                        });
                }

                SyncContext.Configuration.SetDisablePageTypeUpdation(true);
            });

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_create_a_new_PageDefinition =
            () => SyncContext.PageDefinitionFactory.List().Count()
                      .ShouldEqual(numberOfPageDefinitionsBeforeSynchronization);
    }
}
