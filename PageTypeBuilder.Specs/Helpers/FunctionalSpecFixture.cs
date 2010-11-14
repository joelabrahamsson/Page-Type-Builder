using System;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers
{
    public abstract class FunctionalSpecFixture
    {
        public static TypeBuilder CreateTypedPageDataDescendant(Action<TypeSpecification> typeSpecificationExpression)
        {
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");

            TypeBuilder typeBuilder = moduleBuilder.CreateClass(type =>
            {
                typeSpecificationExpression(type);
                type.ParentType = typeof(TypedPageData);
            });

            return typeBuilder;
        }
    }
}
