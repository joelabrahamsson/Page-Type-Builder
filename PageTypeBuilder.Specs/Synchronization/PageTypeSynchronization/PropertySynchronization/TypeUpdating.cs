using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers.Fakes;
using Refraction;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.TypeUpdating
{
    [Subject("Synchronization")]
    public abstract class TypeUpdatingSpec : SynchronizationSpecs
    {
        protected static string propertyName = "PropertyName";
        protected static int pageDefinitionId;
        protected static IPageType existingPageType;

        Establish context = () =>
            {
                existingPageType = new FakePageType(SyncContext.PageDefinitionRepository);
                existingPageType.Name = "MyPageType";
                SyncContext.PageTypeRepository.Save(existingPageType);
                existingPageType = SyncContext.PageTypeRepository.Load(existingPageType.Name);
            };
    }

    public class when_a_property_maps_to_an_existing_property_with_a_different_type_that_has_the_same_data_type
        : TypeUpdatingSpec
    {
        Establish context = () =>
            {
                var existingPageDefinition = new PageDefinition
                    {
                        Name = propertyName,
                        EditCaption = propertyName,
                        PageTypeID = existingPageType.ID,
                        Type =
                            SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType
                            <PropertyXhtmlString>()
                    };
                SyncContext.PageDefinitionRepository.Save(existingPageDefinition);
                pageDefinitionId = SyncContext.PageDefinitionRepository.List(existingPageType.ID)
                    .Where(x => x.Name == propertyName).First().ID;

                var assembly = Create.Assembly(with =>
                                with.Class(existingPageType.Name)
                                    .Inheriting<TypedPageData>()
                                    .AnnotatedWith<PageTypeAttribute>()
                                    .AutomaticProperty<string>(x =>
                                        x.Named(propertyName)
                                         .AnnotatedWith<PageTypePropertyAttribute>(
                                            new {Type = typeof (PropertyLongString)})));

                SyncContext.AssemblyLocator.Add(assembly);
            };
        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_PageDefinition_to_be_of_the_new_type =
            () => SyncContext.PageDefinitionRepository.Load(pageDefinitionId)
                      .Type.DefinitionType.ShouldEqual(typeof(PropertyLongString));
    }

    public class when_a_property_maps_to_an_existing_property_with_a_different_type_that_has_a_different_data_type
        : TypeUpdatingSpec
    {
        Establish context = () =>
        {
            var existingPageDefinition = new PageDefinition
            {
                Name = propertyName,
                EditCaption = propertyName,
                PageTypeID = existingPageType.ID,
                Type = SyncContext.PageDefinitionTypeRepository.GetPageDefinitionType
                    <PropertyXhtmlString>()
            };
            SyncContext.PageDefinitionRepository.Save(existingPageDefinition);
            pageDefinitionId = SyncContext.PageDefinitionRepository.List(existingPageType.ID)
                .Where(x => x.Name == propertyName).First().ID;

            var assembly = Create.Assembly(with =>
                            with.Class(existingPageType.Name)
                                .Inheriting<TypedPageData>()
                                .AnnotatedWith<PageTypeAttribute>()
                                .AutomaticProperty<string>(x =>
                                    x.Named(propertyName)
                                     .AnnotatedWith<PageTypePropertyAttribute>(
                                        new { Type = typeof(PropertyNumber) })));

            SyncContext.AssemblyLocator.Add(assembly);
        };
        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_update_the_PageDefinition_to_be_of_the_new_type =
            () => SyncContext.PageDefinitionRepository.Load(pageDefinitionId)
                      .Type.DefinitionType.ShouldEqual(typeof(PropertyXhtmlString));
    }
}
