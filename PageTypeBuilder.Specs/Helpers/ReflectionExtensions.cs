using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class ReflectionExtensions
    {
        public static ModuleBuilder CreateModuleWithReferenceToPageTypeBuilder(string assemblyName)
        {
            return CreateModule(assembly =>
            {
                assembly.Name = assemblyName;
                assembly.AttributeSpecification.Add(new PageTypePropertyAttribute());
            });
        }

        public static ModuleBuilder CreateModule(Action<AssemblySpecification> assemblySpecificationExpression)
        {
            var assemblySpec = new AssemblySpecification();
            assemblySpecificationExpression(assemblySpec);

            AssemblyBuilder assemblyBuilder = CreateAssembly(assemblySpec);

            foreach (var assemblyAttribute in assemblySpec.AttributeSpecification)
            {
                AddAttribute(assemblyBuilder, assemblyAttribute.GetType());
            }

            return assemblyBuilder.DefineDynamicModule(assemblySpec.Name, assemblySpec.Name + ".dll");
        }

        private static AssemblyBuilder CreateAssembly(AssemblySpecification assemblySpec)
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblySpec.Name),
                AssemblyBuilderAccess.RunAndSave);
        }

        private static void AddAttribute(AssemblyBuilder assemblyBuilder, Type attributeType)
        {
            ConstructorInfo pageTypePropertyAttributeCtor = attributeType.GetConstructor(new Type[] { });
            CustomAttributeBuilder pageTypePropertyAttributeBuilder =
                new CustomAttributeBuilder(pageTypePropertyAttributeCtor, new object[] { });
            assemblyBuilder.SetCustomAttribute(pageTypePropertyAttributeBuilder);
        }

        public static TypeBuilder CreateClass(
            this ModuleBuilder moduleBuilder,
            Action<TypeSpecification> typeSpecificationExpression)
        {
            TypeSpecification typeSpec = new TypeSpecification();
            typeSpecificationExpression(typeSpec);
            TypeBuilder typeBuilder = moduleBuilder.CreateTypeFromSpecification(typeSpec);

            AddTypeAttributes(typeBuilder, typeSpec);

            AddProperties(typeBuilder, typeSpec.Properties);

            typeBuilder.CreateType();
            
            return typeBuilder;
        }

        private static TypeBuilder CreateTypeFromSpecification(this ModuleBuilder moduleBuilder, TypeSpecification typeSpec)
        {
            return moduleBuilder.DefineType(
                typeSpec.Name,
                typeSpec.TypeAttributes,
                typeSpec.ParentType);
        }

        private static void AddTypeAttributes(TypeBuilder typeBuilder, TypeSpecification typeSpec)
        {
            foreach (var attributeTemplate in typeSpec.Attributes)
            {
                if (typeSpec.BeforeAttributeIsAddedToType != null)
                    typeSpec.BeforeAttributeIsAddedToType(attributeTemplate, typeBuilder);
                typeBuilder.AddAttribute(attributeTemplate);
            }
        }

        private static void AddProperties(TypeBuilder typeBuilder, IEnumerable<PropertySpecification> properties)
        {
            foreach (var propertySpecification in properties)
            {
                PropertyBuilder property = typeBuilder.AddProperty(propertySpecification);
                foreach (var attributeTemplate in propertySpecification.Attributes)
                {
                    property.AddAttribute(attributeTemplate);
                }
            }
        }

        public static void AddAttribute(
            this TypeBuilder typeBuilder,
            Attribute attributeTemplate)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeTemplate);

            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        private static CustomAttributeBuilder CreateAttributeWithValuesFromTemplate(Attribute attributeTemplate)
        {
            var properties = GetWritableProperties(attributeTemplate);

            object[] propertyValues = GetPropertyValues(attributeTemplate, properties);

            var propertiesWithValues = new List<PropertyInfo>();
            var nonNullPropertyValues = new List<Object>();
            for (int i = 0; i < properties.Length; i++)
            {
                if (propertyValues[i] == null)
                    continue;

                propertiesWithValues.Add(properties[i]);
                nonNullPropertyValues.Add(propertyValues[i]);
            }

            ConstructorInfo constructor = attributeTemplate.GetType().GetConstructor(new Type[] { });
            return new CustomAttributeBuilder(constructor, new object[] { }, 
                propertiesWithValues.ToArray(), nonNullPropertyValues.ToArray());
        }

        private static PropertyInfo[] GetWritableProperties(Attribute attributeTemplate)
        {
            return attributeTemplate.GetType().GetProperties().Where(prop => prop.CanWrite).ToArray();
        }

        private static object[] GetPropertyValues(Attribute attributeTemplate, PropertyInfo[] properties)
        {
            object[] propertyValues = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                propertyValues[i] = attributeTemplate.GetType()
                    .InvokeMember(properties[i].Name, BindingFlags.GetProperty, null,
                        attributeTemplate, new object[0]);
            }
            return propertyValues;
        }

        public static PropertyBuilder AddProperty(
            this TypeBuilder typeBuilder,
            PropertySpecification propertySpec)
        {

            return typeBuilder.DefineProperty(
                propertySpec.Name, PropertyAttributes.HasDefault, propertySpec.Type, null);
        }

        public static void AddPageTypePropertyAttribute(this PropertyBuilder propertyBuilder, Attribute templateAttribute)
        {
            propertyBuilder.AddAttribute(templateAttribute);
        }

        public static void AddAttribute(this PropertyBuilder propertyBuilder, Attribute attributeTemplate)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeTemplate);
            propertyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public static void AddProperty(this TypeSpecification typeSpec, Action<PropertySpecification> propertySpecExpression)
        {
            var property = new PropertySpecification();
            propertySpecExpression(property);
            typeSpec.Properties.Add(property);
        }
    }
}
