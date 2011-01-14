using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class FakePageType : IPageType
    {
        public FakePageType()
        {
            var template = new PageType();
            AllowedPageTypes = template.AllowedPageTypes;
            DefaultArchivePageLink = template.DefaultArchivePageLink;
            DefaultChildOrderRule = template.DefaultChildOrderRule;
            DefaultPageName = template.DefaultPageName;
            DefaultPeerOrder = template.DefaultPeerOrder;
            DefaultStartPublishOffset = template.DefaultStartPublishOffset;
            DefaultStopPublishOffset = template.DefaultStopPublishOffset;
            DefaultVisibleInMenu = template.DefaultVisibleInMenu;
            Description = template.Description;
            IsAvailable = template.IsAvailable;
            Name = template.Name;
            SortOrder = template.SortOrder;
            Defaults = new PageTypeDefault();
        }

        int[] allowedPageTypes; 
        public int[] AllowedPageTypes
        {
            get { return allowedPageTypes; }
            set 
            {
                if (value == null)
                    allowedPageTypes = new int[0];
                else
                    allowedPageTypes = value;
            }
        }
        public PageReference DefaultArchivePageLink { get; set; }
        public FilterSortOrder DefaultChildOrderRule { get; set; }
        public int DefaultFrameID { get; set; }
        public string DefaultPageName { get; set; }
        public int DefaultPeerOrder { get; set; }
        public TimeSpan DefaultStartPublishOffset { get; set; }
        public TimeSpan DefaultStopPublishOffset { get; set; }
        public PageTypeDefault Defaults { get; set; }
        public bool DefaultVisibleInMenu { get; set; }
        public PageDefinitionCollection Definitions { get; private set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public Guid GUID { get; set; }
        public int ID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNew 
        { 
            get
            {
                return (ID <= 0);
            }
        }
        public string Name { get; set; }
        public void Save()
        {
            throw new NotImplementedException();
        }

        public int SortOrder { get; set; }
    }
}