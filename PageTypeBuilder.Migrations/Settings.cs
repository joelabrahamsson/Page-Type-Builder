using System.Configuration;

namespace PageTypeBuilder.Migrations
{
    public class Settings : ConfigurationSection
    {
        public static Settings GetConfiguration()
        {
            Settings configuration = ConfigurationManager.GetSection("pagetypebuilder.migrations") as Settings;

            if (configuration != null)
                return configuration;

            return new Settings();
        }

        [ConfigurationProperty("disabled", IsRequired = false)]
        public virtual bool Disabled
        {
            get
            {
                return (bool)this["disabled"];
            }
        }

        [ConfigurationProperty("xmlns", IsRequired = false)]
        public string XmlNamespace
        {
            get
            {
                return "http://PageTypeBuilder.Migrations.MigrationsConfiguration";
            }
        }
    }
}
