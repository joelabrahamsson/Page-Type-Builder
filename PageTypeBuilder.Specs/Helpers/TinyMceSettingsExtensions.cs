using System.Text;
using EPiServer.Editor.TinyMCE;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class TinyMceSettingsExtensions
    {
        public static string Serialize(this TinyMCESettings settings)
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
