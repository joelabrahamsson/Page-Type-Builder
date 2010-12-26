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
                assemblyBuilder.AddAttribute(assemblyAttribute.GetType());
            }

            return assemblyBuilder.DefineDynamicModule(assemblySpec.Name, assemblySpec.Name + ".dll");
        }

        private static AssemblyBuilder CreateAssembly(AssemblySpecification assemblySpec)
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblySpec.Name),
                AssemblyBuilderAccess.RunAndSave);
        }

        public static void AddAttribute(this AssemblyBuilder assemblyBuilder, Type attributeType)
        {
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] { });
            CustomAttributeBuilder attributeBuilder =
                new CustomAttributeBuilder(attributeCtor, new object[] { });
            assemblyBuilder.SetCustomAttribute(attributeBuilder);
        }

        public static Type CreateClass(
            this ModuleBuilder moduleBuilder,
            Action<TypeSpecification> typeSpecificationExpression)
        {
            TypeSpecification typeSpec = new TypeSpecification();
            typeSpecificationExpression(typeSpec);
            TypeBuilder typeBuilder = moduleBuilder.CreateTypeFromSpecification(typeSpec);

            AddTypeAttributes(typeBuilder, typeSpec);
            AddProperties(typeBuilder, typeSpec.Properties);

            return typeBuilder.CreateType();
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
                    typeSpec.BeforeAttributeIsAddedToType(attributeTemplate.Template, typeBuilder);
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
            AttributeSpecification attributeSpecification)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeSpecification);

            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public static void AddAttribute(
            this TypeBuilder typeBuilder,
            CustomAttributeBuilder attributeBuilder)
        {
            typeBuilder.SetCustomAttribute(attributeBuilder);
        }

        private static CustomAttributeBuilder CreateAttributeWithValuesFromTemplate(AttributeSpecification attributeSpecification)
        {
            var propertiesWithValues = new List<PropertyInfo>();
            var nonNullPropertyValues = new List<Object>();

            if (attributeSpecification.Template != null)
            {
                var properties = GetWritableProperties(attributeSpecification.Template);

                object[] propertyValues = GetPropertyValues(attributeSpecification.Template, properties);


                for (int i = 0; i < properties.Length; i++)
                {
                    if (propertyValues[i] == null)
                        continue;

                    propertiesWithValues.Add(properties[i]);
                    nonNullPropertyValues.Add(propertyValues[i]);
                }
            }

            ConstructorInfo constructor;
            if (attributeSpecification.Constructor != null)
                constructor = attributeSpecification.Constructor;
            else
                constructor = attributeSpecification.Type.GetConstructor(new Type[] { });

            object[] constructorParams;
            if (attributeSpecification.Constructor != null)
                constructorParams = attributeSpecification.ConstructorParameters;
            else
                constructorParams = new object[] {};


            return new CustomAttributeBuilder(constructor, constructorParams, 
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
            var property = typeBuilder.DefineProperty(
                propertySpec.Name, PropertyAttributes.None, propertySpec.Type, null);

            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertySpec.Name.ToLowerInvariant(), propertySpec.Type, FieldAttributes.Private);

            MethodBuilder getMethodBuilder = CreatePropertyGetMethod(typeBuilder, property, propertySpec.GetterAttributes, fieldBuilder);

            MethodBuilder setMethodBuilder = CreatePropertySetMethod(typeBuilder, property, propertySpec.SetterAttributes, fieldBuilder);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);


            return property;
        }

        static MethodBuilder CreatePropertyGetMethod(TypeBuilder typeBuilder, PropertyBuilder property, MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name, methodAttributes, fieldBuilder.FieldType, Type.EmptyTypes);

            ILGenerator getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);
            return getMethodBuilder;
        }

        static MethodBuilder CreatePropertySetMethod(TypeBuilder typeBuilder, PropertyBuilder property, MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name, methodAttributes, null, new Type[] { fieldBuilder.FieldType });

            ILGenerator setMethodILGenerator = setMethodBuilder.GetILGenerator();
            setMethodILGenerator.Emit(OpCodes.Ldarg_0);
            setMethodILGenerator.Emit(OpCodes.Ldarg_1);
            setMethodILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setMethodILGenerator.Emit(OpCodes.Ret);
            return setMethodBuilder;
        }

        public static void AddPageTypePropertyAttribute(this PropertyBuilder propertyBuilder, AttributeSpecification attributeSpecification)
        {
            propertyBuilder.AddAttribute(attributeSpecification);
        }

        public static void AddAttribute(this PropertyBuilder propertyBuilder, AttributeSpecification attributeSpecification)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeSpecification);
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
