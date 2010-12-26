using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization
{
    //It appears this is not how it currently works, should be fixed
    //Currently the PageTypeLocator class will return the page type that matches
    //the class name of the page type class that has the same name as the one with
    //a name in the attribute resulting in the one with the attribute not being
    //created.
    [Ignore] 
    [Subject("Synchronization")]
    public class when_two_page_type_classes_have_the_same_name_but_one_has_a_different_name_in_the_attribute
        : SynchronizationSpecs
    {
        static string commonClassName = "SharedNameForBothClasses";
        static string nameInAttribute = "PageTypeNameInAttribute";
        Establish context = () =>
            {
                SyncContext.AddPageTypeClassToAppDomain(type =>
                {
                    type.Name = commonClassName;   
                });

                SyncContext.AddPageTypeClassToAppDomain((type, attribute) =>
                {
                    type.Name = commonClassName;
                    attribute.Name = nameInAttribute;
                });
            };

        Because of = () => 
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_PageType_with_the_common_name = () =>
            SyncContext.PageTypeFactory.Load(commonClassName).
                ShouldNotBeNull();

        It should_create_a_PageType_with_the_name_in_the_attribute = () =>
            SyncContext.PageTypeFactory.Load(nameInAttribute).
                ShouldNotBeNull();
    }
}
