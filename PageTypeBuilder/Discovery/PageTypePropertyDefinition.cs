using System;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Discovery
{
    public class PageTypePropertyDefinition
    {
        public PageTypePropertyDefinition(string name, Type propertyType, IPageType pageType, PageTypePropertyAttribute pageTypePropertyAttribute)
        {
            Name = name;
            PropertyType = propertyType;
            PageType = pageType;
            PageTypePropertyAttribute = pageTypePropertyAttribute;
        }

        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public IPageType PageType { get; set; }
        public PageTypePropertyAttribute PageTypePropertyAttribute { get; set; }
    }
}