using System;
using System.Reflection.Emit;
using Machine.Specifications;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using StructureMap;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_page_type_class_only_has_it_self_as_available_page_type : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static InMemoryPageTypeFactory pageTypeFactory = new InMemoryPageTypeFactory();
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                TypeBuilder typeBuilder = CreateTypeThatInheritsFromTypedPageData(type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.BeforeAttributeIsAddedToType = (attribute, t) =>
                    {
                        if (!(attribute is PageTypeAttribute))
                            return;

                        ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t };


                    };
                    type.Attributes.Add(new PageTypeAttribute());
                });

                Container container = CreateContainerWithInMemoryImplementations();
                ((InMemoryAssemblyLocator)container.GetInstance<IAssemblyLocator>()).Add(typeBuilder.Assembly);
                pageTypeFactory = (InMemoryPageTypeFactory)container.GetInstance<IPageTypeFactory>();
                synchronizer = container.GetInstance<PageTypeSynchronizer>();        
            };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_only_has_itself_in_its_AllowedPageTypes_property =
            () => pageTypeFactory.Load(className).AllowedPageTypes.ShouldContainOnly(pageTypeFactory.Load(className).ID);
    }
}
