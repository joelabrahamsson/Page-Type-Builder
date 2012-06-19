using System;
using System.Collections.Generic;
using System.Globalization;
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
        internal List<string> updatedPropertySettingsCacheKeys; 

        public PageDefinitionSpecificPropertySettingsUpdater(
            Func<IPropertySettingsRepository> propertySettingsRepositoryMethod,
            IGlobalPropertySettingsLocator globalPropertySettingsLocator,
            IPageDefinitionRepository pageDefinitionRepository)
        {
            this._propertySettingsRepositoryMethod = propertySettingsRepositoryMethod;
            this.globalPropertySettingsLocator = globalPropertySettingsLocator;
            this.pageDefinitionRepository = pageDefinitionRepository;
            updatedPropertySettingsCacheKeys = new List<string>();
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
                        AddToUpdatedCacheKeys(container);
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
                var globalSettingsUpdater = globalPropertySettingsLocator.GetGlobalPropertySettingsUpdaters().First(u => u.WrappedInstanceType == useGlobalSettingsAttribute.Type);
                var wrapper = _propertySettingsRepositoryMethod().GetGlobals(globalSettingsUpdater.SettingsType)
                    .First(w => globalSettingsUpdater.Match(w));
                PropertySettingsWrapper existingWrapper = container.Settings.ContainsKey(globalSettingsUpdater.SettingsType.FullName)
                    ? container.Settings[globalSettingsUpdater.SettingsType.FullName]
                    : null;
                if (existingWrapper == null || existingWrapper.Id != wrapper.Id)
                {
                    container.Settings[globalSettingsUpdater.SettingsType.FullName] = wrapper;
                    //TODO: Add spec validating that exception is thrown with the below uncommented (An item with the same key has already been added.)
                    //container.Settings.Add(globalSettingsUpdater.SettingsType.FullName, wrapper);
                    _propertySettingsRepositoryMethod().Save(container);
                    AddToUpdatedCacheKeys(container);
                }
            }
        }

        private void AddToUpdatedCacheKeys(PropertySettingsContainer container)
        {
            string cacheKey = string.Format(CultureInfo.InvariantCulture, "EP:{0}", new object[] { typeof(PropertySettingsRepository).FullName });
            cacheKey = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { cacheKey, container.Id });
            updatedPropertySettingsCacheKeys.Add(cacheKey);
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

        private static List<PropertySettingsUpdater> GetPropertySettingsUpdaters(PageTypeDefinition pageTypeDefinition, PageTypePropertyDefinition propertyDefinition)
        {
            var settingsUpdaters =
                from attribute in GetPropertyAttributes(propertyDefinition, pageTypeDefinition)
                from interfaceType in attribute.GetType().GetInterfaces()
                where interfaceType.IsGenericType
                    && typeof(IUpdatePropertySettings<>).IsAssignableFrom(interfaceType.GetGenericTypeDefinition())
                let settingsType = interfaceType.GetGenericArguments().First()
                select new PropertySettingsUpdater(settingsType, attribute);

            return settingsUpdaters.ToList();
        }

        private static IEnumerable<object> GetPropertyAttributes(PageTypePropertyDefinition propertyDefinition, PageTypeDefinition pageTypeDefinition)
        {
            // Binding flags supporting both public and non-public instance properties
            const BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            PropertyInfo prop = null;

            int propertyGroupDashIndex = propertyDefinition.Name.IndexOf("-", StringComparison.Ordinal);
            if (propertyGroupDashIndex >= 0)
            {
                // the property definition is a property belonging to a property group
                string propertyGroupPropertyName = propertyDefinition.Name.Substring(0, propertyGroupDashIndex);
                string propertyName = propertyDefinition.Name.Substring(propertyGroupDashIndex + 1);

                PropertyInfo propertyGroupProperty = pageTypeDefinition.Type.GetProperties(propertyBindingFlags).FirstOrDefault(p => String.Equals(p.Name, propertyGroupPropertyName));
                //if (propertyGroupProperty == null)
                //{
                //    // TODO: Enable exceptions for a development fail-fast mode?
                //    var message = String.Format("Unable to locate the property group-property \"{0}\" in PageType \"{1}\".",
                //        propertyGroupPropertyName, pageTypeDefinition.GetPageTypeName());
                //    throw new PageTypeBuilderException(message);
                //}
                if (propertyGroupProperty != null)
                {
                    prop = propertyGroupProperty.PropertyType.GetProperties().FirstOrDefault(p => String.Equals(p.Name, propertyName));
                }
            }
            else
            {
                prop =
                    pageTypeDefinition.Type.GetProperties(propertyBindingFlags).FirstOrDefault(p => String.Equals(p.Name, propertyDefinition.Name));
            }

            if (prop == null)
            {
                // TODO: Enable exceptions for a development fail-fast mode? This is a serious error that could else be harder to find.
                //var message = String.Format("Unable to locate the property \"{0}\" in PageType \"{1}\".",
                //    propertyDefinition.Name, pageTypeDefinition.GetPageTypeName());
                //throw new PageTypeBuilderException(message);
                return Enumerable.Empty<object>();
            }

            return prop.GetCustomAttributes(true);
        }
    }
}