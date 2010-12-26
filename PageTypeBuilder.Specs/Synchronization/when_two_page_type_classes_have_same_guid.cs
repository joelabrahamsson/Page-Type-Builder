using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization
{
    [Ignore]
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_same_guid
        : SynchronizationSpecs
    {
        static Exception thrownException;

        Establish context = () =>
        {
            string guid = new Guid().ToString();
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "aPageType";
                type.Attributes.Add(CreatePageTypeAttributeSpecification(guid));
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "anotherPageType";
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

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
