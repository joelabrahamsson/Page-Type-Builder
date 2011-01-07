using System;
using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_with_a_different_class_name_but_the_same_GUID_as_an_existing_page_type_exists
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static int idOfExistingPageType;

        Establish context = () =>
        {
            var guid = Guid.NewGuid();

            IPageType existingPageType = new NativePageType();
            existingPageType.Name = "ADifferentName";
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            existingPageType.GUID = guid;
            SyncContext.PageTypeFactory.Save(existingPageType);
            idOfExistingPageType = existingPageType.ID;

            SyncContext.AddTypeInheritingFromTypedPageData(type =>
            {
                type.Name = className;
                type.Attributes.Add(AttributeHelper.CreatePageTypeAttributeSpecification(guid.ToString()));
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_update_the_existing_page_type_to_have_the_name_of_the_class =
            () => SyncContext.PageTypeFactory.Load(idOfExistingPageType).Name.ShouldEqual(className);
    }
}
