using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

namespace PageTypeBuilder.Specs.Helpers
{
    public class AttributeHelper
    {
        public static AttributeSpecification CreatePageTypeAttributeSpecification(string guid)
        {
            return new AttributeSpecification
            {
                Constructor = typeof(PageTypeAttribute).GetConstructor(new[] { typeof(string) }),
                ConstructorParameters = new object[] { guid }
            };
        }
    }
}
