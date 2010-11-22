using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using StructureMap;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs
{
    public class when_a_new_property_with_PageTypePropertyAttribute_has_been_added_to_a_page_type_class : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        private static InMemoryPageDefinitionFactory pageDefinitionFactory = new InMemoryPageDefinitionFactory();
        static string propertyName = "PropertyName";
        static PageTypePropertyAttribute propertyAttribute;

        Establish context = () =>
            {
                propertyAttribute = new PageTypePropertyAttribute();
                propertyAttribute.EditCaption = "Property's Edit Caption";
                propertyAttribute.HelpText = "Property's help text";
                propertyAttribute.EditCaption = "Property's edit caption";

                var pageTypeClass = CreateTypedPageDataDescendant(type =>
                    {
                        type.Name = "MyPageTypeClass";
                        type.Attributes.Add(new PageTypeAttribute());
                        type.AddProperty(prop =>
                            {
                                prop.Name = propertyName;
                                prop.Type = typeof (string);
                                prop.Attributes = new List<Attribute>
                                                    {propertyAttribute};
                            });
                    });

                var assemblyLocator = new InMemoryAssemblyLocator();
                assemblyLocator.Add(pageTypeClass.Assembly);

                Container container = new Container(new InMemoryComponentsRegistry());
                container.Configure(config =>
                                        {
                                            config.For<IAssemblyLocator>().Use(assemblyLocator);
                                        });
                pageDefinitionFactory = (InMemoryPageDefinitionFactory)container.GetInstance<IPageDefinitionFactory>();
                synchronizer = container.GetInstance<PageTypeSynchronizer>();
            };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_definition =
            () => pageDefinitionFactory.List().ShouldNotBeEmpty();

        It should_create_a_page_definition_with_a_name_equal_to_the_propertys_name =
            () => pageDefinitionFactory.List().First().Name.ShouldEqual(propertyName);

        It should_create_a_page_definition_with_a_help_text_equal_to_the_attributes =
            () => pageDefinitionFactory.List().First().HelpText.ShouldEqual(propertyAttribute.HelpText);

        It should_create_a_page_definition_with_an_edit_caption_equal_to_the_attributes =
            () => pageDefinitionFactory.List().First().EditCaption.ShouldEqual(propertyAttribute.EditCaption);

    }
}
