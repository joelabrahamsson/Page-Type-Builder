using System;
using System.Collections.Generic;
using System.Reflection;

namespace PageTypeBuilder.Specs.Helpers.TypeBuildingDsl
{
    public class TypeSpecification
    {
        public TypeSpecification()
        {
            Attributes = new List<AttributeSpecification>();
            Properties = new List<PropertySpecification>();
        }

        public void AddAttributeTemplate(Attribute template)
        {
            Attributes.Add(new AttributeSpecification(template));
        }

        public string Name { get; set; }

        public TypeAttributes TypeAttributes { get; set; }

        public Type ParentType { get; set; }

        public List<AttributeSpecification> Attributes { get; set; }

        public List<PropertySpecification> Properties { get; set; }

        public Action<Attribute, Type> BeforeAttributeIsAddedToType { get; set; }
    }
}
