namespace PageTypeBuilder
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class PageTypePropertyGroupPropertyOverrideAttribute : PageTypePropertyAttribute
    {
        public string PropertyName { get; set; }
    }
}
