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

        public InMemoryTabFactory TabFactory
        {
            get
            {
                return (InMemoryTabFactory) Container.GetInstance<ITabFactory>();
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

        public void AddPageTypeClassToAppDomain(Action<TypeSpecification> typeSpecificationExpression)
        {
            Type type = PageTypeClassFactory.CreatePageTypeClass(typeSpecificationExpression);

            AssemblyLocator.Add(type.Assembly);
        }

        public void AddPageTypeClassToAppDomain(Action<TypeSpecification, PageTypeAttribute> typeSpecificationExpression)
        {
            var attribute = new PageTypeAttribute();
            Type pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
            {
                typeSpecificationExpression(type, attribute);
                type.AddAttributeTemplate(attribute);
            });
            AssemblyLocator.Add(pageTypeClass.Assembly);
        }
    }
}
