using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs
{
    public class when_a_page_type_property_has_a_name_longer_than_50_characters
    {
        static InMemoryContext environmentContext = new InMemoryContext();
        static Exception thrownException;

        Establish context = () => 
            environmentContext.AddPageTypeClass(type =>
                type.Name = 51.CharactersLongAlphanumericString());

        Because of =
            () => thrownException = Catch.Exception(() => environmentContext.PageTypeSynchronizer.SynchronizePageTypes());

        It should_throw_a_PageTypeBuilderException =
            () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
