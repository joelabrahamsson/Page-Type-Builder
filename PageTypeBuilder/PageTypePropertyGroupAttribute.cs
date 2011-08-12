namespace PageTypeBuilder
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PageTypePropertyGroupAttribute : Attribute
    {
        public PageTypePropertyGroupAttribute()
        {
            StartSortOrderFrom = -1;
        }

        public virtual string EditCaptionPrefix { get; set; }
        public virtual int StartSortOrderFrom { get; set; }
        public virtual Type Tab { get; set; }
    }
}