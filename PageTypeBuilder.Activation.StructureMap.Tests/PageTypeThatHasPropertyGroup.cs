namespace PageTypeBuilder.Activation.StructureMap.Tests
{
    public class PageTypeThatHasPropertyGroup : TypedPageData
    {
        [PageTypePropertyGroup]
        public virtual PropertyGroupWithStringCtorParam PropertyGroup { get; set; }
    }
}