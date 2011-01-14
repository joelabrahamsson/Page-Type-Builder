using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageType
    {
        int[] AllowedPageTypes { get; set; }
        PageReference DefaultArchivePageLink { get; set; }
        FilterSortOrder DefaultChildOrderRule { get; set; }
        int DefaultFrameID { get; set; }
        string DefaultPageName { get; set; }
        int DefaultPeerOrder { get; set; }
        TimeSpan DefaultStartPublishOffset { get; set; }
        TimeSpan DefaultStopPublishOffset { get; set; }
        PageTypeDefault Defaults { get; set; }
        bool DefaultVisibleInMenu { get; set; }
        PageDefinitionCollection Definitions { get; }
        string Description { get; set; }
        string FileName { get; set; }
        Guid GUID { get; set; }
        int ID { get; set; }
        bool IsAvailable { get; set; }
        bool IsNew { get; }
        string Name { get; set; }
        void Save();
        int SortOrder { get; set; }
    }
}
