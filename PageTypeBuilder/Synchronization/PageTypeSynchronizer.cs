using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeSynchronizer
    {
        private IPageTypeLocator _pageTypeLocator;
        private IEnumerable<PageTypeDefinition> _pageTypeDefinitions;
        private PageTypeBuilderConfiguration _configuration;
        private GlobalPropertySettingsSynchronizer globalPropertySettingsSynchronizer;

        public PageTypeSynchronizer(IPageTypeDefinitionLocator pageTypeDefinitionLocator, 
            PageTypeBuilderConfiguration configuration, 
            PageTypePropertyUpdater pageTypePropertyUpdater,
            PageTypeDefinitionValidator pageTypeDefinitionValidator,
            PageTypeResolver pageTypeResolver,
            IPageTypeLocator pageTypeLocator,
            PageTypeUpdater pageTypeUpdater,
            TabDefinitionUpdater tabDefinitionUpdater,
            TabLocator tabLocator,
            GlobalPropertySettingsSynchronizer globalPropertySettingsSynchronizer)
        {
            _configuration = configuration;
            PageTypeResolver = pageTypeResolver;
            TabLocator = tabLocator;
            TabDefinitionUpdater = tabDefinitionUpdater;
            _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            PageTypeUpdater = pageTypeUpdater;
            PageTypePropertyUpdater = pageTypePropertyUpdater;
            PageTypeDefinitionValidator = pageTypeDefinitionValidator;
            _pageTypeLocator = pageTypeLocator;
            this.globalPropertySettingsSynchronizer = globalPropertySettingsSynchronizer;
        }

        internal void SynchronizePageTypes()
        {
            if (!_configuration.DisablePageTypeUpdation)
            {
                UpdateTabDefinitions();
                globalPropertySettingsSynchronizer.Synchronize();
            }

            IEnumerable<PageTypeDefinition> pageTypeDefinitions = _pageTypeDefinitions;

            ValidatePageTypeDefinitions(pageTypeDefinitions);

            if (!_configuration.DisablePageTypeUpdation)
                CreateNonExistingPageTypes(pageTypeDefinitions);

            if (_configuration.DisablePageTypeUpdation)
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
            {
                PageTypeUpdater.CreateNewPageType(definition);
            }
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
                PageTypeResolver.AddPageType(pageType.ID, definition.Type);
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
                PageTypePropertyUpdater.UpdatePageTypePropertyDefinitions(pageType, definition);
            }
        }
        
        protected internal virtual PageTypeResolver PageTypeResolver { get; set; }

        protected internal virtual TabLocator TabLocator { get; set; }

        protected internal virtual TabDefinitionUpdater TabDefinitionUpdater { get; set; }

        protected internal virtual PageTypeUpdater PageTypeUpdater { get; set; }

        protected internal virtual PageTypePropertyUpdater PageTypePropertyUpdater { get; set; }

        protected internal virtual PageTypeDefinitionValidator PageTypeDefinitionValidator { get; set; }
    }
}