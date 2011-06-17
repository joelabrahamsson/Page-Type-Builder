using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageTypeRepository : IPageTypeRepository
    {
        private int nextId;
        private List<IPageType> pageTypes;
        private SavesPerIdCounter numberOfSavesPerPageTypeIdCounter = new SavesPerIdCounter();
        private IPageDefinitionRepository pageDefinitionRepository;

        public InMemoryPageTypeRepository(IPageDefinitionRepository pageDefinitionRepository)
        {
            this.pageDefinitionRepository = pageDefinitionRepository;
            Mapper.Configuration.CreateMap<IPageType, FakePageType>();
            Mapper.Configuration.CreateMap<FakePageType, IPageType>();
            Mapper.Configuration.CreateMap<IPageType, IPageType>();

            nextId = 1;
            pageTypes = new List<IPageType>();
        }

        public IPageType Load(string name)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.Name == name);
            if(pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = GetPageTypeToReturn(pageTypeRecord);
            return pageTypeToReturn;
        }

        private FakePageType GetPageTypeToReturn(IPageType pageTypeRecord)
        {
            var pageTypeToReturn = new FakePageType(pageDefinitionRepository);
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public IPageType Load(Guid guid)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.GUID == guid);
            if (pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = GetPageTypeToReturn(pageTypeRecord);
            return pageTypeToReturn;
        }

        public IPageType Load(int id)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.ID == id);
            if (pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = GetPageTypeToReturn(pageTypeRecord);
            return pageTypeToReturn;
        }

        public void Save(IPageType pageTypeToSave)
        {
            if (pageTypeToSave.IsNew)
            {
                pageTypeToSave.ID = nextId;
                nextId++;
                var pageTypeRecord = new FakePageType(pageDefinitionRepository);
                
                Mapper.Map(pageTypeToSave, pageTypeRecord);

                pageTypes.Add(pageTypeRecord);
            }
            else
            {
                var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.ID == pageTypeToSave.ID);
                Mapper.Map(pageTypeToSave, pageTypeRecord);
            }

            numberOfSavesPerPageTypeIdCounter.IncrementNumberOfSaves(pageTypeToSave.ID);
        }

        
        public IEnumerable<IPageType> List()
        {
            return pageTypes.Select(r => GetPageTypeToReturn(r)).Cast<IPageType>();
        }

        public int GetNumberOfSaves(int pageTypeId)
        {
            return numberOfSavesPerPageTypeIdCounter.GetNumberOfSaves(pageTypeId);
        }

        public void ResetNumberOfSaves()
        {
            numberOfSavesPerPageTypeIdCounter.ResetNumberOfSaves();
        }

        public IPageType CreateNew()
        {
            return new FakePageType(pageDefinitionRepository);
        }

        public void Delete(IPageType pageType)
        {
            if(pageType == null)
            {
                throw new ArgumentNullException("pageType");
            }
            var record = pageTypes.Where(p => p.ID == pageType.ID).FirstOrDefault();
            pageTypes.Remove(record);
        }
    }
}
