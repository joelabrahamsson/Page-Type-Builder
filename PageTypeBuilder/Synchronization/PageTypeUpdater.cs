using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

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
        private List<IPageType> _newlyCreatedPageTypes;

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
            _newlyCreatedPageTypes = new List<IPageType>();
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

            _newlyCreatedPageTypes.Add(pageType);
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

            if (CanModifyProperty(pageType, attribute.AvailablePageTypesSet))
                UpdateAvailablePageTypes(pageType, attribute.AvailablePageTypes);
            
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
            return _newlyCreatedPageTypes.Any(current => current.GUID.Equals(pageType.GUID)) || propertySet;
        }

        protected internal virtual void UpdateName(IPageType pageType, PageTypeDefinition definition)
        {
            pageType.Name = definition.GetPageTypeName();
        }

        protected internal virtual void UpdateFilename(IPageType pageType, PageTypeAttribute attribute)
        {
            if (!CanModifyProperty(pageType, attribute.FilenameSet))
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

        protected internal virtual void UpdateAvailablePageTypes(IPageType pageType, Type[] availablePageTypes)
        {
            if (availablePageTypes == null)
            {
                pageType.AllowedPageTypes = null;
                return;
            }
            int[] availablePageTypeIDs = new int[availablePageTypes.Length];
            for (int i = 0; i < availablePageTypes.Length; i++)
            {
                Type availablePageTypeType = availablePageTypes[i];
                PageTypeDefinition availablePageTypeDefinition = _pageTypeDefinitions.First(
                    definitions => definitions.Type.GUID == availablePageTypeType.GUID);
                IPageType availablePageType = GetExistingPageType(availablePageTypeDefinition);
                availablePageTypeIDs[i] = availablePageType.ID;
            }
            pageType.AllowedPageTypes = availablePageTypeIDs;
        }

        public IPageTypeRepository PageTypeRepository { get; set; }
        internal string DefaultFilename { get; set; }
    }
}