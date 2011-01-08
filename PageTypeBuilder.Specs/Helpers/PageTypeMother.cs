using EPiServer.Core;
using EPiServer.Filters;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Specs.Helpers.Fakes;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PageTypeMother
    {
        public static IPageType CreatePageTypeWithEverythingButGuidDifferentThanAttribute(PageTypeAttribute pageTypeAttribute)
        {
            var existingPageType = new FakePageType();
            existingPageType.GUID = pageTypeAttribute.Guid.Value;
            existingPageType.AllowedPageTypes = new int[0];
            existingPageType.Description = pageTypeAttribute.Description + " more text";
            existingPageType.IsAvailable = !pageTypeAttribute.AvailableInEditMode;
            existingPageType.DefaultArchivePageLink =
                new PageReference(pageTypeAttribute.DefaultArchiveToPageID + 1);
            existingPageType.SortOrder = pageTypeAttribute.SortOrder + 1;
            existingPageType.DefaultPageName = pageTypeAttribute.DefaultPageName + " more text";
            existingPageType.DefaultStartPublishOffset =
                (pageTypeAttribute.DefaultStartPublishOffsetMinutes + 1).Minutes();
            existingPageType.DefaultStopPublishOffset =
                (pageTypeAttribute.DefaultStopPublishOffsetMinutes + 1).Minutes();
            existingPageType.DefaultVisibleInMenu = !pageTypeAttribute.DefaultVisibleInMenu;
            existingPageType.DefaultPeerOrder = pageTypeAttribute.DefaultSortIndex + 1;
            existingPageType.DefaultChildOrderRule = (pageTypeAttribute.DefaultChildSortOrder == FilterSortOrder.Index) 
                ? FilterSortOrder.Alphabetical 
                : FilterSortOrder.Index;
            existingPageType.DefaultFrameID = pageTypeAttribute.DefaultFrameID + 1;
            existingPageType.FileName = (pageTypeAttribute.Filename != null && pageTypeAttribute.Filename.Contains(".aspx")) 
                ? pageTypeAttribute.Filename.Replace(".aspx", "_old.aspx") 
                : "~/OldTemplateForThePageType.aspx";
            existingPageType.Name = pageTypeAttribute.Name + " more text";
            return existingPageType;
        }
    }
}
