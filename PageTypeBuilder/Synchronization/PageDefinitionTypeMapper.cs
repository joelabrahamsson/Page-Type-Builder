using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IPageDefinitionTypeFactory pageDefinitionTypeFactory;
        private readonly INativePageDefinitionsMap nativePageDefinitionsMap;

        public PageDefinitionTypeMapper(IPageDefinitionTypeFactory pageDefinitionTypeFactory, INativePageDefinitionsMap nativePageDefinitionsMap)
        {
            this.pageDefinitionTypeFactory = pageDefinitionTypeFactory;
            this.nativePageDefinitionsMap = nativePageDefinitionsMap;
        }

        public virtual PageDefinitionType GetPageDefinitionType(PageTypePropertyDefinition definition)
        {
            return GetPageDefinitionType(
                definition.PageType.Name, definition.Name, definition.PropertyType, definition.PageTypePropertyAttribute);

        }

        public virtual PageDefinitionType GetPageDefinitionType(
            string pageTypeName, string propertyName, 
            Type propertyType, PageTypePropertyAttribute pageTypePropertyAttribute)
        {
            Type pagePropertyType = GetPropertyType(propertyType, pageTypePropertyAttribute);

            if (pagePropertyType == null)
            {
                ThrowUnmappablePropertyTypeException(propertyName, pageTypeName);
            }

            return GetPageDefinitionTypeImpl(pagePropertyType);
        }

        PageDefinitionType GetPageDefinitionTypeImpl(Type pagePropertyType)
        {
            if (nativePageDefinitionsMap.TypeIsNativePropertyType(pagePropertyType))
            {
                return GetNonNativePageDefinitionType(pagePropertyType);
            }
            return GetNativePageDefinitionType(pagePropertyType);
        }

        PageDefinitionType GetNativePageDefinitionType(Type pagePropertyType)
        {
            string pageDefinitionTypeName = pagePropertyType.FullName;
            string assemblyName = pagePropertyType.Assembly.GetName().Name;
            return pageDefinitionTypeFactory.GetPageDefinitionType(pageDefinitionTypeName, assemblyName);
        }

        PageDefinitionType GetNonNativePageDefinitionType(Type pagePropertyType)
        {
            int nativeTypeId = nativePageDefinitionsMap.GetNativeTypeID(pagePropertyType);
            return pageDefinitionTypeFactory.GetPageDefinitionType(nativeTypeId);
        }

        protected void ThrowUnmappablePropertyTypeException(string propertyName, string pageTypeName)
        {
            string errorMessage = "Unable to find a valid EPiServer property type for the property {0} in the page type {1}";
            errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, pageTypeName);
            throw new UnmappablePropertyTypeException(errorMessage);
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
    }
}
