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
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    assemblyName,
                    AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                "MyPageTypeClass",
                TypeAttributes.Public, typeof(TypedPageData));


            ConstructorInfo constructor = typeof(PageTypeAttribute).GetConstructor(new Type[] { });
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, new object[] { });
            typeBuilder.SetCustomAttribute(customAttributeBuilder);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                                        "Property", PropertyAttributes.HasDefault, typeof(string), null);
            ConstructorInfo pageTypePropertyAttributeCtor = typeof(PageTypePropertyAttribute).GetConstructor(new Type[] { });
            CustomAttributeBuilder pageTypePropertyAttributeBuilder = new CustomAttributeBuilder(pageTypePropertyAttributeCtor, new object[] { });
            propertyBuilder.SetCustomAttribute(pageTypePropertyAttributeBuilder);

            typeBuilder.CreateType();

            assemblyBuilder.SetCustomAttribute(customAttributeBuilder);
            return typeBuilder;
        }
    }
}
