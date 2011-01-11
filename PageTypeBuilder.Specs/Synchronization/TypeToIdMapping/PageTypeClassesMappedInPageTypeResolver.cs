using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Synchronization;

namespace PageTypeBuilder.Specs.TypeToIdMapping.PageTypeClassesMappedInPageTypeResolver
{
    [Subject("Synchronization")]
    public class when_a_new_page_type_class_exists
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static Type pageTypeClass;

        Establish context = () =>
            {
                pageTypeClass = SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
                    {
                        type.Name = className;
                    });
            };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_be_able_to_get_the_created_PageTypes_Id_from_PageTypeResolver =
            () => SyncContext.PageTypeResolver.GetPageTypeID(pageTypeClass)
                .ShouldEqual(SyncContext.PageTypeFactory.Load(className).ID);

        It should_be_able_to_get_the_Type_of_the_page_type_class_from_PageTypeResolver_using_the_PageTypes_Id =
            () => SyncContext.PageTypeResolver.GetPageTypeType(SyncContext.PageTypeFactory.Load(className).ID)
                .ShouldEqual(pageTypeClass);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_matching_an_existing_PageType_exists
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static Type pageTypeClass;
        static int idOfExistingPageType;

        Establish context = () =>
        {
            pageTypeClass = SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = className;
            });

            var existingPageType = SyncContext.PageTypeFactory.CreateNew();
            existingPageType.Name = className;
            SyncContext.PageTypeFactory.Save(existingPageType);
            idOfExistingPageType = existingPageType.ID;
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_be_able_to_get_the_matching_PageTypes_Id_from_PageTypeResolver =
            () => SyncContext.PageTypeResolver.GetPageTypeID(pageTypeClass)
                .ShouldEqual(idOfExistingPageType);

        It should_be_able_to_get_the_Type_of_the_page_type_class_from_PageTypeResolver_using_the_PageTypes_Id =
            () => SyncContext.PageTypeResolver.GetPageTypeType(idOfExistingPageType)
                .ShouldEqual(pageTypeClass);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_matching_an_existing_PageType_exists_and_page_type_updation_has_been_disabled_through_configuration
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static Type pageTypeClass;
        static int idOfExistingPageType;

        Establish context = () =>
        {
            pageTypeClass = SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = className;
            });

            var existingPageType = SyncContext.PageTypeFactory.CreateNew();
            existingPageType.Name = className;
            SyncContext.PageTypeFactory.Save(existingPageType);
            idOfExistingPageType = existingPageType.ID;

            SyncContext.Configuration.SetDisablePageTypeUpdation(true);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_be_able_to_get_the_matching_PageTypes_Id_from_PageTypeResolver =
            () => SyncContext.PageTypeResolver.GetPageTypeID(pageTypeClass)
                .ShouldEqual(idOfExistingPageType);

        It should_be_able_to_get_the_Type_of_the_page_type_class_from_PageTypeResolver_using_the_PageTypes_Id =
            () => SyncContext.PageTypeResolver.GetPageTypeType(idOfExistingPageType)
                .ShouldEqual(pageTypeClass);
    }
}
