using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageTypeFactory : IPageTypeFactory
    {
        private int nextId;
        private List<IPageType> pageTypes;
        private SavesPerIdCounter numberOfSavesPerPageTypeIdCounter = new SavesPerIdCounter();
        private InMemoryPageDefinitionFactory pageDefinitionFactory;

        public InMemoryPageTypeFactory(InMemoryPageDefinitionFactory pageDefinitionFactory)
        {
            this.pageDefinitionFactory = pageDefinitionFactory;
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
            var definitions = new PageDefinitionCollection();
            definitions.AddRange(pageDefinitionFactory.List(pageTypeRecord.ID));
            var pageTypeToReturn = new FakePageType(definitions);
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
                var pageTypeRecord = new FakePageType();
                
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
            return new FakePageType();
        }
    }
}
