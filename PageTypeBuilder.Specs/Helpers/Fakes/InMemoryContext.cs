using System;
using System.Linq;
using System.Reflection.Emit;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
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

        public InMemoryPageDefinitionTypeFactory PageDefinitionTypeFactory
        {
            get
            {
                return (InMemoryPageDefinitionTypeFactory) Container.GetInstance<IPageDefinitionTypeFactory>();
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

        public InMemoryPropertySettingsRepository PropertySettingsRepository
        {
            get
            {
                return (InMemoryPropertySettingsRepository) Container.GetInstance<IPropertySettingsRepository>();
            }
        }

        public PageTypeResolver PageTypeResolver
        {
            get
            {
                return Container.GetInstance<PageTypeResolver>();
            }
        }

        public FakePageTypeBuilderConfiguration Configuration
        {
            get
            {
                return (FakePageTypeBuilderConfiguration)Container.GetInstance<PageTypeBuilderConfiguration>();
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

        public Type CreateAndAddPageTypeClassToAppDomain(Action<TypeSpecification> typeSpecificationExpression)
        {
            Type type = PageTypeClassFactory.CreatePageTypeClass(typeSpecificationExpression);

            AssemblyLocator.Add(type.Assembly);

            return type;
        }

        public void CreateAndAddPageTypeClassToAppDomain(Action<TypeSpecification, PageTypeAttribute> typeSpecificationExpression)
        {
            var attribute = new PageTypeAttribute();
            Type pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
            {
                typeSpecificationExpression(type, attribute);
                type.AddAttributeTemplate(attribute);
            });
            AssemblyLocator.Add(pageTypeClass.Assembly);
        }

        public PageDefinition GetPageDefinition(string name, string pageTypeName)
        {
            var pageType = PageTypeFactory.Load(pageTypeName);
            return PageDefinitionFactory.List(pageType.ID)
                .Where(def => def.Name == name)
                .FirstOrDefault();
        }

        public PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer(PageDefinition pageDefinition)
        {
            PropertySettingsContainer container;
            PropertySettingsRepository.TryGetContainer(pageDefinition.SettingsID, out container);
            return container;
        }

        public PropertySettingsContainer GetPageDefinitionsPropertySettingsContainer(string pageDefinitionName, string pageTypeName)
        {
            return GetPageDefinitionsPropertySettingsContainer(GetPageDefinition(pageDefinitionName, pageTypeName));
        }

        public TSettings GetPageDefinitionsPropertySettings<TSettings>(string pageDefinitionName, string pageTypeName)
            where TSettings : class
        {
            var container = GetPageDefinitionsPropertySettingsContainer(GetPageDefinition(pageDefinitionName, pageTypeName));
            
            if (container.Settings.ContainsKey(typeof(TSettings).FullName))
            {
                return container.Settings[typeof (TSettings).FullName].PropertySettings as TSettings;
            }

            return default(TSettings);
        }
    }
}
