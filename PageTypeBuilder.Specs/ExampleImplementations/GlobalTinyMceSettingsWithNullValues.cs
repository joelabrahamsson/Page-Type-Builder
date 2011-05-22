using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public class GlobalTinyMceSettingsWithNullValues : IUpdateGlobalPropertySettings<TinyMCESettings>
    {
        public string DisplayName
        {
            get { return "Global TinyMCE settings with null values"; }
        }

        public string Description
        {
            get { return null; }
        }

        public bool? IsDefault
        {
            get { return null; }
        }

        public bool Match(EPiServer.Core.PropertySettings.PropertySettingsWrapper propertySettingsWrapper)
        {
            return propertySettingsWrapper.DisplayName.Equals(propertySettingsWrapper.DisplayName);
        }

        public void UpdateSettings(TinyMCESettings settings)
        {
        }

        public int GetSettingsHashCode(TinyMCESettings settings)
        {
            return 0;
        }

        public bool OverWriteExistingSettings
        {
            get { return true; }
        }
    }
}
