namespace PageTypeBuilder.Tests.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Abstractions;
    using PageTypeBuilder.Discovery;
    using Xunit;

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

        [Fact]
        public void GivenTypeWithNestedPropertyGroups_GetPageTypePropertyDefinitions_ReturnsListWithSevenPropertyDefinitions()
        {
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IPageType pageType = new NativePageType();
            Type pageTypeType = typeof(TestPageTypeWithPropertyGroups);
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, pageTypeType);

            Assert.Equal(7, propertyDefinitions.Count());

            List<Defintion> defintions = new List<Defintion>
             {
                 new Defintion { EditCaption = "Property one", SortOrder = 100, Name = "LongStringProperty" },
                 new Defintion { EditCaption = "Image one - Image Url", SortOrder = 400, Name = "ImageOne-ImageUrl" },
                 new Defintion { EditCaption = "Image one - Alt text", SortOrder = 410, Name = "ImageOne-AltText" },
                 new Defintion { EditCaption = "Image two - Image Url", SortOrder = 500, Name = "ImageTwo-ImageUrl" },
                 new Defintion { EditCaption = "Image two - Alt text", SortOrder = 510, Name = "ImageTwo-AltText" },
                 new Defintion { EditCaption = "Image three - Image Url", SortOrder = 600, Name = "ImageThree-ImageUrl" },
                 new Defintion { EditCaption = "Image three - Alt text", SortOrder = 610, Name = "ImageThree-AltText" }
             };

            foreach (PageTypePropertyDefinition pageTypePropertyDefinition in propertyDefinitions)
            {
                Defintion defintion = defintions.Where(current => string.Equals(current.Name, pageTypePropertyDefinition.Name)).FirstOrDefault();

                Assert.True(defintion != null);

                Assert.Equal(pageTypePropertyDefinition.PageTypePropertyAttribute.SortOrder, defintion.SortOrder);
                Assert.Equal(pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption, defintion.EditCaption);
            }
        }

        private class Defintion
        {
            public string EditCaption;
            public int SortOrder;
            public string Name;
        }
    }
}