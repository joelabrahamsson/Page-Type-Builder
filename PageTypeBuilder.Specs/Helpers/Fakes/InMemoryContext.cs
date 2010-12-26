using System;
using System.Reflection.Emit;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;
using PageTypeBuilder.Synchronization;
using StructureMap;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryContext
    {
        public InMemoryContext()
        {
            Container = new Container(new InMemoryComponentsRegistry());
        }

        private Container Container { get; set; }

        public InMemoryAssemblyLocator AssemblyLocator
        {
            get
            {
                return (InMemoryAssemblyLocator)Container.GetInstance<IAssemblyLocator>();
            }
        }

        public InMemoryPageTypeFactory PageTypeFactory
        {
            get
            {
                return (InMemoryPageTypeFactory) Container.GetInstance<IPageTypeFactory>();
            }
        }

        public InMemoryPageDefinitionFactory PageDefinitionFactory
        {
            get
            {
                return (InMemoryPageDefinitionFactory) Container.GetInstance<IPageDefinitionFactory>();
            }
        }

        public PageTypeSynchronizer PageTypeSynchronizer
        {
            get
            {
                return Container.GetInstance<PageTypeSynchronizer>();
            }
        }

        public void AddTypeInheritingFromTypedPageData(Action<TypeSpecification> typeSpecificationExpression)
        {
            Type type = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(typeSpecificationExpression);
            
            AssemblyLocator.Add(type.Assembly);
        }

        public void AddTypeInheritingFromTypedPageData(ModuleBuilder module, Action<TypeSpecification> typeSpecificationExpression)
        {
            Type type = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(module, typeSpecificationExpression);

            AssemblyLocator.Add(type.Assembly);
        }

        public void AddPageTypeClass(Action<TypeSpecification> typeSpecificationExpression)
        {
            Type type = PageTypeClassFactory.CreatePageTypeClass(typeSpecificationExpression);

            AssemblyLocator.Add(type.Assembly);
        }

        public void AddPageTypeClass(Action<TypeSpecification> typeSpecificationExpression, Action<PageTypeAttribute> pageTypeAttributeExpression)
        {
            var attribute = new PageTypeAttribute();
            pageTypeAttributeExpression(attribute);
            Type pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
            {
                type.AddAttributeTemplate(attribute);
                typeSpecificationExpression(type);
            });
            AssemblyLocator.Add(pageTypeClass.Assembly);
        }
    }
}
