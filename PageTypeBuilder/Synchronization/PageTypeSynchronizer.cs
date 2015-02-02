using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization.Hooks;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeSynchronizer
    {
        private IPageTypeLocator _pageTypeLocator;
        private IEnumerable<PageTypeDefinition> _pageTypeDefinitions;
        private PageTypeBuilderConfiguration _configuration;
        private GlobalPropertySettingsSynchronizer globalPropertySettingsSynchronizer;
        private IHooksHandler hooksHandler;

        public PageTypeSynchronizer(IPageTypeDefinitionLocator pageTypeDefinitionLocator, 
            PageTypeBuilderConfiguration configuration, 
            PageDefinitionSynchronizationEngine pageDefinitionSynchronizationEngine,
            PageTypeDefinitionValidator pageTypeDefinitionValidator,
            PageTypeResolver pageTypeResolver,
            IPageTypeLocator pageTypeLocator,
            PageTypeUpdater pageTypeUpdater,
            TabDefinitionUpdater tabDefinitionUpdater,
            TabLocator tabLocator,
            GlobalPropertySettingsSynchronizer globalPropertySettingsSynchronizer,
            IHooksHandler hooksHandler)
        {
            _configuration = configuration;
            this.pageTypeResolver = pageTypeResolver;
            TabLocator = tabLocator;
            TabDefinitionUpdater = tabDefinitionUpdater;
            _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            PageTypeUpdater = pageTypeUpdater;
            PageDefinitionSynchronizationEngine = pageDefinitionSynchronizationEngine;
            PageTypeDefinitionValidator = pageTypeDefinitionValidator;
            _pageTypeLocator = pageTypeLocator;
            this.globalPropertySettingsSynchronizer = globalPropertySettingsSynchronizer;
            this.hooksHandler = hooksHandler;
        }

        internal void SynchronizePageTypes(bool forceDisablePageTypeUpdates = false)
        {
            hooksHandler.InvokePreSynchronizationHooks();

            if (!forceDisablePageTypeUpdates && !_configuration.DisablePageTypeUpdation)
            {
                UpdateTabDefinitions();
                globalPropertySettingsSynchronizer.Synchronize();
            }

            IEnumerable<PageTypeDefinition> pageTypeDefinitions = _pageTypeDefinitions;

            ValidatePageTypeDefinitions(pageTypeDefinitions);

            if (!forceDisablePageTypeUpdates && !_configuration.DisablePageTypeUpdation)
                CreateNonExistingPageTypes(pageTypeDefinitions);

            if (forceDisablePageTypeUpdates || _configuration.DisablePageTypeUpdation)
            {
                IEnumerable<PageTypeDefinition> nonExistingPageTypes = GetNonExistingPageTypes(pageTypeDefinitions);
                pageTypeDefinitions = pageTypeDefinitions.Except(nonExistingPageTypes).ToList();
            }
            else
            {
                UpdatePageTypes(pageTypeDefinitions);
                UpdatePageTypePropertyDefinitions(pageTypeDefinitions);
            }

            AddPageTypesToResolver(pageTypeDefinitions);
        }

        protected internal virtual void UpdateTabDefinitions()
        {
            IEnumerable<Tab> definedTabs = TabLocator.GetDefinedTabs();
            TabDefinitionUpdater.UpdateTabDefinitions(definedTabs);
        }

        protected internal virtual void ValidatePageTypeDefinitions(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            PageTypeDefinitionValidator.ValidatePageTypeDefinitions(pageTypeDefinitions);
        }

        protected internal virtual void CreateNonExistingPageTypes(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            IEnumerable<PageTypeDefinition> nonExistingPageTypes = GetNonExistingPageTypes(pageTypeDefinitions);

            foreach (PageTypeDefinition definition in nonExistingPageTypes)
                PageTypeUpdater.CreateNewPageType(definition);
        }

        protected internal virtual IEnumerable<PageTypeDefinition> GetNonExistingPageTypes(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            return pageTypeDefinitions.Where(definition => _pageTypeLocator.GetExistingPageType(definition) == null);
        }

        protected internal virtual void AddPageTypesToResolver(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                IPageType pageType = _pageTypeLocator.GetExistingPageType(definition);
                pageTypeResolver.AddPageType(pageType.ID, definition.Type);
            }
        }

        protected internal virtual void UpdatePageTypes(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                PageTypeUpdater.UpdatePageType(definition);
            }
        }

        protected internal virtual void UpdatePageTypePropertyDefinitions(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                IPageType pageType = _pageTypeLocator.GetExistingPageType(definition);
                PageDefinitionSynchronizationEngine.UpdatePageTypePropertyDefinitions(pageType, definition);
            }
        }

        private PageTypeResolver pageTypeResolver;

        protected internal virtual TabLocator TabLocator { get; set; }

        protected internal virtual TabDefinitionUpdater TabDefinitionUpdater { get; set; }

        protected internal virtual PageTypeUpdater PageTypeUpdater { get; set; }

        protected internal virtual PageDefinitionSynchronizationEngine PageDefinitionSynchronizationEngine { get; set; }

        protected internal virtual PageTypeDefinitionValidator PageTypeDefinitionValidator { get; set; }
    }
}