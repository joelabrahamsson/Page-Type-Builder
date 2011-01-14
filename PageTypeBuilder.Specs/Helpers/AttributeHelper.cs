using System;
using EPiServer.Filters;
using PageTypeBuilder.Specs.Helpers.Fakes;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Helpers
{
    public class AttributeHelper
    {
        public static AttributeSpecification CreatePageTypeAttributeSpecification(string guid)
        {
            return new AttributeSpecification
            {
                Constructor = typeof(PageTypeAttribute).GetConstructor(new[] { typeof(string) }),
                ConstructorParameters = new object[] { guid }
            };
        }

        public static PageTypeAttribute CreatePageTypeAttributeWithEverythingSpeficied(InMemoryContext syncContext)
        {
            var guid = Guid.NewGuid();

            var pageTypeAttribute = new PageTypeAttribute(guid.ToString());
            
            var anotherPageTypeClass = syncContext.CreateAndAddPageTypeClassToAppDomain(type => { });
            var existingPageType = syncContext.PageTypeFactory.CreateNew();
            existingPageType.Name = anotherPageTypeClass.Name;
            syncContext.PageTypeFactory.Save(existingPageType);
            syncContext.PageTypeResolver.AddPageType(existingPageType.ID, anotherPageTypeClass);

            var availablePageTypes = new[] { anotherPageTypeClass };
            pageTypeAttribute.AvailablePageTypes = availablePageTypes;
            pageTypeAttribute.AvailableInEditMode = false;
            pageTypeAttribute.Description = "A description";
            pageTypeAttribute.SortOrder = 123;
            pageTypeAttribute.DefaultArchiveToPageID = 567;
            pageTypeAttribute.DefaultPageName = "Default page name";
            pageTypeAttribute.DefaultStartPublishOffsetMinutes = 1234;
            pageTypeAttribute.DefaultStopPublishOffsetMinutes = 12345;
            pageTypeAttribute.DefaultVisibleInMenu = false;
            pageTypeAttribute.DefaultChildSortOrder = FilterSortOrder.Alphabetical;
            pageTypeAttribute.DefaultSortIndex = 345;
            pageTypeAttribute.DefaultFrameID = 1;
            pageTypeAttribute.Filename = "~/TemplateForThePageType.aspx";
            pageTypeAttribute.Name = "Page type name";

            return pageTypeAttribute;
        }
    }
}
