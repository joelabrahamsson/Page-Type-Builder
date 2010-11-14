using System;
using System.Collections.Generic;
using System.Linq;
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
        static InMemoryPageDefinitionFactory pageDefinitionFactory = new InMemoryPageDefinitionFactory();
        static string propertyName = "PropertyName";
        static PageTypePropertyAttribute pageTypePropertyAttribute;

        Establish context = () =>
                                {
                                    pageTypePropertyAttribute = new PageTypePropertyAttribute();
                                    pageTypePropertyAttribute.EditCaption = "Property's Edit Caption";

                                    var pageTypeClass = CreateTypedPageDataDescendant(type =>
                                        {
                                            type.Name = "MyPageTypeClass";
                                            type.Attributes.Add(new PageTypeAttribute { Description = "Testing123" });
                                            type.AddProperty(prop =>
                                                {
                                                    prop.Name = propertyName;
                                                    prop.Type = typeof (string);
                                                    prop.Attributes = new List<Attribute>
                                                                        {pageTypePropertyAttribute};
                                                });
                                        });

                                    var assemblyLocator = new InMemoryAssemblyLocator();
                                    assemblyLocator.Add(pageTypeClass.Assembly);

                                    Mock<TabFactory> tabFactory = new Mock<TabFactory>();
                                    tabFactory.Setup(f => f.List()).Returns(new TabDefinitionCollection { new TabDefinition()});

                                    synchronizer = new PageTypeSynchronizer(
                                        new PageTypeDefinitionLocator(assemblyLocator), 
                                        new PageTypeBuilderConfiguration(),
                                        pageTypeFactory,
                                        pageDefinitionFactory,
                                        new InMemoryPageDefinitionTypeFactory(),
                                        tabFactory.Object,
                                        new Mock<PageTypeValueExtractor>().Object,
                                        new Mock<PageTypeResolver>().Object);
                                };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => pageDefinitionFactory.List().ShouldNotBeEmpty();

        It should_create_a_page_definition_with_a_name_equal_to_the_propertys_name =
            () => pageDefinitionFactory.List().First().Name.ShouldEqual(propertyName);

    }
}
