using System;
using System.Collections.Generic;

namespace PageTypeBuilder.Specs.Helpers
{
    public class PropertySpecification
    {
        public PropertySpecification()
        {
            Attributes = new List<Attribute>();
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<Attribute> Attributes { get; set; }
    }
}
