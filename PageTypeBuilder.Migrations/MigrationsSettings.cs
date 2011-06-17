using System.Configuration;

namespace PageTypeBuilder.Migrations
{
    public class MigrationsConfiguration : ConfigurationSection
    {
        public static MigrationsConfiguration GetConfiguration()
        {
            MigrationsConfiguration configuration = ConfigurationManager.GetSection("pageTypeBuilderMigrations") as MigrationsConfiguration;

            if (configuration != null)
                return configuration;

            return new MigrationsConfiguration();
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
