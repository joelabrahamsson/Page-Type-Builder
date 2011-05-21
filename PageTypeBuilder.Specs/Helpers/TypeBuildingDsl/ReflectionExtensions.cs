using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace PageTypeBuilder.Specs.Helpers.TypeBuildingDsl
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
                typeSpec.ParentType,
                typeSpec.Interfaces.ToArray());
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
                typeBuilder.AddProperty(propertySpecification);
        }

        public static void AddAttribute(
            this TypeBuilder typeBuilder,
            AttributeSpecification attributeSpecification)
        {
            CustomAttributeBuilder customAttributeBuilder = CreateAttributeWithValuesFromTemplate(attributeSpecification);

            typeBuilder.SetCustomAttribute(customAttributeBuilder);
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
            return attributeTemplate.GetType().GetProperties().Where(prop => IsSettableProperty(prop)).ToArray();
        }

        private static bool IsSettableProperty(PropertyInfo prop)
        {
            return prop.GetSetMethod() != null && prop.GetSetMethod().IsPublic;
        }

        private static object[] GetPropertyValues(Attribute attributeTemplate, PropertyInfo[] properties)
        {
            object[] propertyValues = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                propertyValues[i] = properties[i].GetValue(attributeTemplate, BindingFlags.GetProperty, null, null, null);
            }
            return propertyValues;
        }

        public static void AddProperty(
            this TypeBuilder typeBuilder,
            PropertySpecification propertySpec)
        {
            

            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertySpec.Name.ToLowerInvariant(), propertySpec.Type, FieldAttributes.Private);

            MethodBuilder getterMethodBuilder;
            if (propertySpec.GetterImplementation != null)
            {
                getterMethodBuilder = propertySpec.GetterImplementation(typeBuilder);
            }
            else
            {
                getterMethodBuilder = CreatePropertyGetMethodWithBackingField(
                    typeBuilder, propertySpec.Name, propertySpec.GetterAttributes, fieldBuilder);
            }

            MethodBuilder setterMethodBuilder = CreatePropertySetMethodWithBackingField(
                typeBuilder, propertySpec.Name, propertySpec.SetterAttributes, 
                fieldBuilder);

            if (propertySpec.AnnotateAsCompilerGenerated)
            {
                AddCompilerGeneratedAttribute(getterMethodBuilder);
                AddCompilerGeneratedAttribute(setterMethodBuilder);
            }

            if (typeBuilder.BaseType.GetProperty(propertySpec.Name) != null)
                return;

            var property = typeBuilder.DefineProperty(
                propertySpec.Name, PropertyAttributes.None, propertySpec.Type, null);

            property.SetGetMethod(getterMethodBuilder);
            property.SetSetMethod(setterMethodBuilder);

            foreach (var attributeTemplate in propertySpec.Attributes)
            {
                property.AddAttribute(attributeTemplate);
            }
        }

        static MethodBuilder CreatePropertyGetMethodWithBackingField(
            TypeBuilder typeBuilder, string name, 
            MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + name, methodAttributes, fieldBuilder.FieldType, Type.EmptyTypes);

            ILGenerator getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        static void AddCompilerGeneratedAttribute(MethodBuilder getMethodBuilder)
        {
            var compilerGeneratedAttributeCtor = 
                typeof (CompilerGeneratedAttribute).GetConstructor(new Type[0]);
            var compilerGeneratedAttribute = 
                new CustomAttributeBuilder(compilerGeneratedAttributeCtor, new object[0]);    
            getMethodBuilder.SetCustomAttribute(compilerGeneratedAttribute);
        }

        static MethodBuilder CreatePropertySetMethodWithBackingField(
            TypeBuilder typeBuilder, string name, 
            MethodAttributes methodAttributes, FieldBuilder fieldBuilder)
        {
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + name, methodAttributes, null, new Type[] { fieldBuilder.FieldType });

            ILGenerator setMethodILGenerator = setMethodBuilder.GetILGenerator();
            setMethodILGenerator.Emit(OpCodes.Ldarg_0);
            setMethodILGenerator.Emit(OpCodes.Ldarg_1);
            setMethodILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setMethodILGenerator.Emit(OpCodes.Ret);

            return setMethodBuilder;
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
