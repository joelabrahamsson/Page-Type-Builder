using System.Reflection.Emit;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static IPageTypeFactory pageTypeFactory = new InMemoryPageTypeFactory();
        static string className = "MyPageTypeClass";

        Establish context = () =>
                                {
                                    TypeBuilder typeBuilder = CreateTypedPageDataDescendant(type =>
                                    {
                                        type.Name = className;
                                        type.Attributes.Add(new PageTypeAttribute { Description = "Testing123" });
                                    });

                                    var assemblyLocator = new InMemoryAssemblyLocator();
                                    assemblyLocator.Add(typeBuilder.Assembly);

                                    synchronizer = new PageTypeSynchronizer(
                                        new PageTypeDefinitionLocator(assemblyLocator), 
                                        new PageTypeBuilderConfiguration(),
                                        pageTypeFactory, 
                                        new Mock<PageDefinitionFactory>().Object,
                                        new Mock<PageDefinitionTypeFactory>().Object,
                                        new Mock<TabFactory>().Object,
                                        new Mock<PageTypeValueExtractor>().Object,
                                        new Mock<PageTypeResolver>().Object);
                                    
                                };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type_with_the_name_of_the_class =
            () => pageTypeFactory.Load(className).ShouldNotBeNull();
    }
}
