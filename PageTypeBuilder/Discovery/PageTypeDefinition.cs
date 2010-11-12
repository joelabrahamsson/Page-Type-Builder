using System;

namespace PageTypeBuilder.Discovery
{
    public class PageTypeDefinition
    {
        public Type Type { get; set; }

        public PageTypeAttribute Attribute { get; set; }

        public virtual string GetPageTypeName()
        {
            string name = Attribute.Name;

            if (name == null)
            {
                name = Type.Name;
            }

            return name;
        }
    }
}