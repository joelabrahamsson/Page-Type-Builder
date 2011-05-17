using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeLocator : IPageTypeLocator
    {
        private IPageTypeFactory _pageTypeFactory;

        public PageTypeLocator(IPageTypeFactory pageTypeFactory)
        {
            _pageTypeFactory = pageTypeFactory;
        }

        public virtual IPageType GetExistingPageType(PageTypeDefinition definition)
        {
            IPageType existingPageType = null;
            Type type = definition.Type;
            PageTypeAttribute attribute = definition.Attribute;
            if (attribute.Guid.HasValue)
            {
                existingPageType = _pageTypeFactory.Load(attribute.Guid.Value);
            }

            if (existingPageType == null && attribute.Name != null)
            {
                existingPageType = _pageTypeFactory.Load(attribute.Name);
            }

            if (existingPageType == null)
            {
                existingPageType = _pageTypeFactory.Load(type.Name);
            }

            return existingPageType;
        }
    }
}
