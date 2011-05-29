using System;
using System.Globalization;
using System.Linq;
using EPiServer.Core;

namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    public class NativePageDefinitionsMap : INativePageDefinitionsMap
    {
        public virtual int GetNativeTypeID(Type pagePropertyType)
        {
            int? nativeTypeId = null;
            for (int typeId = 0; typeId < NativePropertyTypes.Length; typeId++)
            {
                if (NativePropertyTypes[typeId] == pagePropertyType)
                {
                    nativeTypeId = typeId;
                }
            }

            if (!nativeTypeId.HasValue)
            {
                string errorMessage = "Unable to retrieve native type ID. Type {0} is not a native type.";
                errorMessage = String.Format(CultureInfo.InvariantCulture, errorMessage, pagePropertyType.FullName);
                throw new PageTypeBuilderException(errorMessage);
            }

            return nativeTypeId.Value;
        }

        public bool TypeIsNativePropertyType(Type pagePropertyType)
        {
            return NativePropertyTypes.Contains(pagePropertyType);
        }

        protected Type[] NativePropertyTypes
        {
            get
            {
                Type[] nativeProperties = new Type[9];
                nativeProperties[0] = typeof(PropertyBoolean);
                nativeProperties[1] = typeof(PropertyNumber);
                nativeProperties[2] = typeof(PropertyFloatNumber);
                nativeProperties[3] = typeof(PropertyPageType);
                nativeProperties[4] = typeof(PropertyPageReference);
                nativeProperties[5] = typeof(PropertyDate);
                nativeProperties[6] = typeof(PropertyString);
                nativeProperties[7] = typeof(PropertyLongString);
                nativeProperties[8] = typeof(PropertyCategory);

                return nativeProperties;
            }
        }
    }
}