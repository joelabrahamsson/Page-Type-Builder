using PageTypeBuilder.Configuration;
using Xunit;

namespace PageTypeBuilder.Tests.Configuration
{
    public class PageTypeBuilderConfigurationTests
    {
        [Fact]
        public void DisablePageTypeUpdation_DefaultsToFalse()
        {
            PageTypeBuilderConfiguration configuration = new PageTypeBuilderConfiguration();
            
            Assert.Equal<bool>(false, configuration.DisablePageTypeUpdation);
        }

        [Fact]
        public void GivenPageTypeUpdationDisabledInAppConfig_DisablePageTypeUpdation_ReturnsTrue()
        {
            PageTypeBuilderConfiguration configuration = PageTypeBuilderConfiguration.GetConfiguration();

            Assert.Equal<bool>(true, configuration.DisablePageTypeUpdation);
        }
    }
}
