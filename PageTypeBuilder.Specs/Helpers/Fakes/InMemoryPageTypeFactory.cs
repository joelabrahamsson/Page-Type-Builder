using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageTypeFactory : IPageTypeFactory
    {
        private int nextId;
        private List<IPageType> pageTypes;
        private SavesPerIdCounter numberOfSavesPerPageTypeIdCounter = new SavesPerIdCounter();

        public InMemoryPageTypeFactory()
        {
            Mapper.Configuration.CreateMap<IPageType, FakePageType>()
                .ForMember(x => x.DefaultFrameID, m => m.Ignore())
                .ForMember(x => x.FileName, m => m.Ignore());
            Mapper.Configuration.CreateMap<IPageType, IPageType>();

            nextId = 1;
            pageTypes = new List<IPageType>();
        }

        public IPageType Load(string name)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.Name == name);
            if(pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = new FakePageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public IPageType Load(Guid guid)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.GUID == guid);
            if (pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = new FakePageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public IPageType Load(int id)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.ID == id);
            if (pageTypeRecord == null)
                return pageTypeRecord;

            var pageTypeToReturn = new FakePageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
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
            return pageTypes;
        }

        public int GetNumberOfSaves(int pageTypeId)
        {
            return numberOfSavesPerPageTypeIdCounter.GetNumberOfSaves(pageTypeId);
        }

        public void ResetNumberOfSaves()
        {
            numberOfSavesPerPageTypeIdCounter.ResetNumberOfSaves();
        }
    }

    public class FakePageType : IPageType
    {
        public FakePageType()
        {
            AllowedPageTypes = new int[0];
        }

        int[] allowedPageTypes; 
        public int[] AllowedPageTypes
        {
            get { return allowedPageTypes; }
            set 
            {
                if (value == null)
                    allowedPageTypes = new int[0];
                else
                    allowedPageTypes = value;
            }
        }
        public PageReference DefaultArchivePageLink { get; set; }
        public FilterSortOrder DefaultChildOrderRule { get; set; }
        public int DefaultFrameID { get; set; }
        public string DefaultPageName { get; set; }
        public int DefaultPeerOrder { get; set; }
        public TimeSpan DefaultStartPublishOffset { get; set; }
        public TimeSpan DefaultStopPublishOffset { get; set; }
        public PageTypeDefault Defaults { get; set; }
        public bool DefaultVisibleInMenu { get; set; }
        public PageDefinitionCollection Definitions { get; private set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public Guid GUID { get; set; }
        public int ID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNew { get; private set; }
        public string Name { get; set; }
        public void Save()
        {
            throw new NotImplementedException();
        }

        public int SortOrder { get; set; }
    }
}
