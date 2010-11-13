using System;
using EPiServer.Filters;

namespace PageTypeBuilder
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PageTypeAttribute : Attribute 
    {
        internal const int DefaultSortOrder = 100;
        internal const bool DefaultAvailableInEditMode = true;
        internal const bool DefaultDefaultVisibleInMenu = true;
        internal const int DefaultDefaultSortIndex = -1;
        internal const int DefaultDefaultArchiveToPageID = -1;

        public PageTypeAttribute() :this(null) {}

        public PageTypeAttribute(string guid)
        {
            if (guid != null)
            {
                Guid = new Guid(guid);
            }

            SortOrder = DefaultSortOrder;
            AvailableInEditMode = DefaultAvailableInEditMode;
            DefaultVisibleInMenu = DefaultDefaultVisibleInMenu;
            DefaultSortIndex = DefaultDefaultSortIndex;
            DefaultArchiveToPageID = DefaultDefaultArchiveToPageID;
        }

        public virtual Guid? Guid { get; private set; }

        public virtual string Name { get; set; }

        public virtual string Filename { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual string Description { get; set; }

        public virtual bool AvailableInEditMode { get; set; }

        public virtual string DefaultPageName { get; set; }

        public virtual int DefaultStartPublishOffsetMinutes { get; set; }

        public virtual int DefaultStopPublishOffsetMinutes { get; set; }

        public virtual bool DefaultVisibleInMenu { get; set; }

        public virtual int DefaultSortIndex { get; set; }

        public virtual FilterSortOrder DefaultChildSortOrder { get; set; }

        public virtual int DefaultArchiveToPageID { get; set; }

        public virtual int DefaultFrameID { get; set; }

        public virtual Type[] AvailablePageTypes { get; set; }
    }
}