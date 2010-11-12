using System;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Discovery
{
    public class PageTypePropertyDefinition
    {
        public PageTypePropertyDefinition(string name, Type propertyType, PageType pageType, PageTypePropertyAttribute pageTypePropertyAttribute)
        {
            Name = name;
            PropertyType = propertyType;
            PageType = pageType;
            PageTypePropertyAttribute = pageTypePropertyAttribute;
        }

        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public PageType PageType { get; set; }
        public PageTypePropertyAttribute PageTypePropertyAttribute { get; set; }
    }
}