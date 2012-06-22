﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;

namespace PageTypeBuilder.Synchronization.Validation
{
    public class PageTypeDefinitionPropertiesValidator
    {
        PageDefinitionTypeMapper pageDefinitionTypeMapper;

        public PageTypeDefinitionPropertiesValidator(PageDefinitionTypeMapper pageDefinitionTypeMapper)
        {
            this.pageDefinitionTypeMapper = pageDefinitionTypeMapper;
        }

        protected internal virtual void ValidatePageTypeProperties(PageTypeDefinition definition)
        {
            List<PropertyInfo> propertiesForPageType = definition.Type.GetPageTypePropertiesOnClass().ToList();

            foreach (PropertyInfo propertyInfo in propertiesForPageType)
                ValidatePageTypeProperty(propertyInfo);

            // validate any page type property group propery defininitions
            foreach (PropertyInfo propertyGroupProperty in definition.Type.GetPageTypePropertyGroupProperties())
            {
                PropertyInfo[] propertyGroupProperties = propertyGroupProperty.PropertyType.GetPublicOrPrivateProperties();

                foreach (PropertyInfo property in propertyGroupProperties)
                {
                    PageTypePropertyAttribute attribute = property.GetCustomAttributes<PageTypePropertyAttribute>().FirstOrDefault();

                    if (attribute == null)
                        continue;

                    ValidatePageTypeProperty(property);
                }
            }            
        }

        protected internal virtual void ValidatePageTypeProperty(PropertyInfo propertyInfo)
        {
            ValidateCompilerGeneratedProperty(propertyInfo);
            ValidatePageTypePropertyAttribute(propertyInfo);
            ValidatePageTypePropertyType(propertyInfo);
        }

        protected internal virtual void ValidateCompilerGeneratedProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.GetterOrSetterIsCompilerGenerated())
                return;

            ValidateCompilerGeneratedPropertyGetterOrSetterNotPrivate(propertyInfo);

            if (propertyInfo.IsVirtual())
                return;

            string notVirtualErrorMessage = "{0} in {1} must be virtual as it is compiler generated and has {2}.";
            notVirtualErrorMessage = string.Format(CultureInfo.InvariantCulture, notVirtualErrorMessage, propertyInfo.Name,
                                         propertyInfo.DeclaringType.Name, typeof(PageTypePropertyAttribute).Name);
            throw new PageTypeBuilderException(notVirtualErrorMessage);
        }

        private void ValidateCompilerGeneratedPropertyGetterOrSetterNotPrivate(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetterOrSetterIsPrivate())
            {
                string privateGetterOrSetterErrorMessage = "{0} in {1} must not have a private getter or setter as it is compiler generated and has {2}.";
                privateGetterOrSetterErrorMessage = string.Format(CultureInfo.InvariantCulture, privateGetterOrSetterErrorMessage, propertyInfo.Name,
                                                                  propertyInfo.DeclaringType.Name, typeof(PageTypePropertyAttribute).Name);
                throw new PageTypeBuilderException(privateGetterOrSetterErrorMessage);
            }
        }

        protected internal virtual void ValidatePageTypePropertyAttribute(PropertyInfo propertyInfo)
        {
            PageTypePropertyAttribute propertyAttribute = propertyInfo.GetCustomAttributes<PageTypePropertyAttribute>().First();
            ValidatePageTypeAttributeTabProperty(propertyInfo, propertyAttribute);
        }

        protected internal virtual void ValidatePageTypeAttributeTabProperty(PropertyInfo propertyInfo, PageTypePropertyAttribute attribute)
        {
            if (attribute.Tab == null)
                return;

            if (!typeof(Tab).IsAssignableFrom(attribute.Tab))
            {

                string errorMessage =
                    "{0} in {1} has a {2} with Tab property set to type that does not inherit from {3}.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyInfo.Name,
                                             propertyInfo.DeclaringType.Name, typeof(PageTypePropertyAttribute).Name,
                                             typeof(Tab));
                throw new PageTypeBuilderException(errorMessage);
            }

            if (attribute.Tab.IsAbstract)
            {
                string errorMessage =
                    "{0} in {1} has a {2} with Tab property set to a type that is abstract.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyInfo.Name,
                                             propertyInfo.DeclaringType.Name, typeof(PageTypePropertyAttribute).Name);
                throw new PageTypeBuilderException(errorMessage);
            }
        }

        protected internal virtual void ValidatePageTypePropertyType(PropertyInfo propertyInfo)
        {
            PageTypePropertyAttribute pageTypePropertyAttribute =
                (PageTypePropertyAttribute)
                    propertyInfo.GetCustomAttributes(typeof(PageTypePropertyAttribute), false).First();

            if (pageDefinitionTypeMapper.GetPageDefinitionType(
                propertyInfo.DeclaringType.Name, propertyInfo.Name, propertyInfo.PropertyType, pageTypePropertyAttribute) == null)
            {
                string errorMessage = "Unable to map the type for the property {0} in {1} to a suitable EPiServer CMS property.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyInfo.Name, propertyInfo.DeclaringType.Name);
                throw new UnmappablePropertyTypeException(errorMessage);
            }
        }
    }
}