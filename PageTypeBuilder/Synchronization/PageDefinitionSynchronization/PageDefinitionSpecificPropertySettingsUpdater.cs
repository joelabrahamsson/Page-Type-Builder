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
        Func<IPropertySettingsRepository> _propertySettingsRepositoryMethod;
        IGlobalPropertySettingsLocator globalPropertySettingsLocator;
        IPageDefinitionRepository pageDefinitionRepository;

        public PageDefinitionSpecificPropertySettingsUpdater(
            Func<IPropertySettingsRepository> propertySettingsRepositoryMethod,
            IGlobalPropertySettingsLocator globalPropertySettingsLocator,
            IPageDefinitionRepository pageDefinitionRepository)
        {
            this._propertySettingsRepositoryMethod = propertySettingsRepositoryMethod;
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
            List<PropertySettingsUpdater> settingsUpdaters = GetPropertySettingsUpdaters(pageTypeDefinition, propertyDefinition);
            if(!settingsUpdaters.Any())
            {
                return;
            }
            var container = GetPropertySettingsContainer(pageDefinition);
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
                        _propertySettingsRepositoryMethod().Save(container);
                    }
                });
        }

        void UpdatePageDefinitionsGlobalPropertySettings(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition, PageDefinition pageDefinition)
        {
            object[] attributes = GetPropertyAttributes(propertyDefinition, pageTypeDefinition);
            var useGlobalSettingsAttribute = attributes.OfType<UseGlobalSettingsAttribute>().FirstOrDefault();
            if (useGlobalSettingsAttribute != null)
            {
                var container = GetPropertySettingsContainer(pageDefinition);
                //TODO: Should validate not null and valid type at startup
                var globalSettingsUpdater = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters().Where(u => u.WrappedInstanceType == useGlobalSettingsAttribute.Type).First();
                var wrapper = _propertySettingsRepositoryMethod().GetGlobals(globalSettingsUpdater.SettingsType)
                    .Where(w => globalSettingsUpdater.Match(w))
                    .First();
                PropertySettingsWrapper existingWrapper = container.Settings.ContainsKey(globalSettingsUpdater.SettingsType.FullName)
                    ? container.Settings[globalSettingsUpdater.SettingsType.FullName]
                    : null;
                if (existingWrapper == null || existingWrapper.Id != wrapper.Id)
                {
                    container.Settings[globalSettingsUpdater.SettingsType.FullName] = wrapper;
                    //TODO: Add spec validating that exception is thrown with the below uncommented (An item with the same key has already been added.)
                    //container.Settings.Add(globalSettingsUpdater.SettingsType.FullName, wrapper);
                    _propertySettingsRepositoryMethod().Save(container);
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
                if (!_propertySettingsRepositoryMethod().TryGetContainer(pageDefinition.SettingsID, out container))
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
            // Binding flags supporting both public and non-public instance properties
            const BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            PropertyInfo prop = null;

            if (propertyDefinition.Name.Contains("-"))
            {
                // the property definition is a property belonging to a property group
                int index = propertyDefinition.Name.IndexOf("-");
                string propertyGroupPropertyName = propertyDefinition.Name.Substring(0, index);
                string propertyName = propertyDefinition.Name.Substring(index + 1);

                PropertyInfo propertyGroupProperty = pageTypeDefinition.Type.GetProperties(propertyBindingFlags).Where(p => String.Equals(p.Name, propertyGroupPropertyName)).FirstOrDefault();
                //if (propertyGroupProperty == null)
                //{
                //    // TODO: Enable exceptions for a development fail-fast mode?
                //    var message = String.Format("Unable to locate the property group-property \"{0}\" in PageType \"{1}\".",
                //        propertyGroupPropertyName, pageTypeDefinition.GetPageTypeName());
                //    throw new PageTypeBuilderException(message);
                //}
                if (propertyGroupProperty != null)
                {
                    prop = propertyGroupProperty.PropertyType.GetProperties().Where(p => String.Equals(p.Name, propertyName)).FirstOrDefault();
                }
            }
            else
            {
                prop =
                    pageTypeDefinition.Type.GetProperties(propertyBindingFlags).Where(p => String.Equals(p.Name, propertyDefinition.Name)).
                        FirstOrDefault();
            }

            if (prop == null)
            {
                // TODO: Enable exceptions for a development fail-fast mode? This is a serious error that could else be harder to find.
                //var message = String.Format("Unable to locate the property \"{0}\" in PageType \"{1}\".",
                //    propertyDefinition.Name, pageTypeDefinition.GetPageTypeName());
                //throw new PageTypeBuilderException(message);
                return new object[0];
            }

            return prop.GetCustomAttributes(true);
        }
    }
}