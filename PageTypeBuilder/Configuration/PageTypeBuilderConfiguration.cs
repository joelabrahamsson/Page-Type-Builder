using System.Configuration;
using EPiServer.Framework.Configuration;

namespace PageTypeBuilder.Configuration
{
    public class PageTypeBuilderConfiguration : ConfigurationSection
    {
        public static PageTypeBuilderConfiguration GetConfiguration()
        {
            var configuration = ConfigurationManager.GetSection("pageTypeBuilder") as PageTypeBuilderConfiguration;

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

        [ConfigurationProperty("scanAssembly", IsRequired = false)]
        public AssemblyElementCollection ScanAssembly
        {
            get
            {
                return (AssemblyElementCollection)this["scanAssembly"];
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
