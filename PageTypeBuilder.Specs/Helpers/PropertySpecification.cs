using System;
using System.Collections.Generic;
using System.Reflection;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PropertySpecification
    {
        public PropertySpecification()
        {
            Attributes = new List<Attribute>();
            GetterAttributes = MethodAttributes.Public;
            SetterAttributes = MethodAttributes.Public;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<Attribute> Attributes { get; set; }

        public MethodAttributes GetterAttributes { get; set; }

        public MethodAttributes SetterAttributes { get; set; }
    }
}
