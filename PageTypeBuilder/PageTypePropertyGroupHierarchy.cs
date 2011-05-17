namespace PageTypeBuilder
{
    internal class PageTypePropertyGroupHierarchy
    {
        public string Value { get; set; }

        public static string ResolvePropertyName(string currentHierarchy, string propertyName)
        {
            return !string.IsNullOrEmpty(currentHierarchy)
                    ? string.Concat(currentHierarchy, "-", propertyName)
                    : propertyName;
        }
    }
}