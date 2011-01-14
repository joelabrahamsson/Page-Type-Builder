using System;
using System.Collections.Generic;

namespace PageTypeBuilder.Specs.Helpers.TypeBuildingDsl
{
    public class AssemblySpecification
    {
        public AssemblySpecification()
        {
            AttributeSpecification = new List<Attribute>();
        }

        public string Name { get; set; }

        public List<Attribute> AttributeSpecification { get; set; }
    }
}
