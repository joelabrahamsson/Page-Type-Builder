using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_new_class_inheriting_from_TypedPageData_with_a_PageType_attribute_has_been_added : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static Mock<PageTypeFactory> fakePageTypeFactory;

        Establish context = () =>
                                {
                                    TypeBuilder typeBuilder = CreateTypedPageDataDescendant(type =>
                                    {
                                        type.Name = "MyPageTypeClass";
                                        type.Attributes.Add(new PageTypeAttribute { Description = "Testing123" });
                                    });

                                    Mock<IAssemblyLocator> assemblyLocator = new Mock<IAssemblyLocator>();
                                    assemblyLocator.Setup(l => l.GetAssemblies()).Returns(new List<Assembly> {typeBuilder.Assembly});

                                    fakePageTypeFactory = new Mock<PageTypeFactory>();
                                    
                                    fakePageTypeFactory.Setup(f => f.Load("MyPageTypeClass")).Returns(new PageType());

                                    synchronizer = new PageTypeSynchronizer(
                                        new PageTypeDefinitionLocator(assemblyLocator.Object), 
                                        new PageTypeBuilderConfiguration(), 
                                        fakePageTypeFactory.Object, 
                                        new Mock<PageDefinitionFactory>().Object,
                                        new Mock<PageDefinitionTypeFactory>().Object,
                                        new Mock<TabFactory>().Object,
                                        new Mock<PageTypeValueExtractor>().Object,
                                        new Mock<PageTypeResolver>().Object);
                                    
                                };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_create_a_new_page_type =
            () => fakePageTypeFactory.Verify( f => f.Save(Moq.It.IsAny<PageType>()));
    }
}
