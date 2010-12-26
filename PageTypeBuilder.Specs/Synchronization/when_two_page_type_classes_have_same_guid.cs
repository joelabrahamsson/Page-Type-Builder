using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;
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
            Guid guid = new Guid();
            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "aPageType";
                type.Attributes.Add(new AttributeSpecification
                                        {
                                            Constructor = typeof(PageTypeAttribute).GetConstructor(new [] { typeof(string) }),
                                            ConstructorParameters = new object[] { guid.ToString()}
                                        });
            });

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = "anotherPageType";
                type.Attributes.Add(new AttributeSpecification
                {
                    Constructor = typeof(PageTypeAttribute).GetConstructor(new[] { typeof(string) }),
                    ConstructorParameters = new object[] { guid.ToString() }
                });
            });
        };

        Because of = () =>
            thrownException = Catch.Exception(
                () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_a_PageTypeBuilderException = () =>
            thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
