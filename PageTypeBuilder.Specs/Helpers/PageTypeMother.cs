using System;
using System.Linq;
using EPiServer.Core;
using EPiServer.Filters;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers.Fakes;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PageTypeMother
    {
        public static IPageType CreatePageTypeWithSameValuesAsAttribute(InMemoryContext syncContext, PageTypeAttribute pageTypeAttribute)
        {
            var pageType = syncContext.PageTypeFactory.CreateNew();
            if(pageTypeAttribute.Guid.HasValue)
                pageType.GUID = pageTypeAttribute.Guid.Value;
            pageType.AllowedPageTypes = pageTypeAttribute.AvailablePageTypes
                .Select(type => syncContext.PageTypeResolver.GetPageTypeID(type).Value).ToArray();
            pageType.Description = pageTypeAttribute.Description;
            pageType.IsAvailable = pageTypeAttribute.AvailableInEditMode;
            pageType.DefaultArchivePageLink =
                new PageReference(pageTypeAttribute.DefaultArchiveToPageID);
            pageType.SortOrder = pageTypeAttribute.SortOrder;
            pageType.DefaultPageName = pageTypeAttribute.DefaultPageName;
            pageType.DefaultStartPublishOffset =
                (pageTypeAttribute.DefaultStartPublishOffsetMinutes).Minutes();
            pageType.DefaultStopPublishOffset =
                (pageTypeAttribute.DefaultStopPublishOffsetMinutes).Minutes();
            pageType.DefaultVisibleInMenu = pageTypeAttribute.DefaultVisibleInMenu;
            pageType.DefaultPeerOrder = pageTypeAttribute.DefaultSortIndex;
            pageType.DefaultChildOrderRule = pageTypeAttribute.DefaultChildSortOrder;
            pageType.DefaultFrameID = pageTypeAttribute.DefaultFrameID;
            pageType.FileName = pageTypeAttribute.Filename;
            pageType.Name = pageTypeAttribute.Name;
            return pageType;
        }

        public static IPageType CreatePageTypeWithEverythingButGuidDifferentThanAttribute(InMemoryContext syncContext, PageTypeAttribute pageTypeAttribute)
        {
            var pageType = syncContext.PageTypeFactory.CreateNew();
            pageType.GUID = pageTypeAttribute.Guid.Value;
            if(pageTypeAttribute.AvailablePageTypes.Length == 0)
                throw new Exception("This method only supports attributes that have atleast one type in AvailablePageTypes");
            pageType.AllowedPageTypes = new int[0];
            pageType.Description = pageTypeAttribute.Description + " more text";
            pageType.IsAvailable = !pageTypeAttribute.AvailableInEditMode;
            pageType.DefaultArchivePageLink =
                new PageReference(pageTypeAttribute.DefaultArchiveToPageID + 1);
            pageType.SortOrder = pageTypeAttribute.SortOrder + 1;
            pageType.DefaultPageName = pageTypeAttribute.DefaultPageName + " more text";
            pageType.DefaultStartPublishOffset =
                (pageTypeAttribute.DefaultStartPublishOffsetMinutes + 1).Minutes();
            pageType.DefaultStopPublishOffset =
                (pageTypeAttribute.DefaultStopPublishOffsetMinutes + 1).Minutes();
            pageType.DefaultVisibleInMenu = !pageTypeAttribute.DefaultVisibleInMenu;
            pageType.DefaultPeerOrder = pageTypeAttribute.DefaultSortIndex + 1;
            pageType.DefaultChildOrderRule = (pageTypeAttribute.DefaultChildSortOrder == FilterSortOrder.Index) 
                ? FilterSortOrder.Alphabetical 
                : FilterSortOrder.Index;
            pageType.DefaultFrameID = pageTypeAttribute.DefaultFrameID + 1;
            pageType.FileName = (pageTypeAttribute.Filename != null && pageTypeAttribute.Filename.Contains(".aspx")) 
                ? pageTypeAttribute.Filename.Replace(".aspx", "_old.aspx") 
                : "~/OldTemplateForThePageType.aspx";
            pageType.Name = pageTypeAttribute.Name + " more text";
            return pageType;
        }
    }
}
