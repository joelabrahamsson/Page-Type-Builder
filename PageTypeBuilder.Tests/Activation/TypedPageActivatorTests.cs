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
    }
}
