namespace PageTypeBuilder.Synchronization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Abstractions;
    using Configuration;
    using Discovery;
    using Hooks;
    using PageDefinitionSynchronization;
    using Validation;

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

            using (new TimingsLogger("Getting page type definitions"))
            {
                _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            }

            PageTypeUpdater = pageTypeUpdater;
            PageDefinitionSynchronizationEngine = pageDefinitionSynchronizationEngine;
            PageTypeDefinitionValidator = pageTypeDefinitionValidator;
            _pageTypeLocator = pageTypeLocator;
            this.globalPropertySettingsSynchronizer = globalPropertySettingsSynchronizer;
            this.hooksHandler = hooksHandler;
        }


        public void SynchronizePageTypes()
        {
            SynchronizePageTypes(false);
        }

        public void SynchronizePageTypes(bool forcePageTypeUpdation)
        {
            SynchronizationHelper.AssemblyLocator = TabLocator.AssemblyLocator;
            bool disablePageTypeUpdation = _configuration.DisablePageTypeUpdation;

            if (forcePageTypeUpdation)
                disablePageTypeUpdation = false;

            bool oneTimeSynchornizationEnabled = !disablePageTypeUpdation && SynchronizationHelper.OneTimeSynchornizationEnabled;
            bool iAmSynching = true;
            
            if (oneTimeSynchornizationEnabled)
            {
                SynchronizationHelper.ClearLogInfo();
                bool isBeingSynchronized = SynchronizationHelper.IsBeingSynchronized(out iAmSynching);

                if (isBeingSynchronized && !iAmSynching)
                {
                    using (new TimingsLogger("Waiting for synchornization to finish on synching site."))
                    {
                        while (SynchronizationHelper.IsBeingSynchronized(out iAmSynching) && !iAmSynching)
                            Thread.Sleep(_configuration.OneTimeSynchornizationPollTime);
                    }
                }
            }

            try
            {
                using (new TimingsLogger("Pre synchoronization hooks"))
                {
                    if (iAmSynching)
                        hooksHandler.InvokePreSynchronizationHooks();
                }

                if (!disablePageTypeUpdation && iAmSynching)
                {
                    using (new TimingsLogger("updating tab definitions"))
                    {
                        UpdateTabDefinitions();
                    }

                    using (new TimingsLogger("updating global property settings"))
                    {
                        globalPropertySettingsSynchronizer.Synchronize();
                    }
                }

                IEnumerable<PageTypeDefinition> pageTypeDefinitions = _pageTypeDefinitions.ToList();

                if (!disablePageTypeUpdation && _configuration.PerformValidation && iAmSynching)
                {
                    using (new TimingsLogger("Validating page type definitions"))
                    {
                        ValidatePageTypeDefinitions(pageTypeDefinitions);
                    }
                }

                if (!disablePageTypeUpdation && iAmSynching)
                {
                    using (new TimingsLogger("Creating non existing page types"))
                    {
                        CreateNonExistingPageTypes(pageTypeDefinitions);
                    }
                }

                if (disablePageTypeUpdation || !iAmSynching)
                {
                    using (new TimingsLogger("Getting non existing page types"))
                    {
                        IEnumerable<PageTypeDefinition> nonExistingPageTypes = GetNonExistingPageTypes(pageTypeDefinitions);
                        pageTypeDefinitions = pageTypeDefinitions.Except(nonExistingPageTypes).ToList();
                    }
                }
                else
                {
                    using (new TimingsLogger("Updating page types"))
                    {
                        UpdatePageTypes(pageTypeDefinitions);
                    }

                    using (new TimingsLogger("Updating page type property definitions"))
                    {
                        UpdatePageTypePropertyDefinitions(pageTypeDefinitions);
                    }
                }

                using (new TimingsLogger("Adding page types to resolver"))
                {
                    AddPageTypesToResolver(pageTypeDefinitions);
                }

                if (oneTimeSynchornizationEnabled && iAmSynching)
                {
                    SynchronizationHelper.UpateSynchronizationCache(PageTypeUpdater.UpdatedPageTypeIds, 
                        PageDefinitionSynchronizationEngine.PageDefinitionUpdater.UpdatedPageDefinitions,
                        TabDefinitionUpdater.updatedTabIds,
                        PageDefinitionSynchronizationEngine.PageDefinitionSpecificPropertySettingsUpdater.updatedPropertySettingsCacheKeys,
                        globalPropertySettingsSynchronizer.globalSettingsIds);

                    SynchronizationHelper.SynchingComplete();
                }

                if (oneTimeSynchornizationEnabled && !iAmSynching)
                    SynchronizationHelper.InvalidateCache();
            }
            catch (Exception)
            {
                if (oneTimeSynchornizationEnabled && iAmSynching)
                    SynchronizationHelper.RevertSyncronizationStatus();

                throw;
            }
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
            IEnumerable<PageTypeDefinition> nonExistingPageTypes;
            
            using (new TimingsLogger("GetNonExistingPageTypes"))
            {
                nonExistingPageTypes = GetNonExistingPageTypes(pageTypeDefinitions).ToList();
            }

            using (new TimingsLogger("PageTypeUpdater.CreateNewPageTypes"))
            {
                foreach (PageTypeDefinition definition in nonExistingPageTypes)
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