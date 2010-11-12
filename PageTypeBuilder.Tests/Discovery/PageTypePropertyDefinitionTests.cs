using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class PageTypePropertyDefinitionTests
    {
        [Fact]
        public void GivenName_Constructor_SetsNameProperty()
        {
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            string name = TestValueUtility.CreateRandomString();
            Type propertyType = typeof(string);
            PageType pageType = new PageType();
            
            PageTypePropertyDefinition definition = new PageTypePropertyDefinition(name, propertyType, pageType, attribute);


            Assert.Equal<string>(name, definition.Name);
        }

        [Fact]
        public void GivenType_Constructor_SetsPropertyTypeProperty()
        {
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            string name = TestValueUtility.CreateRandomString();
            Type propertyType = typeof(string);
            PageType pageType = new PageType();

            PageTypePropertyDefinition definition = new PageTypePropertyDefinition(name, propertyType, pageType, attribute);


            Assert.Equal<Type>(propertyType, definition.PropertyType);
        }

        [Fact]
        public void GivenPageType_Constructor_SetsPageTypeProperty()
        {
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            string name = TestValueUtility.CreateRandomString();
            Type propertyType = typeof(string);
            PageType pageType = new PageType();
            pageType.GUID = Guid.NewGuid();

            PageTypePropertyDefinition definition = new PageTypePropertyDefinition(name, propertyType, pageType, attribute);


            Assert.Equal<PageType>(pageType, definition.PageType, new PageTypeComparer());
        }

        private class PageTypeComparer : IComparer<PageType>
        {

            public int Compare(PageType x, PageType y)
            {
                if (x.GUID == y.GUID)
                    return 0;

                return -1;
            }
        }

        [Fact]
        public void GivenPageTypePropertyAttribute_Constructor_SetsPageTypePropertyAttributeProperty()
        {
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            attribute.HelpText = TestValueUtility.CreateRandomString();
            string name = TestValueUtility.CreateRandomString();
            Type propertyType = typeof(string);
            PageType pageType = new PageType();

            PageTypePropertyDefinition definition = new PageTypePropertyDefinition(name, propertyType, pageType, attribute);


            Assert.Equal<PageTypePropertyAttribute>(attribute, definition.PageTypePropertyAttribute, 
                                                    new PageTypePropertyComparer());
        }

        private class PageTypePropertyComparer : IComparer<PageTypePropertyAttribute>
        {
            public int Compare(PageTypePropertyAttribute x, PageTypePropertyAttribute y)
            {
                if (x.HelpText == y.HelpText)
                    return 0;

                return -1;
            }
        }
    }
}