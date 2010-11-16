using System.Reflection.Emit;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static InMemoryPageTypeFactory pageTypeFactory = new InMemoryPageTypeFactory();
        static string className = "MyPageTypeClass";
        static PageTypeAttribute pageTypeAttribute;

        Establish context = () =>
            {
                pageTypeAttribute = new PageTypeAttribute
                    {
                        Description = "A description of the page type"
                    };

                TypeBuilder typeBuilder = CreateTypedPageDataDescendant(type =>
                    {
                        type.Name = className;
                        type.Attributes.Add(pageTypeAttribute);
                    });

                var assemblyLocator = new InMemoryAssemblyLocator();
                assemblyLocator.Add(typeBuilder.Assembly);

                synchronizer = new PageTypeSynchronizer(
                    new PageTypeDefinitionLocator(assemblyLocator),
                    new PageTypeBuilderConfiguration(),
                    pageTypeFactory,
                    new PageTypePropertyUpdater(new InMemoryPageDefinitionFactory(), new InMemoryPageDefinitionTypeFactory(), new Mock<TabFactory>().Object),
                    new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new InMemoryPageDefinitionTypeFactory())),
                    new Mock<PageTypeValueExtractor>().Object,
                    new Mock<PageTypeResolver>().Object);        
            };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_class =
            () => pageTypeFactory.Load(className).ShouldNotBeNull();

        It should_create_a_new_page_type_with_the_description_entered_in_the_PageType_attribute =
            () => pageTypeFactory.Load(className).Description.ShouldEqual(pageTypeAttribute.Description);
    }
}
