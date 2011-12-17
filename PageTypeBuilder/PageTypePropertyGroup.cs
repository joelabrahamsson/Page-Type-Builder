namespace PageTypeBuilder
{
    public abstract class PageTypePropertyGroup
    {
        protected internal TypedPageData TypedPageData { get; set; }
        internal PageTypePropertyGroupHierarchy Hierarchy { get; set; }

        internal void PopuplateInstance(TypedPageData destination, string hierarchy)
        {
            Hierarchy = new PageTypePropertyGroupHierarchy { Value = hierarchy };
            TypedPageData = destination;
        }
    }
}
