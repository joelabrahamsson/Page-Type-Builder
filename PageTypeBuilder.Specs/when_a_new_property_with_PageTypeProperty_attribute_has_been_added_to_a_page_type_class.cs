using System;
using System.Collections.Generic;
using System.Linq;
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
    public class when_a_new_property_with_PageTypeProperty_attribute_has_been_added_to_a_page_type_class : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static Mock<PageTypeFactory> fakePageTypeFactory;
        static Mock<PageDefinitionFactory> fakePageDefinitionFactory;
        static string propertyName = "PropertyName";

        Establish context = () =>
                                {
                                    TypeBuilder typeBuilder = CreateTypedPageDataDescendant(type =>
                                        {
                                            type.Name = "MyPageTypeClass";
                                            type.Attributes.Add(new PageTypeAttribute { Description = "Testing123" });
                                            type.Properties.Add(new PropertySpecification
                                                                    {
                                                                        Name = propertyName,
                                                                        Type = typeof(string),
                                                                        Attributes = new List<Attribute> { new PageTypePropertyAttribute() }
                                                                    });
                                        });

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

                                    var attribute = (PageTypeAttribute) typeBuilder.GetCustomAttributes(true)[0];
                                    attribute.Description.ShouldEqual("Testing123");
                                };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => fakePageDefinitionFactory.Verify(f => f.Save(Moq.It.IsAny<PageDefinition>()));

        It should_create_a_page_definition_with_a_name_equal_to_the_propertys_name =
            () => fakePageDefinitionFactory.Verify(f => f.Save(Moq.It.Is<PageDefinition>(def => def.Name == propertyName)));

    }

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

    public static class ReflectionExtensions
    {
        public static ModuleBuilder CreateModuleWithReferenceToPageTypeBuilder(string assemblyName)
        {
            return CreateModule(assembly =>
            {
                assembly.Name = assemblyName;
                assembly.AttributeSpecification.Add(new PageTypePropertyAttribute());
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
                AddAttribute(assemblyBuilder, assemblyAttribute.GetType());
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
            var typeBuilder = moduleBuilder.DefineType(
                typeSpec.Name,
                typeSpec.TypeAttributes,
                typeSpec.ParentType);

            foreach (var attributeTemplate in typeSpec.Attributes)
            {
                typeBuilder.AddAttribute(attributeTemplate);
            }

            foreach (var propertySpecification in typeSpec.Properties)
            {
                PropertyBuilder property = typeBuilder.AddProperty(propertySpecification);
                foreach (var attributeTemplate in propertySpecification.Attributes)
                {
                    property.AddAttribute(attributeTemplate);
                }
            }

            typeBuilder.CreateType();

            return typeBuilder;
        }

        public static void AddAttribute(
            this TypeBuilder typeBuilder, 
            Attribute attributeTemplate)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeTemplate);

            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        private static CustomAttributeBuilder CreateAttributeWithValuesFromTemplate(Attribute attributeTemplate)
        {
            ConstructorInfo constructor = attributeTemplate.GetType().GetConstructor(new Type[] { });
            var properties = attributeTemplate.GetType().GetProperties().Where(prop => prop.CanWrite).ToArray();
            object[] propertyValues = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                propertyValues[i] = attributeTemplate.GetType().InvokeMember(properties[i].Name, BindingFlags.GetProperty, null,
                                                                             attributeTemplate, new object[0]);
            }

            var propertiesWithValues = new List<PropertyInfo>();
            var nonNullPropertyValues = new List<Object>();
            for(int i = 0; i < properties.Length; i++)
            {
                if(propertyValues[i] == null)
                    continue;
                
                propertiesWithValues.Add(properties[i]);
                nonNullPropertyValues.Add(propertyValues[i]);
            }

            return new CustomAttributeBuilder(constructor, new object[] { }, propertiesWithValues.ToArray(), nonNullPropertyValues.ToArray());
        }

        public static PropertyBuilder AddProperty(
            this TypeBuilder typeBuilder, 
            PropertySpecification propertySpec)
            //Action<PropertySpecification> propertySpecificationExpression)
        {

            return typeBuilder.DefineProperty(
                propertySpec.Name, PropertyAttributes.HasDefault, propertySpec.Type, null);
        }

        //public static PropertyBuilder AddStringProperty(
        //    this TypeBuilder typeBuilder, 
        //    Action<PropertySpecification> propertySpecificationExpression)
        //{
        //    return AddProperty(typeBuilder, property =>
        //        {
        //            property.Type = typeof (String);
        //            propertySpecificationExpression(property);
        //        });
        //}

        public static void AddPageTypePropertyAttribute(this PropertyBuilder propertyBuilder, Attribute templateAttribute)
        {
            propertyBuilder.AddAttribute(templateAttribute);
        }

        public static void AddAttribute(this PropertyBuilder propertyBuilder, Attribute attributeTemplate)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeTemplate);
            propertyBuilder.SetCustomAttribute(customAttributeBuilder);
        }
    }

    public class TypeSpecification
    {
        public TypeSpecification()
        {
            Attributes = new List<Attribute>();
            Properties = new List<PropertySpecification>();
        }

        public string Name { get; set; }

        public TypeAttributes TypeAttributes { get; set; }

        public Type ParentType { get; set; }

        public List<Attribute> Attributes { get; set; }

        public List<PropertySpecification> Properties { get; set; }
    }

    public class AttributeSpecification
    {
        public Type Type { get; set; }
    }

    public class AssemblySpecification
    {
        public AssemblySpecification()
        {
            AttributeSpecification = new List<Attribute>();
        }

        public string Name { get; set; }

        public List<Attribute> AttributeSpecification { get; set; }
    }

    public class PropertySpecification
    {
        public PropertySpecification()
        {
            Attributes = new List<Attribute>();
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<Attribute> Attributes { get; set; }
    }
}
