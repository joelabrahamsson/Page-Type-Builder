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
                type.ParentType = typeof(TypedPageData);
                typeSpecificationExpression(type);
                    
            });
            
            return typeBuilder;
        }

        public static TypeBuilder CreateTypeInheritingFromTypedPageData(Action<TypeSpecification> typeSpecificationExpression)
        {
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly" + Guid.NewGuid());

            return CreateTypeInheritingFromTypedPageData(moduleBuilder, typeSpecificationExpression);
        }

        public static TypeBuilder CreatePageTypeClass(Action<TypeSpecification> typeSpecificationExpression)
        {
            return CreateTypeInheritingFromTypedPageData(type =>
                {
                    type.Attributes.Add(new PageTypeAttribute());
                    typeSpecificationExpression(type);
                });
        }
    }
}
