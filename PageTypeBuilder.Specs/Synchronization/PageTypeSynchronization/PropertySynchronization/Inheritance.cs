using System;
using System.Linq;
using Machine.Specifications;
using Refraction;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization.Inheritance
{
    [Subject("Synchronization")]
    public class when_a_class_inheriting_from_an_abstract_page_type_class_that_has_a_page_type_property_has_been_added
        : SynchronizationSpecs
    {
        static string propertyName = "PropertyInAbstractClass";

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
                {
                    with.Class("MyAbstractPageType")
                        .Inheriting<TypedPageData>()
                        .AnnotatedWith<PageTypeAttribute>()
                        .Abstract()
                        .AutomaticProperty<string>(x =>
                                          x.Named(propertyName)
                                              .AnnotatedWith<PageTypePropertyAttribute>());
                    
                    with.Class("MyConcretePageType")
                        .Inheriting("MyAbstractPageType");
                });
            
            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_page_definition_whose_PageTypeID_is_equal_to_the_page_types_ID =
            () => SyncContext.PageDefinitionRepository.List().First().PageTypeID
                .ShouldEqual(SyncContext.PageTypeRepository.List().First().ID);

        It should_create_a_page_definition_whose_name_equals_the_propertys_name =
            () => SyncContext.PageDefinitionRepository.List().First().Name
                .ShouldEqual(propertyName);
    }
}
