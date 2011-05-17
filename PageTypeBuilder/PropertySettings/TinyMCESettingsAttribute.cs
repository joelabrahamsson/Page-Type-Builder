using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.PropertySettings
{
    public class TinyMceSettingsAttribute : PropertySettingsAttribute
    {
        private const int IntegerNotSetValue = -1;
        public TinyMceSettingsAttribute()
        {
            Width = IntegerNotSetValue;
            Height = IntegerNotSetValue;
        }

        public override bool UpdateSettings(IPropertySettings settings)
        {
            var tinyMceSettings = (TinyMCESettings) settings;

            string preUpdate = SerializeValues(tinyMceSettings);
            
            tinyMceSettings.ToolbarRows = new List<ToolbarRow>();
            IEnumerable<string[]> toolbarRowSpecifications = GetToolBarRows();
            foreach (var rowSpecification in toolbarRowSpecifications)
            {
                var row = new ToolbarRow();
                foreach (var buttonName in rowSpecification)
                {
                    row.Buttons.Add(buttonName);
                }
                tinyMceSettings.ToolbarRows.Add(row);
            }

            if (Width == IntegerNotSetValue)
                tinyMceSettings.Width = ((TinyMCESettings)tinyMceSettings.GetDefaultValues()).Width;
            else
                tinyMceSettings.Width = Width;

            if (Height == IntegerNotSetValue)
                tinyMceSettings.Height = ((TinyMCESettings)tinyMceSettings.GetDefaultValues()).Height;
            else
                tinyMceSettings.Height = Height;

            if (NonVisualPlugins == null)
                tinyMceSettings.NonVisualPlugins =
                    ((TinyMCESettings)tinyMceSettings.GetDefaultValues()).NonVisualPlugins;
            else
                tinyMceSettings.NonVisualPlugins = NonVisualPlugins;

            string afterUpdate = SerializeValues(tinyMceSettings);
            
            return !afterUpdate.Equals(preUpdate);
        }

        private string SerializeValues(TinyMCESettings tinyMceSettings)
        {
            var buffer = new StringBuilder();

            buffer.Append("ContentCss:");
            buffer.Append(tinyMceSettings.ContentCss);

            buffer.Append("Width:");
            buffer.Append(tinyMceSettings.Width);

            buffer.Append("Height:");
            buffer.Append(tinyMceSettings.Height);
            
            buffer.Append("Toolbars:");
            foreach (var toolbarRow in tinyMceSettings.ToolbarRows)
            {
                buffer.Append("ToolbarRow:");
                foreach (var button in toolbarRow.Buttons)
                {
                    buffer.Append(button);
                    buffer.Append("|");
                }
            }

            buffer.Append("NonVisualPlugins:");
            foreach (var nonVisualPlugin in tinyMceSettings.NonVisualPlugins)
            {
                buffer.Append(nonVisualPlugin);
                buffer.Append("|");
            }
            return buffer.ToString();
        }

        IEnumerable<string[]> GetToolBarRows()
        {
            var allRows = new List<string[]>
                       {
                           FirstToolbarRow,
                           SecondToolbarRow,
                           ThirdToolbarRow,
                           FourthToolbarRow,
                           FifthToolbarRow
                       };

            return allRows.Where(row => row != null);
        }

        public override Type SettingType
        {
            get { return typeof (TinyMCESettings); }
        }

        public string ContentCss { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string[] FirstToolbarRow { get; set; }

        public string[] SecondToolbarRow { get; set; }

        public string[] ThirdToolbarRow { get; set; }

        public string[] FourthToolbarRow { get; set; }

        public string[] FifthToolbarRow { get; set; }

        public string[] NonVisualPlugins { get; set; }

        public override bool OverWriteExistingSettings { get; set; }
    }
}
