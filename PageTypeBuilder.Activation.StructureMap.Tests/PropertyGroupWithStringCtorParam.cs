namespace PageTypeBuilder.Activation.StructureMap.Tests
{
    public class PropertyGroupWithStringCtorParam : PageTypePropertyGroup
    {
        public string Injected;
        public PropertyGroupWithStringCtorParam(string someDependency)
        {
            Injected = someDependency;
        }
    }
}