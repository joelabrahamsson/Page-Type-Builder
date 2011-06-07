using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeLocator : IPageTypeLocator
    {
        private IPageTypeRepository pageTypeRepository;

        public PageTypeLocator(IPageTypeRepository pageTypeRepository)
        {
            this.pageTypeRepository = pageTypeRepository;
        }

        public virtual IPageType GetExistingPageType(PageTypeDefinition definition)
        {
            IPageType existingPageType = null;
            Type type = definition.Type;
            PageTypeAttribute attribute = definition.Attribute;
            if (attribute.Guid.HasValue)
            {
                existingPageType = pageTypeRepository.Load(attribute.Guid.Value);
            }

            if (existingPageType == null && attribute.Name != null)
            {
                existingPageType = pageTypeRepository.Load(attribute.Name);
            }

            if (existingPageType == null)
            {
                existingPageType = pageTypeRepository.Load(type.Name);
            }

            return existingPageType;
        }
    }
}
