using System;
using System.Reflection.Emit;
using PageTypeBuilder.Reflection;
using StructureMap;

namespace PageTypeBuilder.Specs.Helpers
{
    public abstract class FunctionalSpecFixture
    {
        public static TypeBuilder CreateTypeThatInheritsFromTypedPageData(Action<TypeSpecification> typeSpecificationExpression)
        {
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");

            TypeBuilder typeBuilder = moduleBuilder.CreateClass(type =>
            {
                typeSpecificationExpression(type);
                type.ParentType = typeof(TypedPageData);
            });

            return typeBuilder;
        }

        public static Container CreateContainerWithInMemoryImplementations()
        {
            Container container = new Container(new InMemoryComponentsRegistry());
            return container;
        }
    }
}
