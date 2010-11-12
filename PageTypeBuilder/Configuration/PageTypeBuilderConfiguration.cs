using System.Configuration;

namespace PageTypeBuilder.Configuration
{
    public class PageTypeBuilderConfiguration : ConfigurationSection
    {
        public static PageTypeBuilderConfiguration GetConfiguration()
        {
            PageTypeBuilderConfiguration configuration = ConfigurationManager.GetSection("pageTypeBuilder") as PageTypeBuilderConfiguration;

            if (configuration != null)
                return configuration;

            return new PageTypeBuilderConfiguration();
        }

        [ConfigurationProperty("disablePageTypeUpdation", IsRequired = false)]
        public virtual bool DisablePageTypeUpdation
        {
            get
            {
                return (bool)this["disablePageTypeUpdation"];
            }
        }

        [ConfigurationProperty("xmlns", IsRequired = false)]
        public string XmlNamespace
        {
            get
            {
                return "http://PageTypeBuilder.Configuration.PageTypeBuilderConfiguration";
            }
        }
    }
}
