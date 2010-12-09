using System;
using System.Collections.Generic;
using System.Reflection;

namespace PageTypeBuilder.Specs.Helpers
{
    public class TypeSpecification
    {
        public TypeSpecification()
        {
            Attributes = new List<Attribute>();
            Properties = new List<PropertySpecification>();
        }

        public string Name { get; set; }

        public TypeAttributes TypeAttributes { get; set; }

        public Type ParentType { get; set; }

        public List<Attribute> Attributes { get; set; }

        public List<PropertySpecification> Properties { get; set; }

        public Action<Attribute, Type> BeforeAttributeIsAddedToType { get; set; }
    }
}
