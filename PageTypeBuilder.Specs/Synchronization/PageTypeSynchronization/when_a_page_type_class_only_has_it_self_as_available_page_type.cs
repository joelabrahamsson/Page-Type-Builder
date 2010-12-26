using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_only_has_it_self_as_available_page_type
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";

        Establish context = () =>
            {
                 var pageTypeClass = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(type =>
                    {
                        type.Name = "MyPageTypeClass";
                        type.BeforeAttributeIsAddedToType = (attribute, t) =>
                            {
                                if (!(attribute is PageTypeAttribute))
                                    return;

                                ((PageTypeAttribute) attribute).AvailablePageTypes = new[] {t};


                            };
                        type.AddAttributeTemplate(new PageTypeAttribute());
                    });

                 SyncContext.AssemblyLocator.Add(pageTypeClass.Assembly);
            };


        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_page_type_that_has_only_itself_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeFactory.Load(className).AllowedPageTypes
                .ShouldContainOnly(SyncContext.PageTypeFactory.Load(className).ID);
    }
}
