using System;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PageTypeClassFactory
    {
        public static Type CreateTypeInheritingFromTypedPageData(ModuleBuilder moduleBuilder, Action<TypeSpecification> typeSpecificationExpression)
        {
            Type createdClass = moduleBuilder.CreateClass(type =>
            {
                type.Name = "DefaultPageTypeClassName";
                type.ParentType = typeof(TypedPageData);
                typeSpecificationExpression(type);
                    
            });
            
            return createdClass;
        }

        public static Type CreateTypeInheritingFromTypedPageData(Action<TypeSpecification> typeSpecificationExpression)
        {
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly" + Guid.NewGuid());

            return CreateTypeInheritingFromTypedPageData(moduleBuilder, typeSpecificationExpression);
        }

        public static Type CreatePageTypeClass(Action<TypeSpecification> typeSpecificationExpression)
        {
            return CreateTypeInheritingFromTypedPageData(type =>
                {
                    type.Attributes.Add(new PageTypeAttribute());
                    typeSpecificationExpression(type);
                });
        }
    }
}
