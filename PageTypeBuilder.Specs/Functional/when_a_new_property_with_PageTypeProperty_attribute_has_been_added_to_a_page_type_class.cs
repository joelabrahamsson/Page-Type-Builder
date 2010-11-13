using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs
{
    public class when_a_new_property_with_PageTypePropertyAttribute_has_been_added_to_a_page_type_class : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static IPageTypeFactory pageTypeFactory = new InMemoryPageTypeFactory();
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

                                    var assemblyLocator = new InMemoryAssemblyLocator();
                                    assemblyLocator.Add(typeBuilder.Assembly);

                                    fakePageDefinitionFactory = new Mock<PageDefinitionFactory>();

                                    Mock<TabFactory> tabFactory = new Mock<TabFactory>();
                                    tabFactory.Setup(f => f.List()).Returns(new TabDefinitionCollection { new TabDefinition()});

                                    synchronizer = new PageTypeSynchronizer(
                                        new PageTypeDefinitionLocator(assemblyLocator), 
                                        new PageTypeBuilderConfiguration(),
                                        pageTypeFactory, 
                                        fakePageDefinitionFactory.Object,
                                        new InMemoryPageDefinitionTypeFactory(),
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
}
