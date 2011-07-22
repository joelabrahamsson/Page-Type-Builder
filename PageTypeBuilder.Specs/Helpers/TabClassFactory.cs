using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using EPiServer.Security;
using Microsoft.CSharp;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Helpers
{
    public class TabClassFactory
    {

        public static Type CreateTabClass(string className, string tabName, AccessLevel requiredAccess, int sortIndex)
        {
            return CreateTabClass(className, tabName, requiredAccess, sortIndex, TypeAttributes.Public);
        }

        public static Type CreateTabClass(string className, string tabName, AccessLevel requiredAccess, int sortIndex, TypeAttributes typeAttributes)
        {
            var moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AssemblyWithTab");

            var nameProperty = new PropertySpecification();
            nameProperty.Name = "Name";
            nameProperty.Type = typeof(string);
            nameProperty.GetterImplementation = (typeBuilder) =>
            {
                return typeBuilder.DefineMethodReturningString("get_Name", tabName,
                                                               MethodAttributes.Public | MethodAttributes.Virtual);
            };

            var requiredAccessProperty = new PropertySpecification();
            requiredAccessProperty.Name = "RequiredAccess";
            requiredAccessProperty.Type = typeof(AccessLevel);
            requiredAccessProperty.GetterImplementation = (typeBuilder) =>
            {
                return typeBuilder.DefineMethodReturningEnum("get_RequiredAccess", requiredAccess,
                                                             MethodAttributes.Public | MethodAttributes.Virtual);
            };

            var sortIndexProperty = new PropertySpecification();
            sortIndexProperty.Name = "SortIndex";
            sortIndexProperty.Type = typeof(int);
            sortIndexProperty.GetterImplementation = (typeBuilder) =>
            {
                return typeBuilder.DefineMethodReturningInt("get_SortIndex", sortIndex,
                                                            MethodAttributes.Public | MethodAttributes.Virtual);
            };

            return moduleBuilder.CreateClass(type =>
            {
                type.Name = className;
                type.ParentType = typeof(Tab);
                type.Properties.Add(nameProperty);
                type.Properties.Add(requiredAccessProperty);
                type.Properties.Add(sortIndexProperty);
                type.TypeAttributes = typeAttributes;
            });
        }
    }
}
