using System.Reflection;
using PageTypeBuilder.Activation;
using Xunit;

namespace PageTypeBuilder.Tests.Activation
{
    public class PageTypePropertiesProxyGenerationHookTests
    {
        [Fact]
        public void GivenPropertyWithoutPageTypePropertyAttribute_ShouldInterceptMethod_ReturnsFalse()
        {
            PageTypePropertiesProxyGenerationHook hook = new PageTypePropertiesProxyGenerationHook();
            MethodInfo compilerGeneratedMethod = typeof(TestPageType).GetProperty("CompilerGeneratedPropertyWithoutAttribute").GetGetMethod();

            bool returnValue = hook.ShouldInterceptMethod(typeof(TestPageType), compilerGeneratedMethod);

            Assert.Equal<bool>(false, returnValue);
        }

        [Fact]
        public void GivenNotCompilerGeneratedPropertyWithAttribute_ShouldInterceptMethod_ReturnsFalse()
        {
            PageTypePropertiesProxyGenerationHook hook = new PageTypePropertiesProxyGenerationHook();
            MethodInfo compilerGeneratedMethod = typeof(TestPageType).GetProperty("NotGeneratedProperty").GetGetMethod();

            bool returnValue = hook.ShouldInterceptMethod(typeof(TestPageType), compilerGeneratedMethod);

            Assert.Equal<bool>(false, returnValue);
        }

        [Fact]
        public void GivenCompilerGeneratedPropertyWithAttribute_ShouldInterceptMethod_ReturnsTrue()
        {
            PageTypePropertiesProxyGenerationHook hook = new PageTypePropertiesProxyGenerationHook();
            MethodInfo compilerGeneratedMethod = typeof(TestPageType).GetProperty("CompilerGeneratedProperty").GetGetMethod();
            
            bool returnValue = hook.ShouldInterceptMethod(typeof(TestPageType), compilerGeneratedMethod);

            Assert.Equal<bool>(true, returnValue);
        }
        
    }
}
