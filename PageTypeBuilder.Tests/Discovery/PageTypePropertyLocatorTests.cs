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
        public void GivenTypeWithNestedPropertyGroups_GetPageTypePropertyDefinitions_ReturnsListWithSevenPropertyDefinitions()
        {
            PageTypePropertyDefinitionLocator definitionLocator = new PageTypePropertyDefinitionLocator();
            IPageType pageType = new NativePageType();
            Type pageTypeType = typeof(TestPageTypeWithPropertyGroups);
            IEnumerable<PageTypePropertyDefinition> propertyDefinitions = definitionLocator.GetPageTypePropertyDefinitions(pageType, pageTypeType);

            Assert.Equal(16, propertyDefinitions.Count());

            List<Defintion> defintions = new List<Defintion>
             {
                 new Defintion { EditCaption = "Property one", SortOrder = 100, Name = "LongStringProperty" },

                 new Defintion { EditCaption = "Image one - Image Url", SortOrder = 400, Name = "ImageOne-ImageUrl" },
                 new Defintion { EditCaption = "Image one - Alt text", SortOrder = 410, Name = "ImageOne-AltText" },
                 new Defintion { EditCaption = "Image one - Nullable test int property", SortOrder = 420, Name = "ImageOne-NullableTestIntProperty" },
                 new Defintion { EditCaption = "Image one - int test property", SortOrder = 430, Name = "ImageOne-IntTestProperty" },
                 new Defintion { EditCaption = "Image one - string test property", SortOrder = 440, Name = "ImageOne-StringTestProperty" },

                 new Defintion { EditCaption = "Image two - Image Url", SortOrder = 500, Name = "ImageTwo-ImageUrl" },
                 new Defintion { EditCaption = "Image two - Alt text", SortOrder = 510, Name = "ImageTwo-AltText" },
                 new Defintion { EditCaption = "Image two - Nullable test int property", SortOrder = 520, Name = "ImageTwo-NullableTestIntProperty" },
                 new Defintion { EditCaption = "Image two - int test property", SortOrder = 530, Name = "ImageTwo-IntTestProperty" },
                 new Defintion { EditCaption = "Image two - string test property", SortOrder = 540, Name = "ImageTwo-StringTestProperty" },

                 new Defintion { EditCaption = "Image three - Image Url", SortOrder = 600, Name = "ImageThree-ImageUrl" },
                 new Defintion { EditCaption = "Image three - Alt text", SortOrder = 610, Name = "ImageThree-AltText" }, 
                 new Defintion { EditCaption = "Image three - Nullable test int property", SortOrder = 620, Name = "ImageThree-NullableTestIntProperty" },
                 new Defintion { EditCaption = "Image three - int test property", SortOrder = 630, Name = "ImageThree-IntTestProperty" },
                 new Defintion { EditCaption = "Image three - string test property", SortOrder = 640, Name = "ImageThree-StringTestProperty" }
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