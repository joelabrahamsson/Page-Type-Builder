using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;

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

    public class when_renaming_page_type : MigrationHelpersSpec
    {
        static string originalName = "MyPageType";
        static string newName = "MyRenamedPageType";
        static int pageTypeId;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = originalName;
            pageTypeRepository.Save(pageType);
            pageTypeId = pageTypeRepository.Load(originalName).ID;

            migration = MigrationWithExecuteMethod(
                "PageType(\"" + originalName + "\").Rename(\"" + newName + "\")");
        };

        Because of = () => migration.Execute();

        It should_rename_the_page_type
                = () => pageTypeRepository.Load(pageTypeId).Name.ShouldEqual(newName);
    }

    public class when_renaming_non_existing_page_type : MigrationHelpersSpec
    {
        static string originalName = "MyPageType";
        static string newName = "MyRenamedPageType";
        static Exception thrownException;

        Establish context = () =>
        {
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + originalName + "\").Rename(\"" + newName + "\")");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }

    public class when_deleting_page_definition_from_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string propertyName = "MyProperty";

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);
            var pageTypeId = pageTypeRepository.Load(pageTypeName).ID;

            var pageDefinition = new PageDefinition
                                     {
                                         Name = propertyName,
                                         EditCaption = propertyName,
                                         PageTypeID = pageTypeId
                                     };

            pageDefinitionRepository.Save(pageDefinition);

            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + propertyName + "\").Delete()");
        };

        Because of = () => migration.Execute();

        It should_delete_the_PageDefinition
                = () => pageTypeRepository.Load(pageTypeName)
                    .Definitions.ShouldNotContain(p => p.Name == propertyName);
    }

    public class when_deleting_page_definition_from_non_existing_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string propertyName = "MyProperty";
        static Exception thrownException;

        Establish context = () =>
        {
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + propertyName + "\").Delete()");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }

    public class when_deleting_non_existing_page_definition_existing_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string propertyName = "MyProperty";
        static Exception thrownException;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);

            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + propertyName + "\").Delete()");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }

    public class when_renaming_page_definition : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string originalName = "MyProperty";
        static string newName = "RenamedProperty";
        static int pageDefinitionId;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);
            var pageTypeId = pageTypeRepository.Load(pageTypeName).ID;

            var pageDefinition = new PageDefinition
            {
                Name = originalName,
                EditCaption = originalName,
                PageTypeID = pageTypeId
            };

            pageDefinitionRepository.Save(pageDefinition);
            pageDefinitionId = pageDefinitionRepository.List(pageTypeId).Find(d => d.Name == originalName).ID;
            
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + originalName + "\").Rename(\"" + newName + "\")");
        };

        Because of = () => migration.Execute();

        It should_delete_the_PageDefinition
                = () => pageTypeRepository.Load(pageTypeName)
                    .Definitions.Find(d => d.ID == pageDefinitionId).Name
                    .ShouldEqual(newName);
    }

    public class when_renaming_non_existing_page_definition : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string originalName = "MyProperty";
        static string newName = "RenamedProperty";
        static Exception thrownException;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);
            
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + originalName + "\").Rename(\"" + newName + "\")");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }

    public class when_renaming_page_definition_on_non_existing_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string originalName = "MyProperty";
        static string newName = "RenamedProperty";
        static Exception thrownException;

        Establish context = () =>
        {
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + originalName + "\").Rename(\"" + newName + "\")");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
                = () => thrownException.ShouldBeNull();
    }

    public class when_changing_page_definition_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string pageDefinitionName = "MyProperty";
        static Type originalType = typeof (PropertyString);
        static Type newType = typeof (PropertyBoolean);
        static int pageDefinitionId;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);
            var pageTypeId = pageTypeRepository.Load(pageTypeName).ID;

            var originalTypeId = new NativePageDefinitionsMap().GetNativeTypeID(originalType);
            var pageDefinition = new PageDefinition
            {
                Name = pageDefinitionName,
                EditCaption = pageDefinitionName,
                PageTypeID = pageTypeId,
                Type = pageDefinitionTypeRepository.GetPageDefinitionType(originalTypeId)
            };

            pageDefinitionRepository.Save(pageDefinition);
            pageDefinitionId = pageDefinitionRepository.List(pageTypeId).Find(d => d.Name == pageDefinitionName).ID;

            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + pageDefinitionName + "\").ChangeTypeTo<" + newType.FullName + ">()");
        };

        Because of = () => migration.Execute();

        It should_update_the_PageDefinition_to_have_the_new_type
            = () => pageTypeRepository.Load(pageTypeName)
                        .Definitions.Find(d => d.ID == pageDefinitionId)
                        .Type.DefinitionType.ShouldEqual(newType);
    }

    public class when_changing_type_of_non_existing_page_definition : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string pageDefinitionName = "MyProperty";
        static Type newType = typeof(PropertyBoolean);
        static Exception thrownException;

        Establish context = () =>
        {
            var pageType = new FakePageType(pageDefinitionRepository);
            pageType.Name = pageTypeName;
            pageTypeRepository.Save(pageType);
            
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + pageDefinitionName + "\").ChangeTypeTo<" + newType.FullName + ">()");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
            = () => thrownException.ShouldBeNull();
    }

    public class when_changin_type_for_page_definition_in_non_existing_page_type : MigrationHelpersSpec
    {
        static string pageTypeName = "MyPageType";
        static string pageDefinitionName = "MyProperty";
        static Type newType = typeof(PropertyBoolean);
        static Exception thrownException;

        Establish context = () =>
        {
            migration = MigrationWithExecuteMethod(
                "PageType(\"" + pageTypeName + "\").PageDefinition(\"" + pageDefinitionName + "\").ChangeTypeTo<" + newType.FullName + ">()");
        };

        Because of = () => thrownException = Catch.Exception(() => migration.Execute());

        It should_not_throw_an_exception
            = () => thrownException.ShouldBeNull();
    }
}
