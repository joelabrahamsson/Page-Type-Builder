using System;
using System.Linq.Expressions;
using EPiServer.Core;
using Xunit;

namespace PageTypeBuilder.Tests
{
    public class PageDataExtensionMethodsTests
    {
        [Fact]
        public void GivenValue_SetPropertyValue_SetsPropertyValue()
        {
            Expression<Func<TestPageType, string>> expression = page => page.StringTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            typedPageData.Property.Add(propertyName, new PropertyString(""));
            string valueToSet = "Test";
            
            typedPageData.SetPropertyValue(page => page.StringTestProperty, valueToSet);

            Assert.Equal<string>(valueToSet, typedPageData[propertyName] as string);
        }

        [Fact]
        public void GetPropertyValue_ReturnsPropertyValue()
        {
            Expression<Func<TestPageType, string>> expression = page => page.StringTestProperty;
            
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));
            

            string returnedValue = typedPageData.GetPropertyValue(page => page.StringTestProperty);

            Assert.Equal<string>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenPropertyIsReferenceTypeAndNull_GetPropertyValue_ReturnsNull()
        {
            Expression<Func<TestPageType, string>> expression = page => page.StringTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            typedPageData.Property.Add(propertyName, new PropertyString());

            string returnedValue = typedPageData.GetPropertyValue(page => page.StringTestProperty);

            Assert.Null(returnedValue);
        }

        [Fact]
        public void GivenPropertyIsNullableTypeAndNull_GetPropertyValue_ReturnsNullValue()
        {
            Expression<Func<TestPageType, int?>> expression = page => page.NullableIntTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            typedPageData.Property.Add(propertyName, new PropertyNumber());

            int? returnedValue = typedPageData.GetPropertyValue(page => page.NullableIntTestProperty);

            Assert.False(returnedValue.HasValue);
        }

        [Fact]
        public void GivenPropertyIsValueTypeAndNull_GetPropertyValue_ThrowsException()
        {
            Expression<Func<TestPageType, int>> expression = page => page.IntTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            typedPageData.Property.Add(propertyName, new PropertyNumber());

            Exception exception = Record.Exception(() => typedPageData.GetPropertyValue(page => page.IntTestProperty));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenValueExistsAndIncludeDynamicIsFalse_GetPropertyValue_ReturnsPropertyValue()
        {
            Expression<Func<TestPageType, string>> expression = page => page.StringTestProperty;

            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));


            string returnedValue = typedPageData.GetPropertyValue(page => page.StringTestProperty, false);

            Assert.Equal<string>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenValueExistsAndIncludeDynamicIsTrue_GetPropertyValue_ReturnsPropertyValue()
        {
            Expression<Func<TestPageType, string>> expression = page => page.StringTestProperty;

            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));


            string returnedValue = typedPageData.GetPropertyValue(page => page.StringTestProperty, true);

            Assert.Equal<string>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyType_GetPropertyValue_ReturnsPropertyValue()
        {
            Expression<Func<TestPageType, int?>> expression = page => page.NullableIntTestProperty;

            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));


            int returnedValue = typedPageData.GetPropertyValue<TestPageType, int>(page => page.NullableIntTestProperty);

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyTypeAndValueExistsAndIncludeDynamicIsFalse_GetPropertyValue_ReturnsPropertyValue1()
        {
            Expression<Func<TestPageType, int?>> expression = page => page.NullableIntTestProperty;

            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));


            int returnedValue = typedPageData.GetPropertyValue<TestPageType, int>(page => page.NullableIntTestProperty, false);

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyTypeAndValueExistsAndIncludeDynamicIsTrue_GetPropertyValue_ReturnsPropertyValue1()
        {
            Expression<Func<TestPageType, int?>> expression = page => page.NullableIntTestProperty;

            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));


            int returnedValue = typedPageData.GetPropertyValue<TestPageType, int>(page => page.NullableIntTestProperty, true);

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenPropertyValueIsValueType_GetPropertyValue_ReturnsPropertyValue()
        {
            Expression<Func<TestPageType, int>> expression = page => page.IntTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            int propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue));


            int returnedValue = typedPageData.GetPropertyValue(page => page.IntTestProperty);

            Assert.Equal<int>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenExpressionThatIsNotUnaryOrMember_GetPropertyValue_ThrowsException()
        {
            Expression<Func<TestPageType, object>> expression = page => page.IntTestProperty + 1;
            TestPageType typedPageData = new TestPageType();

            Exception exception = Record.Exception(() => typedPageData.GetPropertyValue(expression));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenValueOfValueType_SetPropertyValue_SetsPropertyValue()
        {
            Expression<Func<TestPageType, int>> expression = page => page.IntTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();
            typedPageData.Property.Add(propertyName, new PropertyNumber());
            int valueToSet = 1;

            typedPageData.SetPropertyValue(page => page.IntTestProperty, valueToSet);

            Assert.Equal<int>(valueToSet, (int) typedPageData[propertyName]);
        }

        [Fact]
        public void GivenExpressionThatIsNotUnaryOrMember_SetPropertyValue_ThrowsException()
        {
            Expression<Func<TestPageType, object>> expression = page => page.IntTestProperty + 1;
            TestPageType typedPageData = new TestPageType();

            Exception exception = Record.Exception(() => typedPageData.SetPropertyValue(expression, 1));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenExpressionWithProperty_GetPropertyName_ReturnsThePropertysName()
        {
            Expression<Func<TestPageType, int>> expression = page => page.IntTestProperty;
            MemberExpression methodExpression = (MemberExpression)expression.Body;
            string propertyName = methodExpression.Member.Name;
            TestPageType typedPageData = new TestPageType();

            string returnedValue = typedPageData.GetPropertyName(page => page.IntTestProperty);

            Assert.Equal<string>(propertyName, returnedValue);
        }
    }
}
