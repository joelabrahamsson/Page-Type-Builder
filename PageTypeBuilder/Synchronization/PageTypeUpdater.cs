﻿using System;
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

        public PageTypeUpdater(IPageTypeDefinitionLocator pageTypeDefinitionLocator, 
            IPageTypeFactory pageTypeFactory, 
            IPageTypeValueExtractor pageTypeValueExtractor,
            IPageTypeLocator pageTypeLocator)
        {
            _pageTypeDefinitions = pageTypeDefinitionLocator.GetPageTypeDefinitions();
            PageTypeFactory = pageTypeFactory;
            DefaultFilename = DefaultPageTypeFilename;
            _pageTypeValueExtractor = pageTypeValueExtractor;
            _pageTypeLocator = pageTypeLocator;
        }

        protected internal virtual IPageType GetExistingPageType(PageTypeDefinition definition)
        {
            return _pageTypeLocator.GetExistingPageType(definition);
        }

        protected internal virtual IPageType CreateNewPageType(PageTypeDefinition definition)
        {
            IPageType pageType = PageTypeFactory.CreateNew();

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
            UpdateAvailablePageTypes(pageType, attribute.AvailablePageTypes);
			UpdateAvailablePageTypesIncludeSubclasses(pageType, attribute.AvailablePageTypesIncludeSubclasses);
			UpdateExcludePageTypes(pageType, attribute.ExcludePageTypes);
            
            string newValuesString = SerializeValues(pageType);
            if (newValuesString != oldValueString)
                PageTypeFactory.Save(pageType);
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

        protected internal virtual void UpdateName(IPageType pageType, PageTypeDefinition definition)
        {
            pageType.Name = definition.GetPageTypeName();
        }

        protected internal virtual void UpdateFilename(IPageType pageType, PageTypeAttribute attribute)
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

        protected internal virtual void UpdateSortOrder(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.SortOrder = attribute.SortOrder;
        }

        protected internal virtual void UpdateDescription(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.Description = attribute.Description;
        }

        protected internal virtual void UpdateIsAvailable(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.IsAvailable = attribute.AvailableInEditMode;
        }

        protected internal virtual void UpdateDefaultArchivePageLink(IPageType pageType, PageTypeAttribute attribute)
        {
            if(attribute.DefaultArchiveToPageID != PageTypeAttribute.DefaultDefaultArchiveToPageID)
                pageType.DefaultArchivePageLink = new PageReference(attribute.DefaultArchiveToPageID);
            else
                pageType.DefaultArchivePageLink = null;
        }

        protected internal virtual void UpdateDefaultChildOrderRule(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultChildOrderRule = attribute.DefaultChildSortOrder;
        }

        protected internal virtual void UpdateDefaultPageName(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultPageName = attribute.DefaultPageName;
        }

        protected internal virtual void UpdateDefaultPeerOrder(IPageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultSortIndex != PageTypeAttribute.DefaultDefaultSortIndex)
                pageType.DefaultPeerOrder = attribute.DefaultSortIndex;
            else
                pageType.DefaultPeerOrder = DefaultDefaultPageTypePeerOrder;
        }

        protected internal virtual void UpdateDefaultStartPublishOffset(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultStartPublishOffset = new TimeSpan(0, 0, attribute.DefaultStartPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultStopPublishOffset(IPageType pageType, PageTypeAttribute attribute)
        {
            pageType.DefaultStopPublishOffset = new TimeSpan(0, 0, attribute.DefaultStopPublishOffsetMinutes, 0);
        }

        protected internal virtual void UpdateDefaultVisibleInMenu(IPageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultVisibleInMenu != PageTypeAttribute.DefaultDefaultVisibleInMenu)
                pageType.DefaultVisibleInMenu = attribute.DefaultVisibleInMenu;
            else
                pageType.DefaultVisibleInMenu = DefaultDefaultVisibleInMenu;
        }

        protected internal virtual void UpdateFrame(IPageType pageType, PageTypeAttribute attribute)
        {
            if (attribute.DefaultFrameID != 0)
                pageType.DefaultFrameID = attribute.DefaultFrameID;
        }

        protected internal virtual void UpdateAvailablePageTypes(IPageType pageType, Type[] availablePageTypes)
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
                    definitions => definitions.Type.GUID == availablePageTypeType.GUID);
                IPageType availablePageType = GetExistingPageType(availablePageTypeDefinition);
                availablePageTypeIDs[i] = availablePageType.ID;
            }
            pageType.AllowedPageTypes = availablePageTypeIDs;
        }

		protected internal virtual void UpdateAvailablePageTypesIncludeSubclasses(IPageType pageType, Type[] availablePageTypesIncludeSubclasses)
		{
			if (availablePageTypesIncludeSubclasses == null)
			{
				return;
			}
			var availablePageTypeIDs = new List<int>();
			for (int i = 0; i < _pageTypeDefinitions.Count(); i++)
			{
				PageTypeDefinition availablePageTypeDefinition = _pageTypeDefinitions.ElementAt(i);
				for (int j = 0; j < availablePageTypesIncludeSubclasses.Count(); j++)
				{
					if (!availablePageTypeDefinition.Type.IsSubclassOf(availablePageTypesIncludeSubclasses[j]) &&
						(availablePageTypeDefinition.Type != availablePageTypesIncludeSubclasses[j])) continue;
					IPageType availablePageType = GetExistingPageType(availablePageTypeDefinition);
					availablePageTypeIDs.Add(availablePageType.ID);
				}
			}
			pageType.AllowedPageTypes = availablePageTypeIDs.ToArray();
		}

		protected internal virtual void UpdateExcludePageTypes(IPageType pageType, Type[] excludePageTypes)
		{
			if (excludePageTypes == null)
			{
				return;
			}

			int[] excludePageTypeIDs = new int[excludePageTypes.Length];
			for (int i = 0; i < excludePageTypes.Length; i++)
			{
				Type excludePageTypeType = excludePageTypes[i];
				PageTypeDefinition excludePageTypeDefinition = _pageTypeDefinitions.First(
					definitions => definitions.Type == excludePageTypeType);
				IPageType excludePageType = GetExistingPageType(excludePageTypeDefinition);
				excludePageTypeIDs[i] = excludePageType.ID;
			}

			List<int> allowedPageTypesIDs = new List<int>();
			if (pageType.AllowedPageTypes == null)
			{
				for (int i = 0; i < _pageTypeDefinitions.Count(); i++)
				{
					allowedPageTypesIDs.Add(GetExistingPageType(_pageTypeDefinitions.ElementAt(i)).ID);
				}
			}
			else
			{
				allowedPageTypesIDs = pageType.AllowedPageTypes.ToList();
			}

			foreach (int t in excludePageTypeIDs)
			{
				if (allowedPageTypesIDs.Contains(t))
				{
					allowedPageTypesIDs.Remove(t);
				}
			}
			pageType.AllowedPageTypes = allowedPageTypesIDs.ToArray();
		}

		protected internal virtual void SortPageTypesAlphabetically()
		{
			_pageTypeDefinitions = _pageTypeDefinitions.OrderBy(p => p.GetPageTypeName());
			for (int i = 0; i < _pageTypeDefinitions.Count(); i++)
			{
				IPageType pageType = GetExistingPageType(_pageTypeDefinitions.ElementAt(i));
				pageType.SortOrder = i;
				PageTypeFactory.Save(pageType);
			}
		}

        public IPageTypeFactory PageTypeFactory { get; set; }

        internal string DefaultFilename { get; set; }
    }
}