using Machine.Specifications;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_class_has_the_name_of_an_existing_page_type_and_the_default_Filename
        : SynchronizationSpecs
    {
        static string pageTypeName = "NameOfThePageType";
        static int pageTypeId;

        Establish context = () =>
        {
            IPageType existingPageType = SyncContext.PageTypeFactory.CreateNew();
            existingPageType.Name = pageTypeName;
            existingPageType.FileName = PageTypeUpdater.DefaultPageTypeFilename;
            SyncContext.PageTypeFactory.Save(existingPageType);
            pageTypeId = existingPageType.ID;
            SyncContext.PageTypeFactory.ResetNumberOfSaves();

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
            }); 
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_not_save_the_PageType = () =>
            SyncContext.PageTypeFactory.GetNumberOfSaves(pageTypeId).ShouldEqual(0);
    }
}
