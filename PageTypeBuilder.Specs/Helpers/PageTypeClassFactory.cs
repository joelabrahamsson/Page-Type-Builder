using System;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PageTypeClassFactory
    {
        public static TypeBuilder CreateTypeInheritingFromTypedPageData(ModuleBuilder moduleBuilder, Action<TypeSpecification> typeSpecificationExpression)
        {
            TypeBuilder typeBuilder = moduleBuilder.CreateClass(type =>
            {
                type.Name = "DefaultPageTypeClassName";
                typeSpecificationExpression(type);
                type.ParentType = typeof(TypedPageData);
            });

            return typeBuilder;
        }

        public static TypeBuilder CreateTypeInheritingFromTypedPageData(Action<TypeSpecification> typeSpecificationExpression)
        {
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");

            return CreateTypeInheritingFromTypedPageData(moduleBuilder, typeSpecificationExpression);
        }
    }
}
