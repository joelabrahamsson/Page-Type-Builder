using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization.Validation;

namespace PageTypeBuilder.Synchronization
{
    public class PageDefinitionTypeMapper
    {
        public PageDefinitionTypeMapper(PageDefinitionTypeFactory pageDefinitionTypeFactory)
        {
            PageDefinitionTypeFactory = pageDefinitionTypeFactory;
        }

        private PageDefinitionTypeFactory PageDefinitionTypeFactory { get; set; }

        protected internal virtual PageDefinitionType GetPageDefinitionType(PageTypePropertyDefinition definition)
        {
            return GetPageDefinitionType(
                definition.PageType.Name, definition.Name, definition.PropertyType, definition.PageTypePropertyAttribute);
        }

        protected internal virtual PageDefinitionType GetPageDefinitionType(
            string pageTypeName, string propertyName, 
            Type propertyType, PageTypePropertyAttribute pageTypePropertyAttribute)
        {
            Type pagePropertyType = GetPropertyType(propertyType, pageTypePropertyAttribute);

            if (pagePropertyType == null)
            {
                string errorMessage = "Unable to find a valid EPiServer property type for the property {0} in the page type {1}";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, pageTypeName);
                throw new UnmappablePropertyTypeException(errorMessage);
            }

            PageDefinitionType pageDefinitionType;

            if (TypeIsNativePropertyType(pagePropertyType))
            {
                int nativeTypeID = GetNativeTypeID(pagePropertyType);
                pageDefinitionType = PageDefinitionTypeFactory.GetPageDefinitionType(nativeTypeID);
            }
            else
            {
                string pageDefinitionTypeName = pagePropertyType.FullName;
                string assemblyName = pagePropertyType.Assembly.GetName().Name;
                pageDefinitionType = PageDefinitionTypeFactory.GetPageDefinitionType(pageDefinitionTypeName, assemblyName);
            }
            return pageDefinitionType;
        }

        protected internal virtual Type GetPropertyType(Type propertyType, PageTypePropertyAttribute pageTypePropertyAttribute)
        {
            Type pagePropertyType = pageTypePropertyAttribute.Type;
            if (pagePropertyType == null)
            {
                pagePropertyType = GetDefaultPropertyType(propertyType);
            }
            return pagePropertyType;
        }

        private static Dictionary<Type, Type> defaultPropertyTypeMappings = new Dictionary<Type, Type>
        {
            {typeof(string), typeof(PropertyXhtmlString)},
            {typeof(int), typeof(PropertyNumber)},
            {typeof(int?), typeof(PropertyNumber)},
            {typeof(bool), typeof(PropertyBoolean)},
            {typeof(bool?), typeof(PropertyBoolean)},
            {typeof(DateTime), typeof(PropertyDate)},
            {typeof(DateTime?), typeof(PropertyDate)},
            {typeof(float), typeof(PropertyFloatNumber)},
            {typeof(float?), typeof(PropertyFloatNumber)},
            {typeof(PageReference), typeof(PropertyPageReference)},
            {typeof(PageType), typeof(PropertyPageType)},
            {typeof(LinkItemCollection), typeof(PropertyLinkCollection)}
        };

        public virtual Type GetDefaultPropertyType(Type propertyType)
        {
            if (defaultPropertyTypeMappings.ContainsKey(propertyType))
                return defaultPropertyTypeMappings[propertyType];

            return null;
        }

        public bool TypeIsNativePropertyType(Type pagePropertyType)
        {
            return NativePropertyTypes.Contains(pagePropertyType);
        }

        public virtual int GetNativeTypeID(Type pagePropertyType)
        {
            int? nativeTypeID = null;
            for (int typeID = 0; typeID < NativePropertyTypes.Length; typeID++)
            {
                if (NativePropertyTypes[typeID] == pagePropertyType)
                {
                    nativeTypeID = typeID;
                }
            }

            if (!nativeTypeID.HasValue)
            {
                string errorMessage = "Unable to retrieve native type ID. Type {0} is not a native type.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, pagePropertyType.FullName);
                throw new PageTypeBuilderException(errorMessage);
            }

            return nativeTypeID.Value;
        }

        public Type[] NativePropertyTypes
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
