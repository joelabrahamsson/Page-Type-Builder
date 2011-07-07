using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using log4net;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    public class PageDefinitionUpdater : IPageDefinitionUpdater
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageDefinitionSynchronizationEngine));
        private IPageDefinitionRepository pageDefinitionRepository;
        private ITabDefinitionRepository tabDefinitionRepository;
        private PageDefinitionTypeMapper pageDefinitionTypeMapper;
        private List<string> newlyCreatedPageDefinitions;

        public PageDefinitionUpdater(
            IPageDefinitionRepository pageDefinitionRepository, 
            ITabDefinitionRepository tabDefinitionRepository,
            PageDefinitionTypeMapper pageDefinitionTypeMapper)
        {
            this.pageDefinitionRepository = pageDefinitionRepository;
            this.tabDefinitionRepository = tabDefinitionRepository;
            this.pageDefinitionTypeMapper = pageDefinitionTypeMapper;
            newlyCreatedPageDefinitions = new List<string>();
        }

        public virtual void CreateNewPageDefinition(PageTypePropertyDefinition propertyDefinition)
        {
            PageDefinition pageDefinition = new PageDefinition();
            pageDefinition.PageTypeID = propertyDefinition.PageType.ID;
            pageDefinition.Name = propertyDefinition.Name;
            pageDefinition.EditCaption = propertyDefinition.GetEditCaptionOrName();
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

        public virtual void UpdateExistingPageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            string oldValues = SerializeValues(pageDefinition);

            UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition);

            string updatedValues = SerializeValues(pageDefinition);
            if (updatedValues != oldValues)
            {
                log.Debug(string.Format("Updating PageDefintion, old values: {0}, new values: {1}.", oldValues, updatedValues));
                pageDefinitionRepository.Save(pageDefinition);
            }
        }

        protected virtual string SerializeValues(PageDefinition pageDefinition)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Name: ");
            builder.Append(pageDefinition.Name);
            builder.Append("|");
            builder.Append("Type: ");
            builder.Append(pageDefinition.Type.TypeName);
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
            builder.Append(pageDefinition.DefaultValue);
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

        protected virtual void UpdatePageDefinitionValues(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            pageDefinition.Name = pageTypePropertyDefinition.Name;

            PageTypePropertyAttribute propertyAttribute = pageTypePropertyDefinition.PageTypePropertyAttribute;
            
            var specifiedType = GetPageDefinitionType(pageTypePropertyDefinition);
            var currentType = pageDefinition.Type;
            
            if(specifiedType.DataType == currentType.DataType)
                pageDefinition.Type = specifiedType;

            if (CanModifyProperty(pageDefinition, propertyAttribute.EditCaptionSet))
                pageDefinition.EditCaption = pageTypePropertyDefinition.GetEditCaptionOrName();

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

            if (CanModifyProperty(pageDefinition, propertyAttribute.SortOrderSet))
                pageDefinition.FieldOrder = GetFieldOrder(pageDefinition, propertyAttribute);

            if (CanModifyProperty(pageDefinition, propertyAttribute.TabSet))
                UpdatePageDefinitionTab(pageDefinition, propertyAttribute);
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
                Tab definedTab = (Tab) Activator.CreateInstance(propertyAttribute.Tab);
                tab = tabDefinitionRepository.GetTabDefinition(definedTab.Name);
            }
            pageDefinition.Tab = tab;
        }
    }
}
