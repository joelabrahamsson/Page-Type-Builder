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
        private readonly IPropertySettingsRepository propertySettingsRepository;
        private readonly IGlobalPropertySettingsLocator globalPropertySettingsLocator;
        private readonly IPageDefinitionRepository pageDefinitionRepository;

        public PageDefinitionSpecificPropertySettingsUpdater(
            IPropertySettingsRepository propertySettingsRepository,
            IGlobalPropertySettingsLocator globalPropertySettingsLocator,
            IPageDefinitionRepository pageDefinitionRepository)
        {
            this.propertySettingsRepository = propertySettingsRepository;
            this.globalPropertySettingsLocator = globalPropertySettingsLocator;
            this.pageDefinitionRepository = pageDefinitionRepository;
        }

        protected internal virtual void UpdatePropertySettings(PageTypeDefinition pageTypeDefinition, PageTypePropertyDefinition propertyDefinition, PageDefinition pageDefinition)
        {
            UpdatePageDefinitionsGlobalPropertySettings(propertyDefinition, pageTypeDefinition, pageDefinition);
            UpdatePageDefinitionsLocalPropertySettings(propertyDefinition, pageTypeDefinition, pageDefinition);
        }

        void UpdatePageDefinitionsLocalPropertySettings(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition, PageDefinition pageDefinition)
        {
            var settingsUpdaters = GetPropertySettingsUpdaters(pageTypeDefinition, propertyDefinition);
            if (settingsUpdaters.Count == 0)
            {
                return;
            }

            var container = GetPropertySettingsContainer(pageDefinition);
            settingsUpdaters.ForEach(updater =>
            {
                var wrapper = container.GetSetting(updater.SettingsType);
                if (wrapper == null)
                {
                    var settingsTypeName = updater.SettingsType.FullName;
                    if (settingsTypeName == null)
                    {
                        throw new PageTypeBuilderException("Missing full type name for PropertySettingsUpdater.SettingsType (FullName is null).");
                    }

                    wrapper = new PropertySettingsWrapper();
                    container.Settings[settingsTypeName] = wrapper;
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
                {
                    return;
                }

                int hashBeforeUpdate = updater.GetSettingsHashCode(wrapper.PropertySettings);
                updater.UpdateSettings(wrapper.PropertySettings);
                int hashAfterUpdate = updater.GetSettingsHashCode(wrapper.PropertySettings);
                if (hashBeforeUpdate != hashAfterUpdate || !settingsAlreadyExists)
                {
                    propertySettingsRepository.Save(container);
                }
            });
        }

        void UpdatePageDefinitionsGlobalPropertySettings(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition, PageDefinition pageDefinition)
        {
            IEnumerable<object> attributes = GetPropertyAttributes(propertyDefinition, pageTypeDefinition);
            var useGlobalSettingsAttribute = attributes.OfType<UseGlobalSettingsAttribute>().FirstOrDefault();
            if (useGlobalSettingsAttribute != null)
            {
                var container = GetPropertySettingsContainer(pageDefinition);

                //TODO: Should validate not null and valid type at startup
                var globalSettingsUpdater = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters()
                    .First(u => u.WrappedInstanceType == useGlobalSettingsAttribute.Type);
                var wrapper = propertySettingsRepository.GetGlobals(globalSettingsUpdater.SettingsType)
                    .First(globalSettingsUpdater.Match);

                var settingsTypeName = globalSettingsUpdater.SettingsType.FullName;
                if (settingsTypeName == null)
                {
                    throw new PageTypeBuilderException("Missing full type name for SettingsType (FullName is null).");
                }

                PropertySettingsWrapper existingWrapper = container.Settings.ContainsKey(settingsTypeName)
                    ? container.Settings[settingsTypeName]
                    : null;
                if (existingWrapper == null || existingWrapper.Id != wrapper.Id)
                {
                    container.Settings[settingsTypeName] = wrapper;
                    //TODO: Add spec validating that exception is thrown with the below uncommented (An item with the same key has already been added.)
                    //container.Settings.Add(globalSettingsUpdater.SettingsType.FullName, wrapper);
                    propertySettingsRepository.Save(container);
                }
            }
        }

        private PropertySettingsContainer GetPropertySettingsContainer(PageDefinition pageDefinition)
        {
            PropertySettingsContainer container;

            if (pageDefinition.SettingsID == Guid.Empty)
            {
                pageDefinition.SettingsID = Guid.NewGuid();
                pageDefinitionRepository.Save(pageDefinition);
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

        private static List<PropertySettingsUpdater> GetPropertySettingsUpdaters(PageTypeDefinition pageTypeDefinition, PageTypePropertyDefinition propertyDefinition)
        {
            var updaters =
                from attribute in GetPropertyAttributes(propertyDefinition, pageTypeDefinition)
                from interfaceType in attribute.GetType().GetInterfaces()
                where interfaceType.IsGenericType
                    && typeof(IUpdatePropertySettings<>).IsAssignableFrom(interfaceType.GetGenericTypeDefinition())
                let settingsType = interfaceType.GetGenericArguments().First()
                select new PropertySettingsUpdater(settingsType, attribute);

            return updaters.ToList();
        }

        private static IEnumerable<object> GetPropertyAttributes(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition)
        {
            // Binding flags supporting both public and non-public instance properties
            const BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            PropertyInfo prop;
            int hyphenIndex = propertyDefinition.Name.IndexOf("-", StringComparison.Ordinal);
            if (hyphenIndex >= 0)
            {
                // the property definition is a property belonging to a property group
                string propertyGroupPropertyName = propertyDefinition.Name.Substring(0, hyphenIndex);
                string propertyName = propertyDefinition.Name.Substring(hyphenIndex + 1);

                PropertyInfo propertyGroupProperty = pageTypeDefinition.Type.GetProperties(propertyBindingFlags)
                    .FirstOrDefault(p => String.Equals(p.Name, propertyGroupPropertyName, StringComparison.Ordinal));
                if (propertyGroupProperty == null)
                {
                    var message = String.Format("Unable to locate the property group-property \"{0}\" in PageType \"{1}\".",
                        propertyGroupPropertyName, pageTypeDefinition.GetPageTypeName());
                    throw new PageTypeBuilderException(message);
                }

                prop = propertyGroupProperty.PropertyType.GetProperties(propertyBindingFlags)
                    .FirstOrDefault(p => String.Equals(p.Name, propertyName, StringComparison.Ordinal));
            }
            else
            {
                prop = pageTypeDefinition.Type.GetProperties(propertyBindingFlags)
                    .FirstOrDefault(p => String.Equals(p.Name, propertyDefinition.Name, StringComparison.Ordinal));
            }

            if (prop == null)
            {
                var message = String.Format("Unable to locate the property \"{0}\" in PageType \"{1}\".",
                    propertyDefinition.Name, pageTypeDefinition.GetPageTypeName());
                throw new PageTypeBuilderException(message);
            }

            return prop.GetCustomAttributes(true);
        }
    }
}