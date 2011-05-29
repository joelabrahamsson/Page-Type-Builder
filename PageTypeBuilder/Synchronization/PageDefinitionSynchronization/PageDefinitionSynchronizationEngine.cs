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

        public PageDefinitionSynchronizationEngine(
            IPageDefinitionUpdater pageDefinitionUpdater,
            PageTypePropertyDefinitionLocator pageTypePropertyDefinitionLocator,
            PageDefinitionSpecificPropertySettingsUpdater pageDefinitionSpecificPropertySettingsUpdater)
        {
            this.pageTypePropertyDefinitionLocator = pageTypePropertyDefinitionLocator;
            this.pageDefinitionUpdater = pageDefinitionUpdater;
            this.pageDefinitionSpecificPropertySettingsUpdater = pageDefinitionSpecificPropertySettingsUpdater;
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
                    pageDefinitionUpdater.CreateNewPageDefinition(propertyDefinition);
                    pageDefinition = GetExistingPageDefinition(pageType, propertyDefinition);
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
            return pageType.Definitions.FirstOrDefault(definition => definition.Name == propertyDefinition.Name);
        }
    }
}