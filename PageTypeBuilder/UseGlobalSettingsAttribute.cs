using System;

namespace PageTypeBuilder
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UseGlobalSettingsAttribute : Attribute
    {
        public UseGlobalSettingsAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}
