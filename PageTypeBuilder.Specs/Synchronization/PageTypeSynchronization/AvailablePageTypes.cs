using System.CodeDom;
using System.Reflection;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.AvailablePageTypes
{
    using System;
    using Machine.Specifications;
    using Helpers;
    using Helpers.TypeBuildingDsl;
    using Refraction;

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

                    ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t };


                };
                type.AddAttributeTemplate(new PageTypeAttribute());
            });

            SyncContext.AssemblyLocator.Add(pageTypeClass.Assembly);
        };


        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_page_type_that_has_only_itself_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeRepository.Load(className).AllowedPageTypes
                .ShouldContainOnly(SyncContext.PageTypeRepository.Load(className).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_has_it_self_and_another_page_type_class_as_available_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";

        Establish context = () =>
        {
            var module = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("DynamicAssembly");
            var anotherModule = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AnotherAssembly");
            var another = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(anotherModule, type =>
            {
                type.Name = otherClassName;
                type.AddAttributeTemplate(new PageTypeAttribute());
            });
            Type typeBuilder = PageTypeClassFactory.CreateTypeInheritingFromTypedPageData(module, type =>
            {
                type.Name = "MyPageTypeClass";
                type.BeforeAttributeIsAddedToType = (attribute, t) =>
                {
                    if (!(attribute is PageTypeAttribute))
                        return;

                    ((PageTypeAttribute)attribute).AvailablePageTypes = new[] { t, another };


                };
                type.AddAttributeTemplate(new PageTypeAttribute());
            });

            SyncContext.AssemblyLocator.Add(typeBuilder.Assembly);
            SyncContext.AssemblyLocator.Add(another.Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_the_IDs_of_those_two_page_types_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeRepository.Load(className).ID,
                    SyncContext.PageTypeRepository.Load(otherClassName).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_has_a_page_type_class_and_a_sub_class_inheriting_TypedPageData_as_available_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";
        static string andAnotherClassName = "AndAnotherClassName";
        static string abstractClassName = "Abstract";

        

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                CodeTypeDeclaration abstractClass = with.Class(abstractClassName)
                    .Inheriting<TypedPageData>();

                with.Class(otherClassName)
                    .Inheriting(abstractClassName)
                    .AnnotatedWith<PageTypeAttribute>(new { Name = otherClassName }, Guid.NewGuid().ToString());

                CodeTypeDeclaration anotherClass = with.Class(andAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new {Name = andAnotherClassName}, Guid.NewGuid().ToString());

                CodeTypeDeclaration[] codeTypeDeclarations = new[] { abstractClass, anotherClass };

                with.Class(className)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = className, AvailablePageTypes = codeTypeDeclarations }, Guid.NewGuid().ToString());
            });

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_the_IDs_of_those_two_page_types_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeRepository.Load(otherClassName).ID,
                    SyncContext.PageTypeRepository.Load(andAnotherClassName).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_class_has_a_page_type_class_and_a_interface_as_available_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";
        static string andAnotherClassName = "AndAnotherClassName";
        static string abstractClassName = "Abstract";

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                CodeTypeDeclaration pageTypeInterface = with.Interface("PageTypeInterface");

                with.Class(otherClassName)
                    .Inheriting<TypedPageData>()
                    .Implementing(pageTypeInterface)
                    .AnnotatedWith<PageTypeAttribute>(new { Name = otherClassName }, Guid.NewGuid().ToString());

                CodeTypeDeclaration anotherClass = with.Class(andAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = andAnotherClassName }, Guid.NewGuid().ToString());

                CodeTypeDeclaration[] codeTypeDeclarations = new[] { pageTypeInterface, anotherClass };

                with.Class(className)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = className, AvailablePageTypes = codeTypeDeclarations }, Guid.NewGuid().ToString());
            });

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_the_IDs_of_those_two_page_types_in_its_AllowedPageTypes_property =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeRepository.Load(otherClassName).ID,
                    SyncContext.PageTypeRepository.Load(andAnotherClassName).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_has_it_self_as_an_excluded_page_type
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                with.Class(otherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = otherClassName }, Guid.NewGuid().ToString());

                var pageTypeClass = with.Class(className).Inheriting<TypedPageData>();
                CodeTypeDeclaration[] codeTypeDeclarations = new[] { pageTypeClass };

                pageTypeClass.AnnotatedWith<PageTypeAttribute>(new { Name = className, ExcludedPageTypes = codeTypeDeclarations }, Guid.NewGuid().ToString());
            });

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_has_the_ID_of_the_other_class_name_and_not_its_self =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(SyncContext.PageTypeRepository.Load(otherClassName).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_has_it_self_and_another_sub_class_inheriting_TypedPageData_as_excluded_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";
        static string andAnotherClassName = "AndAnotherClassName";
        static string andHeresAnotherClassName = "AndHeresAnotherClassName";
        static string abstractClassName = "Abstract";

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                CodeTypeDeclaration abstractClass = with.Class(abstractClassName)
                    .Inheriting<TypedPageData>();

                with.Class(otherClassName)
                    .Inheriting(abstractClassName)
                    .AnnotatedWith<PageTypeAttribute>(new { Name = otherClassName }, Guid.NewGuid().ToString());

                CodeTypeDeclaration anotherClass = with.Class(andAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = andAnotherClassName }, Guid.NewGuid().ToString());

                with.Class(andHeresAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = andHeresAnotherClassName }, Guid.NewGuid().ToString()).GetType();

                CodeTypeDeclaration[] codeTypeDeclarations = new[] { abstractClass, anotherClass };

                with.Class(className)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = className, ExcludedPageTypes = codeTypeDeclarations }, Guid.NewGuid().ToString());
            });

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_does_not_contain_the_Ids_of_any_of_the_specified_excluded_page_types =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeRepository.Load(andHeresAnotherClassName).ID,
                    SyncContext.PageTypeRepository.Load(className).ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_has_it_self_and_another_inteface_as_excluded_page_types
        : SynchronizationSpecs
    {
        static string className = "MyPageTypeClass";
        static string otherClassName = "Another";
        static string andAnotherClassName = "AndAnotherClassName";
        static string andHeresAnotherClassName = "AndHeresAnotherClassName";

        Establish context = () =>
        {
            var assembly = Create.Assembly(with =>
            {
                CodeTypeDeclaration pageTypeInterface = with.Interface("PageTypeInterface");

                with.Class(otherClassName)
                    .Inheriting<TypedPageData>()
                    .Implementing(pageTypeInterface)
                    .AnnotatedWith<PageTypeAttribute>(new { Name = otherClassName }, Guid.NewGuid().ToString());

                CodeTypeDeclaration anotherClass = with.Class(andAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = andAnotherClassName }, Guid.NewGuid().ToString());

                with.Class(andHeresAnotherClassName)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = andHeresAnotherClassName }, Guid.NewGuid().ToString()).GetType();

                CodeTypeDeclaration[] codeTypeDeclarations = new[] { pageTypeInterface, anotherClass };

                with.Class(className)
                    .Inheriting<TypedPageData>()
                    .AnnotatedWith<PageTypeAttribute>(new { Name = className, ExcludedPageTypes = codeTypeDeclarations }, Guid.NewGuid().ToString());
            });

            SyncContext.AssemblyLocator.Add(assembly);
        };

        Because of = () =>
            SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_ensure_that_the_corresponding_page_type_does_not_contain_the_Ids_of_any_of_the_specified_excluded_page_types =
            () => SyncContext.PageTypeRepository.Load(className)
                .AllowedPageTypes.ShouldContainOnly(
                    SyncContext.PageTypeRepository.Load(andHeresAnotherClassName).ID,
                    SyncContext.PageTypeRepository.Load(className).ID);
    }
}
