namespace PageTypeBuilder
{
    using System;
    using EPiServer.DataAbstraction;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PageTypePropertyAttribute : Attribute
    {
        private const bool DefaultDisplayInEditMode = true;
        internal const int SortOrderNoValue = -1;

        private Type _type;
        private string _editCaption;
        private string _helpText;
        private Type _tab;
        private bool _required;
        private bool _searchable;
        private object _defaultValue;
        private DefaultValueType _defaultValueType;
        private bool _uniqueValuePerLanguage;
        private bool _displayInEditMode;
        private int _sortOrder;

        public PageTypePropertyAttribute()
        {
            _displayInEditMode = DefaultDisplayInEditMode;
            _sortOrder = -1;
        }

        public virtual Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                TypeSet = true;
            }
        }

        public virtual string EditCaption
        {
            get
            {
                return _editCaption;
            }
            set
            {
                _editCaption = value;
                EditCaptionSet = true;
            }
        }

        public virtual string HelpText
        {
            get
            {
                return _helpText;
            }
            set
            {
                _helpText = value;
                HelpTextSet = true;
            }
        }

        public virtual Type Tab
        {
            get
            {
                return _tab;
            }
            set
            {
                _tab = value;
                TabSet = true;
            }
        }

        public virtual bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
                RequiredSet = true;
            }
        }

        public virtual bool Searchable
        {
            get
            {
                return _searchable;
            }
            set
            {
                _searchable = value;
                SearchableSet = true;
            }
        }

        public virtual object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
                DefaultValueSet = true;
            }
        }

        public virtual DefaultValueType DefaultValueType
        {
            get
            {
                return _defaultValueType;
            }
            set
            {
                _defaultValueType = value;
                DefaultValueTypeSet = true;
            }
        }

        public virtual bool UniqueValuePerLanguage
        {
            get
            {
                return _uniqueValuePerLanguage;
            }
            set
            {
                _uniqueValuePerLanguage = value;
                UniqueValuePerLanguageSet = true;
            }
        }

        public virtual bool DisplayInEditMode
        {
            get
            {
                return _displayInEditMode;
            }
            set
            {
                _displayInEditMode = value;
                DisplayInEditModeSet = true;
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

        internal bool TypeSet { get; set; }

        internal bool EditCaptionSet { get; set; }

        internal bool HelpTextSet { get; set; }

        internal bool TabSet { get; set; }

        internal bool RequiredSet { get; set; }

        internal bool SearchableSet { get; set; }

        internal bool DefaultValueSet { get; set; }

        internal bool DefaultValueTypeSet { get; set; }

        internal bool UniqueValuePerLanguageSet { get; set; }

        internal bool DisplayInEditModeSet { get; set; }

        internal bool SortOrderSet { get; set; }

    }
}