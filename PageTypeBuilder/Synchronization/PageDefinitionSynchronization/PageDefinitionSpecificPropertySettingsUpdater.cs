using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    public class PageDefinitionSpecificPropertySettingsUpdater
    {
        IPropertySettingsRepository propertySettingsRepository;
        IGlobalPropertySettingsLocator globalPropertySettingsLocator;
        IPageDefinitionFactory pageDefinitionFactory;

        public PageDefinitionSpecificPropertySettingsUpdater(
            IPropertySettingsRepository propertySettingsRepository,
            IGlobalPropertySettingsLocator globalPropertySettingsLocator,
            IPageDefinitionFactory pageDefinitionFactory)
        {
            this.propertySettingsRepository = propertySettingsRepository;
            this.globalPropertySettingsLocator = globalPropertySettingsLocator;
            this.pageDefinitionFactory = pageDefinitionFactory;
        }

        protected internal virtual void UpdatePropertySettings(PageTypeDefinition pageTypeDefinition, PageTypePropertyDefinition propertyDefinition, PageDefinition pageDefinition)
        {
            PropertySettingsContainer container = GetPropertySettingsContainer(pageDefinition);

            UpdatePageDefinitionsGlobalPropertySettings(propertyDefinition, pageTypeDefinition, container);
            UpdatePageDefinitionsLocalPropertySettings(propertyDefinition, pageTypeDefinition, container);
        }

        void UpdatePageDefinitionsLocalPropertySettings(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition, PropertySettingsContainer container)
        {
            List<PropertySettingsUpdater> settingsUpdaters = GetPropertySettingsUpdaters(pageTypeDefinition, propertyDefinition);
            settingsUpdaters.ForEach(updater =>
                {
                    var wrapper = container.GetSetting(updater.SettingsType);
                    if (wrapper == null)
                    {
                        wrapper = new PropertySettingsWrapper();
                        container.Settings[updater.SettingsType.FullName] = wrapper;
                        //TODO: Add spec validating that exception is thrown with the below uncommented (An item with the same key has already been added.)
                        //container.Settings.Add(updater.SettingsType.FullName, wrapper);
                    }

                    bool settingsAlreadyExists = true;
                    if (wrapper.PropertySettings == null)
                    {
                        wrapper.PropertySettings = ((IPropertySettings)Activator.CreateInstance(updater.SettingsType)).GetDefaultValues();
                        settingsAlreadyExists = false;
                    }

                    if (settingsAlreadyExists && !updater.OverWriteExisting)
                        return;

                    int hashBeforeUpdate = updater.GetSettingsHashCode(wrapper.PropertySettings);
                    updater.UpdateSettings(wrapper.PropertySettings);
                    int hashAfterUpdate = updater.GetSettingsHashCode(wrapper.PropertySettings);
                    if (hashBeforeUpdate != hashAfterUpdate || !settingsAlreadyExists)
                    {
                        propertySettingsRepository.Save(container);
                    }
                });
        }

        void UpdatePageDefinitionsGlobalPropertySettings(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition, PropertySettingsContainer container)
        {
            object[] attributes = GetPropertyAttributes(propertyDefinition, pageTypeDefinition);
            var useGlobalSettingsAttribute = attributes.OfType<UseGlobalSettingsAttribute>().FirstOrDefault();
            if (useGlobalSettingsAttribute != null)
            {
                //TODO: Should validate not null and valid type at startup
                var globalSettingsUpdater = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters().Where(u => u.WrappedInstanceType == useGlobalSettingsAttribute.Type).First();
                var wrapper = propertySettingsRepository.GetGlobals(globalSettingsUpdater.SettingsType)
                    .Where(w => globalSettingsUpdater.Match(w))
                    .First();
                container.Settings[globalSettingsUpdater.SettingsType.FullName] = wrapper;
                //TODO: Add spec validating that exception is thrown with the below uncommented (An item with the same key has already been added.)
                //container.Settings.Add(globalSettingsUpdater.SettingsType.FullName, wrapper);
                propertySettingsRepository.Save(container);
            }
        }

        private PropertySettingsContainer GetPropertySettingsContainer(PageDefinition pageDefinition)
        {
            PropertySettingsContainer container;

            if (pageDefinition.SettingsID == Guid.Empty)
            {
                pageDefinition.SettingsID = Guid.NewGuid();
                pageDefinitionFactory.Save(pageDefinition);
                container = new PropertySettingsContainer(pageDefinition.SettingsID);
            }
            else
            {
                if (!propertySettingsRepository.TryGetContainer(pageDefinition.SettingsID, out container))
                {
                    container = new PropertySettingsContainer(pageDefinition.SettingsID);
                }
            }
            return container;
        }

        private List<PropertySettingsUpdater> GetPropertySettingsUpdaters(PageTypeDefinition pageTypeDefinition, PageTypePropertyDefinition propertyDefinition)
        {
            object[] attributes = GetPropertyAttributes(propertyDefinition, pageTypeDefinition);
            var settingsUpdaters = new List<PropertySettingsUpdater>();
            foreach (var attribute in attributes)
            {
                foreach (var interfaceType in attribute.GetType().GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    if (!typeof(IUpdatePropertySettings<>).IsAssignableFrom(interfaceType.GetGenericTypeDefinition()))
                        continue;
                    var settingsType = interfaceType.GetGenericArguments().First();
                    var updater = new PropertySettingsUpdater(settingsType, attribute);
                    settingsUpdaters.Add(updater);
                }
            }
            return settingsUpdaters;
        }

        private object[] GetPropertyAttributes(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition)
        {
            PropertyInfo prop;

            if (propertyDefinition.Name.Contains("-"))
            {
                // the property definition is a property belonging to a property group
                int index = propertyDefinition.Name.IndexOf("-");
                string propertyGroupPropertyName = propertyDefinition.Name.Substring(0, index);
                string propertyName = propertyDefinition.Name.Substring(index + 1);

                PropertyInfo propertyGroupProperty = pageTypeDefinition.Type.GetProperties().Where(p => String.Equals(p.Name, propertyGroupPropertyName)).FirstOrDefault();
                prop = propertyGroupProperty.PropertyType.GetProperties().Where(p => String.Equals(p.Name, propertyName)).FirstOrDefault();
            }
            else
            {
                prop =
                    pageTypeDefinition.Type.GetProperties().Where(p => String.Equals(p.Name, propertyDefinition.Name)).
                        FirstOrDefault();
            }
            return prop.GetCustomAttributes(true);
        }
    }
}