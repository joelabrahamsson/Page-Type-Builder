namespace PageTypeBuilder.Discovery
{
    using System;
    using Abstractions;

    public class PageTypePropertyDefinition
    {
        public PageTypePropertyDefinition(string name, Type propertyType, IPageType pageType, PageTypePropertyAttribute pageTypePropertyAttribute,
            PageTypePropertyGroupPropertyOverrideAttribute pageTypePropertyGroupPropertyOverrideAttribute)
        {
            Name = name;
            PropertyType = propertyType;
            PageType = pageType;
            PageTypePropertyAttribute = pageTypePropertyAttribute;
            PageTypePropertyGroupPropertyOverrideAttribute = pageTypePropertyGroupPropertyOverrideAttribute;
        }

        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public IPageType PageType { get; set; }
        public PageTypePropertyAttribute PageTypePropertyAttribute { get; set; }
        public PageTypePropertyGroupPropertyOverrideAttribute PageTypePropertyGroupPropertyOverrideAttribute { get; set; }
    }
}