using System;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.Fakes;

namespace PageTypeBuilder.Specs.Migrations.Helpers
{
    public class when_deleting_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);

            migration = MigrationWithExecuteMethod("PageType(\"" + pageTypeName + "\").Delete();");
        };

        Because of = () => migration.Execute();

        It should_delete_the_page_type_with_that_name
                = () => pageTypeRepository.Load(pageTypeName).ShouldBeNull();
    }

    public class when_deleting_none_existing_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static Exception thrownException;

        Establish context = () =>
        {
            migration = MigrationWithExecuteMethod("PageType(\"" + pageTypeName + "\").Delete();");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }
}
