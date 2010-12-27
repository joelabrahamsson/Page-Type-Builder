using System;
using System.Reflection;
using System.Reflection.Emit;
using EPiServer.Security;
using Machine.Specifications;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Synchronization.TabSynchronization
{
    [Subject("Synchronization")]
    public class when_a_new_class_inheriting_from_Tab_has_been_added
        : SynchronizationSpecs
    {
        static string className = "NameOfTheTabClass";
        static string tabName = "ValueOfNameProperty";
        static AccessLevel requiredAccess = AccessLevel.Publish;
        static int sortIndex = 12;

        Establish context = () =>
        {
            var moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AssemblyWithTab");

            var nameProperty = new PropertySpecification();
            nameProperty.Name = "Name";
            nameProperty.Type = typeof (string);
            nameProperty.GetterImplementation = (typeBuilder) =>
                {
                    MethodBuilder getMethodBuilder =
                        typeBuilder.DefineMethod("get_Name", MethodAttributes.Public | MethodAttributes.Virtual, typeof(string), Type.EmptyTypes);

                    typeBuilder.DefineMethodOverride(getMethodBuilder, typeof(Tab).GetMethod("get_Name"));

                    ILGenerator ilGenerator = getMethodBuilder.GetILGenerator();
                    ilGenerator.Emit(OpCodes.Ldstr, tabName);
                    ilGenerator.Emit(OpCodes.Ret);
                    return getMethodBuilder;
                };

            var requiredAccessProperty = new PropertySpecification();
            requiredAccessProperty.Name = "RequiredAccess";
            requiredAccessProperty.Type = typeof(AccessLevel);
            requiredAccessProperty.GetterImplementation = (typeBuilder) =>
            {
                MethodBuilder getMethodBuilder =
                    typeBuilder.DefineMethod("get_RequiredAccess", MethodAttributes.Public | MethodAttributes.Virtual, typeof(AccessLevel), Type.EmptyTypes);

                typeBuilder.DefineMethodOverride(getMethodBuilder, typeof(Tab).GetMethod("get_RequiredAccess"));
                ILGenerator ilGenerator = getMethodBuilder.GetILGenerator();
                ilGenerator.DeclareLocal(typeof(AccessLevel));
                ilGenerator.Emit(OpCodes.Ldc_I4, (int) requiredAccess);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);
                return getMethodBuilder;
            };

            var sortIndexProperty = new PropertySpecification();
            sortIndexProperty.Name = "SortIndex";
            sortIndexProperty.Type = typeof(int);
            sortIndexProperty.GetterImplementation = (typeBuilder) =>
            {
                MethodBuilder getMethodBuilder =
                    typeBuilder.DefineMethod("get_SortIndex", MethodAttributes.Public | MethodAttributes.Virtual, typeof(int), Type.EmptyTypes);

                typeBuilder.DefineMethodOverride(getMethodBuilder, typeof(Tab).GetMethod("get_SortIndex"));
                ILGenerator ilGenerator = getMethodBuilder.GetILGenerator();
                ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldc_I4, sortIndex);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);
                return getMethodBuilder;
            };
            
            moduleBuilder.CreateClass(type =>
                {
                    type.Name = className;
                    type.ParentType = typeof(Tab);
                    type.Properties.Add(nameProperty);
                    type.Properties.Add(requiredAccessProperty);
                    type.Properties.Add(sortIndexProperty);
                });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_create_a_TabDefinition_with_the_name_returned_by_the_class_Name_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).ShouldNotBeNull();

        It should_create_a_TabDefinition_with_RequiredAccess_equal_to_the_returned_value_by_the_class_RequiredAccess_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).RequiredAccess.ShouldEqual(requiredAccess);

        It should_create_a_TabDefinition_with_SortIndex_equal_to_the_value_returned_by_the_class_SortIndex_property = () =>
            SyncContext.TabFactory.GetTabDefinition(tabName).SortIndex.ShouldEqual(sortIndex);
    }
}
