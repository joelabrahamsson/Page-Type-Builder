using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;

namespace PageTypeBuilder.Abstractions
{
    public class WrappedPageType : IPageType
    {
        PageType wrapped;
        public WrappedPageType(PageType pageTypeToWrap)
        {
            wrapped = pageTypeToWrap;
        }

        public int[] AllowedPageTypes
        {
            get { return wrapped.AllowedPageTypes; }
            set { wrapped.AllowedPageTypes = value; }
        }

        public PageReference DefaultArchivePageLink
        {
            get { return wrapped.DefaultArchivePageLink; }
            set { wrapped.DefaultArchivePageLink = value; }
        }

        public FilterSortOrder DefaultChildOrderRule
        {
            get { return wrapped.DefaultChildOrderRule; }
            set { wrapped.DefaultChildOrderRule = value; }
        }

        public int DefaultFrameID
        {
            get { return wrapped.DefaultFrameID; }
            set { wrapped.DefaultFrameID = value; }
        }

        public string DefaultPageName
        {
            get { return wrapped.DefaultPageName; }
            set { wrapped.DefaultPageName = value; }
        }

        public int DefaultPeerOrder
        {
            get { return wrapped.DefaultPeerOrder; }
            set { wrapped.DefaultPeerOrder = value; }
        }

        public TimeSpan DefaultStartPublishOffset
        {
            get { return wrapped.DefaultStartPublishOffset; }
            set { wrapped.DefaultStartPublishOffset = value; }
        }

        public TimeSpan DefaultStopPublishOffset
        {
            get { return wrapped.DefaultStopPublishOffset; }
            set { wrapped.DefaultStopPublishOffset = value; }
        }

        public PageTypeDefault Defaults
        {
            get { return wrapped.Defaults; }
            set { wrapped.Defaults = value; }
        }

        public bool DefaultVisibleInMenu
        {
            get { return wrapped.DefaultVisibleInMenu; }
            set { wrapped.DefaultVisibleInMenu = value; }
        }

        public PageDefinitionCollection Definitions
        {
            get { return wrapped.Definitions; }
        }

        public string Description
        {
            get { return wrapped.Description; }
            set { wrapped.Description = value; }
        }

        public string FileName
        {
            get { return wrapped.FileName; }
            set { wrapped.FileName = value; }
        }

        public Guid GUID
        {
            get { return wrapped.GUID; }
            set { wrapped.GUID = value; }
        }

        public int ID
        {
            get { return wrapped.ID; }
            set { wrapped.ID = value; }
        }

        public bool IsAvailable
        {
            get { return wrapped.IsAvailable; }
            set { wrapped.IsAvailable = value; }
        }

        public bool IsNew
        {
            get { return wrapped.IsNew; }
        }

        public string Name
        {
            get { return wrapped.Name; }
            set { wrapped.Name = value; }
        }

        public void Save()
        {
            wrapped.Save();
        }

        public int SortOrder
        {
            get { return wrapped.SortOrder; }
            set { wrapped.SortOrder = value; }
        }

        public void Delete()
        {
            wrapped.Delete();
        }
    }
}
