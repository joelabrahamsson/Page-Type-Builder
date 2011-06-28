namespace PageTypeBuilder
{
    using System;
    using EPiServer.Filters;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PageTypeAttribute : Attribute 
    {
        internal const int DefaultSortOrder = 100;
        internal const bool DefaultAvailableInEditMode = true;
        internal const bool DefaultDefaultVisibleInMenu = true;
        internal const int DefaultDefaultSortIndex = -1;
        internal const int DefaultDefaultArchiveToPageID = -1;

        private string _fileName;
        private int _sortOrder;
        private string _description;
        private bool _availableInEditMode;
        private string _defaultPageName;
        private int _defaultStartPublishOffsetMinutes;
        private int _defaultStopPublishOffsetMinutes;
        private bool _defaultVisibleInMenu;
        private int _defaultSortIndex;
        private FilterSortOrder _defaultChildSortOrder;
        private int _defaultArchiveToPageID;
        private int _defaultFrameID;
        private Type[] _availablePageTypes;

        public PageTypeAttribute() : this(null) {}

        public PageTypeAttribute(string guid)
        {
            if (guid != null)
                Guid = new Guid(guid);

            _sortOrder = DefaultSortOrder;
            _availableInEditMode = DefaultAvailableInEditMode;
            _defaultVisibleInMenu = DefaultDefaultVisibleInMenu;
            _defaultSortIndex = DefaultDefaultSortIndex;
            _defaultArchiveToPageID = DefaultDefaultArchiveToPageID;
        }

        public virtual Guid? Guid { get; private set; }

        public virtual string Name { get; set; }

        public virtual string Filename
        {
            get
            {
                return _fileName;
            }
            set 
            { 
                _fileName = value;
                FilenameSet = true;
            }
        }

        public virtual int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
                SortOrderSet = true;
            }
        }

        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                DescriptionSet = true;
            }
        }

        public virtual bool AvailableInEditMode
        {
            get
            {
                return _availableInEditMode;
            }
            set
            {
                _availableInEditMode = value;
                AvailableInEditModeSet = true;
            }
        }

        public virtual string DefaultPageName
        {
            get
            {
                return _defaultPageName;
            }
            set
            {
                _defaultPageName = value;
                DefaultPageNameSet = true;
            }
        }

        public virtual int DefaultStartPublishOffsetMinutes
        {
            get
            {
                return _defaultStartPublishOffsetMinutes;
            }
            set
            {
                _defaultStartPublishOffsetMinutes = value;
                DefaultStartPublishOffsetMinutesSet = true;
            }
        }

        public virtual int DefaultStopPublishOffsetMinutes
        {
            get
            {
                return _defaultStopPublishOffsetMinutes;
            }
            set
            {
                _defaultStopPublishOffsetMinutes = value;
                DefaultStopPublishOffsetMinutesSet = true;
            }
        }

        public virtual bool DefaultVisibleInMenu
        {
            get
            {
                return _defaultVisibleInMenu;
            }
            set
            {
                _defaultVisibleInMenu = value;
                DefaultVisibleInMenuSet = true;
            }
        }

        public virtual int DefaultSortIndex
        {
            get
            {
                return _defaultSortIndex;
            }
            set
            {
                _defaultSortIndex = value;
                DefaultSortIndexSet = true;
            }
        }

        public virtual FilterSortOrder DefaultChildSortOrder
        {
            get
            {
                return _defaultChildSortOrder;
            }
            set
            {
                _defaultChildSortOrder = value;
                DefaultChildSortOrderSet = true;
            }
        }

        public virtual int DefaultArchiveToPageID
        {
            get
            {
                return _defaultArchiveToPageID;
            }
            set
            {
                _defaultArchiveToPageID = value;
                DefaultArchiveToPageIDSet = true;
            }
        }

        public virtual int DefaultFrameID
        {
            get
            {
                return _defaultFrameID;
            }
            set
            {
                _defaultFrameID = value;
                DefaultFrameIDSet = true;
            }
        }

        public virtual Type[] AvailablePageTypes
        {
            get
            {
                return _availablePageTypes;
            }
            set
            {
                _availablePageTypes = value;
                AvailablePageTypesSet = true;
            }
        }

        internal bool FilenameSet { get; set; }

        internal bool SortOrderSet { get; set; }

        internal bool DescriptionSet { get; set; }

        internal bool AvailableInEditModeSet { get; set; }

        internal bool DefaultPageNameSet { get; set; }

        internal bool DefaultStartPublishOffsetMinutesSet { get; set; }

        internal bool DefaultStopPublishOffsetMinutesSet { get; set; }

        internal bool DefaultVisibleInMenuSet { get; set; }

        internal bool DefaultSortIndexSet { get; set; }

        internal bool DefaultChildSortOrderSet { get; set; }

        internal bool DefaultArchiveToPageIDSet { get; set; }

        internal bool DefaultFrameIDSet { get; set; }

        internal bool AvailablePageTypesSet { get; set; }

    }
}