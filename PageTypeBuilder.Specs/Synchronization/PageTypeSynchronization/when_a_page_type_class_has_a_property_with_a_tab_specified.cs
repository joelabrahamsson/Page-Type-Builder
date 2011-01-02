using System.Linq;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_has_a_property_with_a_tab_specified
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyName";
        static string tabName = "NameOfTheTab";

        Establish context = () =>
        {
            var tabClass = TabClassFactory.CreateTabClass(
            "NameOfClass", tabName, AccessLevel.Undefined, 0);

            SyncContext.AssemblyLocator.Add(tabClass.Assembly);
            PageTypePropertyAttribute propertyAttribute;
            propertyAttribute = new PageTypePropertyAttribute();
            propertyAttribute.Tab = tabClass;

            SyncContext.AddPageTypeClassToAppDomain(type =>
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(string);
                    prop.AddAttributeTemplate(propertyAttribute);
                }));
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_page_definition_whose_Tab_property_matches_the_specified_Tab =
            () => SyncContext.PageDefinitionFactory.List().First().Tab.ID.ShouldEqual(SyncContext.TabFactory.GetTabDefinition(tabName).ID);
    }
}
