using System;
using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Discovery
{
    public class PageTypeDefinitionLocator
    {
        private List<PageTypeDefinition> _pageTypeDefinitions;

        public virtual List<PageTypeDefinition> GetPageTypeDefinitions()
        {
            List<Type> pageTypes = AttributedTypesUtility.GetTypesWithAttribute(typeof(PageTypeAttribute));
            pageTypes = pageTypes.Where(type => !type.IsAbstract).ToList();

            List<PageTypeDefinition> pageTypeDefinitions = new List<PageTypeDefinition>();
            foreach (Type type in pageTypes)
            {
                PageTypeAttribute pageTypeAttribute = (PageTypeAttribute)AttributedTypesUtility.GetAttribute(type, typeof(PageTypeAttribute));
                PageTypeDefinition definition = new PageTypeDefinition
                                                    {
                                                        Type = type,
                                                        Attribute = pageTypeAttribute
                                                    };
                pageTypeDefinitions.Add(definition);
            }

            _pageTypeDefinitions = pageTypeDefinitions;

            return _pageTypeDefinitions;
        }
    }
}