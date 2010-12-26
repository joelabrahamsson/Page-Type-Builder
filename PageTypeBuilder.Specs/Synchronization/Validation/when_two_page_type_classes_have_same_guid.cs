using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.Validation
{
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_same_guid
        : SynchronizationSpecs
    {
        static Exception thrownException;
        static string firstPageTypeName = "aPageType";
        static string secondPageTypeName = "anotherPageType";

        Establish context = () =>
        {
            string guid = new Guid().ToString();
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = firstPageTypeName;
                type.Attributes.Add(CreatePageTypeAttributeSpecification(guid));
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = secondPageTypeName;
                type.Attributes.Add(CreatePageTypeAttributeSpecification(guid));
            });
        };

        static AttributeSpecification CreatePageTypeAttributeSpecification(string guid)
        {
            return new AttributeSpecification
                       {
                           Constructor = typeof(PageTypeAttribute).GetConstructor(new [] { typeof(string) }),
                           ConstructorParameters = new object[] { guid}
                       };
        }

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_an_Exception = () =>
            thrownException.ShouldNotBeNull();

        [Ignore]
        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();

        [Ignore] 
        It should_throw_Exception_containing_the_first_page_types_name = () =>
            thrownException.Message.ShouldContain(firstPageTypeName);

        [Ignore]
        It should_throw_Exception_containing_the_second_page_types_name = () =>
            thrownException.Message.ShouldContain(secondPageTypeName);
    }
}
