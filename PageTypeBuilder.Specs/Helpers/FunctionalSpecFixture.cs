using System;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers
{
    public abstract class FunctionalSpecFixture
    {
        public static TypeBuilder CreateTypedPageDataDescendant(Action<TypeSpecification> typeSpecificationExpression)
        {
            //Create an assembly
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");

            //Create a new page type class within the module
            TypeBuilder typeBuilder = moduleBuilder.CreateClass(type =>
            {
                typeSpecificationExpression(type);
                type.ParentType = typeof(TypedPageData);
            });

            return typeBuilder;
        }
    }
}
