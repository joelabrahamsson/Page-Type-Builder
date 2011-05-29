using System;
using System.Linq;
using System.Text;
using EPiServer.DataAbstraction;
using log4net;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public class PageDefinitionUpdater : IPageDefinitionUpdater
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageDefinitionSynchronizationEngine));
        private IPageDefinitionFactory pageDefinitionFactory;
        private ITabFactory tabFactory;

        public PageDefinitionUpdater(
            IPageDefinitionFactory pageDefinitionFactory, 
            ITabFactory tabFactory)
        {
            this.pageDefinitionFactory = pageDefinitionFactory;
            this.tabFactory = tabFactory;
        }

        public virtual void UpdatePageDefinition(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            string oldValues = SerializeValues(pageDefinition);

            UpdatePageDefinitionValues(pageDefinition, pageTypePropertyDefinition);

            string updatedValues = SerializeValues(pageDefinition);
            if (updatedValues != oldValues)
            {
                log.Debug(string.Format("Updating PageDefintion, old values: {0}, new values: {1}.", oldValues, updatedValues));
                pageDefinitionFactory.Save(pageDefinition);
            }
        }

        protected virtual string SerializeValues(PageDefinition pageDefinition)
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
            builder.Append("Tab.ID:");
            builder.Append(pageDefinition.Tab.ID);
            builder.Append("|"); 

            return builder.ToString();
        }

        protected virtual void UpdatePageDefinitionValues(PageDefinition pageDefinition, PageTypePropertyDefinition pageTypePropertyDefinition)
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
            pageDefinition.FieldOrder = GetFieldOrder(pageDefinition, propertyAttribute);
            UpdatePageDefinitionTab(pageDefinition, propertyAttribute);
        }

        protected virtual int GetFieldOrder(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            int fieldOrder = propertyAttribute.SortOrder;
            if(fieldOrder == PageTypePropertyAttribute.SortOrderNoValue)
            {
                fieldOrder = 0;
                if(pageDefinition.FieldOrder != 0)
                {
                    fieldOrder = pageDefinition.FieldOrder;
                }
            }
            return fieldOrder;
        }

        protected virtual void UpdatePageDefinitionTab(PageDefinition pageDefinition, PageTypePropertyAttribute propertyAttribute)
        {
            var tab = tabFactory.List().First();
            if (propertyAttribute.Tab != null)
            {
                Tab definedTab = (Tab) Activator.CreateInstance(propertyAttribute.Tab);
                tab = tabFactory.GetTabDefinition(definedTab.Name);
            }
            pageDefinition.Tab = tab;
        }
    }
}
