using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeSynchronizer
    {
        private IPageTypeLocator _pageTypeLocator;
        private List<PageTypeDefinition> _pageTypeDefinitions;
        private PageTypeBuilderConfiguration _configuration;

        public PageTypeSynchronizer(IPageTypeDefinitionLocator pageTypeDefinitionLocator, PageTypeBuilderConfiguration configuration)
            : this(pageTypeDefinitionLocator, configuration, new PageTypeFactory(), new PageTypePropertyUpdater(), new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())), new PageTypeValueExtractor(), PageTypeResolver.Instance, new PageTypeLocator(new PageTypeFactory())) { }

        public PageTypeSynchronizer(IPageTypeDefinitionLocator pageTypeDefinitionLocator, PageTypeBuilderConfiguration configuration, PageTypeFactory pageTypeFactory)
            : this(pageTypeDefinitionLocator, configuration, pageTypeFactory, new PageTypePropertyUpdater(), new PageTypeDefinitionValidator(new PageDefinitionTypeMapper(new PageDefinitionTypeFactory())), new PageTypeValueExtractor(), PageTypeResolver.Instance, new PageTypeLocator(pageTypeFactory)) { }

        public PageTypeSynchronizer(IPageTypeDefinitionLocator pageTypeDefinitionLocator, 
            PageTypeBuilderConfiguration configuration, 
            IPageTypeFactory pageTypeFactory,
            PageTypePropertyUpdater pageTypePropertyUpdater,
            PageTypeDefinitionValidator pageTypeDefinitionValidator,
            IPageTypeValueExtractor pageTypeValueExtractor,
            PageTypeResolver pageTypeResolver,
            IPageTypeLocator pageTypeLocator)
        {
            _configuration = configuration;
            PageTypeResolver = pageTypeResolver;
            TabLocator = new TabLocator();
            TabDefinitionUpdater = new TabDefinitionUpdater();
            _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            PageTypeUpdater = new PageTypeUpdater(pageTypeDefinitionLocator, pageTypeFactory, pageTypeValueExtractor, pageTypeLocator);
            PageTypePropertyUpdater = pageTypePropertyUpdater;
            PageTypeDefinitionValidator = pageTypeDefinitionValidator;
            _pageTypeLocator = pageTypeLocator;
        }

        internal void SynchronizePageTypes()
        {
            if (!_configuration.DisablePageTypeUpdation)
                UpdateTabDefinitions();

            List<PageTypeDefinition> pageTypeDefinitions = _pageTypeDefinitions;

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
            List<Tab> definedTabs = TabLocator.GetDefinedTabs();
            TabDefinitionUpdater.UpdateTabDefinitions(definedTabs);
        }

        protected internal virtual void ValidatePageTypeDefinitions(List<PageTypeDefinition> pageTypeDefinitions)
        {
            PageTypeDefinitionValidator.ValidatePageTypeDefinitions(pageTypeDefinitions);
        }

        protected internal virtual void CreateNonExistingPageTypes(List<PageTypeDefinition> pageTypeDefinitions)
        {
            IEnumerable<PageTypeDefinition> nonExistingPageTypes = GetNonExistingPageTypes(pageTypeDefinitions);
            foreach (PageTypeDefinition definition in nonExistingPageTypes)
            {
                PageTypeUpdater.CreateNewPageType(definition);
            }
        }

        protected internal virtual IEnumerable<PageTypeDefinition> GetNonExistingPageTypes(List<PageTypeDefinition> pageTypeDefinitions)
        {
            return pageTypeDefinitions.Where(definition => _pageTypeLocator.GetExistingPageType(definition) == null);
        }

        protected internal virtual void AddPageTypesToResolver(List<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                PageType pageType = _pageTypeLocator.GetExistingPageType(definition);
                PageTypeResolver.AddPageType(pageType.ID, definition.Type);
            }
        }

        protected internal virtual void UpdatePageTypes(List<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                PageTypeUpdater.UpdatePageType(definition);
            }
        }

        protected internal virtual void UpdatePageTypePropertyDefinitions(List<PageTypeDefinition> pageTypeDefinitions)
        {
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                PageType pageType = _pageTypeLocator.GetExistingPageType(definition);
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