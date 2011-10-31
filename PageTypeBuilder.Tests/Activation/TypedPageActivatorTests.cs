using EPiServer.Core;
using PageTypeBuilder.Activation;
using Xunit;

namespace PageTypeBuilder.Tests.Activation
{
    public class TypedPageActivatorTests
    {
        [Fact]
        public void GivenPageWithPropertyValue_CreateAndPopulateTypedInstance_ReturnsTypedInstanceWithPropertyValue()
        {
            PageData sourcePage = new PageData();
            string propertyName = TestValueUtility.CreateRandomString();
            sourcePage.Property.Add(propertyName, new PropertyString());
            string propertyValue = TestValueUtility.CreateRandomString();
            sourcePage.SetValue(propertyName, propertyValue);
            TypedPageActivator activator = new TypedPageActivator();

            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(sourcePage, typeof(TestPageType));

            Assert.Equal<string>(propertyValue, page.GetValue(propertyName) as string);
        }

        [Fact]
        public void GivenPageFromCreateAndPopulateTypedInstance_CompilerGeneratedPropertySetter_IsAutoImplemented()
        {
            TypedPageActivator activator = new TypedPageActivator();
            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(new PageData(), typeof(TestPageType));
            page.Property.Add("CompilerGeneratedProperty", new PropertyString());
            string propertyValue = TestValueUtility.CreateRandomString();
            
            page.CompilerGeneratedProperty = propertyValue;

            Assert.Equal<string>(propertyValue, page["CompilerGeneratedProperty"] as string);
        }

        [Fact]
        public void GivenPageFromCreateAndPopulateTypedInstance_CompilerGeneratedPropertyGetter_IsAutoImplemented()
        {
            TypedPageActivator activator = new TypedPageActivator();
            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(new PageData(), typeof(TestPageType));
            page.Property.Add("CompilerGeneratedProperty", new PropertyString());
            string propertyValue = TestValueUtility.CreateRandomString();
            page.SetValue("CompilerGeneratedProperty", propertyValue);

            string returnedPropertyValue = page.CompilerGeneratedProperty;

            Assert.Equal<string>(propertyValue, returnedPropertyValue);
        }

        [Fact]
        public void GivenANullableIntProperty_WhenAccessedWithCompilerGeneratedPropertyGetter_ShouldReturnTheUnderlyingValue()
        {
            TypedPageActivator activator = new TypedPageActivator();
            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(new PageData(), typeof(TestPageType));
            var property = new PropertyNumber();
            page.Property.Add("NullableIntTestProperty", property);
            int propertyValue = 123;
            page.SetValue("NullableIntTestProperty", propertyValue);

            int returnedPropertyValue = page.NullableIntTestProperty.Value;

            Assert.Equal<int>(propertyValue, returnedPropertyValue);
        }

        [Fact]
        public void GivenANullableDoubleProperty_WhenAccessedWithCompilerGeneratedPropertyGetter_ShouldReturnTheUnderlyingValue()
        {
            TypedPageActivator activator = new TypedPageActivator();
            var originalPage = new PageData();
            var property = new PropertyFloatNumber();
            originalPage.Property.Add("NullableDoubleTestProperty", property);
            double propertyValue = 123.45;
            originalPage.SetValue("NullableDoubleTestProperty", propertyValue);
            
            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(originalPage, typeof(TestPageType));
            

            double returnedPropertyValue = page.NullableDoubleTestProperty.Value;

            Assert.Equal<double>(propertyValue, returnedPropertyValue);
        }

        [Fact]
        public void GivenANullableDoubleProperty_WhenSetAndAccessedWithCompilerGeneratedPropertyGetter_ShouldReturnTheSetValue()
        {
            TypedPageActivator activator = new TypedPageActivator();
            var originalPage = new PageData();
            var property = new PropertyFloatNumber();
            originalPage.Property.Add("NullableDoubleTestProperty", property);

            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(originalPage, typeof(TestPageType));
            double propertyValue = 123.45;
            page.NullableDoubleTestProperty = propertyValue;

            double returnedPropertyValue = page.NullableDoubleTestProperty.Value;

            Assert.Equal<double>(propertyValue, returnedPropertyValue);
        }

        [Fact]
        public void GivenANullableDoublePropertyWithNullValue_WhenAccessedWithCompilerGeneratedPropertyGetter_ShouldReturnTheUnderlyingValue()
        {
            TypedPageActivator activator = new TypedPageActivator();
            TestPageType page = (TestPageType)activator.CreateAndPopulateTypedInstance(new PageData(), typeof(TestPageType));
            var property = new PropertyFloatNumber();
            page.Property.Add("NullableDoubleTestProperty", property);
            
            double? returnedPropertyValue = page.NullableDoubleTestProperty;

            Assert.False(returnedPropertyValue.HasValue);
        }

        [Fact]
        public void GivenPageWithPropertyValueAndPropertyGroups_CreateAndPopulateTypedInstance_ReturnsTypedInstanceWithPropertyValues()
        {
            PageData sourcePage = new PageData();

            sourcePage.Property.Add("LongStringProperty", new PropertyString());
            sourcePage.SetValue("LongStringProperty", "one");
            sourcePage.Property.Add("ImageOne-ImageUrl", new PropertyString());
            sourcePage.SetValue("ImageOne-ImageUrl", "two");
            sourcePage.Property.Add("ImageOne-AltText", new PropertyString());
            sourcePage.SetValue("ImageOne-AltText", "three");
            sourcePage.Property.Add("ImageTwo-ImageUrl", new PropertyString());
            sourcePage.SetValue("ImageTwo-ImageUrl", "four");
            sourcePage.Property.Add("ImageTwo-AltText", new PropertyString());
            sourcePage.SetValue("ImageTwo-AltText", "five");
            sourcePage.Property.Add("ImageThree-ImageUrl", new PropertyString());
            sourcePage.SetValue("ImageThree-ImageUrl", "six");
            sourcePage.Property.Add("ImageThree-AltText", new PropertyString());
            sourcePage.SetValue("ImageThree-AltText", "seven");

            TypedPageActivator activator = new TypedPageActivator();

            TestPageTypeWithPropertyGroups page = activator.CreateAndPopulateTypedInstance(sourcePage, typeof(TestPageTypeWithPropertyGroups)) as TestPageTypeWithPropertyGroups;

            Assert.Equal("one", page.LongStringProperty);
            Assert.Equal("two", page.ImageOne.ImageUrl);
            Assert.Equal("three", page.ImageOne.AltText);
            Assert.Equal("four", page.ImageTwo.ImageUrl);
            Assert.Equal("five", page.ImageTwo.AltText);
            Assert.Equal("six", page.ImageThree.ImageUrl);
            Assert.Equal("seven", page.ImageThree.AltText);

        }
    }
}
