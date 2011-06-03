using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.ExampleImplementations
{
    public abstract class AbstractGlobalPropertySettings : IUpdateGlobalPropertySettings<TinyMCESettings>
    {
        public void UpdateSettings(TinyMCESettings settings)
        {
            settings.Width = int.MaxValue;
        }

        public static bool MatchesUpdatedSettings(TinyMCESettings settings)
        {
            return settings.Width == int.MaxValue;
        }

        public int GetSettingsHashCode(TinyMCESettings tinyMceSettings)
        {
            return tinyMceSettings.Width;
        }

        public virtual bool OverWriteExistingSettings
        {
            get { return true; }
        }

        public virtual string DisplayName
        {
            get { return "Global TinyMCE settings"; }
        }

        public string Description
        {
            get { return "Description for the settings"; }
        }

        public virtual bool? IsDefault { get; set; }


        public bool Match(EPiServer.Core.PropertySettings.PropertySettingsWrapper propertySettingsWrapper)
        {
            return DisplayName.Equals(propertySettingsWrapper.DisplayName);
        }

        public static bool HasDefaultValuesExceptUpdated(TinyMCESettings settings)
        {
            var defaultValues = (TinyMCESettings)new TinyMCESettings().GetDefaultValues();

            return Serialize(settings).Equals(Serialize(defaultValues));
        }

        public static string Serialize(TinyMCESettings settings)
        {
            var buffer = new StringBuilder();
            buffer.Append(settings.ContentCss);
            AppendDelimiter(buffer);
            buffer.Append(settings.Height);
            AppendDelimiter(buffer);
            buffer.Append("NonVisualPlugins");
            foreach (var nonVisualPlugin in settings.NonVisualPlugins)
            {
                buffer.Append(nonVisualPlugin);
                AppendDelimiter(buffer);
            }
            buffer.Append("ToolbarRows");
            foreach (var toolbarRow in settings.ToolbarRows)
            {
                buffer.Append("Row");
                foreach (var button in toolbarRow.Buttons)
                {
                    buffer.Append(button);
                    AppendDelimiter(buffer);
                }
            }
            return buffer.ToString();
        }

        private static void AppendDelimiter(StringBuilder buffer)
        {
            buffer.Append("|");
        }
    }
}
