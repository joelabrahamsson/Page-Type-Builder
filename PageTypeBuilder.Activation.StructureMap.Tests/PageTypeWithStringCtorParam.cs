
namespace PageTypeBuilder.Activation.StructureMap.Tests
{
    public class PageTypeWithStringCtorParam : TypedPageData
    {
        public string Injected;
        public PageTypeWithStringCtorParam(string someDependency)
        {
            Injected = someDependency;
        }
    }
}
