using System;
using System.Reflection;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder
{
    using EPiServer.Core;
    using EPiServer.Filters;

    public abstract class TypedPageData : PageData
    {
        internal static void PopuplateInstance(PageData source, TypedPageData destination)
        {
            destination.ShallowCopy(source);
        }

        /// <summary>
        /// Gets a value indicating whether the page changed on publish.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the page changed on publish; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ChangedOnPublish
        {
            get
            {
                object value = GetValue("PageChangedOnPublish");
                return value != null && (bool)value;
            }
        }

        /// <summary>
        /// Gets the child order rule.
        /// </summary>
        public virtual FilterSortOrder ChildOrderRule
        {
            get
            {
                object value = GetValue("PageChildOrderRule");
                return value == null ? FilterSortOrder.None : (FilterSortOrder)value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether publishing is delayed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if publishing is delayed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool DelayedPublish
        {
            get
            {
                object value = GetValue("PageDelayedPublish");
                return value != null && (bool)value;
            }
        }

        /// <summary>
        /// Gets the external URL.
        /// </summary>
        public virtual string ExternalURL
        {
            get
            {
                object value = GetValue("PageExternalUrl");
                return value == null ? string.Empty : value as string;
            }
        }

        /// <summary>
        /// Gets the peer order.
        /// </summary>
        public virtual int? PeerOrder
        {
            get
            {
                object value = GetValue("PagePeerOrder");
                return value == null ? null : (int?)value;
            }
        }

        /// <summary>
        /// Gets the shortcut link.
        /// </summary>
        public virtual PageReference ShortcutLink
        {
            get
            {
                object value = GetValue("PageShortcutLink");
                return value == null ? PageReference.EmptyReference : value as PageReference;
            }
        }

        /// <summary>
        /// Gets the target frame.
        /// </summary>
        public string TargetFrame
        {
            get
            {
                object value = GetValue("PageTargetFrame");
                return value == null ? string.Empty : value as string;
            }
        }

        /// <summary>
        /// Creates a writable clone of the PageData object
        /// </summary>
        /// <returns></returns>
        public new PageData CreateWritableClone()
        {
            PageData page = base.CreateWritableClone();

            foreach (PropertyInfo property in page.GetType().GetPageTypePropertyGroupProperties())
            {
                Type propertyType = property.PropertyType;

                if (!(propertyType.IsSubclassOf(typeof(PageTypePropertyGroup))))
                    continue;

                PageTypePropertyGroup propertyGroup = property.GetValue(this, null) as PageTypePropertyGroup;

                if (propertyGroup == null)
                    continue;

                propertyGroup.TypedPageData = page as TypedPageData;
            }

            return page;
        }

    }
}