using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using EPiServer.Security;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Helpers
{
    public class TabClassFactory
    {
        public static Type CreateTabClass(string className, string tabName, AccessLevel requiredAccess, int sortIndex)
        {
            var moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AssemblyWithTab");

            var nameProperty = new PropertySpecification();
            nameProperty.Name = "Name";
            nameProperty.Type = typeof(string);
            nameProperty.GetterImplementation = (typeBuilder) =>
            {
                MethodBuilder getMethodBuilder =
                    typeBuilder.DefineMethod("get_Name", MethodAttributes.Public | MethodAttributes.Virtual, typeof(string), Type.EmptyTypes);

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

                ILGenerator ilGenerator = getMethodBuilder.GetILGenerator();
                ilGenerator.DeclareLocal(typeof(AccessLevel));
                ilGenerator.Emit(OpCodes.Ldc_I4, (int)requiredAccess);
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

                ILGenerator ilGenerator = getMethodBuilder.GetILGenerator();
                ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldc_I4, sortIndex);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);
                return getMethodBuilder;
            };

            return moduleBuilder.CreateClass(type =>
            {
                type.Name = "NameOfTheTabClass";
                type.ParentType = typeof(Tab);
                type.Properties.Add(nameProperty);
                type.Properties.Add(requiredAccessProperty);
                type.Properties.Add(sortIndexProperty);
            });
        }
    }
}
