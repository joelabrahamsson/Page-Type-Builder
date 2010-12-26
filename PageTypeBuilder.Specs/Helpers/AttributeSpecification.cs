using System;
using System.Reflection;

namespace PageTypeBuilder.Specs.Helpers
{
    public class AttributeSpecification
    {
        public AttributeSpecification() {}

        public AttributeSpecification(Attribute template)
        {
            Template = template;
            Type = template.GetType();
        }

        public Attribute Template { get; set; }

        public Type Type { get; set; }

        public ConstructorInfo Constructor { get; set; }

        public object[] ConstructorParameters { get; set; }
    }
}
