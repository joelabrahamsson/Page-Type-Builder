namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using log4net;
    using Abstractions;
    using Discovery;

    public class PageDefinitionUpdater : IPageDefinitionUpdater
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageDefinitionSynchronizationEngine));
        private IPageDefinitionRepository pageDefinitionRepository;
        private ITabDefinitionRepository tabDefinitionRepository;
        private PageDefinitionTypeMapper pageDefinitionTypeMapper;
        private List<string> newlyCreatedPageDefinitions;
        private List<int> updatedPageDefinitions;

        public List<int> UpdatedPageDefinitions
        {
            get { return updatedPageDefinitions; }
        }

        public PageDefinitionUpdater(
            IPageDefinitionRepository pageDefinitionRepository,
            ITabDefinitionRepository tabDefinitionRepository,
            PageDefinitionTypeMapper pageDefinitionTypeMapper)
        {
            this.pageDefinitionRepository = pageDefinitionRepository;
            this.tabDefinitionRepository = tabDefinitionRepository;
            this.pageDefinitionTypeMapper = pageDefinitionTypeMapper;
            newlyCreatedPageDefinitions = new List<string>();
            updatedPageDefinitions = new List<int>();
        }

        public virtual void CreateNewPageDefinition(PageTypePropertyDefinition propertyDefinition)
        {
            PageDefinition pageDefinition = new PageDefinition();
            pageDefinition.PageTypeID = propertyDefinition.PageType.ID;
            pageDefinition.Name = propertyDefinition.Name;
            pageDefinition.EditCaption = propertyDefinition.GetEditCaptionOrName(false);
            SetPageDefinitionType(pageDefinition, propertyDefinition);

            newlyCreatedPageDefinitions.Add(GetPageDefinitionKey(pageDefinition));
            UpdatePageDefinitionValues(pageDefinition, propertyDefinition);

            pageDefinitionRepository.Save(pageDefinition);
        }

        protected internal virtual void SetPageDefinitionType(PageDefinition pageDefinition, PageTypePropertyDefinition propertyDefinition)
        {
            pageDefinition.Type = GetPageDefinitionType(propertyDefinition);
        }

        protected internal virtual PageDefinitionType GetPageDefinitionType(PageTypePropertyDefinition definition)
        {
            return pageDefinitionTypeMapper.GetPageDefinitionType(definition);
        }

        private void HandleDefaultValues(PageDefinition pageDefinition)
        {
            if (pageDefinition.DefaultValue != null && pageDefinition.DefaultValue == string.Empty)
                pageDefinition.DefaultValue = null;

            if (pageDefinition.DefaultValue != null)
            {
                object value;

                try
                {
                    value = PropertyData.CreatePropertyDataObject(pageDefinition.Type.DataType).ParseToObject(pageDefinition.DefaultValue).Value;
                }
                catch
                {
                    value = null;
                }

                if (value == null)
                {
                    pageDefinition.DefaultValue = null;
                }
            }

            if (pageDefinition.DefaultValue == null && pageDefinition.DefaultValueType == DefaultValueType.Value && pageDefinition.Type.DataType != PropertyDataType.Boolean)
                pageDefinition.DefaultValueType = DefaultValueType.None;
        }

        public virtual void UpdateExistingPageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            // LC: Reset default value if no default value type is defined
            if (pageTypePropertyDefinition.PageTypePropertyAttribute.DefaultValueSet && pageTypePropertyDefinition.PageTypePropertyAttribute.DefaultValueType == DefaultValueType.None || pageTypePropertyDefinition.PageTypePropertyAttribute.DefaultValueType == DefaultValueType.Inherit)
                pageTypePropertyDefinition.PageTypePropertyAttribute.DefaultValue = null;

            string oldValues = SerializeValues(pageDefinition);

            UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition);

            // LC: Change to follow logic in PageDefintion.Save for resetting default values and default value types
            HandleDefaultValues(pageDefinition);

            string updatedValues = SerializeValues(pageDefinition);

            if (updatedValues == oldValues) 
                return;

            log.Debug(string.Format("Updating PageDefintion, old values: {0}, new values: {1}.", oldValues, updatedValues));
                
            using (new TimingsLogger(string.Format("Updating page definition '{0}', page type: {1}{2}{3}{2}{4}", pageDefinition.Name, pageDefinition.PageTypeID, Environment.NewLine, oldValues, updatedValues)))
            {
                pageDefinitionRepository.Save(pageDefinition);
            }

            updatedPageDefinitions.Add(pageDefinition.ID);
        }

        protected virtual string SerializeValues(PageDefinition pageDefinition)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Name: ");
            builder.Append(pageDefinition.Name);
            builder.Append("|");
            builder.Append("Type: ");
            //builder.Append(pageDefinition.Type.TypeName);
            builder.Append(string.Format("{0}_{1}", pageDefinition.Type.ID, pageDefinition.Type.Name));
            builder.Append("|");
            builder.Append("EditCaption:");
            builder.Append(pageDefinition.EditCaption);
            builder.Append("|");
            builder.Append("HelpText:");
            builder.Append(pageDefinition.HelpText);
            builder.Append("|");
            builder.Append("Required:");
            builder.Append(pageDefinition.Required);
            builder.Append("|");
            builder.Append("Searchable:");
            builder.Append(pageDefinition.Searchable);
            builder.Append("|");
            builder.Append("DefaultValue:");
            
            string defaultValue = pageDefinition.DefaultValue;

            if (pageDefinition.Type.DataType == PropertyDataType.Boolean && string.Equals(defaultValue, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(defaultValue, "false", StringComparison.OrdinalIgnoreCase))
                defaultValue = defaultValue.ToLower();

            builder.Append(defaultValue);
            builder.Append("|");
            builder.Append("DefaultValueType:");
            builder.Append(pageDefinition.DefaultValueType);
            builder.Append("|");
            builder.Append("LanguageSpecific:");
            builder.Append(pageDefinition.LanguageSpecific);
            builder.Append("|");
            builder.Append("DisplayEditUI:");
            builder.Append(pageDefinition.DisplayEditUI);
            builder.Append("|");
            builder.Append("FieldOrder:");
            builder.Append(pageDefinition.FieldOrder);
            builder.Append("|");
            builder.Append("Tab.ID:");
            builder.Append(pageDefinition.Tab.ID);
            builder.Append("|");

            return builder.ToString();
        }

        private bool CanModifyProperty(PageDefinition pageDefinition, bool propertySet)
        {
            return newlyCreatedPageDefinitions.Contains(GetPageDefinitionKey(pageDefinition)) || propertySet;
        }

        private string GetPageDefinitionKey(PageDefinition pageDefinition)
        {
            return string.Format("{0}_{1}", pageDefinition.PageTypeID, pageDefinition.Name);
        }

        protected virtual void UpdatePageDefinitionValues(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition,
            bool propertyGroupOverride)
        {
            if (!propertyGroupOverride)
                pageDefinition.Name = pageTypePropertyDefinition.Name;

            PageTypePropertyAttribute propertyAttribute = propertyGroupOverride
                                                              ? pageTypePropertyDefinition.PageTypePropertyGroupPropertyOverrideAttribute
                                                              : pageTypePropertyDefinition.PageTypePropertyAttribute;

            if (!propertyGroupOverride)
            {
                var specifiedType = GetPageDefinitionType(pageTypePropertyDefinition);
                var currentType = pageDefinition.Type;

                if (specifiedType.DataType == currentType.DataType)
                    pageDefinition.Type = specifiedType;

                if (CanModifyProperty(pageDefinition, propertyAttribute.TabSet))
                    UpdatePageDefinitionTab(pageDefinition, propertyAttribute);

                if (CanModifyProperty(pageDefinition, propertyAttribute.SortOrderSet))
                    pageDefinition.FieldOrder = GetFieldOrder(pageDefinition, propertyAttribute);
            }

            if (CanModifyProperty(pageDefinition, propertyAttribute.EditCaptionSet))
                pageDefinition.EditCaption = pageTypePropertyDefinition.GetEditCaptionOrName(propertyGroupOverride);
            else if (!propertyAttribute.EditCaptionSet && string.IsNullOrEmpty(pageDefinition.EditCaption))
                pageDefinition.EditCaption = pageTypePropertyDefinition.GetEditCaptionOrName(propertyGroupOverride);

            if (CanModifyProperty(pageDefinition, propertyAttribute.HelpTextSet))
                pageDefinition.HelpText = propertyAttribute.HelpText ?? string.Empty;

            if (CanModifyProperty(pageDefinition, propertyAttribute.RequiredSet))
                pageDefinition.Required = propertyAttribute.Required;

            if (CanModifyProperty(pageDefinition, propertyAttribute.SearchableSet))
                pageDefinition.Searchable = propertyAttribute.Searchable;

            if (CanModifyProperty(pageDefinition, propertyAttribute.DefaultValueSet))
                pageDefinition.DefaultValue = propertyAttribute.DefaultValue != null ? propertyAttribute.DefaultValue.ToString() : string.Empty;

            if (CanModifyProperty(pageDefinition, propertyAttribute.DefaultValueTypeSet))
                pageDefinition.DefaultValueType = propertyAttribute.DefaultValueType;

            if (CanModifyProperty(pageDefinition, propertyAttribute.UniqueValuePerLanguageSet))
                pageDefinition.LanguageSpecific = propertyAttribute.UniqueValuePerLanguage;

            if (CanModifyProperty(pageDefinition, propertyAttribute.DisplayInEditModeSet))
                pageDefinition.DisplayEditUI = propertyAttribute.DisplayInEditMode;
        }

        protected virtual void UpdatePageDefinitionValues(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition, false);

            if (pageTypePropertyDefinition.PageTypePropertyGroupPropertyOverrideAttribute != null)
                UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition, true);
        }

        protected virtual int GetFieldOrder(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            int fieldOrder = propertyAttribute.SortOrder;

            if (fieldOrder == PageTypePropertyAttribute.SortOrderNoValue)
            {
                fieldOrder = 0;

                if (pageDefinition.FieldOrder != 0)
                    fieldOrder = pageDefinition.FieldOrder;
            }

            return fieldOrder;
        }

        protected virtual void UpdatePageDefinitionTab(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            var tab = tabDefinitionRepository.List().First();
            if (propertyAttribute.Tab != null)
            {
                Tab definedTab = (Tab)Activator.CreateInstance(propertyAttribute.Tab);
                tab = tabDefinitionRepository.GetTabDefinition(definedTab.Name);
            }
            pageDefinition.Tab = tab;
        }
    }
}
