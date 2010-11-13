using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs
{
    public class when_a_new_property_with_PageTypeProperty_attribute_has_been_added_to_a_page_type_class
    {
        static PageTypeSynchronizer synchronizer;
        static Mock<PageTypeFactory> fakePageTypeFactory;
        static Mock<PageDefinitionFactory> fakePageDefinitionFactory;

        Establish context = () =>
                                {
                                    TypeBuilder typeBuilder = CreatePageTypeClass();

                                    Mock<IAssemblyLocator> assemblyLocator = new Mock<IAssemblyLocator>();
                                    assemblyLocator.Setup(l => l.GetAssemblies()).Returns(new List<Assembly> {typeBuilder.Assembly});

                                    fakePageTypeFactory = new Mock<PageTypeFactory>();
                                    
                                    fakePageTypeFactory.Setup(f => f.Load("MyPageTypeClass")).Returns(new PageType());

                                    fakePageDefinitionFactory = new Mock<PageDefinitionFactory>();

                                    Mock<PageDefinitionTypeFactory> fakePageDefinitionTypeFactory = new Mock<PageDefinitionTypeFactory>();
                                    fakePageDefinitionTypeFactory.Setup(
                                        f => f.GetPageDefinitionType(Moq.It.IsAny<string>(), Moq.It.IsAny<string>())).
                                        Returns(new PageDefinitionType(1, PropertyDataType.String, ""));

                                    Mock<TabFactory> tabFactory = new Mock<TabFactory>();
                                    tabFactory.Setup(f => f.List()).Returns(new TabDefinitionCollection { new TabDefinition()});

                                    synchronizer = new PageTypeSynchronizer(
                                        new PageTypeDefinitionLocator(assemblyLocator.Object), 
                                        new PageTypeBuilderConfiguration(), 
                                        fakePageTypeFactory.Object, 
                                        fakePageDefinitionFactory.Object,
                                        fakePageDefinitionTypeFactory.Object,
                                        tabFactory.Object,
                                        new Mock<PageTypeValueExtractor>().Object,
                                        new Mock<PageTypeResolver>().Object);
                                    
                                };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => fakePageDefinitionFactory.Verify(f => f.Save(Moq.It.IsAny<PageDefinition>()));

        private static TypeBuilder CreatePageTypeClass()
        {
            //Create an assembly
            ModuleBuilder moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");

            //Create a new page type class within the module
            TypeBuilder typeBuilder = moduleBuilder.CreateClass(type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.ParentType = typeof (TypedPageData);
                });


            typeBuilder.AddAttribute(attribute => attribute.Type = typeof(PageTypeAttribute));

            PropertyBuilder propertyBuilder = typeBuilder.AddStringProperty(property => property.Name = "Property");            
            propertyBuilder.AddPageTypePropertyAttribute();

            typeBuilder.CreateType();

            PageTypeAttribute attribute2 = typeBuilder.GetCustomAttributes(true)[0] as PageTypeAttribute;
            if (attribute2 != null)
                attribute2.Description = "Testing";

            return typeBuilder;
        }
    }

    public static class ReflectionExtensions
    {
        public static ModuleBuilder CreateModuleWithReferenceToPageTypeBuilder(string assemblyName)
        {
            return CreateModule(assembly =>
            {
                assembly.Name = assemblyName;
                assembly.AttributeSpecification.Add(new AttributeSpecification { Type = typeof(PageTypePropertyAttribute) });
            });
        }

        public static ModuleBuilder CreateModule(Action<AssemblySpecification> assemblySpecificationExpression)
        {
            var assemblySpec = new AssemblySpecification();
            assemblySpecificationExpression(assemblySpec);

            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName(assemblySpec.Name),
                    AssemblyBuilderAccess.RunAndSave);

            foreach (var assemblyAttribute in assemblySpec.AttributeSpecification)
            {
                AddAttribute(assemblyBuilder, assemblyAttribute.Type);
            }
            
            return assemblyBuilder.DefineDynamicModule(assemblySpec.Name, assemblySpec.Name + ".dll");
        }

        private static void AddAttribute(AssemblyBuilder assemblyBuilder, Type attributeType)
        {
            ConstructorInfo pageTypePropertyAttributeCtor = attributeType.GetConstructor(new Type[] { });
            CustomAttributeBuilder pageTypePropertyAttributeBuilder = 
                new CustomAttributeBuilder(pageTypePropertyAttributeCtor, new object[] { });
            assemblyBuilder.SetCustomAttribute(pageTypePropertyAttributeBuilder);
        }

        public static TypeBuilder CreateClass(
            this ModuleBuilder moduleBuilder, 
            Action<TypeSpecification> typeSpecificationExpression)
        {
            TypeSpecification typeSpec = new TypeSpecification();
            typeSpecificationExpression(typeSpec);
            return moduleBuilder.DefineType(
                typeSpec.Name,
                typeSpec.TypeAttributes,
                typeSpec.ParentType);
        }

        public static void AddAttribute(
            this TypeBuilder typeBuilder, 
            Action<AttributeSpecification> attributeSpecificationExpression)
        {
            var attributeSpec = new AttributeSpecification();
            attributeSpecificationExpression(attributeSpec);

            ConstructorInfo constructor = attributeSpec.Type.GetConstructor(new Type[] { });
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, new object[] { });
            
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public static PropertyBuilder AddProperty(
            this TypeBuilder typeBuilder, 
            Action<PropertySpecification> propertySpecificationExpression)
        {
            var propertySpec = new PropertySpecification();
            propertySpecificationExpression(propertySpec);

            return typeBuilder.DefineProperty(
                propertySpec.Name, PropertyAttributes.HasDefault, propertySpec.Type, null);
        }

        public static PropertyBuilder AddStringProperty(
            this TypeBuilder typeBuilder, 
            Action<PropertySpecification> propertySpecificationExpression)
        {
            return AddProperty(typeBuilder, property =>
                {
                    property.Type = typeof (String);
                    propertySpecificationExpression(property);
                });
        }

        public static void AddPageTypePropertyAttribute(this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.AddAttribute(attribute => attribute.Type = typeof (PageTypePropertyAttribute));
        }

        public static void AddAttribute(this PropertyBuilder propertyBuilder, Action<AttributeSpecification> attributeSpecificationExpression)
        {
            var attributeSpec = new AttributeSpecification();
            attributeSpecificationExpression(attributeSpec);

            ConstructorInfo attributeCtor = attributeSpec.Type.GetConstructor(new Type[] { });
            CustomAttributeBuilder pageTypePropertyAttributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { });
            propertyBuilder.SetCustomAttribute(pageTypePropertyAttributeBuilder);
        }
    }

    public class TypeSpecification
    {
        public string Name { get; set; }

        public TypeAttributes TypeAttributes { get; set; }

        public Type ParentType { get; set; }
    }

    public class AttributeSpecification
    {
        public Type Type { get; set; }
    }

    public class AssemblySpecification
    {
        public AssemblySpecification()
        {
            AttributeSpecification = new List<AttributeSpecification>();
        }

        public string Name { get; set; }

        public List<AttributeSpecification> AttributeSpecification { get; set; }
    }

    public class PropertySpecification
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
