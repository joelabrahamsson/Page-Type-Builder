using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeUpdater
    {
        public const string DefaultPageTypeFilename = "~/default.aspx";
        internal const int DefaultDefaultPageTypePeerOrder = -1;
        internal const bool DefaultDefaultVisibleInMenu = true;

        private IPageTypeLocator _pageTypeLocator;
        private IEnumerable<PageTypeDefinition> _pageTypeDefinitions;
        private IPageTypeValueExtractor _pageTypeValueExtractor;
        internal List<IPageType> NewlyCreatedPageTypes;

        public PageTypeUpdater(IPageTypeDefinitionLocator pageTypeDefinitionLocator, 
            IPageTypeRepository pageTypeRepository, 
            IPageTypeValueExtractor pageTypeValueExtractor,
            IPageTypeLocator pageTypeLocator)
        {
            _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            PageTypeRepository = pageTypeRepository;
            DefaultFilename = DefaultPageTypeFilename;
            _pageTypeValueExtractor = pageTypeValueExtractor;
            _pageTypeLocator = pageTypeLocator;
            NewlyCreatedPageTypes = new List<IPageType>();
        }

        protected internal virtual IPageType GetExistingPageType(PageTypeDefinition definition)
        {
            return _pageTypeLocator.GetExistingPageType(definition);
        }

        protected internal virtual IPageType CreateNewPageType(PageTypeDefinition definition)
        {
            IPageType pageType = PageTypeRepository.CreateNew();

            PageTypeAttribute attribute = definition.Attribute;

            string name = attribute.Name;
            if (name == null)
            {
                name = definition.Type.Name;
            }
            pageType.Name = name;
            
            if (definition.Attribute.Guid.HasValue)
                pageType.GUID = definition.Attribute.Guid.Value;

            string filename = attribute.Filename;
            if (string.IsNullOrEmpty(filename))
            {
                filename = DefaultFilename;
            }
            pageType.FileName = filename;
            
            PageTypeRepository.Save(pageType);

            NewlyCreatedPageTypes.Add(pageType);
            return pageType;
        }

        protected internal virtual void UpdatePageType(PageTypeDefinition definition)
        {
            IPageType pageType = GetExistingPageType(definition);
            PageTypeAttribute attribute = definition.Attribute;
            string oldValueString = SerializeValues(pageType);
            UpdateName(pageType, definition);
            UpdateFilename(pageType, attribute);
            UpdateSortOrder(pageType, attribute);
            UpdateDescription(pageType, attribute);
            UpdateIsAvailable(pageType, attribute);
            UpdateDefaultArchivePageLink(pageType, attribute);
            UpdateDefaultChildOrderRule(pageType, attribute);
            UpdateDefaultPageName(pageType, attribute);
            UpdateDefaultPeerOrder(pageType, attribute);
            UpdateDefaultStartPublishOffset(pageType, attribute);
            UpdateDefaultStopPublishOffset(pageType, attribute);
            UpdateDefaultVisibleInMenu(pageType, attribute);
            UpdateFrame(pageType, attribute);

            bool availablePageTypesSet = false;

            if (CanModifyProperty(pageType, attribute.AvailablePageTypesSet))
            {
                UpdateAvailablePageTypes(pageType, attribute.AvailablePageTypes);

                if (attribute.AvailablePageTypes != null && attribute.AvailablePageTypes.Length > 0)
                    availablePageTypesSet = true;
            }

            if (!availablePageTypesSet && CanModifyProperty(pageType, attribute.ExcludedPageTypesSet) && attribute.ExcludedPageTypes != null)
                UpdateAvailablePageTypesExcluded(pageType, attribute.ExcludedPageTypes);

            string newValuesString = SerializeValues(pageType);

            if (newValuesString != oldValueString)
                PageTypeRepository.Save(pageType);
        }

        protected internal virtual string SerializeValues(IPageType pageType)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(pageType.Name);
            builder.Append("|");
            builder.Append(_pageTypeValueExtractor.GetFileName(pageType));
            builder.Append("|");
            builder.Append(pageType.SortOrder);
            builder.Append("|");
            builder.Append(pageType.Description);
            builder.Append("|");
            builder.Append(pageType.IsAvailable);
            builder.Append("|");
            builder.Append(pageType.DefaultArchivePageLink);
            builder.Append("|");
            builder.Append(pageType.DefaultChildOrderRule);
            builder.Append("|");
            builder.Append(pageType.DefaultPageName);
            builder.Append("|");
            builder.Append(pageType.DefaultPeerOrder);
            builder.Append("|");
            builder.Append(pageType.DefaultStartPublishOffset);
            builder.Append("|");
            builder.Append(pageType.DefaultStopPublishOffset);
            builder.Append("|");
            builder.Append(pageType.DefaultVisibleInMenu);
            builder.Append("|");
            builder.Append(_pageTypeValueExtractor.GetDefaultFrameId(pageType));
            builder.Append("|");
            foreach (int pageTypeID in pageType.AllowedPageTypes.OrderBy(id => id))
            {
                builder.Append(pageTypeID);
                builder.Append("||");
            }

            return builder.ToString();
        }

        private bool CanModifyProperty(IPageType pageType, bool propertySet)
        {
            return NewlyCreatedPageTypes.Any(current => current.GUID.Equals(pageType.GUID)) || propertySet;
        }

        protected internal virtual void UpdateName(IPageType pageType, PageTypeDefinition definition)
        {
            pageType.Name = definition.GetPageTypeName();
        }

        protected internal virtual void UpdateFilename(IPageType pageType, PageTypeAttribute attribute)
        {
            bool setFileName = false;

            if (string.IsNullOrEmpty(pageType.FileName))
                setFileName = true;

            if (!CanModifyProperty(pageType, attribute.FilenameSet) && !setFileName)
                return;

            string filename = GetFilename(attribute);
            pageType.FileName = filename;
        }

        private string GetFilename(PageTypeAttribute attribute)
        {
            string filename = attribute.Filename;
            if (string.IsNullOrEmpty(filename))
            {
                filename = DefaultFilename;
            }
            return filename;
        }

        protected internal virtual void UpdateSortOrder(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.SortOrderSet))
                return;

            pageType.SortOrder = attribute.SortOrder;
        }

        protected internal virtual void UpdateDescription(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DescriptionSet))
                return;

            pageType.Description = attribute.Description;
        }

        protected internal virtual void UpdateIsAvailable(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.AvailableInEditModeSet))
                return;

            pageType.IsAvailable = attribute.AvailableInEditMode;
        }

        protected internal virtual void UpdateDefaultArchivePageLink(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultArchiveToPageIDSet))
                return;

            if (attribute.DefaultArchiveToPageID != PageTypeAttribute.DefaultDefaultArchiveToPageID)
                pageType.DefaultArchivePageLink = new PageReference(attribute.DefaultArchiveToPageID);
            else
                pageType.DefaultArchivePageLink = null;
        }

        protected internal virtual void UpdateDefaultChildOrderRule(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultChildSortOrderSet))
                return;

            pageType.DefaultChildOrderRule = attribute.DefaultChildSortOrder;
        }

        protected internal virtual void UpdateDefaultPageName(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultPageNameSet))
                return;

            pageType.DefaultPageName = attribute.DefaultPageName;
        }

        protected internal virtual void UpdateDefaultPeerOrder(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultSortIndexSet))
                return;

            if (attribute.DefaultSortIndex != PageTypeAttribute.DefaultDefaultSortIndex)
                pageType.DefaultPeerOrder = attribute.DefaultSortIndex;
            else
                pageType.DefaultPeerOrder = DefaultDefaultPageTypePeerOrder;
        }

        protected internal virtual void UpdateDefaultStartPublishOffset(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultStartPublishOffsetMinutesSet))
                return;

            pageType.DefaultStartPublishOffset = new TimeSpan(0, 0, attribute.DefaultStartPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultStopPublishOffset(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultStopPublishOffsetMinutesSet))
                return;

            pageType.DefaultStopPublishOffset = new TimeSpan(0, 0, attribute.DefaultStopPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultVisibleInMenu(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultVisibleInMenuSet))
                return;

            if (attribute.DefaultVisibleInMenu != PageTypeAttribute.DefaultDefaultVisibleInMenu)
                pageType.DefaultVisibleInMenu = attribute.DefaultVisibleInMenu;
            else
                pageType.DefaultVisibleInMenu = DefaultDefaultVisibleInMenu;
        }

        protected internal virtual void UpdateFrame(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.DefaultFrameIDSet))
                return;

            if (attribute.DefaultFrameID != 0)
                pageType.DefaultFrameID = attribute.DefaultFrameID;
        }

        protected internal virtual void UpdateAvailablePageTypes(IPageType pageType, Type[] availablePageTypeTypes)
        {
            if (availablePageTypeTypes == null)
            {
                pageType.AllowedPageTypes = null;
                return;
            }
            
            List<int> availablePageTypeIds = new List<int>();

            foreach (Type availablePageTypeType in availablePageTypeTypes)
            {
                Type currentAvailablePageTypeType = availablePageTypeType;

                if (IsValidPageType(currentAvailablePageTypeType))
                {
                    IPageType availablePageType = GetExistingPageType(_pageTypeDefinitions.
                        First(definition => definition.Type.GUID == currentAvailablePageTypeType.GUID));

                    if (!availablePageTypeIds.Contains(availablePageType.ID))
                        availablePageTypeIds.Add(availablePageType.ID);
                }
                else if (currentAvailablePageTypeType.IsSubclassOf(typeof(TypedPageData)))
                {
                    availablePageTypeIds.AddRange(GetPageDefinitionsThatInheritFromType(_pageTypeDefinitions, currentAvailablePageTypeType)
                        .Where(id => !availablePageTypeIds.Contains(id)));
                }
                else if (currentAvailablePageTypeType.IsInterface)
                {
                    availablePageTypeIds.AddRange(GetPageDefinitionsThatImplementInteface(_pageTypeDefinitions, currentAvailablePageTypeType)
                        .Where(id => !availablePageTypeIds.Contains(id)));
                }
            }

            pageType.AllowedPageTypes = availablePageTypeIds.ToArray();
        }

        protected internal void UpdateAvailablePageTypesExcluded(IPageType pageType, Type[] excludedPageTypeTypes)
        {
            if (excludedPageTypeTypes == null || excludedPageTypeTypes.Length == 0)
            {
                pageType.AllowedPageTypes = null;
                return;
            }
            List<int> availablePageTypeIds = PageTypeRepository.List().Select(currentPageType => currentPageType.ID).ToList();

            foreach (Type excludedPageTypeType in excludedPageTypeTypes)
            {
                Type currentExcludedPageTypeType = excludedPageTypeType;

                if (IsValidPageType(currentExcludedPageTypeType))
                {
                    IPageType availablePageType = GetExistingPageType(_pageTypeDefinitions.
                        First(definition => definition.Type.GUID == currentExcludedPageTypeType.GUID));

                    if (availablePageTypeIds.Contains(availablePageType.ID))
                        availablePageTypeIds.Remove(availablePageType.ID);
                }
                else if (currentExcludedPageTypeType.IsSubclassOf(typeof(TypedPageData)))
                {
                    foreach (int pageTypeId in GetPageDefinitionsThatInheritFromType(_pageTypeDefinitions, currentExcludedPageTypeType).Where(availablePageTypeIds.Contains))
                        availablePageTypeIds.Remove(pageTypeId);
                }
                else if (currentExcludedPageTypeType.IsInterface)
                {
                    foreach (int pageTypeId in GetPageDefinitionsThatImplementInteface(_pageTypeDefinitions, currentExcludedPageTypeType).Where(availablePageTypeIds.Contains))
                        availablePageTypeIds.Remove(pageTypeId);
                }
            }

            pageType.AllowedPageTypes = availablePageTypeIds.ToArray();
        }

        private IEnumerable<int> GetPageDefinitionsThatInheritFromType(IEnumerable<PageTypeDefinition> pageTypeDefinitions, Type subClassType)
        {
            return pageTypeDefinitions
                .Where(definition => definition.Type.IsSubclassOf(subClassType))
                .Select(definition => GetExistingPageType(definition).ID);
        }

        private IEnumerable<int> GetPageDefinitionsThatImplementInteface(IEnumerable<PageTypeDefinition> pageTypeDefinitions, Type interfaceType)
        {
            return pageTypeDefinitions
                .Where(definition => definition.Type.GetInterfaces().Any(currentInterface => currentInterface.Equals(interfaceType)))
                .Select(definition => GetExistingPageType(definition).ID);
        }
        
        protected internal virtual bool IsValidPageType(Type type)
        {
            IEnumerable<Type> types = new List<Type> { type };
            return types.WithAttribute<PageTypeAttribute>().Count() > 0 && !type.IsAbstract;
        }

        public IPageTypeRepository PageTypeRepository { get; set; }
        internal string DefaultFilename { get; set; }
    }
}