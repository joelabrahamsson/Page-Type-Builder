using System;
using EPiServer.Core;
using Xunit;

namespace PageTypeBuilder.Tests
{
    public class PageTypeResolverTests
    {
        [Fact]
        public void Instance_ReturnsInstanceOfPageTypeResolver()
        {
            PageTypeResolver instance = PageTypeResolver.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void GivenTypeAdded_GetPageTypeType_ReturnsCorrectType()
        {
            int id = 1;
            Type type = typeof(string);
            PageTypeResolver.Instance = new PageTypeResolver();

            PageTypeResolver.Instance.AddPageType(id, type);
            Type addedType = PageTypeResolver.Instance.GetPageTypeType(id);

            Assert.NotNull(addedType);
            Assert.Equal<Type>(type, addedType);
        }

        [Fact]
        public void GivenTypeAlreadyAdded_AddPageType_DoesNotThrowException()
        {
            int id = 1;
            Type type = typeof(string);
            PageTypeResolver.Instance = new PageTypeResolver();
            PageTypeResolver.Instance.AddPageType(id, type);

            Exception exception = Record.Exception(() => PageTypeResolver.Instance.AddPageType(id, type));

            Assert.Null(exception);
        }

        [Fact]
        public void GivenTypeAlreadyAddedWithDifferentID_AddPageType_ThrowsException()
        {
            int id = 1;
            int otherID = 2;
            Type type = typeof(string);
            PageTypeResolver.Instance = new PageTypeResolver();
            PageTypeResolver.Instance.AddPageType(id, type);

            Exception exception = Record.Exception(() => PageTypeResolver.Instance.AddPageType(otherID, type));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenIDAlreadyAddedWithDifferentType_AddPageType_ThrowsException()
        {
            int id = 1;
            Type type = typeof(string);
            Type otherType = typeof(StringComparer);
            PageTypeResolver.Instance = new PageTypeResolver();
            PageTypeResolver.Instance.AddPageType(id, type);

            Exception exception = Record.Exception(() => PageTypeResolver.Instance.AddPageType(id, otherType));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenTypeAdded_GetPageType_ReturnsCorrectType()
        {
            int id = 1;
            Type type = typeof(string);
            PageTypeResolver.Instance = new PageTypeResolver();

            PageTypeResolver.Instance.AddPageType(id, type);
            int? returnedID = PageTypeResolver.Instance.GetPageTypeID(type);

            Assert.True(returnedID.HasValue);
            Assert.Equal<int>(id, returnedID.Value);
        }

        [Fact]
        public void GivenPageDataWithUnmappedType_ConvertToTyped_ReturnsPageDataObject()
        {
            PageTypeResolver.Instance = new PageTypeResolver();
            PageData pageData = new PageData();
            pageData.Property.Add("PageTypeID", new PropertyNumber(1));

            PageData returnedObject = PageTypeResolver.Instance.ConvertToTyped(pageData);

            Assert.NotNull(returnedObject);
            Assert.Equal<Type>(typeof(PageData), returnedObject.GetType());
        }

        [Fact]
        public void GivenPageDataWithMappedType_ConvertToTyped_ReturnsObjectOfCorrectType()
        {
            int pageTypeID = 1;
            Type type = typeof(TestPageType);
            PageTypeResolver.Instance = new PageTypeResolver();
            PageTypeResolver.Instance.AddPageType(pageTypeID, type);
            PageData pageData = new PageData();
            pageData.Property.Add("PageTypeID", new PropertyNumber(pageTypeID));

            PageData returnedObject = PageTypeResolver.Instance.ConvertToTyped(pageData);

            Assert.NotNull(returnedObject);
            Assert.True(returnedObject is TestPageType);
        }
    }
}
