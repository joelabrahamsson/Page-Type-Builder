using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.PropertySettings
{
    public class TinyMceSettingsAttribute : Attribute, IPropertySettingsAttribute
    {
        //Factory method?
        public TinyMceSettingsAttribute()
        {
            Width = 500;
            Height = 300;
            ContentCss = string.Empty;
        }

        public bool UpdateSettings(IPropertySettings settings)
        {
            var tinyMceSettings = (TinyMCESettings) settings;
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
            
            
            return true;
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

        public Type SettingType
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

        public bool OverWriteExistingSettings { get; set; }
    }
}
