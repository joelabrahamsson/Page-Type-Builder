using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PageTypeBuilder.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Assembly> AssembliesWithReferenceToAssemblyOf<T>(this IAssemblyLocator assemblyLocator)
        {
            return assemblyLocator.AssembliesWithReferenceToAssemblyOf(typeof (T));
        }

        public static IEnumerable<Assembly> AssembliesWithReferenceToAssemblyOf(this IAssemblyLocator assemblyLocator, Type type)
        {
            string assemblyName = type.Assembly.GetName().Name;

            return assemblyLocator.GetAssemblies()
                    .Where(a => a.GetReferencedAssemblies()
                        .Any(r => r.Name == assemblyName));
        }
    
        public static IEnumerable<Type> TypesAssignableTo<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes().AssignableTo(typeof (T)));
        }

        public static IEnumerable<Type> TypesWithAttribute<T>(this IEnumerable<Assembly> assemblies)
            where T : Attribute
        {
            return assemblies.SelectMany(a => a.GetTypes().WithAttribute<T>());
        }

        public static IEnumerable<Type> Types(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes());
        }
    }
}
