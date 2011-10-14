namespace PageTypeBuilder.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    using Reflection;

    public class PageTypePropertyDefinitionLocator
    {
        public virtual IEnumerable<PageTypePropertyDefinition> GetPageTypePropertyDefinitions(IPageType pageType, Type pageTypeType)
        {
            var properties = pageTypeType.GetPageTypePropertiesOnClass();

            List<PageTypePropertyDefinition> pageTypePropertyDefinitions = new List<PageTypePropertyDefinition>();
            foreach (PropertyInfo property in properties)
            {
                PageTypePropertyAttribute attribute = GetPageTypePropertyAttribute(property);

                if (attribute == null)
                    continue; 

                pageTypePropertyDefinitions.Add(new PageTypePropertyDefinition(property.Name, property.PropertyType, pageType, attribute));
            }

            // add all page type group property definitions
            properties = pageTypeType.GetPageTypePropertyGroupProperties();

            foreach (PropertyInfo property in properties.Where(property => property.PropertyType.BaseType == typeof(PageTypePropertyGroup)))
            {
                PageTypePropertyGroupAttribute pageTypePropertyGroupAttribute = GetPropertyAttribute<PageTypePropertyGroupAttribute>(property);
                List<PageTypePropertyDefinition> definitions = GetPropertyGroupPropertyDefinitions(pageType, property).ToList();


                for (int i = 0; i < definitions.Count; i++)
                {
                    definitions[i].PageTypePropertyAttribute.Tab = pageTypePropertyGroupAttribute.Tab;
                }

                pageTypePropertyDefinitions.AddRange(definitions);
            }

            return pageTypePropertyDefinitions;
        }

        internal PageTypePropertyAttribute GetPageTypePropertyAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes<PageTypePropertyAttribute>().FirstOrDefault();
        }

        internal T GetPropertyAttribute<T>(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes<T>().FirstOrDefault();
        }

        private IEnumerable<PageTypePropertyDefinition> GetPropertyGroupPropertyDefinitions(IPageType pageType, PropertyInfo propertyGroupProperty)
        {
            PageTypePropertyGroupAttribute groupAttribute = GetPropertyAttribute<PageTypePropertyGroupAttribute>(propertyGroupProperty);
            PropertyInfo[] propertyGroupProperties = propertyGroupProperty.PropertyType.GetPublicOrPrivateProperties();

            foreach (PropertyInfo property in propertyGroupProperties)
            {
                PageTypePropertyAttribute attribute = GetPropertyAttribute<PageTypePropertyAttribute>(property);

                if (attribute == null)
                    continue;

                string resolvedPropertyName = PageTypePropertyGroupHierarchy.ResolvePropertyName(propertyGroupProperty.Name, property.Name);
                attribute = AdjustPropertyGroupAttributeProperties(attribute, groupAttribute);

                yield return new PageTypePropertyDefinition(resolvedPropertyName, property.PropertyType, pageType, attribute);
            }
        }

        private PageTypePropertyAttribute AdjustPropertyGroupAttributeProperties(PageTypePropertyAttribute attribute, PageTypePropertyGroupAttribute groupAttribute)
        {
            if (groupAttribute != null)
            {
                if (!string.IsNullOrEmpty(groupAttribute.EditCaptionPrefix))
                    attribute.EditCaption = groupAttribute.EditCaptionPrefix + attribute.EditCaption;

                if (groupAttribute.StartSortOrderFrom > 0)
                    attribute.SortOrder = groupAttribute.StartSortOrderFrom + attribute.SortOrder;
            }
            return attribute;
        }
    }
}