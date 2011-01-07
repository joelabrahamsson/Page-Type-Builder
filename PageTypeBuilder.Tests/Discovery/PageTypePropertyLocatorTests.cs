using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class PageTypePropertyLocatorTests
    {
        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttribute_GetPageTypePropertyDefinitions_ReturnsListWithOnePropertyDefinition()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("TestAssembly"),
                                                                                            AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module", "Module.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("PageTypeType");
            string propertyName = TestValueUtility.CreateRandomString();
            Type propertyType = typeof(string);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
            ConstructorInfo pageTypePropertyAttributeConstructor = typeof(PageTypePropertyAttribute).GetConstructor(new Type[0]);
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(pageTypePropertyAttributeConstructor, new object[0]);
            propertyBuilder.SetCustomAttribute(customAttributeBuilder);
            Type type = typeBuilder.CreateType();
            IPageType pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();

            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);

            Assert.Equal<int>(1, propertyDefinitions.Count());
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromInterface_GetPageTypePropertyDefinitions_ReturnsListWithOnePropertyDefinition()
        {
            var type = typeof(TestPageTypeWithInterface);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<int>(1, propertyDefinitions.Count());
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromInterface_GetPageTypePropertyDefinitions_ReturnsDefinitionFromInterface()
        {
            var type = typeof(TestPageTypeWithInterface);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<string>(TestEditCaptions.FromInterfaceA, propertyDefinitions.ElementAt(0).PageTypePropertyAttribute.EditCaption);
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromInterfaceOverriddenInPageType_GetPageTypePropertyDefinitions_ReturnsListWithOnePropertyDefinition()
        {
            var type = typeof(TestPageTypeWithInterfaceWhichAlsoDefinesProperty);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<int>(1, propertyDefinitions.Count());
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromInterfaceOverriddenInPageType_GetPageTypePropertyDefinitions_ReturnsDefinitionFromPageType()
        {
            var type = typeof(TestPageTypeWithInterfaceWhichAlsoDefinesProperty);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<string>(TestEditCaptions.FromPageType, propertyDefinitions.First().PageTypePropertyAttribute.EditCaption);
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromClashingInterfacesButOverriddenInPageType_GetPageTypePropertyDefinitions_ReturnsListWithOnePropertyDefinition()
        {
            var type = typeof(TestPageTypeWithClashingInterfacesWhichAlsoDefinesProperty);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<int>(1, propertyDefinitions.Count());
        }

        [Fact]
        public void GivenTypeWithOnePageTypePropertyAttributeFromClashingInterfacesButOverriddenInPageType_GetPageTypePropertyDefinitions_ReturnsDefinitionFromPageType()
        {
            var type = typeof(TestPageTypeWithClashingInterfacesWhichAlsoDefinesProperty);
            var pageType = new NativePageType();
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, type);
            Assert.Equal<string>(TestEditCaptions.FromPageType, propertyDefinitions.First().PageTypePropertyAttribute.EditCaption);
        }
    }
}