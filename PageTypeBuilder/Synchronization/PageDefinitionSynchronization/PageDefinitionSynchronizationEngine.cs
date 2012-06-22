using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    public class PageDefinitionSynchronizationEngine
    {
        private PageTypePropertyDefinitionLocator pageTypePropertyDefinitionLocator;
        private IPageDefinitionUpdater pageDefinitionUpdater;
        PageDefinitionSpecificPropertySettingsUpdater pageDefinitionSpecificPropertySettingsUpdater;
        IPageTypeRepository pageTypeRepository;

        internal IPageDefinitionUpdater PageDefinitionUpdater
        {
            get { return pageDefinitionUpdater; }
        }

        internal PageDefinitionSpecificPropertySettingsUpdater PageDefinitionSpecificPropertySettingsUpdater
        {
            get { return pageDefinitionSpecificPropertySettingsUpdater; }
        }

        public PageDefinitionSynchronizationEngine(
            IPageDefinitionUpdater pageDefinitionUpdater,
            PageTypePropertyDefinitionLocator pageTypePropertyDefinitionLocator,
            PageDefinitionSpecificPropertySettingsUpdater pageDefinitionSpecificPropertySettingsUpdater,
            IPageTypeRepository pageTypeRepository)
        {
            this.pageTypePropertyDefinitionLocator = pageTypePropertyDefinitionLocator;
            this.pageDefinitionUpdater = pageDefinitionUpdater;
            this.pageDefinitionSpecificPropertySettingsUpdater = pageDefinitionSpecificPropertySettingsUpdater;
            this.pageTypeRepository = pageTypeRepository;
        }

        protected internal virtual void UpdatePageTypePropertyDefinitions(IPageType pageType, PageTypeDefinition pageTypeDefinition)
        {
            IEnumerable<PageTypePropertyDefinition> definitions = 
                pageTypePropertyDefinitionLocator.GetPageTypePropertyDefinitions(pageType, pageTypeDefinition.Type);

            foreach (PageTypePropertyDefinition propertyDefinition in definitions)
            {
                PageDefinition pageDefinition = GetExistingPageDefinition(pageType, propertyDefinition);
             
                if (pageDefinition == null)
                {
                    using (new TimingsLogger(string.Format("Creating new page definition '{0}' for page type {1}: ", propertyDefinition.Name, pageType.Name)))
                    {
                        pageDefinitionUpdater.CreateNewPageDefinition(propertyDefinition);
                        pageDefinition = GetExistingPageDefinition(pageType, propertyDefinition);
                    }
                }
                else
                {
                    pageDefinitionUpdater.UpdateExistingPageDefinition(pageDefinition, propertyDefinition);
                }

                pageDefinitionSpecificPropertySettingsUpdater.UpdatePropertySettings(pageTypeDefinition, propertyDefinition, pageDefinition);
            }
        }

        protected virtual PageDefinition GetExistingPageDefinition(IPageType pageType, PageTypePropertyDefinition propertyDefinition)
        {
            var definitions = pageTypeRepository.Load(pageType.ID).Definitions;
            return definitions.FirstOrDefault(definition => string.Equals(definition.Name, propertyDefinition.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}