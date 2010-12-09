using System.Reflection.Emit;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;
using StructureMap;
using It = Machine.Specifications.It;

namespace PageTypeBuilder.Specs.Functional
{
    public class when_a_page_type_class_has_it_self_and_another_page_type_class_as_available_page_types : FunctionalSpecFixture
    {
        static PageTypeSynchronizer synchronizer;
        static InMemoryPageTypeFactory pageTypeFactory = new InMemoryPageTypeFactory();
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";

        Establish context = () =>
            {
                var module = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");
                var anotherModule = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AnotherAssembly");
                var another = CreateTypeThatInheritsFromTypedPageData(anotherModule, type =>
                    {
                        type.Name = otherClassName;
                        type.Attributes.Add(new PageTypeAttribute());
                    });
                TypeBuilder typeBuilder = CreateTypeThatInheritsFromTypedPageData(module, type =>
                {
                    type.Name = "MyPageTypeClass";
                    type.BeforeAttributeIsAddedToType = (attribute, t) =>
                    {
                        if (!(attribute is PageTypeAttribute))
                            return;

                        ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t, another };


                    };
                    type.Attributes.Add(new PageTypeAttribute());
                });

                ((AssemblyBuilder)another.Assembly).Save("AnotherAssembly.dll");
                Container container = CreateContainerWithInMemoryImplementations();
                ((InMemoryAssemblyLocator)container.GetInstance<IAssemblyLocator>()).Add(typeBuilder.Assembly);
                ((InMemoryAssemblyLocator)container.GetInstance<IAssemblyLocator>()).Add(another.Assembly);
                pageTypeFactory = (InMemoryPageTypeFactory)container.GetInstance<IPageTypeFactory>();
                synchronizer = container.GetInstance<PageTypeSynchronizer>();        
            };

        Because synchronization = 
            () => synchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_both_page_types_in_its_AllowedPageTypes_property =
            () => pageTypeFactory.Load(className).AllowedPageTypes.ShouldContainOnly(
                pageTypeFactory.Load(className).ID,
                pageTypeFactory.Load(otherClassName).ID);
    }
}
