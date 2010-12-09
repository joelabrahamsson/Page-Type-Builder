using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization.Validation
{
    public class PageTypeDefinitionValidator
    {
        public static int MaximumPageTypeNameLength = 50;

        readonly Type baseTypeForPageTypes = typeof(TypedPageData);

        public PageTypeDefinitionValidator(PageDefinitionTypeMapper pageDefinitionTypeMapper)
        {
            PropertiesValidator = new PageTypeDefinitionPropertiesValidator(pageDefinitionTypeMapper);
        }

        public virtual void ValidatePageTypeDefinitions(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            ValidatePageTypesHaveGuidOrUniqueName(pageTypeDefinitions);
            
            foreach (PageTypeDefinition definition in pageTypeDefinitions)
            {
                ValidatePageTypeDefinition(definition, pageTypeDefinitions);
            }
        }

        protected internal virtual void ValidatePageTypesHaveGuidOrUniqueName(IEnumerable<PageTypeDefinition> pageTypeDefinitions)
        {
            IEnumerable<IGrouping<string, PageTypeDefinition>> definitionsWithNoGuidAndSameName
                = pageTypeDefinitions
                    .Where(definition => definition.Attribute.Guid == null)
                    .GroupBy(definition => definition.GetPageTypeName())
                    .Where(groupedDefinitions => groupedDefinitions.Count() > 1);
            if (definitionsWithNoGuidAndSameName.Count() > 0)
            {
                string pageTypeName = null;
                string typeNames = null;
                const string separator = " and ";
                foreach (PageTypeDefinition definition in definitionsWithNoGuidAndSameName.First())
                {
                    pageTypeName = definition.GetPageTypeName();
                    typeNames += definition.Type.Name + " and ";
                }
                typeNames = typeNames.Remove(typeNames.Length - separator.Length);
                string errorMessage = "There are multiple types with the same page type name. The name is {0} and the types are {1}.";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, pageTypeName, typeNames);

                throw new PageTypeBuilderException(errorMessage);
            }
        }

        public virtual void ValidatePageTypeDefinition(PageTypeDefinition definition, IEnumerable<PageTypeDefinition> allPageTypeDefinitions)
        {
            ValidateNameLength(definition);

            ValidateInheritsFromBasePageType(definition);

            ValidateAvailablePageTypes(definition, allPageTypeDefinitions);

            PropertiesValidator.ValidatePageTypeProperties(definition);
        }

        protected internal virtual void ValidateNameLength(PageTypeDefinition definition)
        {
            if(definition.GetPageTypeName().Length <= MaximumPageTypeNameLength)
                return;

            string errorMessage = "The page type class {0} has a name that is longer than {1}. EPiServer does not save more than {1} characters and the name is often used to identify page types.";
            errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, definition.Type.Name, MaximumPageTypeNameLength);

            throw new PageTypeBuilderException(errorMessage);
        }

        protected internal virtual void ValidateInheritsFromBasePageType(PageTypeDefinition definition)
        {
            Type typeToCheck = definition.Type;
            if (!baseTypeForPageTypes.IsAssignableFrom(typeToCheck))
            {
                string errorMessage = "The type {0} has a {1} attribute but does not inherit from {2}";
                errorMessage = string.Format(CultureInfo.InvariantCulture, errorMessage, typeToCheck.FullName, typeof(PageTypeAttribute).FullName,
                                             baseTypeForPageTypes.FullName);

                throw new PageTypeBuilderException(errorMessage);
            }
        }

        protected internal virtual void ValidateAvailablePageTypes(PageTypeDefinition definition, IEnumerable<PageTypeDefinition> allPageTypeDefinitions)
        {
            if(definition.Attribute.AvailablePageTypes == null)
                return;

            foreach (Type availablePageType in definition.Attribute.AvailablePageTypes)
            {
                if (definition.Attribute.AvailablePageTypes.Count(t => t == availablePageType) > 1)
                    throw new PageTypeBuilderException(string.Format(CultureInfo.InvariantCulture, "The page type {0}'s AvailablePageType attribute contains the type {1} several times.", 
                        definition.Type.FullName, availablePageType.FullName));

                if (allPageTypeDefinitions.Count(d => d.Type.GUID == availablePageType.GUID) == 0)
                    throw new PageTypeBuilderException(string.Format(CultureInfo.InvariantCulture, "The page type {0} has the type {1} specified in it's AvailablePageType attribute "
                        + "which is not a defined page type", definition.Type.FullName, availablePageType.FullName));
            }
        }

        internal PageTypeDefinitionPropertiesValidator PropertiesValidator { get; set; }
    }
}