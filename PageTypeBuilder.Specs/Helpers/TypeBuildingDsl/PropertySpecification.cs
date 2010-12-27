using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PageTypeBuilder.Specs.Helpers.TypeBuildingDsl
{
    public class PropertySpecification
    {
        ///TODO: Require Name and Type in ctor
        public PropertySpecification()
        {
            Attributes = new List<AttributeSpecification>();
            GetterAttributes = MethodAttributes.Public;
            SetterAttributes = MethodAttributes.Public;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public void AddAttributeTemplate(Attribute template)
        {
            Attributes.Add(new AttributeSpecification(template));
        }

        public List<AttributeSpecification> Attributes { get; set; }

        public MethodAttributes GetterAttributes { get; set; }

        public MethodAttributes SetterAttributes { get; set; }

        public Func<TypeBuilder, MethodBuilder> GetterImplementation { get; set; }
    }
}
