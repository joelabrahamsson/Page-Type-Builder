using System.Text;
using EPiServer.SpecializedProperties;

namespace PageTypeBuilder.Tests
{
    [PageType]
    public class TestPageType : TypedPageData
    {
        [PageTypeProperty]
        public string StringTestProperty { get; set; }

        public int? NullableIntTestProperty { get; set; }

        public int IntTestProperty { get; set; }

        [PageTypeProperty]
        public virtual string CompilerGeneratedProperty { get; set; }

        [PageTypeProperty]
        public string NotGeneratedProperty
        {
            get { return null; }
            set { }
        }

        public string CompilerGeneratedPropertyWithoutAttribute { get; set; }

        [PageTypeProperty]
        public string CompilerGeneratedNonVirtualProperty { get; set; }

        [PageTypeProperty]
        public virtual string CompilerGeneratedPropertyWithPrivateGetter { private get; set; }

        [PageTypeProperty]
        public virtual string CompilerGeneratedPropertyWithPrivateSetter { get; private set; }

        [PageTypeProperty(Tab = typeof(string))]
        public string PropertyWithTabSetToTypeNotInheritingFromTab { get; set; }

        [PageTypeProperty(Tab = typeof(TestTabAbstract))]
        public string PropertyWithTabSetAbstractTabSubClass { get; set; }

        [PageTypeProperty(Tab = typeof(TestTab))]
        public string PropertyWithValidTab { get; set; }

        [PageTypeProperty]
        public StringBuilder PropertyWithInvalidTypeAndNoTypeSpecified { get; set; }

        [PageTypeProperty(Type = typeof(StringBuilder))]
        public string PropertyWithInvalidTypeSpecified { get; set; }

        [PageTypeProperty(Type = typeof(PropertyXhtmlString))]
        public StringBuilder PropertyWithValidTypeSpecified { get; set; }
    }
}
