using System.Collections.Generic;
using System.Configuration;

namespace PageTypeBuilder.Configuration
{
    public class PageTypeBuilderConfiguration : ConfigurationSection
    {

        public enum AssemblyVersionStamp
        {
            LastWriteTime,
            Version
        }

        private static PageTypeBuilderConfiguration _configuration;

        public static PageTypeBuilderConfiguration GetConfiguration()
        {
            if (_configuration != null)
                return _configuration;

            _configuration = ConfigurationManager.GetSection("pageTypeBuilder") as PageTypeBuilderConfiguration;

            return _configuration ?? new PageTypeBuilderConfiguration();
        }

        [ConfigurationProperty("disablePageTypeUpdation", IsRequired = false)]
        public virtual bool DisablePageTypeUpdation
        {
            get
            {
                return (bool)this["disablePageTypeUpdation"];
            }
        }

        [ConfigurationProperty("oneTimeSynchornizationEnabled", IsRequired = false)]
        public virtual bool OneTimeSynchornizationEnabled
        {
            get
            {
                return (bool)this["oneTimeSynchornizationEnabled"];
            }
        }

        [ConfigurationProperty("oneTimeSynchornizationPollTime", IsRequired = false, DefaultValue = 2000)]
        public virtual int OneTimeSynchornizationPollTime
        {
            get
            {
                return (int)this["oneTimeSynchornizationPollTime"];
            }
        }

        [ConfigurationProperty("oneTimeSynchronizationLogFileDirectoryPath", IsRequired = false, DefaultValue = "")]
        public virtual string OneTimeSynchronizationLogFileDirectoryPath
        {
            get
            {
                return (string)this["oneTimeSynchronizationLogFileDirectoryPath"];
            }
        }

        [ConfigurationProperty("oneTimeSynchronizationAssemblyVersionStamp", IsRequired = false, DefaultValue = AssemblyVersionStamp.Version)]
        public virtual AssemblyVersionStamp OneVersionSynchronizationAssemblyVersionStamp
        {
            get
            {
                return (AssemblyVersionStamp)this["oneTimeSynchronizationAssemblyVersionStamp"];
            }
        }

        [ConfigurationProperty("timingsLogFileDirectoryPath", IsRequired = false, DefaultValue = "")]
        public virtual string TimingsLogFileDirectoryPath
        {
            get
            {
                return (string)this["timingsLogFileDirectoryPath"];
            }
        }

        [ConfigurationProperty("performValidation", IsRequired = false, DefaultValue = true)]
        public virtual bool PerformValidation
        {
            get
            {
                return (bool)this["performValidation"];
            }
        }

        [ConfigurationProperty("scanAssemblies", IsRequired = false)]
        public ScanAssemblyCollection ScanAssemblies
        {

            get { return ((ScanAssemblyCollection)(this["scanAssemblies"])); }

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

    [ConfigurationCollection(typeof(ScanAssemblyElement))]
    public class ScanAssemblyCollection : ConfigurationElementCollection
    {

        List<ScanAssemblyElement> _elements = new List<ScanAssemblyElement>();

        protected override ConfigurationElement CreateNewElement()
        {
            ScanAssemblyElement newElement = new ScanAssemblyElement();
            _elements.Add(newElement);
            return newElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return _elements.Find(e => e.Equals(element));
        }

        public new IEnumerator<ScanAssemblyElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

    }

    public class ScanAssemblyElement : ConfigurationElement
    {

        [ConfigurationProperty("assemblyName", IsKey = true, IsRequired = true)]
        public string AssemblyName
        {
            get
            {
                return ((string)(base["assemblyName"]));
            }
            set
            {
                base["assemblyName"] = value;
            }
        }
    }

}
