namespace PageTypeBuilder
{
    public abstract class PageTypePropertyGroup
    {
        public TypedPageData TypedPageData { get; internal set; }
        internal PageTypePropertyGroupHierarchy Hierarchy { get; set; }

        internal void PopuplateInstance(TypedPageData destination, string hierarchy)
        {
            Hierarchy = new PageTypePropertyGroupHierarchy { Value = hierarchy };
            TypedPageData = destination;
        }
    }
}
