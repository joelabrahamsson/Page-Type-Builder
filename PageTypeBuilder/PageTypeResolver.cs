using System;
using System.Collections.Generic;
using EPiServer.Core;
using PageTypeBuilder.Activation;

namespace PageTypeBuilder
{
    public class PageTypeResolver
    {
        private Dictionary<int, Type> _typeByPageTypeID = new Dictionary<int, Type>();
        private Dictionary<Type, int> _pageTypeIDByType = new Dictionary<Type, int>();

        private static PageTypeResolver _instance;

        protected internal PageTypeResolver()
        {
            Activator = new TypedPageActivator();
        }

        protected internal virtual void AddPageType(int pageTypeID, Type pageTypeType)
        {
            if(AlreadyAddedToTypeByPageTypeID(pageTypeID, pageTypeType))
                _typeByPageTypeID.Add(pageTypeID, pageTypeType);

            if(AlreadyAddedToPageTypeIDByType(pageTypeID, pageTypeType))
                _pageTypeIDByType.Add(pageTypeType, pageTypeID);
        }

        private bool AlreadyAddedToTypeByPageTypeID(int pageTypeID, Type pageTypeType)
        {
            return !_typeByPageTypeID.ContainsKey(pageTypeID) || _typeByPageTypeID[pageTypeID] != pageTypeType;
        }

        private bool AlreadyAddedToPageTypeIDByType(int pageTypeID, Type pageTypeType)
        {
            return !_pageTypeIDByType.ContainsKey(pageTypeType) || _pageTypeIDByType[pageTypeType] != pageTypeID;
        }

        public virtual Type GetPageTypeType(int pageTypeID)
        {
            Type type = null;

            if (_typeByPageTypeID.ContainsKey(pageTypeID))
            {
                type = _typeByPageTypeID[pageTypeID];
            }

            return type;
        }

        public virtual int? GetPageTypeID(Type type)
        {
            int? pageTypeID = null;

            if (_pageTypeIDByType.ContainsKey(type))
            {
                pageTypeID = _pageTypeIDByType[type];
            }

            return pageTypeID;
        }

        public virtual PageData ConvertToTyped(PageData page)
        {
            Type type = GetPageTypeType(page.PageTypeID);

            if (type == null)
                return page;

            return Activator.CreateAndPopulateTypedInstance(page, type);
        }

        public static PageTypeResolver Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new PageTypeResolver();

                return _instance;
            }

            internal set
            {
                _instance = value;
            }
        }

        public TypedPageActivator Activator { get; set; }
    }
}
