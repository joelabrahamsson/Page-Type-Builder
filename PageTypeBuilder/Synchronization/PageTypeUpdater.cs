using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypeUpdater
    {
        public const string DefaultPageTypeFilename = "~/default.aspx";
        internal const int DefaultDefaultPageTypePeerOrder = -1;
        internal const bool DefaultDefaultVisibleInMenu = true;

        private IEnumerable<PageTypeDefinition> _pageTypeDefinitions;

        public PageTypeUpdater(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            _pageTypeDefinitions = pageTypeDefinitions;
            PageTypeFactory = new PageTypeFactory();
            DefaultFilename = DefaultPageTypeFilename;
        }

        public virtual PageType GetExistingPageType(PageTypeDefinition definition)
        {
            PageType existingPageType = null;
            Type type = definition.Type;
            PageTypeAttribute attribute = definition.Attribute;
            if (attribute.Guid.HasValue)
            {
                existingPageType = PageTypeFactory.Load(attribute.Guid.Value);
            }

            if (existingPageType == null && attribute.Name != null)
            {
                existingPageType = PageTypeFactory.Load(attribute.Name);
            }

            if(existingPageType == null)
            {
                existingPageType = PageTypeFactory.Load(type.Name);
            }

            return existingPageType;
        }

        protected internal virtual PageType CreateNewPageType(PageTypeDefinition definition)
        {
            PageType pageType = new PageType();

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
            
            PageTypeFactory.Save(pageType);

            return pageType;
        }

        protected internal virtual void UpdatePageType(PageTypeDefinition definition)
        {
            PageType pageType = GetExistingPageType(definition);
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
            UpdateAvailablePageTypes(pageType, attribute.AvailablePageTypes);
            
            string newValuesString = SerializeValues(pageType);
            if(newValuesString != oldValueString)
                PageTypeFactory.Save(pageType);
        }

        protected internal virtual string SerializeValues(PageType pageType)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(pageType.Name);
            builder.Append("|");
            builder.Append(pageType.FileName);
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
            builder.Append(pageType.DefaultFrameID);
            builder.Append("|");
            foreach (int pageTypeID in pageType.AllowedPageTypes.OrderBy(id => id))
            {
                builder.Append(pageTypeID);
                builder.Append("||");
            }

            return builder.ToString();
        }

        protected internal virtual void UpdateName(PageType pageType, PageTypeDefinition definition)
        {
            pageType.Name = definition.GetPageTypeName();
        }

        protected internal virtual void UpdateFilename(PageType pageType, PageTypeAttribute attribute)
        {
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

        protected internal virtual void UpdateSortOrder(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.SortOrder = attribute.SortOrder;
        }

        protected internal virtual void UpdateDescription(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.Description = attribute.Description;
        }

        protected internal virtual void UpdateIsAvailable(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.IsAvailable = attribute.AvailableInEditMode;
        }

        protected internal virtual void UpdateDefaultArchivePageLink(PageType pageType, PageTypeAttribute attribute)
        {
            if(attribute.DefaultArchiveToPageID != PageTypeAttribute.DefaultDefaultArchiveToPageID)
                pageType.DefaultArchivePageLink = new PageReference(attribute.DefaultArchiveToPageID);
            else
                pageType.DefaultArchivePageLink = null;
        }

        protected internal virtual void UpdateDefaultChildOrderRule(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultChildOrderRule = attribute.DefaultChildSortOrder;
        }

        protected internal virtual void UpdateDefaultPageName(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultPageName = attribute.DefaultPageName;
        }

        protected internal virtual void UpdateDefaultPeerOrder(PageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultSortIndex != PageTypeAttribute.DefaultDefaultSortIndex)
                pageType.DefaultPeerOrder = attribute.DefaultSortIndex;
            else
                pageType.DefaultPeerOrder = DefaultDefaultPageTypePeerOrder;
        }

        protected internal virtual void UpdateDefaultStartPublishOffset(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultStartPublishOffset = new TimeSpan(0, 0, attribute.DefaultStartPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultStopPublishOffset(PageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultStopPublishOffset = new TimeSpan(0, 0, attribute.DefaultStopPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultVisibleInMenu(PageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultVisibleInMenu != PageTypeAttribute.DefaultDefaultVisibleInMenu)
                pageType.DefaultVisibleInMenu = attribute.DefaultVisibleInMenu;
            else
                pageType.DefaultVisibleInMenu = DefaultDefaultVisibleInMenu;
        }

        protected internal virtual void UpdateFrame(PageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultFrameID != 0)
                pageType.Defaults.DefaultFrame = Frame.Load(attribute.DefaultFrameID);
            else
                pageType.Defaults.DefaultFrame = null;
        }

        protected internal virtual void UpdateAvailablePageTypes(PageType pageType, Type[] availablePageTypes)
        {
            if(availablePageTypes == null)
            {
                pageType.AllowedPageTypes = null;
                return;
            }
            int[] availablePageTypeIDs = new int[availablePageTypes.Length];
            for (int i = 0; i < availablePageTypes.Length; i++)
            {
                Type availablePageTypeType = availablePageTypes[i];
                PageTypeDefinition availablePageTypeDefinition = _pageTypeDefinitions.First(
                    definitions => definitions.Type == availablePageTypeType);
                PageType availablePageType = GetExistingPageType(availablePageTypeDefinition);
                availablePageTypeIDs[i] = availablePageType.ID;
            }
            pageType.AllowedPageTypes = availablePageTypeIDs;
        }

        public PageTypeFactory PageTypeFactory { get; set; }

        internal string DefaultFilename { get; set; }
    }
}