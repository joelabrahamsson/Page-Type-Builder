using System.Configuration;
using System.Web;

namespace PageTypeBuilder.Configuration
{
	public class PageTypeBuilderConfiguration : ConfigurationSection
	{
		/// <summary>
		/// The original option for disabling page type synchronization.
		/// </summary>
		private static readonly ConfigurationProperty disablePageTypeUpdation =
			new ConfigurationProperty("disablePageTypeUpdation", typeof(bool), false);

		/// <summary>
		/// A synonym of disablePageTypeUpdation.
		/// </summary>
		private static readonly ConfigurationProperty disablePageTypeSync =
			new ConfigurationProperty("disablePageTypeSync", typeof(bool), false);

		/// <summary>
		/// Used to set the path to a configuration file which, when it exists, will
		/// disable page type synchronization. It's useful for disabling sync in
		/// a multi-site EPiServer Enterprise installation, after the first site
		/// has been started.
		/// </summary>
		private static readonly ConfigurationProperty disablePageTypeUpdationFile =
			new ConfigurationProperty("disablePageTypeUpdationFile", typeof(string), "");

		/// <summary>
		/// A synonym of disablePageTypeUpdationFile.
		/// </summary>
		private static readonly ConfigurationProperty disablePageTypeSyncFile =
			new ConfigurationProperty("disablePageTypeSyncFile", typeof(string), "");

		private static readonly ConfigurationPropertyCollection properties =
			new ConfigurationPropertyCollection
		    {
				new ConfigurationProperty("xmlns", typeof(string), "http://PageTypeBuilder.Configuration.PageTypeBuilderConfiguration"),
		        disablePageTypeUpdation,
				disablePageTypeSync,
				disablePageTypeUpdationFile,
				disablePageTypeSyncFile
		    };

		protected override ConfigurationPropertyCollection Properties
		{
			get { return properties; }
		}

		public static PageTypeBuilderConfiguration GetConfiguration()
		{
			var configuration = ConfigurationManager.GetSection("pageTypeBuilder") as PageTypeBuilderConfiguration;
			return configuration ?? new PageTypeBuilderConfiguration();
		}

		public virtual bool DisablePageTypeUpdation
		{
			get
			{
				return (bool)this["disablePageTypeUpdation"]
					|| (bool)this["disablePageTypeSync"]
					|| IsDisabledByFile("disablePageTypeUpdationFile")
					|| IsDisabledByFile("disablePageTypeSyncFile");
			}
		}

		private bool IsDisabledByFile(string configFileKey)
		{
			var configFile = (string)this[configFileKey];
			if (string.IsNullOrEmpty(configFile))
			{
				return false;
			}

			var ctx = HttpContext.Current;
			if (ctx != null)
			{
				configFile = ctx.Server.MapPath(configFile);
			}

			return System.IO.File.Exists(configFile);
		}
	}
}
