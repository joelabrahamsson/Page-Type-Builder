using System;
using System.Collections.Generic;
using PageTypeBuilder.Discovery;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class PageTypeDefinitionTests
    {
        [Fact]
        public void GivenAType_TypeProperty_SetsTypeProperyValue()
        {
            Type value = typeof(object);
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            pageTypeDefinition.Type = value;

            Assert.Equal<Type>(value, pageTypeDefinition.Type);
        }

        [Fact]
        public void GivenAPageTypeAttribute_AttributeProperty_SetsAttributePropertyValue()
        {
            PageTypeAttribute attribute = new PageTypeAttribute();
            attribute.Description = TestValueUtility.CreateRandomString();
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            pageTypeDefinition.Attribute = attribute;

            Assert.Equal<PageTypeAttribute>(attribute, pageTypeDefinition.Attribute, new PageTypeAttrbitueComparer());
        }

        private class PageTypeAttrbitueComparer :IComparer<PageTypeAttribute>
        {
            public int Compare(PageTypeAttribute x, PageTypeAttribute y)
            {
                if (x.Description == y.Description)
                    return 0;

                return -1;
            }
        }
    }
}