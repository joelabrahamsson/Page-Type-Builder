using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PageTypeBuilder.Reflection
{
    public interface IAssemblyLocator
    {
        IEnumerable<Assembly> GetAssemblies();
    }

    public class AppDomainAssemblyLocator : IAssemblyLocator
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    internal class AttributedTypesUtility
    {
        internal static List<Type> GetTypesWithAttribute(IAssemblyLocator assemblyLocator,  Type attributeType)
        {
            string attributeAssemblyName = attributeType.Assembly.GetName().Name;

            List<Type> typesWithAttribute = new List<Type>();
            IEnumerable<Assembly> assemblies = assemblyLocator.GetAssemblies();
            
            foreach (Assembly assembly in assemblies)
            {
                AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                if (referencedAssemblies.Count(a => a.Name == attributeAssemblyName) == 0)
                    continue;

                List<Type> typesWithAttributeInAssembly = GetTypesWithAttributeInAssembly(assembly, attributeType);
                typesWithAttribute.AddRange(typesWithAttributeInAssembly);
            }

            return typesWithAttribute;
        }

        private static List<Type> GetTypesWithAttributeInAssembly(Assembly assembly, Type attributeType)
        {
            List<Type> typesWithAttribute = new List<Type>();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (TypeHasAttribute(type, attributeType))
                    typesWithAttribute.Add(type);
            }

            return typesWithAttribute;
        }

        private static bool TypeHasAttribute(Type type, Type attributeType)
        {
            bool typeHasAttribute = false;
            object[] attributes = GetAttributes(type);
            foreach (object attribute in attributes)
            {
                if (attributeType.IsAssignableFrom(attribute.GetType()))
                    typeHasAttribute = true;
            }

            return typeHasAttribute;
        }

        private static object[] GetAttributes(Type type)
        {
            return type.GetCustomAttributes(true);;
        }

        internal static Attribute GetAttribute(Type type, Type attributeType)
        {
            Attribute attribute = null;

            object[] attributes = type.GetCustomAttributes(true);
            foreach (object attributeInType in attributes)
            {
                if (attributeType.IsAssignableFrom(attributeInType.GetType()))
                    attribute = (Attribute) attributeInType;
            }

            return attribute;
        }
    }
}