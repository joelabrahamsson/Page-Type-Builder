using System;
using System.Linq.Expressions;
using EPiServer.Core;
using PageTypeBuilder.Activation;
using Xunit;

namespace PageTypeBuilder.Tests
{
    public class PageTypePropertyGroupsExtensionMethodsTests
    {
        [Fact]
        public void GivenValue_SetPropertyGroupPropertyValue_SetsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.ImageUrl);

            typedPageData.Property.Add(propertyName, new PropertyString(""));
            string valueToSet = "Test";

            typedPageData.ImageOne.SetPropertyGroupPropertyValue(propertyGroup => propertyGroup.ImageUrl, valueToSet);

            Assert.Equal<string>(valueToSet, typedPageData.ImageOne.ImageUrl as string);
        }

        [Fact]
        public void GetPropertyGroupPropertyValue_ReturnsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.ImageUrl);
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));

            string returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.ImageUrl);

            Assert.Equal<string>(propertyValue, returnedValue);
        }


        [Fact]
        public void GetPropertyGroupProperty_ReturnsProperty()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.ImageUrl);
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));

            PropertyData propertyData = typedPageData.ImageOne.GetPropertyGroupProperty(propertyGroup => propertyGroup.ImageUrl);
            Assert.Equal(typedPageData.Property[propertyName], propertyData);

            PropertyString propertyString = typedPageData.ImageOne.GetPropertyGroupProperty<Image, PropertyString>(propertyGroup => propertyGroup.ImageUrl);
            Assert.Equal(typedPageData.Property[propertyName], propertyString);
        }

        [Fact]
        public void GivenPropertyIsReferenceTypeAndNull_GetPropertyGroupPropertyValue_ReturnsNull()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.ImageUrl);
            typedPageData.Property.Add(propertyName, new PropertyString());

            string returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.ImageUrl);

            Assert.Null(returnedValue);
        }

        [Fact]
        public void GivenPropertyIsNullableTypeAndNull_GetPropertyGroupPropertyValue_ReturnsNullValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.NullableTestIntProperty);
            typedPageData.Property.Add(propertyName, new PropertyNumber());

            int? returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.NullableTestIntProperty);

            Assert.False(returnedValue.HasValue);
        }

        [Fact]
        public void GivenPropertyIsValueTypeAndNull_GetPropertyGroupPropertyValue_ThrowsException()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.IntTestProperty);
            typedPageData.Property.Add(propertyName, new PropertyNumber());

            Exception exception = Record.Exception(() => typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.IntTestProperty));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenValueExistsAndIncludeDynamicIsFalse_GetPropertyGroupPropertyValue_ReturnsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.StringTestProperty);

            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));

            string returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.StringTestProperty, false);

            Assert.Equal<string>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenValueExistsAndIncludeDynamicIsTrue_GetPropertyGroupPropertyValue_ReturnsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.StringTestProperty);
            string propertyValue = "Test";
            typedPageData.Property.Add(propertyName, new PropertyString(propertyValue));


            string returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.StringTestProperty, true);

            Assert.Equal<string>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyType_GetPropertyGroupPropertyValue_ReturnsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.NullableTestIntProperty);
            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));

            int returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue<Image, int>(propertyGroup => propertyGroup.NullableTestIntProperty);

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyTypeAndValueExistsAndIncludeDynamicIsFalse_GetPropertyGroupPropertyValue_ReturnsPropertyValue1()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.NullableTestIntProperty);

            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));


            int returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue<Image, int?>(propertyGroup => propertyGroup.NullableTestIntProperty, false).Value;

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenOtherReturnTypeThanPropertyTypeAndValueExistsAndIncludeDynamicIsTrue_GetPropertyGroupPropertyValue_ReturnsPropertyValue1()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.NullableTestIntProperty);
            int? propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue.Value));


            int returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue<Image, int>(propertyGroup => propertyGroup.NullableTestIntProperty, true);

            Assert.Equal<int>(propertyValue.Value, returnedValue);
        }

        [Fact]
        public void GivenPropertyValueIsValueType_GetPropertyGroupPropertyValue_ReturnsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.IntTestProperty);
            int propertyValue = 1;
            typedPageData.Property.Add(propertyName, new PropertyNumber(propertyValue));

            int returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.IntTestProperty);

            Assert.Equal<int>(propertyValue, returnedValue);
        }

        [Fact]
        public void GivenExpressionThatIsNotUnaryOrMember_GetPropertyGroupPropertyValue_ThrowsException()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;

            Exception exception = Record.Exception(() => typedPageData.ImageOne.GetPropertyGroupPropertyValue(propertyGroup => propertyGroup.IntTestProperty + 1));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenValueOfValueType_SetPropertyGroupPropertyValue_SetsPropertyValue()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.IntTestProperty);

            typedPageData.Property.Add(propertyName, new PropertyNumber());
            int valueToSet = 1;

            typedPageData.ImageOne.SetPropertyGroupPropertyValue(propertyGroup => propertyGroup.IntTestProperty, valueToSet);

            Assert.Equal<int>(valueToSet, (int)typedPageData[propertyName]);
        }

        [Fact]
        public void GivenExpressionThatIsNotUnaryOrMember_SetPropertyGroupPropertyValue_ThrowsException()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;

            Exception exception = Record.Exception(() => typedPageData.ImageOne.SetPropertyGroupPropertyValue(propertyGroup => propertyGroup.IntTestProperty + 1, 1));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenExpressionWithProperty_GetPropertyGroupPropertyName_ReturnsThePropertysName()
        {
            TestPageTypeWithPropertyGroups typedPageData = new TestPageTypeWithPropertyGroups();
            typedPageData = new TypedPageActivator().CreateAndPopulateTypedInstance(typedPageData, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;
            string propertyName = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.IntTestProperty);

            string returnedValue = typedPageData.ImageOne.GetPropertyGroupPropertyName(propertyGroup => propertyGroup.IntTestProperty);

            Assert.Equal<string>(propertyName, returnedValue);
        }
    }
}
