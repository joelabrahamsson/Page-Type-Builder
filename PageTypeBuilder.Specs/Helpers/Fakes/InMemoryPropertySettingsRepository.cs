using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPropertySettingsRepository : IPropertySettingsRepository
    {
        private Dictionary<Guid, PropertySettingsContainer> containers = new Dictionary<Guid, PropertySettingsContainer>();
        private Dictionary<Guid, PropertySettingsWrapper> wrappers = new Dictionary<Guid, PropertySettingsWrapper>();
        private SavesPerGuidCounter numberOfSavesPerGuidCounter = new SavesPerGuidCounter();

        public InMemoryPropertySettingsRepository()
        {
            Mapper.Configuration.AllowNullDestinationValues = true;
            Mapper.Configuration.CreateMap<PropertySettingsContainer, PropertySettingsContainer>()
                .ForMember(c => c.PropertyControl, x => x.Ignore());
            Mapper.Configuration.CreateMap<PropertySettingsWrapper, PropertySettingsWrapper>();
        }

        public bool TryGetContainer(Guid id, out PropertySettingsContainer propertySettingsContainer)
        {
            propertySettingsContainer = null;

            if (!containers.ContainsKey(id))
                return false;

            propertySettingsContainer = new PropertySettingsContainer(id);

            Mapper.Map(containers[id], propertySettingsContainer);

            return true;
        }

        public bool TryGetWrapper(Guid id, out PropertySettingsWrapper propertySettingsWrapper)
        {
            propertySettingsWrapper = null;

            if (!wrappers.ContainsKey(id))
                return false;

            var exposedWrapper = new PropertySettingsWrapper();

            Mapper.Map(wrappers[id], exposedWrapper);

            return true;
        }

        public IEnumerable<PropertySettingsWrapper> GetGlobals(Type settingsType)
        {
            return wrappers.Values.Where(w => w.TypeFullName.Equals(settingsType.FullName) && w.IsGlobal);
        }

        public PropertySettingsWrapper GetDefault(Type propertyType)
        {
            throw new NotImplementedException();
        }

        public void SetDefault(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Save(PropertySettingsContainer propertySetting)
        {
            var record = new PropertySettingsContainer(propertySetting.Id);
            Mapper.Map(propertySetting, record);
            containers[propertySetting.Id] = record;  
            numberOfSavesPerGuidCounter.IncrementNumberOfSaves(propertySetting.Id);
        }

        public void SaveGlobal(PropertySettingsWrapper global)
        {
            wrappers.Add(global.Id, global);
        }

        public void Delete(PropertySettingsContainer propertySetting)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteGlobal(Guid id)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfSaves(Guid settingsId)
        {
            return numberOfSavesPerGuidCounter.GetNumberOfSaves(settingsId);
        }

        public void ResetNumberOfSaves()
        {
            numberOfSavesPerGuidCounter.ResetNumberOfSaves();
        }
    }
}
