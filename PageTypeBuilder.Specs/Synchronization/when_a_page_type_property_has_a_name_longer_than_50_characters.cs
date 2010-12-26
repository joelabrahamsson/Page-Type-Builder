using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization
{
    public class when_a_page_type_property_has_a_name_longer_than_50_characters
    {
        static InMemoryContext syncContext = new InMemoryContext();
        static Exception thrownException;

        Establish context = () => 
            syncContext.AddPageTypeClass(type =>
                type.Name = 51.CharactersLongAlphanumericString());

        Because of = () => 
            thrownException = Catch.Exception(
                () => syncContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_a_PageTypeBuilderException = () => 
            thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
