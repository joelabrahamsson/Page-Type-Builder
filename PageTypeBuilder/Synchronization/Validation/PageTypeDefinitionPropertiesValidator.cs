using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;

namespace PageTypeBuilder.Synchronization.Validation
{
    public class PageTypeDefinitionPropertiesValidator
    {
        public PageTypeDefinitionPropertiesValidator(PageDefinitionTypeMapper pageDefinitionTypeMapper)
        {
            PageDefinitionTypeMapper = pageDefinitionTypeMapper;
        }

        protected internal virtual void ValidatePageTypeProperties(PageTypeDefinition definition)
        {
            ValidateNoClashingPropertiesFromInterfaces(definition.Type);
            IEnumerable<PropertyInfo> propertiesForPageType = definition.Type.GetAllValidPageTypePropertiesFromClassAndImplementedInterfaces();

            foreach (PropertyInfo propertyInfo in propertiesForPageType)
            {
                ValidatePageTypeProperty(propertyInfo);
            }
        }

        public virtual void ValidateNoClashingPropertiesFromInterfaces(Type pageTypeType)
        {
            var propertiesOnClass = pageTypeType.GetPageTypePropertiesOnClass();
            IEnumerable<PropertyInfo> propertiesFromInterfaces = pageTypeType.GetPageTypePropertiesFromInterfaces();
            
            var groups = propertiesFromInterfaces.GroupBy(propertyInfo => propertyInfo.Name);
            foreach (var propertiesWithSameNameFromInterfaces in groups)
            {
                if (propertiesWithSameNameFromInterfaces.Count() == 1)
                    continue;

                if (ClassOverridesInterfaceProperty(propertiesWithSameNameFromInterfaces.Key, propertiesOnClass))
                    continue;
                
                var interfaceCount = propertiesWithSameNameFromInterfaces.Count();
                var className = pageTypeType.Name;
                var propertyName = propertiesWithSameNameFromInterfaces.Key;
                throw new PageTypeBuilderException(string.Format("{0} separate interfaces implemented by the class {1} provide a PageTypeProperty definition for the property named {2}. Either remove the ambiguity by renaming properties in the interfaces or provide the class {1} with its own PageTypeProperty definition on the property which will trump the ambiguities in the interfaces.",
                                                                    interfaceCount, className, propertyName));
            }
        }

        private bool ClassOverridesInterfaceProperty(string propertyName, IEnumerable<PropertyInfo> propertiesOnClass)
        {
            return propertiesOnClass.Any(propertyInfo => propertyInfo.Name == propertyName);
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

            if (PageDefinitionTypeMapper.GetPageDefinitionType(
                propertyInfo.DeclaringType.Name, propertyInfo.Name, propertyInfo.PropertyType, pageTypePropertyAttribute) == null)
            {
                string errorMessage = "Unable to map the type for the property {0} in {1} to a suitable EPiServer CMS property.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, propertyInfo.Name, propertyInfo.DeclaringType.Name);
                throw new UnmappablePropertyTypeException(errorMessage);
            }
        }

        internal PageDefinitionTypeMapper PageDefinitionTypeMapper { get; set; }


    }
}