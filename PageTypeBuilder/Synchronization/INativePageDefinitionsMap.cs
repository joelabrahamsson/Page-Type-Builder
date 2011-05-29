using System;

namespace PageTypeBuilder.Synchronization
{
    public interface INativePageDefinitionsMap
    {
        int GetNativeTypeID(Type pagePropertyType);
        bool TypeIsNativePropertyType(Type pagePropertyType);
    }
}