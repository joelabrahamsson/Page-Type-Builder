using System.Text.RegularExpressions;

namespace PageTypeBuilder.Migrations
{
    public static class MigrationExtensions
    {
        private static Regex numberPattern = new Regex("[0-9]*$", RegexOptions.Compiled);
        internal static int Number(this IMigration migration)
        {
            var typeName = migration.GetType().Name;
            return int.Parse(numberPattern.Match(typeName).Captures[0].Value);
        }
    }
}
