using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Discovery
{
    public class PageTypePropertyDefinitionLocator
    {
        public virtual IEnumerable<PageTypePropertyDefinition> GetPageTypePropertyDefinitions(IPageType pageType, Type pageTypeType)
        {
            var properties = pageTypeType.GetAllValidPageTypePropertiesFromClassAndImplementedInterfaces();

            List<PageTypePropertyDefinition> pageTypePropertyDefinitions = new List<PageTypePropertyDefinition>();
            foreach (PropertyInfo property in properties)
            {
                PageTypePropertyAttribute attribute = GetPageTypePropertyAttribute(property);

                if (attribute == null)
                    continue;

                pageTypePropertyDefinitions.Add(new PageTypePropertyDefinition(property.Name, property.PropertyType, pageType, attribute));
            }
            return pageTypePropertyDefinitions;
        }

        internal PageTypePropertyAttribute GetPageTypePropertyAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes<PageTypePropertyAttribute>().FirstOrDefault();
        }
    }
}