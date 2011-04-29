using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using log4net;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class PageTypePropertyUpdater
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageTypePropertyUpdater));

        public PageTypePropertyUpdater()
        {
            PageDefinitionFactory = new PageDefinitionFactory();
            PageDefinitionTypeFactory = new PageDefinitionTypeFactory();
            PageTypePropertyDefinitionLocator = new PageTypePropertyDefinitionLocator();
            PageDefinitionTypeMapper = new PageDefinitionTypeMapper(PageDefinitionTypeFactory);
        }

        protected internal virtual void UpdatePageTypePropertyDefinitions(PageType pageType, PageTypeDefinition pageTypeDefinition)
        {
            List<PageTypePropertyDefinition> definitions = 
                PageTypePropertyDefinitionLocator.GetPageTypePropertyDefinitions(pageType, pageTypeDefinition.Type);
            
            foreach (PageTypePropertyDefinition propertyDefinition in definitions)
            {
                PageDefinition pageDefinition = GetExistingPageDefinition(pageType, propertyDefinition);
                if (pageDefinition == null)
                    pageDefinition = CreateNewPageDefinition(propertyDefinition);

                UpdatePageDefinition(pageDefinition, propertyDefinition);
            }
        }

        protected internal virtual PageDefinition GetExistingPageDefinition(PageType pageType, PageTypePropertyDefinition propertyDefinition)
        {
            return pageType.Definitions.FirstOrDefault(definition => definition.Name == propertyDefinition.Name);
        }

        protected internal virtual PageDefinition CreateNewPageDefinition(PageTypePropertyDefinition propertyDefinition)
        {
            PageDefinition pageDefinition = new PageDefinition();
            pageDefinition.PageTypeID = propertyDefinition.PageType.ID;
            pageDefinition.Name = propertyDefinition.Name;
            pageDefinition.EditCaption = propertyDefinition.GetEditCaptionOrName();
            SetPageDefinitionType(pageDefinition, propertyDefinition);

            PageDefinitionFactory.Save(pageDefinition);
            
            return pageDefinition;
        }

        protected internal virtual void SetPageDefinitionType(PageDefinition pageDefinition, PageTypePropertyDefinition propertyDefinition)
        {
            pageDefinition.Type = GetPageDefinitionType(propertyDefinition);
        }

        protected internal virtual PageDefinitionType GetPageDefinitionType(PageTypePropertyDefinition definition)
        {
            return PageDefinitionTypeMapper.GetPageDefinitionType(definition);
        }

        protected internal virtual void UpdatePageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            string oldValues = SerializeValues(pageDefinition);

            UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition);

            string updatedValues = SerializeValues(pageDefinition);
            if (updatedValues != oldValues)
            {
                log.Debug(string.Format("Updating PageDefintion, old values: {0}, new values: {1}.", oldValues, updatedValues));
                PageDefinitionFactory.Save(pageDefinition);
            }
        }

        protected internal virtual string SerializeValues(PageDefinition pageDefinition)
        {
            StringBuilder builder = new StringBuilder();

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
            builder.Append("LongStringSettings:");
            builder.Append(pageDefinition.LongStringSettings);
            builder.Append("|");
            builder.Append("Tab.ID:");
            builder.Append(pageDefinition.Tab.ID);
            builder.Append("|");

            return builder.ToString();
        }

        protected internal virtual void UpdatePageDefinitionValues(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            PageTypePropertyAttribute propertyAttribute = pageTypePropertyDefinition.PageTypePropertyAttribute;

            pageDefinition.EditCaption = pageTypePropertyDefinition.GetEditCaptionOrName();
            pageDefinition.HelpText = propertyAttribute.HelpText ?? string.Empty;
            pageDefinition.Required = propertyAttribute.Required;
            pageDefinition.Searchable = propertyAttribute.Searchable;
            pageDefinition.DefaultValue = propertyAttribute.DefaultValue != null ? propertyAttribute.DefaultValue.ToString() : string.Empty;
            pageDefinition.DefaultValueType = propertyAttribute.DefaultValueType;
            pageDefinition.LanguageSpecific = propertyAttribute.UniqueValuePerLanguage;
            pageDefinition.DisplayEditUI = propertyAttribute.DisplayInEditMode;
            pageDefinition.FieldOrder = propertyAttribute.SortOrder;
            UpdateLongStringSettings(pageDefinition, propertyAttribute);
            UpdatePageDefinitionTab(pageDefinition, propertyAttribute);
        }

        protected internal virtual void UpdatePageDefinitionTab(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            TabDefinition tab = TabDefinition.List().First();
            if (propertyAttribute.Tab != null)
            {
                Tab definedTab = (Tab) Activator.CreateInstance(propertyAttribute.Tab);
                tab = TabDefinition.Load(definedTab.Name);
            }
            pageDefinition.Tab = tab;
        }

        private void UpdateLongStringSettings(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            EditorToolOption longStringSettings = propertyAttribute.LongStringSettings;
            if (longStringSettings == EditorToolOption.Undefined)
            {
                return;
            }

            if (longStringSettings == default(EditorToolOption) && !propertyAttribute.ClearAllLongStringSettings)
            {
                longStringSettings = EditorToolOption.All;
            }
            pageDefinition.LongStringSettings = longStringSettings;
        }

        internal PageTypePropertyDefinitionLocator PageTypePropertyDefinitionLocator { get; set; }

        internal PageDefinitionFactory PageDefinitionFactory { get; set; }

        internal PageDefinitionTypeFactory PageDefinitionTypeFactory { get; set; }

        internal PageDefinitionTypeMapper PageDefinitionTypeMapper { get; set; }
    }
}